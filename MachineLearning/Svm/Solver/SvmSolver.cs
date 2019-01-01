using System;
using HC.Analytics.MachineLearning.Svm.Kernels;

namespace HC.Analytics.MachineLearning.Svm.Solver
{
    // Generalized SMO+SVMlight algorithm
    // Solves:
    //
    //	Min 0.5(\alpha^T Q \alpha) + b^T \alpha
    //
    //		y^T \alpha = \delta
    //		y_i = +1 or -1
    //		0 <= alpha_i <= Cp for y_i = 1
    //		0 <= alpha_i <= Cn for y_i = -1
    //
    // Given:
    //
    //	Q, b, y, Cp, Cn, and an initial feasible point \alpha
    //	l is the size of vectors and matrices
    //	eps is the stopping criterion
    //
    // solution will be put in \alpha, objective value will be put in obj
    //
    public class SvmSolver
    {
        #region Members

        public int active_size;
        public int[] y;
        public double[] G;		// gradient of objective function
        public static int LOWER_BOUND = 0;
        public static int UPPER_BOUND = 1;
        public static int FREE = 2;
        public int[] alpha_status;	// LOWER_BOUND, UPPER_BOUND, FREE
        public double[] alpha;
        public AbstractQMatrix Q;
        public float[] QD;
        public double eps;
        public double Cp, Cn;
        public double[] b;
        public int[] active_set;
        public double[] G_bar;		// gradient, if we treat free variables as 0
        public int l;
        public bool unshrinked;	// XXX

        public static double INF = double.PositiveInfinity;

        #endregion

        public double GetC(int i)
        {
            return (y[i] > 0) ? Cp : Cn;
        }
        
        public void update_alpha_status(int i)
        {
            if (alpha[i] >= GetC(i))
                alpha_status[i] = UPPER_BOUND;
            else if (alpha[i] <= 0)
                alpha_status[i] = LOWER_BOUND;
            else alpha_status[i] = FREE;
        }
        
        public bool is_upper_bound(int i) { return alpha_status[i] == UPPER_BOUND; }
        
        public bool is_lower_bound(int i) { return alpha_status[i] == LOWER_BOUND; }
        
        public bool is_free(int i) { return alpha_status[i] == FREE; }

        public void SwapIndex(int i, int j)
        {
            Q.SwapIndex(i, j);
            do { int _ = y[i]; y[i] = y[j]; y[j] = _; } while (false);
            do { double _ = G[i]; G[i] = G[j]; G[j] = _; } while (false);
            do { int _ = alpha_status[i]; alpha_status[i] = alpha_status[j]; alpha_status[j] = _; } while (false);
            do { double _ = alpha[i]; alpha[i] = alpha[j]; alpha[j] = _; } while (false);
            do { double _ = b[i]; b[i] = b[j]; b[j] = _; } while (false);
            do { int _ = active_set[i]; active_set[i] = active_set[j]; active_set[j] = _; } while (false);
            do { double _ = G_bar[i]; G_bar[i] = G_bar[j]; G_bar[j] = _; } while (false);
        }

        public void ReconstructGradient()
        {
            // reconstruct inactive elements of G from G_bar and free variables

            if (active_size == l) return;

            int i;
            for (i = active_size; i < l; i++)
                G[i] = G_bar[i] + b[i];

            for (i = 0; i < active_size; i++)
                if (is_free(i))
                {
                    float[] Q_i = Q.GetQ(i, l);
                    double alpha_i = alpha[i];
                    for (int j = active_size; j < l; j++)
                        G[j] += alpha_i * Q_i[j];
                }
        }

        public void Solve(
            int l, 
            AbstractQMatrix Q, 
            double[] b_, 
            int[] y_,
            double[] alpha_, 
            double Cp, 
            double Cn, 
            double eps, 
            SolutionInfo si, 
            int shrinking)
        {
            this.l = l;
            this.Q = Q;
            QD = Q.GetQd();
            b = (double[])b_.Clone();
            y = (int[])y_.Clone();
            alpha = (double[])alpha_.Clone();
            this.Cp = Cp;
            this.Cn = Cn;
            this.eps = eps;
            this.unshrinked = false;

            // initialize alpha_status
            {
                alpha_status = new int[l];
                for (int i = 0; i < l; i++)
                    update_alpha_status(i);
            }

            // initialize active set (for shrinking)
            {
                active_set = new int[l];
                for (int i = 0; i < l; i++)
                    active_set[i] = i;
                active_size = l;
            }

            // initialize gradient
            {
                G = new double[l];
                G_bar = new double[l];
                int i;
                for (i = 0; i < l; i++)
                {
                    G[i] = b[i];
                    G_bar[i] = 0;
                }
                for (i = 0; i < l; i++)
                    if (!is_lower_bound(i))
                    {
                        float[] Q_i = Q.GetQ(i, l);
                        double alpha_i = alpha[i];
                        int j;
                        for (j = 0; j < l; j++)
                            G[j] += alpha_i * Q_i[j];
                        if (is_upper_bound(i))
                            for (j = 0; j < l; j++)
                                G_bar[j] += GetC(i) * Q_i[j];
                    }
            }

            // optimization step

            int iter = 0;
            int counter = Math.Min(l, 1000) + 1;
            int[] working_set = new int[2];

            while (true)
            {
                // show progress and do shrinking

                if (--counter == 0)
                {
                    counter = Math.Min(l, 1000);
                    if (shrinking != 0) DoShrinking();
                    Console.Write(".");
                }

                if (SelectWorkingSet(working_set) != 0)
                {
                    // reconstruct the whole gradient
                    ReconstructGradient();
                    // reset active set size and check
                    active_size = l;
                    Console.Write("*");
                    if (SelectWorkingSet(working_set) != 0)
                        break;
                    else
                        counter = 1;	// do shrinking next iteration
                }

                int i = working_set[0];
                int j = working_set[1];

                ++iter;

                // update alpha[i] and alpha[j], handle bounds carefully

                float[] Q_i = Q.GetQ(i, active_size);
                float[] Q_j = Q.GetQ(j, active_size);

                double C_i = GetC(i);
                double C_j = GetC(j);

                double old_alpha_i = alpha[i];
                double old_alpha_j = alpha[j];

                if (y[i] != y[j])
                {
                    double quad_coef = Q_i[i] + Q_j[j] + 2 * Q_i[j];
                    if (quad_coef <= 0)
                        quad_coef = 1e-12;
                    double delta = (-G[i] - G[j]) / quad_coef;
                    double diff = alpha[i] - alpha[j];
                    alpha[i] += delta;
                    alpha[j] += delta;

                    if (diff > 0)
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = diff;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = -diff;
                        }
                    }
                    if (diff > C_i - C_j)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = C_i - diff;
                        }
                    }
                    else
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = C_j + diff;
                        }
                    }
                }
                else
                {
                    double quad_coef = Q_i[i] + Q_j[j] - 2 * Q_i[j];
                    if (quad_coef <= 0)
                        quad_coef = 1e-12;
                    double delta = (G[i] - G[j]) / quad_coef;
                    double sum = alpha[i] + alpha[j];
                    alpha[i] -= delta;
                    alpha[j] += delta;

                    if (sum > C_i)
                    {
                        if (alpha[i] > C_i)
                        {
                            alpha[i] = C_i;
                            alpha[j] = sum - C_i;
                        }
                    }
                    else
                    {
                        if (alpha[j] < 0)
                        {
                            alpha[j] = 0;
                            alpha[i] = sum;
                        }
                    }
                    if (sum > C_j)
                    {
                        if (alpha[j] > C_j)
                        {
                            alpha[j] = C_j;
                            alpha[i] = sum - C_j;
                        }
                    }
                    else
                    {
                        if (alpha[i] < 0)
                        {
                            alpha[i] = 0;
                            alpha[j] = sum;
                        }
                    }
                }

                // update G

                double delta_alpha_i = alpha[i] - old_alpha_i;
                double delta_alpha_j = alpha[j] - old_alpha_j;

                for (int k = 0; k < active_size; k++)
                {
                    G[k] += Q_i[k] * delta_alpha_i + Q_j[k] * delta_alpha_j;
                }

                // update alpha_status and G_bar

                {
                    bool ui = is_upper_bound(i);
                    bool uj = is_upper_bound(j);
                    update_alpha_status(i);
                    update_alpha_status(j);
                    int k;
                    if (ui != is_upper_bound(i))
                    {
                        Q_i = Q.GetQ(i, l);
                        if (ui)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_i * Q_i[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_i * Q_i[k];
                    }

                    if (uj != is_upper_bound(j))
                    {
                        Q_j = Q.GetQ(j, l);
                        if (uj)
                            for (k = 0; k < l; k++)
                                G_bar[k] -= C_j * Q_j[k];
                        else
                            for (k = 0; k < l; k++)
                                G_bar[k] += C_j * Q_j[k];
                    }
                }

            }

            // calculate rho

            si.rho = CalculateRho();

            // calculate objective value
            {
                double v = 0;
                int i;
                for (i = 0; i < l; i++)
                    v += alpha[i] * (G[i] + b[i]);

                si.obj = v / 2;
            }

            // put back the solution
            {
                for (int i = 0; i < l; i++)
                    alpha_[active_set[i]] = alpha[i];
            }

            si.upper_bound_p = Cp;
            si.upper_bound_n = Cn;


            Console.Write("\noptimization finished, #iter = " + iter + Environment.NewLine);
        }

        // return 1 if already optimal, return 0 otherwise
        int SelectWorkingSet(int[] working_set)
        {
            // return i,j such that
            // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            // j: mimimizes the decrease of obj value
            //    (if quadratic coefficeint <= 0, replace it with tau)
            //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            double Gmax = -INF;
            int Gmax_idx = -1;
            int Gmin_idx = -1;
            double obj_diff_min = INF;

            for (int t = 0; t < active_size; t++)
                if (y[t] == +1)
                {
                    if (!is_upper_bound(t))
                        if (-G[t] >= Gmax)
                        {
                            Gmax = -G[t];
                            Gmax_idx = t;
                        }
                }
                else
                {
                    if (!is_lower_bound(t))
                        if (G[t] >= Gmax)
                        {
                            Gmax = G[t];
                            Gmax_idx = t;
                        }
                }

            int i = Gmax_idx;
            float[] Q_i = null;
            if (i != -1) // null Q_i not accessed: Gmax=-INF if i=-1
                Q_i = Q.GetQ(i, active_size);

            for (int j = 0; j < active_size; j++)
            {
                if (y[j] == +1)
                {
                    if (!is_lower_bound(j))
                    {
                        double grad_diff = Gmax + G[j];
                        if (grad_diff >= eps)
                        {
                            double obj_diff;
                            double quad_coef = Q_i[i] + QD[j] - 2 * y[i] * Q_i[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
                else
                {
                    if (!is_upper_bound(j))
                    {
                        double grad_diff = Gmax - G[j];
                        if (grad_diff >= eps)
                        {
                            double obj_diff;
                            double quad_coef = Q_i[i] + QD[j] + 2 * y[i] * Q_i[j];
                            if (quad_coef > 0)
                                obj_diff = -(grad_diff * grad_diff) / quad_coef;
                            else
                                obj_diff = -(grad_diff * grad_diff) / 1e-12;

                            if (obj_diff <= obj_diff_min)
                            {
                                Gmin_idx = j;
                                obj_diff_min = obj_diff;
                            }
                        }
                    }
                }
            }

            if (Gmin_idx == -1)
                return 1;

            working_set[0] = Gmax_idx;
            working_set[1] = Gmin_idx;
            return 0;
        }

        // return 1 if already optimal, return 0 otherwise
        int MaxViolatingPair(int[] working_set)
        {
            // return i,j which maximize -grad(f)^T d , under constraint
            // if alpha_i == C, d != +1
            // if alpha_i == 0, d != -1

            double Gmax1 = -INF;		// Max { -y_i * grad(f)_i | i in I_up(\alpha) }
            int Gmax1_idx = -1;

            int Gmax2_idx = -1;
            double Gmax2 = -INF;		// Max { y_i * grad(f)_i | i in I_low(\alpha) }

            for (int i = 0; i < active_size; i++)
            {
                if (y[i] == +1)	// y = +1
                {
                    if (!is_upper_bound(i))	// d = +1
                    {
                        if (-G[i] >= Gmax1)
                        {
                            Gmax1 = -G[i];
                            Gmax1_idx = i;
                        }
                    }
                    if (!is_lower_bound(i))	// d = -1
                    {
                        if (G[i] >= Gmax2)
                        {
                            Gmax2 = G[i];
                            Gmax2_idx = i;
                        }
                    }
                }
                else		// y = -1
                {
                    if (!is_upper_bound(i))	// d = +1
                    {
                        if (-G[i] >= Gmax2)
                        {
                            Gmax2 = -G[i];
                            Gmax2_idx = i;
                        }
                    }
                    if (!is_lower_bound(i))	// d = -1
                    {
                        if (G[i] >= Gmax1)
                        {
                            Gmax1 = G[i];
                            Gmax1_idx = i;
                        }
                    }
                }
            }

            if (Gmax1 + Gmax2 < eps)
                return 1;

            working_set[0] = Gmax1_idx;
            working_set[1] = Gmax2_idx;
            return 0;
        }

        void DoShrinking()
        {
            int i, j, k;
            int[] working_set = new int[2];
            if (MaxViolatingPair(working_set) != 0) return;
            i = working_set[0];
            j = working_set[1];
            double Gm1 = -y[j] * G[j];
            double Gm2 = y[i] * G[i];

            // shrink

            for (k = 0; k < active_size; k++)
            {
                if (is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] >= Gm1) continue;
                    }
                    else if (-G[k] >= Gm2) continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] >= Gm2) continue;
                    }
                    else if (G[k] >= Gm1) continue;
                }
                else continue;

                --active_size;
                SwapIndex(k, active_size);
                --k;	// look at the newcomer
            }

            // unshrink, check all variables again before  iterations

            if (unshrinked || -(Gm1 + Gm2) > eps * 10) return;

            unshrinked = true;
            ReconstructGradient();

            for (k = l - 1; k >= active_size; k--)
            {
                if (is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] < Gm1) continue;
                    }
                    else if (-G[k] < Gm2) continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] < Gm2) continue;
                    }
                    else if (G[k] < Gm1) continue;
                }
                else continue;

                SwapIndex(k, active_size);
                active_size++;
                ++k;	// look at the newcomer
            }
        }

        double CalculateRho()
        {
            double r;
            int nr_free = 0;
            double ub = INF, lb = -INF, sum_free = 0;
            for (int i = 0; i < active_size; i++)
            {
                double yG = y[i] * G[i];

                if (is_lower_bound(i))
                {
                    if (y[i] > 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else if (is_upper_bound(i))
                {
                    if (y[i] < 0)
                        ub = Math.Min(ub, yG);
                    else
                        lb = Math.Max(lb, yG);
                }
                else
                {
                    ++nr_free;
                    sum_free += yG;
                }
            }

            if (nr_free > 0)
                r = sum_free / nr_free;
            else
                r = (ub + lb) / 2;

            return r;
        }

    }
}

