#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram1D;
    ////import IHistogram2D;
    /**
    A reference implementation of IHistogram2D.
    The goal is to provide a clear implementation rather than the most efficient implementation.
    However, performance seems fine - filling 6 * 10^5 points/sec, both using FixedAxis or VariableAxis.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public class Histogram2D : AbstractHistogram2D, IHistogram2D
    {
        private int[,] m_entries;
        private double[,] m_errors;
        private double[,] m_heights;
        private double m_meanX;
        private double m_meanY;
        private int m_nEntry; // total number of times fill called
        private double m_rmsX;
        private double m_rmsY;
        private double m_sumWeight; // Sum of all weights
        private double m_sumWeightSquared; // Sum of the squares of the weights
        /**
         * Creates a variable-width histogram.
         * Example: <tt>xEdges = (0.2, 1.0, 5.0, 6.0), yEdges = (-5, 0, 7)</tt> yields 3*2 in-range bins.
         * @param title The histogram title.
         * @param xEdges the bin boundaries the x-axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @param yEdges the bin boundaries the y-axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @throws ArgumentException if <tt>xEdges.Length < 1 || yEdges.Length < 1</tt>.
         */

        public Histogram2D(string title, double[] xEdges, double[] yEdges)
            : this(title, new VariableAxis(xEdges), new VariableAxis(yEdges))
        {
        }

        /**
         * Creates a fixed-width histogram.
         * 
         * @param title The histogram title.
         * @param xBins The number of bins on the X axis.
         * @param xMin The minimum value on the X axis.
         * @param xMax The maximum value on the X axis.
         * @param yBins The number of bins on the Y axis.
         * @param yMin The minimum value on the Y axis.
         * @param yMax The maximum value on the Y axis.
         */

        public Histogram2D(string title, int xBins, double xMin, double xMax,
                           int yBins, double yMin, double yMax)
            : this(title, new FixedAxis(xBins, xMin, xMax), new FixedAxis(yBins, yMin, yMax))
        {
        }

        /**
         * Creates a histogram with the given axis binning.
         * 
         * @param title The histogram title.
         * @param xAxis The x-axis description to be used for binning.
         * @param yAxis The y-axis description to be used for binning.
         */

        public Histogram2D(string title, IAxis xAxis, IAxis yAxis)
            : base(title)
        {
            m_xAxis = xAxis;
            m_yAxis = yAxis;
            int xBins = xAxis.bins();
            int yBins = yAxis.bins();

            m_entries = new int[xBins + 2,yBins + 2];
            m_heights = new double[xBins + 2,yBins + 2];
            m_errors = new double[xBins + 2,yBins + 2];
        }

        #region IHistogram2D Members

        public override int allEntries()
        {
            return m_nEntry;
        }

        public override int binEntries(int indexX, int indexY)
        {
            //return entries[xAxis.map(indexX),yAxis.map(indexY)];
            return m_entries[mapX(indexX), mapY(indexY)];
        }

        public override double binError(int indexX, int indexY)
        {
            //return Math.Sqrt(errors[xAxis.map(indexX),yAxis.map(indexY)]);
            return Math.Sqrt(m_errors[mapX(indexX), mapY(indexY)]);
        }

        public override double binHeight(int indexX, int indexY)
        {
            //return heights[xAxis.map(indexX),yAxis.map(indexY)];
            return m_heights[mapX(indexX), mapY(indexY)];
        }

        public override double equivalentBinEntries()
        {
            return m_sumWeight*m_sumWeight/m_sumWeightSquared;
        }

        public new void fill(double x, double y)
        {
            //int xBin = xAxis.getBin(x);
            //int yBin = xAxis.getBin(y);
            int xBin = mapX(m_xAxis.coordToIndex(x));
            int yBin = mapY(m_yAxis.coordToIndex(y));
            m_entries[xBin, yBin]++;
            m_heights[xBin, yBin]++;
            m_errors[xBin, yBin]++;
            m_nEntry++;
            m_sumWeight++;
            m_sumWeightSquared++;
            m_meanX += x;
            m_rmsX += x;
            m_meanY += y;
            m_rmsY += y;
        }

        public override void fill(double x, double y, double weight)
        {
            //int xBin = xAxis.getBin(x);
            //int yBin = xAxis.getBin(y);
            int xBin = mapX(m_xAxis.coordToIndex(x));
            int yBin = mapY(m_yAxis.coordToIndex(y));
            m_entries[xBin, yBin]++;
            m_heights[xBin, yBin] += weight;
            m_errors[xBin, yBin] += weight*weight;
            m_nEntry++;
            m_sumWeight += weight;
            m_sumWeightSquared += weight*weight;
            m_meanX += x*weight;
            m_rmsX += x*weight*weight;
            m_meanY += y*weight;
            m_rmsY += y*weight*weight;
        }

        /**
         * The precise meaning of the arguments to the public slice
         * methods is somewhat ambiguous, so we define this internal
         * slice method and clearly specify its arguments.
         * <p>
         * <b>Note 0</b>indexY1 and indexY2 use our INTERNAL bin numbering scheme
         * <b>Note 1</b>The slice is done between indexY1 and indexY2 INCLUSIVE
         * <b>Note 2</b>indexY1 and indexY2 may include the use of under and over flow bins
         * <b>Note 3</b>There is no note 3 (yet)
         */

        public override double meanX()
        {
            return m_meanX/m_sumWeight;
        }

        public override double meanY()
        {
            return m_meanY/m_sumWeight;
        }

        public override void reset()
        {
            for (int i = 0; i < m_entries.GetLength(0); i++)
            {
                for (int j = 0; j < m_entries.GetLength(1); j++)
                {
                    m_entries[i, j] = 0;
                    m_heights[i, j] = 0;
                    m_errors[i, j] = 0;
                }
            }
            m_nEntry = 0;
            m_sumWeight = 0;
            m_sumWeightSquared = 0;
            m_meanX = 0;
            m_rmsX = 0;
            m_meanY = 0;
            m_rmsY = 0;
        }

        public override double rmsX()
        {
            return Math.Sqrt(m_rmsX/m_sumWeight - m_meanX*m_meanX/m_sumWeight/m_sumWeight);
        }

        public override double rmsY()
        {
            return Math.Sqrt(m_rmsY/m_sumWeight - m_meanY*m_meanY/m_sumWeight/m_sumWeight);
        }

        /**
         * Used internally for creating slices and projections
         */

        public override double sumAllBinHeights()
        {
            return m_sumWeight;
        }

        #endregion

        public override IHistogram1D internalSliceX(string title, int indexY1, int indexY2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexY2 < indexY1)
            {
                throw new ArgumentException("Invalid bin range");
            }

            int sliceBins = m_xAxis.bins() + 2;
            int[] sliceEntries = new int[sliceBins];
            double[] sliceHeights = new double[sliceBins];
            double[] sliceErrors = new double[sliceBins];

            //for (int i=xAxis.under; i<=xAxis.over; i++)
            for (int i = 0; i < sliceBins; i++)
            {
                for (int j = indexY1; j <= indexY2; j++)
                {
                    sliceEntries[i] += m_entries[i, j];
                    sliceHeights[i] += m_heights[i, j];
                    sliceErrors[i] += m_errors[i, j];
                }
            }
            Histogram1D result = new Histogram1D(title, m_xAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        /**
         * The precise meaning of the arguments to the public slice
         * methods is somewhat ambiguous, so we define this internal
         * slice method and clearly specify its arguments.
         * <p>
         * <b>Note 0</b>indexX1 and indexX2 use our INTERNAL bin numbering scheme
         * <b>Note 1</b>The slice is done between indexX1 and indexX2 INCLUSIVE
         * <b>Note 2</b>indexX1 and indexX2 may include the use of under and over flow bins
         * <b>Note 3</b>There is no note 3 (yet)
         */

        public override IHistogram1D internalSliceY(string title, int indexX1, int indexX2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexX2 < indexX1)
            {
                throw new ArgumentException("Invalid bin range");
            }

            int sliceBins = m_yAxis.bins() + 2;
            int[] sliceEntries = new int[sliceBins];
            double[] sliceHeights = new double[sliceBins];
            double[] sliceErrors = new double[sliceBins];

            for (int i = indexX1; i <= indexX2; i++)
            {
                //for (int j=yAxis.under; j<=yAxis.over; j++)
                for (int j = 0; j < sliceBins; j++)
                {
                    sliceEntries[j] += m_entries[i, j];
                    sliceHeights[j] += m_heights[i, j];
                    sliceErrors[j] += m_errors[i, j];
                }
            }
            Histogram1D result = new Histogram1D(title, m_yAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }

        public void setContents(int[,] entries, double[,] heights, double[,] errors)
        {
            m_entries = entries;
            m_heights = heights;
            m_errors = errors;

            for (int i = 0; i < entries.GetLength(0); i++)
            {
                for (int j = 0; j < entries.GetLength(1); j++)
                {
                    m_nEntry += entries[i, j];
                    m_sumWeight += heights[i, j];
                }
            }
            // TODO: Can we do anything sensible/useful with the other statistics?
            m_sumWeightSquared = double.NaN;
            m_meanX = double.NaN;
            m_rmsX = double.NaN;
            m_meanY = double.NaN;
            m_rmsY = double.NaN;
        }
    }
}
