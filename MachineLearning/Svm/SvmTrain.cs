//import libsvm.*;
//import java.io.*;
//import java.util.*;

using System;
using System.Collections;
using System.IO;

namespace HC.Analytics.MachineLearning.Svm
{
    public class SvmTrain
    {
        #region Members

        private SvmParameters param;		// set by parse_command_line
        private SvmProblem prob;		// set by read_problem
        private SvmModel model;
        private string input_file_name;		// set by parse_command_line
        private string model_file_name;		// set by parse_command_line
        private string error_msg;
        private int cross_validation;
        private int nr_fold;

        #endregion

        private static void ExitWithHelp()
        {
            Console.Write(
                "Usage: svm_train [options] training_set_file [model_file]\n"
                + "options:\n"
                + "-s svm_type : set type of SVM (default 0)\n"
                + "	0 -- C-SVC\n"
                + "	1 -- nu-SVC\n"
                + "	2 -- one-class SVM\n"
                + "	3 -- epsilon-SVR\n"
                + "	4 -- nu-SVR\n"
                + "-t kernel_type : set type of kernel function (default 2)\n"
                + "	0 -- linear: u'*v\n"
                + "	1 -- polynomial: (gamma*u'*v + coef0)^degree\n"
                + "	2 -- radial basis function: exp(-gamma*|u-v|^2)\n"
                + "	3 -- sigmoid: tanh(gamma*u'*v + coef0)\n"
                + "-d degree : set degree in kernel function (default 3)\n"
                + "-g gamma : set gamma in kernel function (default 1/k)\n"
                + "-r coef0 : set coef0 in kernel function (default 0)\n"
                + "-c cost : set the parameter C of C-SVC, epsilon-SVR, and nu-SVR (default 1)\n"
                + "-n nu : set the parameter nu of nu-SVC, one-class SVM, and nu-SVR (default 0.5)\n"
                + "-p epsilon : set the epsilon in loss function of epsilon-SVR (default 0.1)\n"
                + "-m cachesize : set cache memory size in MB (default 40)\n"
                + "-e epsilon : set tolerance of termination criterion (default 0.001)\n"
                + "-h shrinking: whether to use the shrinking heuristics, 0 or 1 (default 1)\n"
                + "-b probability_estimates: whether to train a SVC or SVR model for probability estimates, 0 or 1 (default 0)\n"
                + "-wi weight: set the parameter C of class i to weight*C, for C-SVC (default 1)\n"
                + "-v n: n-fold cross validation mode\n"
            );
        }

        private void DoCrossValidation()
        {
            int i;
            int total_correct = 0;
            double total_error = 0;
            double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;
            double[] target = new double[prob.l];

            SvmClass.SvmCrossValidation(prob, param, nr_fold, target);
            if (param.svm_type == SvmParameters.EPSILON_SVR ||
                param.svm_type == SvmParameters.NU_SVR)
            {
                for (i = 0; i < prob.l; i++)
                {
                    double y = prob.y[i];
                    double v = target[i];
                    total_error += (v - y) * (v - y);
                    sumv += v;
                    sumy += y;
                    sumvv += v * v;
                    sumyy += y * y;
                    sumvy += v * y;
                }
                Console.Write("Cross Validation Mean squared error = " + total_error / prob.l + Environment.NewLine);
                Console.Write("Cross Validation Squared correlation coefficient = " +
                              ((prob.l * sumvy - sumv * sumy) * (prob.l * sumvy - sumv * sumy)) /
                              ((prob.l * sumvv - sumv * sumv) * (prob.l * sumyy - sumy * sumy)) + Environment.NewLine
                );
            }
            else
                for (i = 0; i < prob.l; i++)
                    if (target[i] == prob.y[i])
                        ++total_correct;
            Console.Write("Cross Validation Accuracy = " + 100.0 * total_correct / prob.l + "%\n");
        }

        private void Run(string[] argv)
        {
            ParseCommandLine(argv);
            ReadProblem();
            error_msg = SvmClass.SvmCheckParameter(prob, param);

            if (error_msg != null)
            {
                Console.Write("Error: " + error_msg + Environment.NewLine);
            }
            if (cross_validation != 0)
            {
                DoCrossValidation();
            }
            else
            {
                model = SvmClass.SvmTrain(prob, param);
                SvmClass.SvmSaveModel(model_file_name, model);
            }
        }

        public static void Main(string[] argv)
        {
            SvmTrain t = new SvmTrain();
            t.Run(argv);
        }

        private static double Atof(string s)
        {
            return double.Parse(s);
        }

        private static int Atoi(string s)
        {
            return int.Parse(s);
        }

        private void ParseCommandLine(string[] argv)
        {
            int i;

            param = new SvmParameters();
            // default values
            param.svm_type = SvmParameters.C_SVC;
            param.kernel_type = SvmParameters.RBF;
            param.degree = 3;
            param.gamma = 0;	// 1/k
            param.coef0 = 0;
            param.nu = 0.5;
            param.cache_size = 40;
            param.C = 1;
            param.eps = 1e-3;
            param.p = 0.1;
            param.shrinking = 1;
            param.probability = 0;
            param.nr_weight = 0;
            param.weight_label = new int[0];
            param.weight = new double[0];
            cross_validation = 0;

            // parse options
            for (i = 0; i < argv.Length; i++)
            {
                if (argv[i][0] != '-') break;
                if (++i >= argv.Length)
                    ExitWithHelp();
                switch (argv[i - 1][1])
                {
                    case 's':
                        param.svm_type = Atoi(argv[i]);
                        break;
                    case 't':
                        param.kernel_type = Atoi(argv[i]);
                        break;
                    case 'd':
                        param.degree = Atof(argv[i]);
                        break;
                    case 'g':
                        param.gamma = Atof(argv[i]);
                        break;
                    case 'r':
                        param.coef0 = Atof(argv[i]);
                        break;
                    case 'n':
                        param.nu = Atof(argv[i]);
                        break;
                    case 'm':
                        param.cache_size = Atof(argv[i]);
                        break;
                    case 'c':
                        param.C = Atof(argv[i]);
                        break;
                    case 'e':
                        param.eps = Atof(argv[i]);
                        break;
                    case 'p':
                        param.p = Atof(argv[i]);
                        break;
                    case 'h':
                        param.shrinking = Atoi(argv[i]);
                        break;
                    case 'b':
                        param.probability = Atoi(argv[i]);
                        break;
                    case 'v':
                        cross_validation = 1;
                        nr_fold = Atoi(argv[i]);
                        if (nr_fold < 2)
                        {
                            Console.Write("n-fold cross validation: n must >= 2\n");
                            ExitWithHelp();
                        }
                        break;
                    case 'w':
                        ++param.nr_weight;
                    {
                        int[] old = param.weight_label;
                        param.weight_label = new int[param.nr_weight];
                        Array.Copy(old, 0, param.weight_label, 0, param.nr_weight - 1);
                    }
                    {
                        double[] old = param.weight;
                        param.weight = new double[param.nr_weight];
                        Array.Copy(old, 0, param.weight, 0, param.nr_weight - 1);
                    }

                        param.weight_label[param.nr_weight - 1] = Atoi(argv[i - 1].Substring(2));
                        param.weight[param.nr_weight - 1] = Atof(argv[i]);
                        break;
                    default:
                        Console.Write("unknown option\n");
                        ExitWithHelp();
                        break;
                }
            }

            // determine filenames

            if (i >= argv.Length)
                ExitWithHelp();

            input_file_name = argv[i];

            if (i < argv.Length - 1)
                model_file_name = argv[i + 1];
            else
            {
                int p = argv[i].LastIndexOf('/');
                ++p;	// whew...
                model_file_name = argv[i].Substring(p) + ".model";
            }
        }

        // read in a problem (in svmlight format)

        private void ReadProblem()
        {

            StreamReader fp = new StreamReader(input_file_name);

            ArrayList vy = new ArrayList();
            ArrayList vx = new ArrayList();
            int max_index = 0;

            while (true)
            {
                string line = fp.ReadLine();
                if (line == null) break;

                string[] st = line.Split(" \t\n\r\f:".ToCharArray());

                vy.Add(st[0]);
                int m = (st.Length - 1) / 2;
                SvmNode[] x = new SvmNode[m];
                int intTokenIndex = 0;
                for (int j = 0; j < m; j++)
                {
                    x[j] = new SvmNode();
                    intTokenIndex++;
                    x[j].index = Atoi(st[intTokenIndex]);
                    intTokenIndex++;
                    x[j].value = Atof(st[intTokenIndex]);
                }
                if (m > 0) max_index = Math.Max(max_index, x[m - 1].index);
                vx.Add(x);
            }

            prob = new SvmProblem();
            prob.l = vy.Count;
            prob.x = new SvmNode[prob.l][];
            for (int i = 0; i < prob.l; i++)
                prob.x[i] = (SvmNode[])vx[i];
            prob.y = new double[prob.l];
            for (int i = 0; i < prob.l; i++)
                prob.y[i] = Atof((string)vy[i]);

            if (param.gamma == 0)
                param.gamma = 1.0 / max_index;

            fp.Close();
        }
    }
}

