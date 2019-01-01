#region

using System;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class EmpiricalDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Cumulative distribution function
        /// </summary>
        protected double[] m_dblCdfArr;

        protected int m_interpolationType;

        #endregion

        #region Constructors

        /**
         * Constructs an Empirical distribution.
         * The probability distribution function (pdf) is an array of positive real numbers.
         * It need not be provided in the form of relative probabilities, absolute probabilities are also accepted.
         * The <tt>pdf</tt> must satisfy both of the following conditions
         * <ul>
         * <li><tt>0.0 &lt;= pdf[i] : 0&lt;=i&lt;=pdf.Length-1</tt>
         * <li><tt>0.0 &lt; Sum(pdf[i]) : 0&lt;=i&lt;=pdf.Length-1</tt>
         * </ul>
         * @param pdf the probability distribution function.
         * @param interpolationType can be either <tt>Empirical.NO_INTERPOLATION</tt> or <tt>Empirical.LINEAR_INTERPOLATION</tt>.
         * @param randomGenerator a uniform random number generator.
         * @throws ArgumentException if at least one of the three conditions above is violated.
         */

        public EmpiricalDist(
            double[] dblPdfArr,
            int interpolationType,
            RngWrapper rng) : base(rng)
        {
            SetState(
                dblPdfArr,
                interpolationType);
        }

        #endregion

        #region Initialization

        /**
         * Sets the distribution parameters.
         * The <tt>pdf</tt> must satisfy both of the following conditions
         * <ul>
         * <li><tt>0.0 &lt;= pdf[i] : 0 &lt; =i &lt;= pdf.Length-1</tt>
         * <li><tt>0.0 &lt; Sum(pdf[i]) : 0 &lt;=i &lt;= pdf.Length-1</tt>
         * </ul>
         * @param pdf probability distribution function.
         * @param interpolationType can be either <tt>Empirical.NO_INTERPOLATION</tt> or <tt>Empirical.LINEAR_INTERPOLATION</tt>.
         * @throws ArgumentException if at least one of the three conditions above is violated.
         */

        public void SetState(
            double[] dblPdfArr,
            int interpolationType)
        {
            if (interpolationType != Constants.LINEAR_INTERPOLATION &&
                interpolationType != Constants.NO_INTERPOLATION)
            {
                throw new ArgumentException(
                    "Illegal Interpolation Type");
            }

            m_interpolationType = interpolationType;

            if (dblPdfArr == null || dblPdfArr.Length == 0)
            {
                m_dblCdfArr = null;
                //throw new ArgumentException("Non-existing pdf");
                return;
            }

            // compute cumulative distribution function (cdf) from probability distribution function (pdf)
            int nBins = dblPdfArr.Length;
            m_dblCdfArr = new double[nBins + 1];

            m_dblCdfArr[0] = 0;
            for (int ptn = 0; ptn < nBins; ++ptn)
            {
                double prob = dblPdfArr[ptn];
                if (prob < 0.0)
                {
                    throw new ArgumentException("Negative probability");
                }
                m_dblCdfArr[ptn + 1] = m_dblCdfArr[ptn] + prob;
            }
            if (m_dblCdfArr[nBins] <= 0.0)
            {
                throw new ArgumentException("At leat one probability must be > 0.0");
            }
            for (int ptn = 0; ptn < nBins + 1; ++ptn)
            {
                m_dblCdfArr[ptn] /= m_dblCdfArr[nBins];
            }
            // cdf is now cached...
        }

        #endregion

        #region Public

        /**
         * Returns the cumulative distribution function.
         */

        public double Cdf(int intIndex)
        {
            if (intIndex < 0)
            {
                return 0.0;
            }
            if (intIndex >= m_dblCdfArr.Length - 1)
            {
                return 1.0;
            }
            return m_dblCdfArr[intIndex];
        }

        /**
         * Returns a random number from the distribution.
         */

        public override double NextDouble()
        {
            double rand = m_rng.NextDouble();
            if (m_dblCdfArr == null)
            {
                return rand; // Non-existing pdf
            }
            // binary search in cumulative distribution function:
            int nBins = m_dblCdfArr.Length - 1;
            int nbelow = 0; // largest k such that I[k] is known to be <= rand
            int nabove = nBins; // largest k such that I[k] is known to be >  rand

            while (nabove > nbelow + 1)
            {
                int middle = (nabove + nbelow + 1) >> 1; // div 2
                if (rand >= m_dblCdfArr[middle])
                {
                    nbelow = middle;
                }
                else
                {
                    nabove = middle;
                }
            }
            // after this binary search, nabove is always nbelow+1 and they straddle rand:

            if (m_interpolationType == Constants.NO_INTERPOLATION)
            {
                return ((double) nbelow)/nBins;
            }
            else if (m_interpolationType == Constants.LINEAR_INTERPOLATION)
            {
                double binMeasure = m_dblCdfArr[nabove] - m_dblCdfArr[nbelow];
                // binMeasure is always aProbFunc[nbelow],
                // but we don't have aProbFunc any more so we subtract.

                if (binMeasure == 0.0)
                {
                    // rand lies right in a bin of measure 0.  Simply return the center
                    // of the range of that bin.  (Any value between k/N and (k+1)/N is
                    // equally good, in this rare case.)
                    return (nbelow + 0.5)/nBins;
                }

                double binFraction = (rand - m_dblCdfArr[nbelow])/binMeasure;
                return (nbelow + binFraction)/nBins;
            }
            else
            {
                throw new ArgumentException("Illegal interpolation type"); // illegal interpolation type
            }
        }

        /**
         * Returns the probability distribution function.
         */

        public override double Pdf(double dblX)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns the probability distribution function.
         */

        public double Pdf(int intIndex)
        {
            if (intIndex < 0 || intIndex >= m_dblCdfArr.Length - 1)
            {
                return 0.0;
            }
            return m_dblCdfArr[intIndex - 1] - m_dblCdfArr[intIndex];
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            string interpolation = null;
            if (m_interpolationType == Constants.NO_INTERPOLATION)
            {
                interpolation = "NO_INTERPOLATION";
            }
            if (m_interpolationType == Constants.LINEAR_INTERPOLATION)
            {
                interpolation = "LINEAR_INTERPOLATION";
            }
            return "Empirical(" + ((m_dblCdfArr != null) ? m_dblCdfArr.Length : 0) + "," + interpolation + ")";
        }

        #endregion

        #region Private

        /**
         * Not yet commented.
         * @return int
         */

        private int xnBins()
        {
            return m_dblCdfArr.Length - 1;
        }

        #endregion
    }
}
