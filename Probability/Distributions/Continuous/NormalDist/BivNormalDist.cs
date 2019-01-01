#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous.NormalDist
{
    public class BivNormalDist : MultNormalDist
    {
        #region Constructors

        public BivNormalDist(
            double[] dblMeanArr,
            double[,] dblCovArr,
            RngWrapper rng)
            : base(
                dblMeanArr,
                dblCovArr,
                rng)
        {
        }

        public BivNormalDist(
            Vector meanVector,
            MatrixClass covMatrix,
            RngWrapper rng)
            : base(
                meanVector,
                covMatrix,
                rng)
        {
        }

        #endregion

        #region Constants

        private const int INT_RND_SEED = 23;

        #endregion

        #region Public

        /**
          *  A function for computing bivariate normal probabilities. <BR>
          *
          *
          *@param  x      upper limit
          *@param  sigma  covariate matrix
          *@return        double result
          *@see           #Cdf(double,double,double)
          */

        public override double Cdf(double[] dblXArr)
        {
            if (!(dblXArr.Length == 2 &&
                  CovMatrix.GetColumnDimension() == 2 &&
                  CovMatrix.GetRowDimension() == 2))
            {
                throw new ArgumentException("X and mu have to be double[2], and sigma has to be double[2,2]");
            }
            if (CovMatrix.Get(0, 1) != CovMatrix.Get(1, 0))
            {
                throw new ArgumentException("sigma must be symetric");
            }
            // bring x array to zero-mean
            double[] dblXMeanZeroArr = new double[2];
            dblXMeanZeroArr[0] = dblXArr[0] - MeanVector.Get(0);
            dblXMeanZeroArr[0] = dblXArr[1] - MeanVector.Get(1);

            return Cdf(dblXMeanZeroArr[0]/Math.Sqrt(CovMatrix.Get(0, 0)),
                       dblXMeanZeroArr[1]/Math.Sqrt(CovMatrix.Get(1, 1)),
                       CovMatrix.Get(0, 1)/Math.Sqrt(CovMatrix.Get(0, 0)*CovMatrix.Get(1, 1)));
        }


        /**
         *  A function for computing bivariate normal probabilities. <BR>
         *
         *
         *@param  lower  lower limits of integrations
         *@param  upper  upper limits of integration
         *@param  sigma  covariance matrix
         *@return        probability
         *@see           #Cdf(double,double,double)
         */

        public override double Cdf(
            double[] dblLowerLimitArr,
            double[] dblUpperLimitArr)
        {
            if (dblLowerLimitArr.Length != 2 || dblUpperLimitArr.Length != 2 || CovMatrix.GetColumnDimension() == 2 ||
                CovMatrix.GetRowDimension() == 2)
            {
                throw new ArgumentException("all matrix are not length of 2");
            }

            // bring c array to zero-mean
            double[] c = new double[2];
            c[0] = dblLowerLimitArr[0] - MeanVector.Get(0);
            c[0] = dblLowerLimitArr[1] - MeanVector.Get(1);

            // bring d array to zero-mean
            double[] d = new double[2];
            d[0] = dblUpperLimitArr[0] - MeanVector.Get(0);
            d[0] = dblUpperLimitArr[1] - MeanVector.Get(1);


            double[] sd = {
                              Math.Sqrt(CovMatrix.Get(0, 0)),
                              Math.Sqrt(CovMatrix.Get(1, 1))
                          };
            for (int i = 0; i < 2; i++)
            {
                c[i] /= sd[i];
                d[i] /= sd[i];
            }
            return Cdf(c, d, CovMatrix.Get(1, 0)/(sd[0]*sd[1]));
        }

        /**
             * Pdf of bivariate normal
             * @return value of pdf
             * @param x  x={fist dimension, second dimension}
             * @param cov covariance matrix
             */

        public override double Pdf(
            double[] dblXArr)
        {
            double[] sd = {Math.Sqrt(CovMatrix.Get(0, 0)), Math.Sqrt(CovMatrix.Get(1, 1))};
            return Pdf(
                (dblXArr[0] - MeanVector.Get(0))/sd[0], (dblXArr[1] - MeanVector.Get(1))/sd[1],
                CovMatrix.Get(0, 1)/(sd[1]*sd[0]));
        }

        #endregion

        #region Private

        /**
         *  A function for computing bivariate normal probabilities. <BR>
         *
         *
         *@param  lower  lower limit of integration
         *@param  upper  upper limit of integration
         *@param  rho    correlation coeffitient
         *@return        probability
         *@author:       <A href="http://www.kutsyy.com>Vadum Kutsyy <\A>
         *@see           #Cdf(double,double,double)
         */
        // note the input to this method has been normalized
        private double Cdf(double[] lower, double[] upper, double rho)
        {
            if (lower.Length == 1 && upper.Length == 1)
            {
                return m_univNormalDistStd.Cdf(
                    lower[0],
                    upper[0]);
            }
            if (lower.Length != 2 || upper.Length != 2)
            {
                throw new ArgumentException("all matrix are not length of 2");
            }
            if (lower[0] == upper[0] || lower[1] == upper[1])
            {
                return 0;
            }
            if (lower[0] > upper[0] || lower[1] > upper[1])
            {
                throw new ArgumentException("lower limit bigger than upper");
            }
            int[] inf = {
                            3, 3
                        };
            for (int i = 0; i < 2; i++)
            {
                if (lower[i] == Double.NegativeInfinity)
                {
                    if (upper[i] != Double.PositiveInfinity)
                    {
                        inf[i] = 0;
                    }
                }
                else if (upper[i] == Double.PositiveInfinity)
                {
                    inf[i] = 1;
                }
                else
                {
                    inf[i] = 2;
                }
            }
            switch (inf[0])
            {
                case 0:
                    switch (inf[1])
                    {
                        case 0:
                            return Cdf(upper[0], upper[1], rho);
                        case 1:
                            return Cdf(upper[0], -lower[1], -rho);
                        case 2:
                            return Cdf(upper[0], upper[1], rho) - Cdf(upper[0],
                                                                      lower[1], rho);
                        default:
                            return m_univNormalDistStd.Cdf(upper[1]);
                    }
                case 1:
                    switch (inf[1])
                    {
                        case 0:
                            return Cdf(-lower[0], upper[1], -rho);
                        case 1:
                            return Cdf(-lower[0], -lower[1], rho);
                        case 2:
                            return Cdf(-lower[0], -lower[1], rho) - Cdf(-lower[0],
                                                                        -upper[1], rho);
                        default:
                            return 1 - m_univNormalDistStd.Cdf(lower[1]);
                    }
                case 2:
                    switch (inf[1])
                    {
                        case 0:
                            return Cdf(upper[0], upper[1], rho) - Cdf(lower[0],
                                                                      upper[1], rho);
                        case 1:
                            return Cdf(-lower[0], -lower[1], rho) - Cdf(-upper[0],
                                                                        -lower[1], rho);
                        case 2:
                            {
                                double t = Cdf(upper[0], upper[1], rho) + Cdf(lower[0],
                                                                              lower[1], rho) - Cdf(upper[0], lower[1],
                                                                                                   rho) -
                                           Cdf(lower[0], upper[1], rho);
                                if (t < 0 || t > 1)
                                {
                                    t = Cdf(-upper[0], -upper[1], rho) + Cdf(-lower[0],
                                                                             -lower[1], rho) - Cdf(-upper[0],
                                                                                                   -lower[1], rho) -
                                        Cdf(-lower[0],
                                            -upper[1], rho);
                                }
                                if (t < 0 || t > 1)
                                {
                                    throw new ArgumentException("result of probability is outside the reagen");
                                }
                                return t;
                            }
                        default:
                            return m_univNormalDistStd.Cdf(upper[1]) - m_univNormalDistStd.Cdf(lower[1]);
                    }
                default:
                    switch (inf[1])
                    {
                        case 0:
                            return m_univNormalDistStd.Cdf(upper[0]);
                        case 1:
                            return 1 - m_univNormalDistStd.Cdf(lower[0]);
                        case 2:
                            return m_univNormalDistStd.Cdf(upper[0]) - m_univNormalDistStd.Cdf(lower[0]);
                        default:
                            return 1;
                    }
            }
        }

        /**
         *  A function for computing bivariate normal probabilities. <BR>
         *  Based on algorithms by <BR>
         *  Yihong Ge, Department of Computer Science and Electrical Engineering,
         *  Washington State University, Pullman, WA 99164-2752 <BR>
         *  and <BR>
         *  Alan Genz <a href="http://www.sci.wsu.edu/math/faculty/genz/homepage">
         *  http://www.sci.wsu.edu/math/faculty/genz/homepage</a> <BR>
         *  Department of Mathematics, Washington State University, Pullman, WA
         *  99164-3113, Email : alangenz@wsu.edu <BR>
         *  <BR>
         *
         *
         *@param  sh  integration limit
         *@param  sk  integration limit
         *@param  r   correlation coefficient
         *@return     result
         */

        private double Cdf(double sh, double sk, double r)
        {
            sh = -sh;
            sk = -sk;
            if (sh == Double.PositiveInfinity)
            {
                return m_univNormalDistStd.Cdf(sk);
            }
            if (sh == Double.NegativeInfinity || sk == Double.NegativeInfinity)
            {
                return 0;
            }
            if (sk == Double.PositiveInfinity)
            {
                return m_univNormalDistStd.Cdf(sh);
            }
            double[,] w = {
                              {
                                  0.1713244923791705e+00, 0.4717533638651177e-01, 0.1761400713915212e-01
                              }, {
                                     0.3607615730481384e+00, 0.1069393259953183e+00, 0.4060142980038694e-01
                                 }, {
                                        0.4679139345726904e+00, 0.1600783285433464e+00, 0.6267204833410906e-01
                                    }, {
                                           0, 0.2031674267230659e+00, 0.8327674157670475e-01
                                       }, {
                                              0, 0.2334925365383547e+00, 0.1019301198172404e+00
                                          }, {
                                                 0, 0.2491470458134029e+00, 0.1181945319615184e+00
                                             }, {
                                                    0, 0, 0.1316886384491766e+00
                                                }, {
                                                       0, 0, 0.1420961093183821e+00
                                                   }, {
                                                          0, 0, 0.1491729864726037e+00
                                                      }, {
                                                             0, 0, 0.1527533871307259e+00
                                                         }
                          };
            double[,] x = {
                              {
                                  -0.9324695142031522e+00, -0.9815606342467191e+00, -0.9931285991850949e+00
                              }, {
                                     -0.6612093864662647e+00, -0.9041172563704750e+00, -0.9639719272779138e+00
                                 }, {
                                        -0.2386191860831970e+00, -0.7699026741943050e+00, -0.9122344282513259e+00
                                    }, {
                                           0, -0.5873179542866171e+00, -0.8391169718222188e+00
                                       }, {
                                              0, -0.3678314989981802e+00, -0.7463319064601508e+00
                                          }, {
                                                 0, -0.1252334085114692e+00, -0.6360536807265150e+00
                                             }, {
                                                    0, 0, -0.5108670019508271e+00
                                                }, {
                                                       0, 0, -0.3737060887154196e+00
                                                   }, {
                                                          0, 0, -0.2277858511416451e+00
                                                      }, {
                                                             0, 0, -0.7652652113349733e-01
                                                         }
                          };
            double bvn;
            double dblAs;
            double a;
            double b;
            double c;
            double d;
            double rs;
            double xs;
            //double mvnphi;
            double sn;
            double asr;
            double h;
            double k;
            double bs;
            double hs;
            double hk;
            int lg = 10;
            int ng = 2;
            if (Math.Abs(r) < 0.3)
            {
                ng = 0;
                lg = 3;
            }
            else if (Math.Abs(r) < 0.75)
            {
                ng = 1;
                lg = 6;
            }
            ;
            h = sh;
            k = sk;
            hk = h*k;
            bvn = 0;
            if (Math.Abs(r) < 0.925)
            {
                hs = (h*h + k*k)/2;
                asr = Math.Asin(r);
                for (int i = 0; i < lg; i++)
                {
                    sn = Math.Sin(asr*(x[i, ng] + 1)/2.0);
                    bvn += w[i, ng]*Math.Exp((sn*hk - hs)/(1 - sn*sn));
                    sn = Math.Sin(asr*(-x[i, ng] + 1)/2);
                    bvn += w[i, ng]*Math.Exp((sn*hk - hs)/(1 - sn*sn));
                }
                bvn = bvn*asr/(4*Math.PI) + m_univNormalDistStd.Cdf(-h)*m_univNormalDistStd.Cdf(-k);
            }
            else
            {
                if (r < 0)
                {
                    k = -k;
                    hk = -hk;
                }
                if (Math.Abs(r) < 1)
                {
                    dblAs = (1 - r)*(1 + r);
                    a = Math.Sqrt(dblAs);
                    bs = (h - k)*(h - k);
                    c = (4 - hk)/8;
                    d = (12 - hk)/16;
                    bvn = a*Math.Exp(-(bs/dblAs + hk)/2)*(1 - c*(bs - dblAs)*(1 - d*bs/5)/3
                                                          + c*d*dblAs*dblAs/5);
                    if (hk > -160)
                    {
                        b = Math.Sqrt(bs);
                        bvn -= Math.Exp(-hk/2)*Math.Sqrt(2*Math.PI)*m_univNormalDistStd.Cdf(-b/a)*b*(
                                                                                                        1 -
                                                                                                        c*bs*
                                                                                                        (1 - d*bs/5)/3);
                    }
                    a /= 2;
                    for (int i = 0; i < lg; i++)
                    {
                        xs = a*(x[i, ng] + 1)*a*(x[i, ng] + 1);
                        rs = Math.Sqrt(1 - xs);
                        bvn += a*w[i, ng]*(Math.Exp(-bs/(2*xs) - hk/(1 + rs))/rs
                                           - Math.Exp(-(bs/xs + hk)/2)*(1 + c*xs*(1 + d*xs)));
                        xs = dblAs*(-x[i, ng] + 1)*(-x[i, ng] + 1)/4;
                        rs = Math.Sqrt(1 - xs);
                        bvn += a*w[i, ng]*Math.Exp(-(bs/xs + hk)/2)*(Math.Exp(-hk*(
                                                                                      1 - rs)/(2*(1 + rs)))/rs -
                                                                     (1 + c*xs*(1 + d*xs)));
                    }
                    bvn = -bvn/(Math.PI*2);
                }
                if (r > 0)
                {
                    bvn += m_univNormalDistStd.Cdf(-Math.Max(h, k));
                }
                if (r < 0)
                {
                    bvn = -bvn + Math.Max(0, m_univNormalDistStd.Cdf(-h) - m_univNormalDistStd.Cdf(-k));
                }
            }
            return bvn;
        }

        /**
             * Pdf of bivariate normal
             * @return  value of pdf
             * @param x first dimension
             * @param y second dimension
             * @param r correlation coefficient
             */

        public double Pdf(double x, double y)
        {
            return Pdf(x, y, CovArr[0, 1]);
        }

        public static double Pdf(double x, double y, double r)
        {
            if (Double.IsInfinity(x) || Double.IsInfinity(y))
            {
                return 0;
            }
            return Math.Exp(-(x*x + y*y - 2.0*r*x*y)/(2.0*(1 - r*r)))
                   /(2.0*Math.PI*Math.Sqrt(1.0 - r*r));
        }

        #endregion
    }
}
