#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram1D;
    /**
    A reference implementation of IHistogram1D.
    The goal is to provide a clear implementation rather than the most efficient implementation.
    However, performance seems fine - filling 1.2 * 10^6 points/sec, both using FixedAxis or VariableAxis.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public class Histogram1D : AbstractHistogram1D
    {
        private int[] m_entries;
        private double[] m_errors;
        private double[] m_heights;
        private double m_mean;
        private int m_nEntry; // total number of times fill called
        private double m_rms;
        private double m_sumWeight; // Sum of all weights
        private double m_sumWeightSquared; // Sum of the squares of the weights

        /**
         * Creates a variable-width histogram.
         * Example: <tt>edges = (0.2, 1.0, 5.0)</tt> yields an axis with 2 in-range bins <tt>[0.2,1.0), [1.0,5.0)</tt> and 2 extra bins <tt>[-inf,0.2), [5.0,inf]</tt>.
         * @param title The histogram title.
         * @param edges the bin boundaries the axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @throws ArgumentException if <tt>edges.Length < 1</tt>.
         */

        public Histogram1D(string title, double[] edges)
            : this(title, new VariableAxis(edges))
        {
        }

        /**
         * Creates a fixed-width histogram.
         * 
         * @param title The histogram title.
         * @param bins The number of bins.
         * @param Min The minimum value on the X axis.
         * @param Max The maximum value on the X axis.
         */

        public Histogram1D(string title, int bins, double min, double max)
            : this(title, new FixedAxis(bins, min, max))
        {
        }

        /**
         * Creates a histogram with the given axis binning.
         * 
         * @param title The histogram title.
         * @param axis The axis description to be used for binning.
         */

        public Histogram1D(string title, IAxis axis)
            : base(title)
        {
            m_xAxis = axis;
            int bins = axis.bins();
            m_entries = new int[bins + 2];
            m_heights = new double[bins + 2];
            m_errors = new double[bins + 2];
        }

        public new int allEntries() // perhaps to be deleted (default impl. in superclass sufficient)
        {
            return m_nEntry;
        }

        public override int binEntries(int index)
        {
            //return entries[xAxis.map(index)];
            return m_entries[map(index)];
        }

        public override double binError(int index)
        {
            //return Math.Sqrt(errors[xAxis.map(index)]);
            return Math.Sqrt(m_errors[map(index)]);
        }

        public override double binHeight(int index)
        {
            //return heights[xAxis.map(index)];
            return m_heights[map(index)];
        }

        public override double equivalentBinEntries()
        {
            return m_sumWeight*m_sumWeight/m_sumWeightSquared;
        }

        public override void fill(double x)
        {
            //int bin = xAxis.getBin(x);
            int bin = map(m_xAxis.coordToIndex(x));
            m_entries[bin]++;
            m_heights[bin]++;
            m_errors[bin]++;
            m_nEntry++;
            m_sumWeight++;
            m_sumWeightSquared++;
            m_mean += x;
            m_rms += x*x;
        }

        public override void fill(double x, double weight)
        {
            //int bin = xAxis.getBin(x);
            int bin = map(m_xAxis.coordToIndex(x));
            m_entries[bin]++;
            m_heights[bin] += weight;
            m_errors[bin] += weight*weight;
            m_nEntry++;
            m_sumWeight += weight;
            m_sumWeightSquared += weight*weight;
            m_mean += x*weight;
            m_rms += x*weight*weight;
        }

        public override double mean()
        {
            return m_mean/m_sumWeight;
        }

        public override void reset()
        {
            for (int i = 0; i < m_entries.Length; i++)
            {
                m_entries[i] = 0;
                m_heights[i] = 0;
                m_errors[i] = 0;
            }
            m_nEntry = 0;
            m_sumWeight = 0;
            m_sumWeightSquared = 0;
            m_mean = 0;
            m_rms = 0;
        }

        public override double rms()
        {
            return Math.Sqrt(m_rms/m_sumWeight - m_mean*m_mean/m_sumWeight/m_sumWeight);
        }

        /**
         * Used internally for creating slices and projections
         */

        public void setContents(int[] entries, double[] heights, double[] errors)
        {
            m_entries = entries;
            m_heights = heights;
            m_errors = errors;

            for (int i = 0; i < entries.Length; i++)
            {
                m_nEntry += entries[i];
                m_sumWeight += heights[i];
            }
            // TODO: Can we do anything sensible/useful with the other statistics?
            m_sumWeightSquared = double.NaN;
            m_mean = double.NaN;
            m_rms = double.NaN;
        }
    }
}
