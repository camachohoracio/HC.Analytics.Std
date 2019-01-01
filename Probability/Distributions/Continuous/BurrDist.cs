#region

using System;
using HC.Analytics.Mathematics.LinearAlgebra;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class BurrDist : AbstractUnivContDist
    {
        #region Members

        /// <summary>
        /// Own instance
        /// </summary>
        private static readonly BurrDist m_ownInstance = new BurrDist(
            1, 1, 1, new RngWrapper(INT_RND_SEED));

        private double m_dblK;
        private double m_dblR;
        private int m_intNr;

        #endregion

        #region Constants

        private const int INT_RND_SEED = 4;

        #endregion

        #region Constructors

        public BurrDist(
            double dblR,
            double dblK,
            int intNr,
            RngWrapper rng)
            : base(rng)
        {
            SetState(
                dblR,
                dblK,
                intNr);
        }

        #endregion

        #region Parameters

        public double R
        {
            get { return m_dblR; }
            set
            {
                m_dblR = value;
                SetState(
                    m_dblR,
                    m_dblK,
                    m_intNr);
            }
        }

        public double K
        {
            get { return m_dblK; }
            set
            {
                m_dblK = value;
                SetState(
                    m_dblR,
                    m_dblK,
                    m_intNr);
            }
        }

        public int Nr
        {
            get { return m_intNr; }
            set
            {
                m_intNr = value;
                SetState(
                    m_dblR,
                    m_dblK,
                    m_intNr);
            }
        }

        #endregion

        #region Initialization

        private void SetState(
            double dblR,
            double dblK,
            int intNr)
        {
            m_dblR = dblR;
            m_dblK = dblK;
            m_intNr = intNr;
        }

        #endregion

        #region Public

        public override double NextDouble()
        {
            /******************************************************************
             *                                                                *
             *        Burr II, VII, VIII, X Distributions - Inversion         *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - burr1 samples a random number from one of the   *
             *                Burr II, VII, VIII, X distributions with        *
             *                parameter  r > 0 , where the no. of the         *
             *                distribution is indicated by a pointer          *
             *                variable.                                       *
             * REFERENCE :  - L. Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New York.          *
             * SUBPROGRAM : - drand(seed) ... (0,1)-uniform generator with    *
             *                unsigned long integer *seed.                    *
             *                                                                *
             ******************************************************************/

            double y;
            y = Math.Exp(Math.Log(
                             m_rng.NextDouble())/R); /* y=u^(1/r) */
            switch (Nr)
            {
                    // BURR II
                case 2:
                    return (-Math.Log(1/y - 1));

                    // BURR VII
                case 7:
                    return (Math.Log(2*y/(2 - 2*y))/2);

                    // BURR VIII
                case 8:
                    return (Math.Log(Math.Tan(y*Math.PI/2.0)));

                    // BURR X
                case 10:
                    return (Math.Sqrt(-Math.Log(1 - y)));
            }
            return nextBurr2();
        }

        public override double Cdf(double dblX)
        {
            throw new NotImplementedException();
        }

        public override double CdfInv(double dblProbability)
        {
            throw new NotImplementedException();
        }

        public override double Pdf(double dblX)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        /**
         * Returns a random number from the Burr III, IV, V, VI, IX, XII distributions.
         * <p>
         * <b>Implementation:</b> Inversion method.
         * This is a port of <tt>burr2.c</tt> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
         * C-RAND's implementation, in turn, is based upon
         * <p>
         * L. Devroye (1986): Non-Uniform Random Variate Generation, Springer Verlag, New York.
         * <p>
         * @param r must be &gt; 0.
         * @param k must be &gt; 0.
         * @param nr the number of the burr distribution (e.g. 3,4,5,6,9,12).
         */

        private double nextBurr2()
        {
            /******************************************************************
             *                                                                *
             *      Burr III, IV, V, VI, IX, XII Distribution - Inversion     *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION :   - burr2 samples a random number from one of the   *
             *                Burr III, IV, V, VI, IX, XII distributions with *
             *                parameters r > 0 and k > 0, where the no. of    *
             *                the distribution is indicated by a pointer      *
             *                variable.                                       *
             * REFERENCE :  - L. Devroye (1986): Non-Uniform Random Variate   *
             *                Generation, Springer Verlag, New York.          *
             * SUBPROGRAM : - drand(seed) ... (0,1)-Uniform generator with    *
             *                unsigned long integer *seed.                    *
             *                                                                *
             ******************************************************************/
            double y, u;
            u = m_rng.NextDouble(); // U(0/1)
            y = Math.Exp(-Math.Log(u)/R) - 1.0; // u^(-1/r) - 1
            switch (Nr)
            {
                case 3: // BURR III
                    return (Math.Exp(-Math.Log(y)/K)); // y^(-1/k)

                case 4: // BURR IV
                    y = Math.Exp(K*Math.Log(y)) + 1.0; // y^k + 1
                    y = K/y;
                    return (y);

                case 5: // BURR V
                    y = Math.Atan(-Math.Log(y/K)); // arctan[Log(y/k)]
                    return (y);

                case 6: // BURR VI
                    y = -Math.Log(y/K)/R;
                    y = Math.Log(y + Math.Sqrt(y*y + 1.0));
                    return (y);

                case 9: // BURR IX
                    y = 1.0 + 2.0*u/(K*(1.0 - u));
                    y = Math.Exp(Math.Log(y)/R) - 1.0; // y^(1/r) -1
                    return Math.Log(y);

                case 12: // BURR XII
                    return Math.Exp(Math.Log(y)/K); // y^(1/k)
            }
            return 0;
        }

        #endregion

        #region StaticMethods

        public static double PdfStatic(
            double dblR,
            double dblK,
            int intNr,
            double dblX)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.Pdf(dblX);
        }

        public static double CdfStatic(
            double dblR,
            double dblK,
            int intNr,
            double dblX)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.Cdf(dblX);
        }

        public static double CdfInvStatic(
            double dblR,
            double dblK,
            int intNr,
            double dblProbability)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.CdfInv(dblProbability);
        }

        public static double NextDoubleStatic(
            double dblR,
            double dblK,
            int intNr)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.NextDouble();
        }

        public static double[] NextDoubleArrStatic(
            double dblR,
            double dblK,
            int intNr,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.NextDoubleArr(intSampleSize);
        }

        public static Vector NextDoubleVectorStatic(
            double dblR,
            double dblK,
            int intNr,
            int intSampleSize)
        {
            m_ownInstance.SetState(
                dblR,
                dblK,
                intNr);

            return m_ownInstance.NextDoubleVector(intSampleSize);
        }

        #endregion
    }
}
