//package libsvm;
//import java.io.*;
//import java.util.*;

//
// Kernel Cache
//
// l is the number of total data items
// size is the cache size limit in ints
//
using System;
using System.IO;
using HC.Analytics.MachineLearning.Svm.Kernels;
using HC.Analytics.MachineLearning.Svm.Solver;

namespace HC.Analytics.MachineLearning.Svm
{
    public class SvmClass
    {
        //
        // construct and solve various formulations
        //
        private static void SolveCSvc(
            SvmProblem prob,
            SvmParameters param,
            double[] alpha,
            SolutionInfo si,
            double Cp,
            double Cn)
        {
            int l = prob.l;
            double[] minus_ones = new double[l];
            int[] y = new int[l];

            int i;

            for (i = 0; i < l; i++)
            {
                alpha[i] = 0;
                minus_ones[i] = -1;
                if (prob.y[i] > 0)
                {
                    y[i] = +1;
                }
                else
                {
                    y[i] = -1;
                }
            }

            SvmSolver s = new SvmSolver();
            s.Solve(
                l, 
                new SvcQ(prob, param, y), 
                minus_ones, 
                y,
                alpha, 
                Cp, 
                Cn, 
                param.eps, 
                si, 
                param.shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
                sum_alpha += alpha[i];

            if (Cp == Cn)
            {
                Console.Write("nu = " + sum_alpha / (Cp * prob.l) + Environment.NewLine);
            }

            for (i = 0; i < l; i++)
            {
                alpha[i] *= y[i];
            }
        }

        private static void SolveNuSvc(SvmProblem prob, SvmParameters param,
                        double[] alpha, SolutionInfo si)
        {
            int i;
            int l = prob.l;
            double nu = param.nu;

            int[] y = new int[l];

            for (i = 0; i < l; i++)
                if (prob.y[i] > 0)
                    y[i] = +1;
                else
                    y[i] = -1;

            double sum_pos = nu * l / 2;
            double sum_neg = nu * l / 2;

            for (i = 0; i < l; i++)
                if (y[i] == +1)
                {
                    alpha[i] = Math.Min(1.0, sum_pos);
                    sum_pos -= alpha[i];
                }
                else
                {
                    alpha[i] = Math.Min(1.0, sum_neg);
                    sum_neg -= alpha[i];
                }

            double[] zeros = new double[l];

            for (i = 0; i < l; i++)
                zeros[i] = 0;

            SolverNu s = new SolverNu();
            s.Solve(l, new SvcQ(prob, param, y), zeros, y,
                alpha, 1.0, 1.0, param.eps, si, param.shrinking);
            double r = si.r;

            Console.Write("C = " + 1 / r + Environment.NewLine);

            for (i = 0; i < l; i++)
                alpha[i] *= y[i] / r;

            si.rho /= r;
            si.obj /= (r * r);
            si.upper_bound_p = 1 / r;
            si.upper_bound_n = 1 / r;
        }

        private static void SolveOneClass(SvmProblem prob, SvmParameters param,
                            double[] alpha, SolutionInfo si)
        {
            int l = prob.l;
            double[] zeros = new double[l];
            int[] ones = new int[l];
            int i;

            int n = (int)(param.nu * prob.l);	// # of alpha's at upper bound

            for (i = 0; i < n; i++)
                alpha[i] = 1;
            if (n < prob.l)
                alpha[n] = param.nu * prob.l - n;
            for (i = n + 1; i < l; i++)
                alpha[i] = 0;

            for (i = 0; i < l; i++)
            {
                zeros[i] = 0;
                ones[i] = 1;
            }

            SvmSolver s = new SvmSolver();
            s.Solve(l, new OneClassQ(prob, param), zeros, ones,
                alpha, 1.0, 1.0, param.eps, si, param.shrinking);
        }

        private static void SolveEpsilonSvr(SvmProblem prob, SvmParameters param,
                        double[] alpha, SolutionInfo si)
        {
            int l = prob.l;
            double[] alpha2 = new double[2 * l];
            double[] linear_term = new double[2 * l];
            int[] y = new int[2 * l];
            int i;

            for (i = 0; i < l; i++)
            {
                alpha2[i] = 0;
                linear_term[i] = param.p - prob.y[i];
                y[i] = 1;

                alpha2[i + l] = 0;
                linear_term[i + l] = param.p + prob.y[i];
                y[i + l] = -1;
            }

            SvmSolver s = new SvmSolver();
            s.Solve(2 * l, new SvrQ(prob, param), linear_term, y,
                alpha2, param.C, param.C, param.eps, si, param.shrinking);

            double sum_alpha = 0;
            for (i = 0; i < l; i++)
            {
                alpha[i] = alpha2[i] - alpha2[i + l];
                sum_alpha += Math.Abs(alpha[i]);
            }
            Console.Write("nu = " + sum_alpha / (param.C * l) + Environment.NewLine);
        }

        private static void SolveNuSvr(SvmProblem prob, SvmParameters param,
                        double[] alpha, SolutionInfo si)
        {
            int l = prob.l;
            double C = param.C;
            double[] alpha2 = new double[2 * l];
            double[] linear_term = new double[2 * l];
            int[] y = new int[2 * l];
            int i;

            double sum = C * param.nu * l / 2;
            for (i = 0; i < l; i++)
            {
                alpha2[i] = alpha2[i + l] = Math.Min(sum, C);
                sum -= alpha2[i];

                linear_term[i] = -prob.y[i];
                y[i] = 1;

                linear_term[i + l] = prob.y[i];
                y[i + l] = -1;
            }

            SolverNu s = new SolverNu();
            s.Solve(2 * l, new SvrQ(prob, param), linear_term, y,
                alpha2, C, C, param.eps, si, param.shrinking);

            Console.Write("epsilon = " + (-si.r) + Environment.NewLine);

            for (i = 0; i < l; i++)
                alpha[i] = alpha2[i] - alpha2[i + l];
        }


        static SvmDecisionFunction SvmTrainOne(
            SvmProblem prob, SvmParameters param,
            double Cp, double Cn)
        {
            double[] alpha = new double[prob.l];
            SolutionInfo si = new SolutionInfo();
            switch (param.svm_type)
            {
                case SvmParameters.C_SVC:
                    SolveCSvc(prob, param, alpha, si, Cp, Cn);
                    break;
                case SvmParameters.NU_SVC:
                    SolveNuSvc(prob, param, alpha, si);
                    break;
                case SvmParameters.ONE_CLASS:
                    SolveOneClass(prob, param, alpha, si);
                    break;
                case SvmParameters.EPSILON_SVR:
                    SolveEpsilonSvr(prob, param, alpha, si);
                    break;
                case SvmParameters.NU_SVR:
                    SolveNuSvr(prob, param, alpha, si);
                    break;
            }

            Console.Write("obj = " + si.obj + ", rho = " + si.rho + Environment.NewLine);

            // output SVs

            int nSV = 0;
            int nBSV = 0;
            for (int i = 0; i < prob.l; i++)
            {
                if (Math.Abs(alpha[i]) > 0)
                {
                    ++nSV;
                    if (prob.y[i] > 0)
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_p)
                            ++nBSV;
                    }
                    else
                    {
                        if (Math.Abs(alpha[i]) >= si.upper_bound_n)
                            ++nBSV;
                    }
                }
            }

            Console.Write("nSV = " + nSV + ", nBSV = " + nBSV + Environment.NewLine);

            SvmDecisionFunction f = new SvmDecisionFunction();
            f.alpha = alpha;
            f.rho = si.rho;
            return f;
        }

        // Platt's binary SVM Probablistic Output: an improvement from Lin et al.
        private static void SigmoidTrain(int l, double[] dec_values, double[] labels,
                      double[] probAB)
        {
            double A, B;
            double prior1 = 0, prior0 = 0;
            int i;

            for (i = 0; i < l; i++)
                if (labels[i] > 0) prior1 += 1;
                else prior0 += 1;

            int max_iter = 100; 	// Maximal number of iterations
            double min_step = 1e-10;	// Minimal step taken in line search
            double sigma = 1e-3;	// For numerically strict PD of Hessian
            double eps = 1e-5;
            double hiTarget = (prior1 + 1.0) / (prior1 + 2.0);
            double loTarget = 1 / (prior0 + 2.0);
            double[] t = new double[l];
            double fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize;
            double newA, newB, newf, d1, d2;
            int iter;

            // Initial Point and Initial Fun Value
            A = 0.0; B = Math.Log((prior0 + 1.0) / (prior1 + 1.0));
            double fval = 0.0;

            for (i = 0; i < l; i++)
            {
                if (labels[i] > 0) t[i] = hiTarget;
                else t[i] = loTarget;
                fApB = dec_values[i] * A + B;
                if (fApB >= 0)
                    fval += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                else
                    fval += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
            }
            for (iter = 0; iter < max_iter; iter++)
            {
                // Update Gradient and Hessian (use H' = H + sigma I)
                h11 = sigma; // numerically ensures strict PD
                h22 = sigma;
                h21 = 0.0; g1 = 0.0; g2 = 0.0;
                for (i = 0; i < l; i++)
                {
                    fApB = dec_values[i] * A + B;
                    if (fApB >= 0)
                    {
                        p = Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
                        q = 1.0 / (1.0 + Math.Exp(-fApB));
                    }
                    else
                    {
                        p = 1.0 / (1.0 + Math.Exp(fApB));
                        q = Math.Exp(fApB) / (1.0 + Math.Exp(fApB));
                    }
                    d2 = p * q;
                    h11 += dec_values[i] * dec_values[i] * d2;
                    h22 += d2;
                    h21 += dec_values[i] * d2;
                    d1 = t[i] - p;
                    g1 += dec_values[i] * d1;
                    g2 += d1;
                }

                // Stopping Criteria
                if (Math.Abs(g1) < eps && Math.Abs(g2) < eps)
                    break;

                // Finding Newton direction: -inv(H') * g
                det = h11 * h22 - h21 * h21;
                dA = -(h22 * g1 - h21 * g2) / det;
                dB = -(-h21 * g1 + h11 * g2) / det;
                gd = g1 * dA + g2 * dB;


                stepsize = 1; 		// Line Search
                while (stepsize >= min_step)
                {
                    newA = A + stepsize * dA;
                    newB = B + stepsize * dB;

                    // New function value
                    newf = 0.0;
                    for (i = 0; i < l; i++)
                    {
                        fApB = dec_values[i] * newA + newB;
                        if (fApB >= 0)
                            newf += t[i] * fApB + Math.Log(1 + Math.Exp(-fApB));
                        else
                            newf += (t[i] - 1) * fApB + Math.Log(1 + Math.Exp(fApB));
                    }
                    // Check sufficient decrease
                    if (newf < fval + 0.0001 * stepsize * gd)
                    {
                        A = newA; B = newB; fval = newf;
                        break;
                    }
                    else
                        stepsize = stepsize / 2.0;
                }

                if (stepsize < min_step)
                {
                    Console.Write("Line search fails in two-class probability estimates\n");
                    break;
                }
            }

            if (iter >= max_iter)
                Console.Write("Reaching maximal iterations in two-class probability estimates\n");
            probAB[0] = A; probAB[1] = B;
        }

        private static double SigmoidPredict(double decision_value, double A, double B)
        {
            double fApB = decision_value * A + B;
            if (fApB >= 0)
                return Math.Exp(-fApB) / (1.0 + Math.Exp(-fApB));
            else
                return 1.0 / (1 + Math.Exp(fApB));
        }

        // Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
        private static void MulticlassProbability(int k, double[][] r, double[] p)
        {
            int t, j;
            int iter = 0, max_iter = 100;
            double[][] Q = new double[k][];
            for (int i = 0; i < k; i++)
            {
                Q[i] = new double[k];
            }

            double[] Qp = new double[k];
            double pQp, eps = 0.005 / k;

            for (t = 0; t < k; t++)
            {
                p[t] = 1.0 / k;  // Valid if k = 1
                Q[t][t] = 0;
                for (j = 0; j < t; j++)
                {
                    Q[t][t] += r[j][t] * r[j][t];
                    Q[t][j] = Q[j][t];
                }
                for (j = t + 1; j < k; j++)
                {
                    Q[t][t] += r[j][t] * r[j][t];
                    Q[t][j] = -r[j][t] * r[t][j];
                }
            }
            for (iter = 0; iter < max_iter; iter++)
            {
                // stopping condition, recalculate QP,pQP for numerical accuracy
                pQp = 0;
                for (t = 0; t < k; t++)
                {
                    Qp[t] = 0;
                    for (j = 0; j < k; j++)
                        Qp[t] += Q[t][j] * p[j];
                    pQp += p[t] * Qp[t];
                }
                double max_error = 0;
                for (t = 0; t < k; t++)
                {
                    double error = Math.Abs(Qp[t] - pQp);
                    if (error > max_error)
                        max_error = error;
                }
                if (max_error < eps) break;

                for (t = 0; t < k; t++)
                {
                    double diff = (-Qp[t] + pQp) / Q[t][t];
                    p[t] += diff;
                    pQp = (pQp + diff * (diff * Q[t][t] + 2 * Qp[t])) / (1 + diff) / (1 + diff);
                    for (j = 0; j < k; j++)
                    {
                        Qp[j] = (Qp[j] + diff * Q[t][j]) / (1 + diff);
                        p[j] /= (1 + diff);
                    }
                }
            }
            if (iter >= max_iter)
                Console.Write("Exceeds max_iter in multiclass_prob\n");
        }

        // Cross-validation decision values for probability estimates
        private static void SvmBinarySvcProbability(SvmProblem prob, SvmParameters param, double Cp, double Cn, double[] probAB)
        {
            int i;
            int nr_fold = 5;
            int[] perm = new int[prob.l];
            double[] dec_values = new double[prob.l];
            Random rng = new Random();
            // random shuffle
            for (i = 0; i < prob.l; i++) perm[i] = i;
            for (i = 0; i < prob.l; i++)
            {
                int j = i + (int)(rng.NextDouble() * (prob.l - i));
                do { int _ = perm[i]; perm[i] = perm[j]; perm[j] = _; } while (false);
            }
            for (i = 0; i < nr_fold; i++)
            {
                int begin = i * prob.l / nr_fold;
                int end = (i + 1) * prob.l / nr_fold;
                int j, k;
                SvmProblem subprob = new SvmProblem();

                subprob.l = prob.l - (end - begin);
                subprob.x = new SvmNode[subprob.l][];
                subprob.y = new double[subprob.l];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                for (j = end; j < prob.l; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                int p_count = 0, n_count = 0;
                for (j = 0; j < k; j++)
                    if (subprob.y[j] > 0)
                        p_count++;
                    else
                        n_count++;

                if (p_count == 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 0;
                else if (p_count > 0 && n_count == 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = 1;
                else if (p_count == 0 && n_count > 0)
                    for (j = begin; j < end; j++)
                        dec_values[perm[j]] = -1;
                else
                {
                    SvmParameters subparam = (SvmParameters)param.Clone();
                    subparam.probability = 0;
                    subparam.C = 1.0;
                    subparam.nr_weight = 2;
                    subparam.weight_label = new int[2];
                    subparam.weight = new double[2];
                    subparam.weight_label[0] = +1;
                    subparam.weight_label[1] = -1;
                    subparam.weight[0] = Cp;
                    subparam.weight[1] = Cn;
                    SvmModel submodel = SvmTrain(subprob, subparam);
                    for (j = begin; j < end; j++)
                    {
                        double[] dec_value = new double[1];
                        SvmPredictValues(submodel, prob.x[perm[j]], dec_value);
                        dec_values[perm[j]] = dec_value[0];
                        // ensure +1 -1 order; reason not using CV subroutine
                        dec_values[perm[j]] *= submodel.label[0];
                    }
                }
            }
            SigmoidTrain(prob.l, dec_values, prob.y, probAB);
        }

        // Return parameter of a Laplace distribution 
        private static double SvmSvrProbability(SvmProblem prob, SvmParameters param)
        {
            int i;
            int nr_fold = 5;
            double[] ymv = new double[prob.l];
            double mae = 0;

            SvmParameters newparam = (SvmParameters)param.Clone();
            newparam.probability = 0;
            SvmCrossValidation(prob, newparam, nr_fold, ymv);
            for (i = 0; i < prob.l; i++)
            {
                ymv[i] = prob.y[i] - ymv[i];
                mae += Math.Abs(ymv[i]);
            }
            mae /= prob.l;
            double std = Math.Sqrt(2 * mae * mae);
            int count = 0;
            mae = 0;
            for (i = 0; i < prob.l; i++)
                if (Math.Abs(ymv[i]) > 5 * std)
                    count = count + 1;
                else
                    mae += Math.Abs(ymv[i]);
            mae /= (prob.l - count);
            Console.Write("Prob. model for test data: target value = predicted value + z,\nz: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" + mae + Environment.NewLine);
            return mae;
        }

        // label: label name, start: begin of each class, count: #data of classes, perm: indices to the original data
        // perm, Length l, must be allocated before calling this subroutine
        private static void SvmGroupClasses(SvmProblem prob, int[] nr_class_ret, int[][] label_ret, int[][] start_ret, int[][] count_ret, int[] perm)
        {
            int l = prob.l;
            int max_nr_class = 16;
            int nr_class = 0;
            int[] label = new int[max_nr_class];
            int[] count = new int[max_nr_class];
            int[] data_label = new int[l];
            int i;

            for (i = 0; i < l; i++)
            {
                int this_label = (int)(prob.y[i]);
                int j;
                for (j = 0; j < nr_class; j++)
                {
                    if (this_label == label[j])
                    {
                        ++count[j];
                        break;
                    }
                }
                data_label[i] = j;
                if (j == nr_class)
                {
                    if (nr_class == max_nr_class)
                    {
                        max_nr_class *= 2;
                        int[] new_data = new int[max_nr_class];
                        Array.Copy(label, 0, new_data, 0, label.Length);
                        label = new_data;
                        new_data = new int[max_nr_class];
                        Array.Copy(count, 0, new_data, 0, count.Length);
                        count = new_data;
                    }
                    label[nr_class] = this_label;
                    count[nr_class] = 1;
                    ++nr_class;
                }
            }

            int[] start = new int[nr_class];
            start[0] = 0;
            for (i = 1; i < nr_class; i++)
                start[i] = start[i - 1] + count[i - 1];
            for (i = 0; i < l; i++)
            {
                perm[start[data_label[i]]] = i;
                ++start[data_label[i]];
            }
            start[0] = 0;
            for (i = 1; i < nr_class; i++)
                start[i] = start[i - 1] + count[i - 1];

            nr_class_ret[0] = nr_class;
            label_ret[0] = label;
            start_ret[0] = start;
            count_ret[0] = count;
        }

        //
        // Interface functions
        //
        public static SvmModel SvmTrain(SvmProblem prob, SvmParameters param)
        {
            SvmModel model = new SvmModel();
            model.param = param;

            if (param.svm_type == SvmParameters.ONE_CLASS ||
               param.svm_type == SvmParameters.EPSILON_SVR ||
               param.svm_type == SvmParameters.NU_SVR)
            {
                // regression or one-class-svm
                model.nr_class = 2;
                model.label = null;
                model.nSV = null;
                model.probA = null; model.probB = null;
                model.sv_coef = new double[1][];

                if (param.probability == 1 &&
                   (param.svm_type == SvmParameters.EPSILON_SVR ||
                    param.svm_type == SvmParameters.NU_SVR))
                {
                    model.probA = new double[1];
                    model.probA[0] = SvmSvrProbability(prob, param);
                }

                SvmDecisionFunction f = SvmTrainOne(prob, param, 0, 0);
                model.rho = new double[1];
                model.rho[0] = f.rho;

                int nSV = 0;
                int i;
                for (i = 0; i < prob.l; i++)
                    if (Math.Abs(f.alpha[i]) > 0) ++nSV;
                model.l = nSV;
                model.SV = new SvmNode[nSV][];
                model.sv_coef[0] = new double[nSV];
                int j = 0;
                for (i = 0; i < prob.l; i++)
                    if (Math.Abs(f.alpha[i]) > 0)
                    {
                        model.SV[j] = prob.x[i];
                        model.sv_coef[0][j] = f.alpha[i];
                        ++j;
                    }
            }
            else
            {
                // classification
                int l = prob.l;
                int[] tmp_nr_class = new int[1];
                int[][] tmp_label = new int[1][];
                int[][] tmp_start = new int[1][];
                int[][] tmp_count = new int[1][];
                int[] perm = new int[l];

                // group training data of the same class
                SvmGroupClasses(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);
                int nr_class = tmp_nr_class[0];
                int[] label = tmp_label[0];
                int[] start = tmp_start[0];
                int[] count = tmp_count[0];
                SvmNode[][] x = new SvmNode[l][];
                int i;
                for (i = 0; i < l; i++)
                    x[i] = prob.x[perm[i]];

                // calculate weighted C

                double[] weighted_C = new double[nr_class];
                for (i = 0; i < nr_class; i++)
                    weighted_C[i] = param.C;
                for (i = 0; i < param.nr_weight; i++)
                {
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (param.weight_label[i] == label[j])
                            break;
                    if (j == nr_class)
                        Console.Write("warning: class label " + param.weight_label[i] + " specified in weight is not found\n");
                    else
                        weighted_C[j] *= param.weight[i];
                }

                // train k*(k-1)/2 models

                bool[] nonzero = new bool[l];
                for (i = 0; i < l; i++)
                    nonzero[i] = false;
                SvmDecisionFunction[] f = new SvmDecisionFunction[nr_class * (nr_class - 1) / 2];

                double[] probA = null, probB = null;
                if (param.probability == 1)
                {
                    probA = new double[nr_class * (nr_class - 1) / 2];
                    probB = new double[nr_class * (nr_class - 1) / 2];
                }

                int p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        SvmProblem sub_prob = new SvmProblem();
                        int si = start[i], sj = start[j];
                        int ci = count[i], cj = count[j];
                        sub_prob.l = ci + cj;
                        sub_prob.x = new SvmNode[sub_prob.l][];
                        sub_prob.y = new double[sub_prob.l];
                        int k;
                        for (k = 0; k < ci; k++)
                        {
                            sub_prob.x[k] = x[si + k];
                            sub_prob.y[k] = +1;
                        }
                        for (k = 0; k < cj; k++)
                        {
                            sub_prob.x[ci + k] = x[sj + k];
                            sub_prob.y[ci + k] = -1;
                        }

                        if (param.probability == 1)
                        {
                            double[] probAB = new double[2];
                            SvmBinarySvcProbability(sub_prob, param, weighted_C[i], weighted_C[j], probAB);
                            probA[p] = probAB[0];
                            probB[p] = probAB[1];
                        }

                        f[p] = SvmTrainOne(sub_prob, param, weighted_C[i], weighted_C[j]);
                        for (k = 0; k < ci; k++)
                            if (!nonzero[si + k] && Math.Abs(f[p].alpha[k]) > 0)
                                nonzero[si + k] = true;
                        for (k = 0; k < cj; k++)
                            if (!nonzero[sj + k] && Math.Abs(f[p].alpha[ci + k]) > 0)
                                nonzero[sj + k] = true;
                        ++p;
                    }

                // build output

                model.nr_class = nr_class;

                model.label = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    model.label[i] = label[i];

                model.rho = new double[nr_class * (nr_class - 1) / 2];
                for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    model.rho[i] = f[i].rho;

                if (param.probability == 1)
                {
                    model.probA = new double[nr_class * (nr_class - 1) / 2];
                    model.probB = new double[nr_class * (nr_class - 1) / 2];
                    for (i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    {
                        model.probA[i] = probA[i];
                        model.probB[i] = probB[i];
                    }
                }
                else
                {
                    model.probA = null;
                    model.probB = null;
                }

                int nnz = 0;
                int[] nz_count = new int[nr_class];
                model.nSV = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                {
                    int nSV = 0;
                    for (int j = 0; j < count[i]; j++)
                        if (nonzero[start[i] + j])
                        {
                            ++nSV;
                            ++nnz;
                        }
                    model.nSV[i] = nSV;
                    nz_count[i] = nSV;
                }

                Console.Write("Total nSV = " + nnz + Environment.NewLine);

                model.l = nnz;
                model.SV = new SvmNode[nnz][];
                p = 0;
                for (i = 0; i < l; i++)
                    if (nonzero[i]) model.SV[p++] = x[i];

                int[] nz_start = new int[nr_class];
                nz_start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    nz_start[i] = nz_start[i - 1] + nz_count[i - 1];

                model.sv_coef = new double[nr_class - 1][];
                for (i = 0; i < nr_class - 1; i++)
                    model.sv_coef[i] = new double[nnz];

                p = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        // classifier (i,j): coefficients with
                        // i are in sv_coef[j-1][nz_start[i]...],
                        // j are in sv_coef[i][nz_start[j]...]

                        int si = start[i];
                        int sj = start[j];
                        int ci = count[i];
                        int cj = count[j];

                        int q = nz_start[i];
                        int k;
                        for (k = 0; k < ci; k++)
                            if (nonzero[si + k])
                                model.sv_coef[j - 1][q++] = f[p].alpha[k];
                        q = nz_start[j];
                        for (k = 0; k < cj; k++)
                            if (nonzero[sj + k])
                                model.sv_coef[i][q++] = f[p].alpha[ci + k];
                        ++p;
                    }
            }
            return model;
        }

        // Stratified cross validation
        public static void SvmCrossValidation(
            SvmProblem prob, 
            SvmParameters param, 
            int nr_fold, double[] target)
        {
            int i;
            int[] fold_start = new int[nr_fold + 1];
            int l = prob.l;
            int[] perm = new int[l];

            // stratified cv may not give leave-one-out rate
            // Each class to l folds -> some folds may have zero elements
            if ((param.svm_type == SvmParameters.C_SVC ||
                param.svm_type == SvmParameters.NU_SVC) && nr_fold < l)
            {
                int[] tmp_nr_class = new int[1];
                int[][] tmp_label = new int[1][];
                int[][] tmp_start = new int[1][];
                int[][] tmp_count = new int[1][];

                SvmGroupClasses(prob, tmp_nr_class, tmp_label, tmp_start, tmp_count, perm);

                int nr_class = tmp_nr_class[0];
                int[] label = tmp_label[0];
                int[] start = tmp_start[0];
                int[] count = tmp_count[0];
                Random rng = new Random();
                // random shuffle and then data grouped by fold using the array perm
                int[] fold_count = new int[nr_fold];
                int c;
                int[] index = new int[l];
                for (i = 0; i < l; i++)
                    index[i] = perm[i];
                for (c = 0; c < nr_class; c++)
                    for (i = 0; i < count[c]; i++)
                    {
                        int j = i + (int)(rng.NextDouble() * (count[c] - i));
                        do { int _ = index[start[c] + j]; index[start[c] + j] = index[start[c] + i]; index[start[c] + i] = _; } while (false);
                    }
                for (i = 0; i < nr_fold; i++)
                {
                    fold_count[i] = 0;
                    for (c = 0; c < nr_class; c++)
                        fold_count[i] += (i + 1) * count[c] / nr_fold - i * count[c] / nr_fold;
                }
                fold_start[0] = 0;
                for (i = 1; i <= nr_fold; i++)
                    fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
                for (c = 0; c < nr_class; c++)
                    for (i = 0; i < nr_fold; i++)
                    {
                        int begin = start[c] + i * count[c] / nr_fold;
                        int end = start[c] + (i + 1) * count[c] / nr_fold;
                        for (int j = begin; j < end; j++)
                        {
                            perm[fold_start[i]] = index[j];
                            fold_start[i]++;
                        }
                    }
                fold_start[0] = 0;
                for (i = 1; i <= nr_fold; i++)
                    fold_start[i] = fold_start[i - 1] + fold_count[i - 1];
            }
            else
            {
                Random rng = new Random();
                for (i = 0; i < l; i++) perm[i] = i;
                for (i = 0; i < l; i++)
                {
                    int j = i + (int)(rng.NextDouble() * (l - i));
                    do { int _ = perm[i]; perm[i] = perm[j]; perm[j] = _; } while (false);
                }
                for (i = 0; i <= nr_fold; i++)
                    fold_start[i] = i * l / nr_fold;
            }

            for (i = 0; i < nr_fold; i++)
            {
                int begin = fold_start[i];
                int end = fold_start[i + 1];
                int j, k;
                SvmProblem subprob = new SvmProblem();

                subprob.l = l - (end - begin);
                subprob.x = new SvmNode[subprob.l][];
                subprob.y = new double[subprob.l];

                k = 0;
                for (j = 0; j < begin; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                for (j = end; j < l; j++)
                {
                    subprob.x[k] = prob.x[perm[j]];
                    subprob.y[k] = prob.y[perm[j]];
                    ++k;
                }
                SvmModel submodel = SvmTrain(subprob, param);
                if (param.probability == 1 &&
                   (param.svm_type == SvmParameters.C_SVC ||
                    param.svm_type == SvmParameters.NU_SVC))
                {
                    double[] prob_estimates = new double[SvmGetNrClass(submodel)];
                    for (j = begin; j < end; j++)
                        target[perm[j]] = SvmPredictProbability(submodel, prob.x[perm[j]], prob_estimates);
                }
                else
                    for (j = begin; j < end; j++)
                        target[perm[j]] = SvmPredict(submodel, prob.x[perm[j]]);
            }
        }

        public static int SvmGetSvmType(SvmModel model)
        {
            return model.param.svm_type;
        }

        public static int SvmGetNrClass(SvmModel model)
        {
            return model.nr_class;
        }

        public static void SvmGetLabels(SvmModel model, int[] label)
        {
            if (model.label != null)
                for (int i = 0; i < model.nr_class; i++)
                    label[i] = model.label[i];
        }

        public static double SvmGetSvrProbability(SvmModel model)
        {
            if ((model.param.svm_type == SvmParameters.EPSILON_SVR || model.param.svm_type == SvmParameters.NU_SVR) &&
                model.probA != null)
                return model.probA[0];
            else
            {
                Console.Write("Model doesn't contain information for SVR probability inference\n");
                return 0;
            }
        }

        public static void SvmPredictValues(SvmModel model, SvmNode[] x, double[] dec_values)
        {
            if (model.param.svm_type == SvmParameters.ONE_CLASS ||
               model.param.svm_type == SvmParameters.EPSILON_SVR ||
               model.param.svm_type == SvmParameters.NU_SVR)
            {
                double[] sv_coef = model.sv_coef[0];
                double sum = 0;
                for (int i = 0; i < model.l; i++)
                    sum += sv_coef[i] * AbstractKernel.KFunction(x, model.SV[i], model.param);
                sum -= model.rho[0];
                dec_values[0] = sum;
            }
            else
            {
                int i;
                int nr_class = model.nr_class;
                int l = model.l;

                double[] kvalue = new double[l];
                for (i = 0; i < l; i++)
                    kvalue[i] = AbstractKernel.KFunction(x, model.SV[i], model.param);

                int[] start = new int[nr_class];
                start[0] = 0;
                for (i = 1; i < nr_class; i++)
                    start[i] = start[i - 1] + model.nSV[i - 1];

                int p = 0;
                int pos = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        double sum = 0;
                        int si = start[i];
                        int sj = start[j];
                        int ci = model.nSV[i];
                        int cj = model.nSV[j];

                        int k;
                        double[] coef1 = model.sv_coef[j - 1];
                        double[] coef2 = model.sv_coef[i];
                        for (k = 0; k < ci; k++)
                            sum += coef1[si + k] * kvalue[si + k];
                        for (k = 0; k < cj; k++)
                            sum += coef2[sj + k] * kvalue[sj + k];
                        sum -= model.rho[p++];
                        dec_values[pos++] = sum;
                    }
            }
        }

        public static double SvmPredict(SvmModel model, SvmNode[] x)
        {
            if (model.param.svm_type == SvmParameters.ONE_CLASS ||
               model.param.svm_type == SvmParameters.EPSILON_SVR ||
               model.param.svm_type == SvmParameters.NU_SVR)
            {
                double[] res = new double[1];
                SvmPredictValues(model, x, res);

                if (model.param.svm_type == SvmParameters.ONE_CLASS)
                    return (res[0] > 0) ? 1 : -1;
                else
                    return res[0];
            }
            else
            {
                int i;
                int nr_class = model.nr_class;
                double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
                SvmPredictValues(model, x, dec_values);

                int[] vote = new int[nr_class];
                for (i = 0; i < nr_class; i++)
                    vote[i] = 0;
                int pos = 0;
                for (i = 0; i < nr_class; i++)
                {
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        if (dec_values[pos++] > 0)
                            ++vote[i];
                        else
                            ++vote[j];
                    }
                }
                int vote_max_idx = 0;
                for (i = 1; i < nr_class; i++)
                {
                    if (vote[i] > vote[vote_max_idx])
                        vote_max_idx = i;
                }
                return model.label[vote_max_idx];
            }
        }

        public static double SvmPredictProbability(
            SvmModel model,
            SvmNode[] x,
            double[] prob_estimates)
        {
            if ((model.param.svm_type == SvmParameters.C_SVC ||
                model.param.svm_type == SvmParameters.NU_SVC) &&
                model.probA != null && model.probB != null)
            {
                int i;
                int nr_class = model.nr_class;
                double[] dec_values = new double[nr_class * (nr_class - 1) / 2];
                SvmPredictValues(model, x, dec_values);

                double min_prob = 1e-7;
                double[][] pairwise_prob = new double[nr_class][];
                for (int intIndex = 0; intIndex < nr_class; intIndex++)
                {
                    pairwise_prob[intIndex] = new double[nr_class];
                }

                int k = 0;
                for (i = 0; i < nr_class; i++)
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        pairwise_prob[i][j] = Math.Min(Math.Max(
                            SigmoidPredict(
                                dec_values[k],
                                model.probA[k],
                                model.probB[k]),
                                min_prob),
                                1 - min_prob);
                        pairwise_prob[j][i] = 1 - pairwise_prob[i][j];
                        k++;
                    }
                MulticlassProbability(nr_class, pairwise_prob, prob_estimates);

                int prob_max_idx = 0;
                for (i = 1; i < nr_class; i++)
                    if (prob_estimates[i] > prob_estimates[prob_max_idx])
                        prob_max_idx = i;
                return model.label[prob_max_idx];
            }
            else
                return SvmPredict(model, x);
        }

        static string[] svm_type_table =
	{
		"c_svc","nu_svc","one_class","epsilon_svr","nu_svr",
	};

        static string[] kernel_type_table =
	{
		"linear","polynomial","rbf","sigmoid",
	};

        public static void SvmSaveModel(string model_file_name, SvmModel model)
        {

            StreamWriter fp = new StreamWriter(model_file_name);

            SvmParameters param = model.param;

            fp.Write("svm_type " + svm_type_table[param.svm_type] + Environment.NewLine);
            fp.Write("kernel_type " + kernel_type_table[param.kernel_type] + Environment.NewLine);

            if (param.kernel_type == SvmParameters.POLY)
                fp.Write("degree " + param.degree + Environment.NewLine);

            if (param.kernel_type == SvmParameters.POLY ||
               param.kernel_type == SvmParameters.RBF ||
               param.kernel_type == SvmParameters.SIGMOID)
                fp.Write("gamma " + param.gamma + Environment.NewLine);

            if (param.kernel_type == SvmParameters.POLY ||
               param.kernel_type == SvmParameters.SIGMOID)
                fp.Write("coef0 " + param.coef0 + Environment.NewLine);

            int nr_class = model.nr_class;
            int l = model.l;
            fp.Write("nr_class " + nr_class + Environment.NewLine);
            fp.Write("total_sv " + l + Environment.NewLine);

            {
                fp.Write("rho");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.rho[i]);
                fp.Write(Environment.NewLine);
            }

            if (model.label != null)
            {
                fp.Write("label");
                for (int i = 0; i < nr_class; i++)
                    fp.Write(" " + model.label[i]);
                fp.Write(Environment.NewLine);
            }

            if (model.probA != null) // regression has probA only
            {
                fp.Write("probA");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.probA[i]);
                fp.Write(Environment.NewLine);
            }
            if (model.probB != null)
            {
                fp.Write("probB");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    fp.Write(" " + model.probB[i]);
                fp.Write(Environment.NewLine);
            }

            if (model.nSV != null)
            {
                fp.Write("nr_sv");
                for (int i = 0; i < nr_class; i++)
                    fp.Write(" " + model.nSV[i]);
                fp.Write(Environment.NewLine);
            }

            fp.Write("SV\n");
            double[][] sv_coef = model.sv_coef;
            SvmNode[][] SV = model.SV;

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < nr_class - 1; j++)
                    fp.Write(sv_coef[j][i] + " ");

                SvmNode[] p = SV[i];
                for (int j = 0; j < p.Length; j++)
                    fp.Write(p[j].index + ":" + p[j].value + " ");
                fp.Write(Environment.NewLine);
            }

            fp.Close();
        }

        private static double Atof(string s)
        {
            return double.Parse(s);
        }

        private static int Atoi(string s)
        {
            return int.Parse(s);
        }

        public static SvmModel SvmLoadModel(string model_file_name)
        {

            StreamReader fp = new StreamReader(model_file_name);

            // read parameters

            SvmModel model = new SvmModel();
            SvmParameters param = new SvmParameters();
            model.param = param;
            model.rho = null;
            model.probA = null;
            model.probB = null;
            model.label = null;
            model.nSV = null;

            while (true)
            {
                string cmd = fp.ReadLine();
                string arg = cmd.Substring(cmd.IndexOf(' ') + 1);

                if (cmd.StartsWith("svm_type"))
                {
                    int i;
                    for (i = 0; i < svm_type_table.Length; i++)
                    {
                        if (arg.IndexOf(svm_type_table[i]) != -1)
                        {
                            param.svm_type = i;
                            break;
                        }
                    }
                    if (i == svm_type_table.Length)
                    {
                        Console.Write("unknown svm type.\n");
                        return null;
                    }
                }
                else if (cmd.StartsWith("kernel_type"))
                {
                    int i;
                    for (i = 0; i < kernel_type_table.Length; i++)
                    {
                        if (arg.IndexOf(kernel_type_table[i]) != -1)
                        {
                            param.kernel_type = i;
                            break;
                        }
                    }
                    if (i == kernel_type_table.Length)
                    {
                        Console.Write("unknown kernel function.\n");
                        return null;
                    }
                }
                else if (cmd.StartsWith("degree"))
                    param.degree = Atof(arg);
                else if (cmd.StartsWith("gamma"))
                    param.gamma = Atof(arg);
                else if (cmd.StartsWith("coef0"))
                    param.coef0 = Atof(arg);
                else if (cmd.StartsWith("nr_class"))
                    model.nr_class = Atoi(arg);
                else if (cmd.StartsWith("total_sv"))
                    model.l = Atoi(arg);
                else if (cmd.StartsWith("rho"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.rho = new double[n];
                    string[] st = arg.Split(' ');

                    for (int i = 0; i < n; i++)
                        model.rho[i] = Atof(st[i]);
                }
                else if (cmd.StartsWith("label"))
                {
                    int n = model.nr_class;
                    model.label = new int[n];
                    string[] st = arg.Split(' ');
                    for (int i = 0; i < n; i++)
                        model.label[i] = Atoi(st[i]);
                }
                else if (cmd.StartsWith("probA"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probA = new double[n];
                    string[] st = arg.Split(' ');
                    for (int i = 0; i < n; i++)
                        model.probA[i] = Atof(st[i]);
                }
                else if (cmd.StartsWith("probB"))
                {
                    int n = model.nr_class * (model.nr_class - 1) / 2;
                    model.probB = new double[n];
                    string[] st = arg.Split(' ');
                    for (int i = 0; i < n; i++)
                        model.probB[i] = Atof(st[i]);
                }
                else if (cmd.StartsWith("nr_sv"))
                {
                    int n = model.nr_class;
                    model.nSV = new int[n];
                    string[] st = arg.Split(' ');
                    for (int i = 0; i < n; i++)
                        model.nSV[i] = Atoi(st[i]);
                }
                else if (cmd.StartsWith("SV"))
                {
                    break;
                }
                else
                {
                    Console.Write("unknown text in model file\n");
                    return null;
                }
            }

            // read sv_coef and SV

            int m = model.nr_class - 1;
            int l = model.l;
            model.sv_coef = new double[m][];
            for (int i = 0; i < m; i++)
            {
                model.sv_coef[i] = new double[l];
            }

            model.SV = new SvmNode[l][];

            for (int i = 0; i < l; i++)
            {
                string line = fp.ReadLine();
                string[] st = line.Split(" \t\n\r\f:".ToCharArray());

                for (int k = 0; k < m; k++)
                    model.sv_coef[k][i] = Atof(st[k]);

                int n = (st.Length - m) / 2;
                model.SV[i] = new SvmNode[n];
                int intIndex = m - 1;
                for (int j = 0; j < n; j++)
                {
                    model.SV[i][j] = new SvmNode();
                    intIndex++;
                    model.SV[i][j].index = Atoi(st[intIndex]);
                    intIndex++;
                    model.SV[i][j].value = Atof(st[intIndex]);
                }
            }

            fp.Close();
            return model;
        }

        public static string SvmCheckParameter(SvmProblem prob, SvmParameters param)
        {
            // svm_type

            int svm_type = param.svm_type;
            if (svm_type != SvmParameters.C_SVC &&
               svm_type != SvmParameters.NU_SVC &&
               svm_type != SvmParameters.ONE_CLASS &&
               svm_type != SvmParameters.EPSILON_SVR &&
               svm_type != SvmParameters.NU_SVR)
                return "unknown svm type";

            // kernel_type

            int kernel_type = param.kernel_type;
            if (kernel_type != SvmParameters.LINEAR &&
               kernel_type != SvmParameters.POLY &&
               kernel_type != SvmParameters.RBF &&
               kernel_type != SvmParameters.SIGMOID)
                return "unknown kernel type";

            // cache_size,eps,C,nu,p,shrinking

            if (param.cache_size <= 0)
                return "cache_size <= 0";

            if (param.eps <= 0)
                return "eps <= 0";

            if (svm_type == SvmParameters.C_SVC ||
               svm_type == SvmParameters.EPSILON_SVR ||
               svm_type == SvmParameters.NU_SVR)
                if (param.C <= 0)
                    return "C <= 0";

            if (svm_type == SvmParameters.NU_SVC ||
               svm_type == SvmParameters.ONE_CLASS ||
               svm_type == SvmParameters.NU_SVR)
                if (param.nu < 0 || param.nu > 1)
                    return "nu < 0 or nu > 1";

            if (svm_type == SvmParameters.EPSILON_SVR)
                if (param.p < 0)
                    return "p < 0";

            if (param.shrinking != 0 &&
               param.shrinking != 1)
                return "shrinking != 0 and shrinking != 1";

            if (param.probability != 0 &&
               param.probability != 1)
                return "probability != 0 and probability != 1";

            if (param.probability == 1 &&
               svm_type == SvmParameters.ONE_CLASS)
                return "one-class SVM probability output not supported yet";

            // check whether nu-svc is feasible

            if (svm_type == SvmParameters.NU_SVC)
            {
                int l = prob.l;
                int max_nr_class = 16;
                int nr_class = 0;
                int[] label = new int[max_nr_class];
                int[] count = new int[max_nr_class];

                int i;
                for (i = 0; i < l; i++)
                {
                    int this_label = (int)prob.y[i];
                    int j;
                    for (j = 0; j < nr_class; j++)
                        if (this_label == label[j])
                        {
                            ++count[j];
                            break;
                        }

                    if (j == nr_class)
                    {
                        if (nr_class == max_nr_class)
                        {
                            max_nr_class *= 2;
                            int[] new_data = new int[max_nr_class];
                            Array.Copy(label, 0, new_data, 0, label.Length);
                            label = new_data;

                            new_data = new int[max_nr_class];
                            Array.Copy(count, 0, new_data, 0, count.Length);
                            count = new_data;
                        }
                        label[nr_class] = this_label;
                        count[nr_class] = 1;
                        ++nr_class;
                    }
                }

                for (i = 0; i < nr_class; i++)
                {
                    int n1 = count[i];
                    for (int j = i + 1; j < nr_class; j++)
                    {
                        int n2 = count[j];
                        if (param.nu * (n1 + n2) / 2 > Math.Min(n1, n2))
                            return "specified nu is infeasible";
                    }
                }
            }

            return null;
        }

        public static int SvmCheckProbabilityModel(SvmModel model)
        {
            if (((model.param.svm_type == SvmParameters.C_SVC || model.param.svm_type == SvmParameters.NU_SVC) &&
            model.probA != null && model.probB != null) ||
            ((model.param.svm_type == SvmParameters.EPSILON_SVR || model.param.svm_type == SvmParameters.NU_SVR) &&
             model.probA != null))
                return 1;
            else
                return 0;
        }

    }
}

