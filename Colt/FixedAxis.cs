#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram;
    /**
    Fixed-width axis; A reference implementation of IAxis.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public class FixedAxis : IAxis
    {
        private readonly double m_dblBinWidth;
        private readonly double m_dblMin;
        private readonly int m_intBins;
        // Package private for ease of use in Histogram1D and Histogram2D
        //private int m_xunder;
        //private int m_xover;
        /**
         * Create an Axis
         * @param bins Number of bins
         * @param Min Minimum for axis
         * @param Max Maximum for axis
         */

        public FixedAxis(int bins, double min, double max)
        {
            if (bins < 1)
            {
                throw new ArgumentException("bins=" + bins);
            }
            if (max <= min)
            {
                throw new ArgumentException("Max <= Min");
            }

            // Note, for internal consistency we save only Min and binWidth
            // and always use these quantities for all calculations. Due to 
            // rounding errors the return value from upperEdge is not necessarily
            // exactly equal to Max

            m_intBins = bins;
            m_dblMin = min;
            m_dblBinWidth = (max - min)/bins;

            // our internal definition of overflow/underflow differs from
            // that of the outside world
            //under = 0;
            //over = bins+1;
        }

        public override double binCentre(int index)
        {
            return m_dblMin + m_dblBinWidth*index + m_dblBinWidth/2;
        }

        public override double binLowerEdge(int index)
        {
            if (index == Constants.UNDERFLOW)
            {
                return Double.NegativeInfinity;
            }
            if (index == Constants.OVERFLOW)
            {
                return upperEdge();
            }
            return m_dblMin + m_dblBinWidth*index;
        }

        public override int bins()
        {
            return m_intBins;
        }

        public override double binUpperEdge(int index)
        {
            if (index == Constants.UNDERFLOW)
            {
                return m_dblMin;
            }
            if (index == Constants.OVERFLOW)
            {
                return Double.PositiveInfinity;
            }
            return m_dblMin + m_dblBinWidth*(index + 1);
        }

        public override double binWidth(int index)
        {
            return m_dblBinWidth;
        }

        public override int coordToIndex(double coord)
        {
            if (coord < m_dblMin)
            {
                return Constants.UNDERFLOW;
            }
            int index = (int) Math.Floor((coord - m_dblMin)/m_dblBinWidth);
            if (index >= m_intBins)
            {
                return Constants.OVERFLOW;
            }

            return index;
        }

        public override double lowerEdge()
        {
            return m_dblMin;
        }

        public override double upperEdge()
        {
            return m_dblMin + m_dblBinWidth*m_intBins;
        }

        /**
         * This //package private method is similar to coordToIndex except
         * that it returns our internal definition for overflow/underflow
         */

        private int xgetBin(double coord)
        {
            if (coord < m_dblMin)
            {
                return 0;
            }
            int index = (int) Math.Floor((coord - m_dblMin)/m_dblBinWidth);
            if (index > m_intBins)
            {
                return 0;
            }
            return index + 1;
        }

        /**
         * Package private method to map from the external representation of bin
         * number to our internal representation of bin number
         */

        private int xmap(int index)
        {
            if (index >= m_intBins)
            {
                throw new ArgumentException("bin=" + index);
            }
            if (index >= 0)
            {
                return index + 1;
            }
            if (index == Constants.UNDERFLOW)
            {
                return 0;
            }
            if (index == Constants.OVERFLOW)
            {
                return 0;
            }
            throw new ArgumentException("bin=" + index);
        }
    }
}
