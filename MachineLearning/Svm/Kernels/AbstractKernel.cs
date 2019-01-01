using System;

namespace HC.Analytics.MachineLearning.Svm.Kernels
{
    public abstract class AbstractKernel : AbstractQMatrix
    {
        private SvmNode[][] x;
        private double[] x_square;

        // svm_parameter
        private int kernel_type;
        private double degree;
        private double gamma;
        private double coef0;


        public override void SwapIndex(int i, int j)
        {
            do { SvmNode[] _ = x[i]; x[i] = x[j]; x[j] = _; } while (false);
            if (x_square != null) do { double _ = x_square[i]; x_square[i] = x_square[j]; x_square[j] = _; } while (false);
        }

        private static double Tanh(double x)
        {
            double e = Math.Exp(x);
            return 1.0 - 2.0 / (e * e + 1);
        }

        public double KernelFunction(int i, int j)
        {
            switch (kernel_type)
            {
                case SvmParameters.LINEAR:
                    return Dot(x[i], x[j]);

                case SvmParameters.POLY:
                    return Math.Pow(gamma * Dot(x[i], x[j]) + coef0, degree);

                case SvmParameters.RBF:
                    return Math.Exp(-gamma * (x_square[i] + x_square[j] - 2 * Dot(x[i], x[j])));

                case SvmParameters.SIGMOID:
                    return Tanh(gamma * Dot(x[i], x[j]) + coef0);

                default:
                    return 0;	
            }
        }

        public AbstractKernel(int l, SvmNode[][] x_, SvmParameters param)
        {
            this.kernel_type = param.kernel_type;
            this.degree = param.degree;
            this.gamma = param.gamma;
            this.coef0 = param.coef0;

            x = (SvmNode[][])x_.Clone();

            if (kernel_type == SvmParameters.RBF)
            {
                x_square = new double[l];
                for (int i = 0; i < l; i++)
                    x_square[i] = Dot(x[i], x[i]);
            }
            else x_square = null;
        }

        static double Dot(SvmNode[] x, SvmNode[] y)
        {
            double sum = 0;
            int xlen = x.Length;
            int ylen = y.Length;
            int i = 0;
            int j = 0;
            while (i < xlen && j < ylen)
            {
                if (x[i].index == y[j].index)
                    sum += x[i++].value * y[j++].value;
                else
                {
                    if (x[i].index > y[j].index)
                        ++j;
                    else
                        ++i;
                }
            }
            return sum;
        }

        public static double KFunction(SvmNode[] x, SvmNode[] y,
                        SvmParameters param)
        {
            switch (param.kernel_type)
            {
                case SvmParameters.LINEAR:
                    return Dot(x, y);
                case SvmParameters.POLY:
                    return Math.Pow(param.gamma * Dot(x, y) + param.coef0, param.degree);
                case SvmParameters.RBF:
                    {
                        double sum = 0;
                        int xlen = x.Length;
                        int ylen = y.Length;
                        int i = 0;
                        int j = 0;
                        while (i < xlen && j < ylen)
                        {
                            if (x[i].index == y[j].index)
                            {
                                double d = x[i++].value - y[j++].value;
                                sum += d * d;
                            }
                            else if (x[i].index > y[j].index)
                            {
                                sum += y[j].value * y[j].value;
                                ++j;
                            }
                            else
                            {
                                sum += x[i].value * x[i].value;
                                ++i;
                            }
                        }

                        while (i < xlen)
                        {
                            sum += x[i].value * x[i].value;
                            ++i;
                        }

                        while (j < ylen)
                        {
                            sum += y[j].value * y[j].value;
                            ++j;
                        }

                        return Math.Exp(-param.gamma * sum);
                    }
                case SvmParameters.SIGMOID:
                    return Tanh(param.gamma * Dot(x, y) + param.coef0);
                default:
                    return 0;	// java
            }
        }
    }
}

