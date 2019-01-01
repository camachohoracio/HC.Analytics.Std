using System;
using HC.Analytics.MachineLearning.Svm.Kernels;

namespace HC.Analytics.MachineLearning.Svm.Solver
{
    //
    // Solver for nu-svm classification and regression
    //
    // additional constraint: e^T \alpha = constant
    //
    public class SolverNu : SvmSolver
    {
        private SolutionInfo si;

        void Solve(int l, AbstractQMatrix Q, double[] b, int[] y,
               double[] alpha, double Cp, double Cn, double eps,
               SolutionInfo si, int shrinking)
        {
            this.si = si;
            base.Solve(l, Q, b, y, alpha, Cp, Cn, eps, si, shrinking);
        }

        // return 1 if already optimal, return 0 otherwise
        int SelectWorkingSet(int[] working_set)
        {
            // return i,j such that y_i = y_j and
            // i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            // j: minimizes the decrease of obj value
            //    (if quadratic coefficeint <= 0, replace it with tau)
            //    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            double Gmaxp = -INF;
            int Gmaxp_idx = -1;

            double Gmaxn = -INF;
            int Gmaxn_idx = -1;

            int Gmin_idx = -1;
            double obj_diff_min = INF;

            for (int t = 0; t < active_size; t++)
                if (y[t] == +1)
                {
                    if (!is_upper_bound(t))
                        if (-G[t] >= Gmaxp)
                        {
                            Gmaxp = -G[t];
                            Gmaxp_idx = t;
                        }
                }
                else
                {
                    if (!is_lower_bound(t))
                        if (G[t] >= Gmaxn)
                        {
                            Gmaxn = G[t];
                            Gmaxn_idx = t;
                        }
                }

            int ip = Gmaxp_idx;
            int in_ = Gmaxn_idx;
            float[] Q_ip = null;
            float[] Q_in = null;
            if (ip != -1) // null Q_ip not accessed: Gmaxp=-INF if ip=-1
                Q_ip = Q.GetQ(ip, active_size);
            if (in_ != -1)
                Q_in = Q.GetQ(in_, active_size);

            for (int j = 0; j < active_size; j++)
            {
                if (y[j] == +1)
                {
                    if (!is_lower_bound(j))
                    {
                        double grad_diff = Gmaxp + G[j];
                        if (grad_diff >= eps)
                        {
                            double obj_diff;
                            double quad_coef = Q_ip[ip] + QD[j] - 2 * Q_ip[j];
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
                        double grad_diff = Gmaxn - G[j];
                        if (grad_diff >= eps)
                        {
                            double obj_diff;
                            double quad_coef = Q_in[in_] + QD[j] - 2 * Q_in[j];
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

            if (y[Gmin_idx] == +1)
                working_set[0] = Gmaxp_idx;
            else
                working_set[0] = Gmaxn_idx;
            working_set[1] = Gmin_idx;

            return 0;
        }

        void DoShrinking()
        {
            double Gmax1 = -INF;	// Max { -y_i * grad(f)_i | y_i = +1, i in I_up(\alpha) }
            double Gmax2 = -INF;	// Max { y_i * grad(f)_i | y_i = +1, i in I_low(\alpha) }
            double Gmax3 = -INF;	// Max { -y_i * grad(f)_i | y_i = -1, i in I_up(\alpha) }
            double Gmax4 = -INF;	// Max { y_i * grad(f)_i | y_i = -1, i in I_low(\alpha) }

            // find maximal violating pair first
            int k;
            for (k = 0; k < active_size; k++)
            {
                if (!is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] > Gmax1) Gmax1 = -G[k];
                    }
                    else if (-G[k] > Gmax3) Gmax3 = -G[k];
                }
                if (!is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] > Gmax2) Gmax2 = G[k];
                    }
                    else if (G[k] > Gmax4) Gmax4 = G[k];
                }
            }

            // shrinking

            double Gm1 = -Gmax2;
            double Gm2 = -Gmax1;
            double Gm3 = -Gmax4;
            double Gm4 = -Gmax3;

            for (k = 0; k < active_size; k++)
            {
                if (is_lower_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (-G[k] >= Gm1) continue;
                    }
                    else if (-G[k] >= Gm3) continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] >= Gm2) continue;
                    }
                    else if (G[k] >= Gm4) continue;
                }
                else continue;

                --active_size;
                SwapIndex(k, active_size);
                --k;	// look at the newcomer
            }

            // unshrink, check all variables again before  iterations

            if (unshrinked || Math.Max(-(Gm1 + Gm2), -(Gm3 + Gm4)) > eps * 10) return;

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
                    else if (-G[k] < Gm3) continue;
                }
                else if (is_upper_bound(k))
                {
                    if (y[k] == +1)
                    {
                        if (G[k] < Gm2) continue;
                    }
                    else if (G[k] < Gm4) continue;
                }
                else continue;

                SwapIndex(k, active_size);
                active_size++;
                ++k;	// look at the newcomer
            }
        }

        double CalculateRho()
        {
            int nr_free1 = 0, nr_free2 = 0;
            double ub1 = INF, ub2 = INF;
            double lb1 = -INF, lb2 = -INF;
            double sum_free1 = 0, sum_free2 = 0;

            for (int i = 0; i < active_size; i++)
            {
                if (y[i] == +1)
                {
                    if (is_lower_bound(i))
                        ub1 = Math.Min(ub1, G[i]);
                    else if (is_upper_bound(i))
                        lb1 = Math.Max(lb1, G[i]);
                    else
                    {
                        ++nr_free1;
                        sum_free1 += G[i];
                    }
                }
                else
                {
                    if (is_lower_bound(i))
                        ub2 = Math.Min(ub2, G[i]);
                    else if (is_upper_bound(i))
                        lb2 = Math.Max(lb2, G[i]);
                    else
                    {
                        ++nr_free2;
                        sum_free2 += G[i];
                    }
                }
            }

            double r1, r2;
            if (nr_free1 > 0)
                r1 = sum_free1 / nr_free1;
            else
                r1 = (ub1 + lb1) / 2;

            if (nr_free2 > 0)
                r2 = sum_free2 / nr_free2;
            else
                r2 = (ub2 + lb2) / 2;

            si.r = (r1 + r2) / 2;
            return (r1 - r2) / 2;
        }
    }
}

