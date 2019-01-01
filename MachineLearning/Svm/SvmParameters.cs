//package libsvm;
using System;

namespace HC.Analytics.MachineLearning.Svm
{

    public class SvmParameters
    {
        /* svm_type */
        public const int C_SVC = 0;
        public const int NU_SVC = 1;
        public const int ONE_CLASS = 2;
        public const int EPSILON_SVR = 3;
        public const int NU_SVR = 4;

        /* kernel_type */
        public const int LINEAR = 0;
        public const int POLY = 1;
        public const int RBF = 2;
        public const int SIGMOID = 3;

        public int svm_type;
        public int kernel_type;
        public double degree;	// for poly
        public double gamma;	// for poly/rbf/sigmoid
        public double coef0;	// for poly/sigmoid

        // these are for training only
        public double cache_size; // in MB
        public double eps;	// stopping criteria
        public double C;	// for C_SVC, EPSILON_SVR and NU_SVR
        public int nr_weight;		// for C_SVC
        public int[] weight_label;	// for C_SVC
        public double[] weight;		// for C_SVC
        public double nu;	// for NU_SVC, ONE_CLASS, and NU_SVR
        public double p;	// for EPSILON_SVR
        public int shrinking;	// use the shrinking heuristics
        public int probability; // do probability estimates

        public Object Clone()
        {
            try
            {
                throw new NotImplementedException();

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private object NotImplementedException()
        {
            throw new NotImplementedException();
        }

        public string ToStringParams()
        {
            return
            "cache_size = " + cache_size + Environment.NewLine +
            "eps = " + eps + Environment.NewLine +
            "C = " + C + Environment.NewLine +
            "nr_weight = " + nr_weight + Environment.NewLine +
            "nu = " + nu + Environment.NewLine +
            "p = " + p + Environment.NewLine +
            "shrinking = " + shrinking + Environment.NewLine +
            "probability = " + probability;
        }

    }
}
