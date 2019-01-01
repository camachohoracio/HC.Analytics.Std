#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram;
    ////import IHistogram1D;
    /**
    Abstract base class extracting and implementing most of the redundancy of the interface.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public abstract class AbstractHistogram1D : Histogram, IHistogram1D
    {
        public IAxis m_xAxis;

        public AbstractHistogram1D(string title)
            : base(title)
        {
        }

        #region IHistogram1D Members

        public override int allEntries()
        {
            return entries() + extraEntries();
        }

        public override int dimensions()
        {
            return 1;
        }

        public override int entries()
        {
            int entries = 0;
            for (int i = m_xAxis.bins(); --i >= 0;)
            {
                entries += binEntries(i);
            }
            return entries;
        }

        public override int extraEntries()
        {
            //return entries[xAxis.under] + entries[xAxis.over];
            return binEntries(Constants.UNDERFLOW) + binEntries(Constants.OVERFLOW);
        }

        /**
         * Package private method to map from the external representation of bin
         * number to our internal representation of bin number
         */

        public abstract int binEntries(int index);

        public abstract double binError(int index);

        public abstract double binHeight(int index);

        public abstract void fill(double x);

        public abstract void fill(double x, double weight);

        public abstract double mean();

        public int[] minMaxBins()
        {
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;
            int minBinX = -1;
            int maxBinX = -1;
            for (int i = m_xAxis.bins(); --i >= 0;)
            {
                double value = binHeight(i);
                if (value < minValue)
                {
                    minValue = value;
                    minBinX = i;
                }
                if (value > maxValue)
                {
                    maxValue = value;
                    maxBinX = i;
                }
            }
            int[] result = {minBinX, maxBinX};
            return result;
        }

        public abstract double rms();

        public override double sumAllBinHeights()
        {
            return sumBinHeights() + sumExtraBinHeights();
        }

        public override double sumBinHeights()
        {
            double sum = 0;
            for (int i = m_xAxis.bins(); --i >= 0;)
            {
                sum += binHeight(i);
            }
            return sum;
        }

        public override double sumExtraBinHeights()
        {
            return binHeight(Constants.UNDERFLOW) + binHeight(Constants.OVERFLOW);
            //return heights[xAxis.under] + heights[xAxis.over];
        }

        public IAxis xAxis()
        {
            return m_xAxis;
        }

        #endregion

        public int map(int index)
        {
            int bins = m_xAxis.bins() + 2;
            if (index >= bins)
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
                return bins - 1;
            }
            throw new ArgumentException("bin=" + index);
        }
    }
}
