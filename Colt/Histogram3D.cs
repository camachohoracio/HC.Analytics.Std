#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram2D;
    ////import IHistogram3D;
    /**
    A reference implementation of IHistogram3D.
    The goal is to provide a clear implementation rather than the most efficient implementation.
    However, performance seems fine - filling 3 * 10^5 points/sec, both using FixedAxis or VariableAxis.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public class Histogram3D : AbstractHistogram3D, IHistogram3D
    {
        private readonly int[,,] m_entries;
        private readonly double[,,] m_errors;
        private readonly double[,,] m_heights;
        private double m_meanX;
        private double m_meanY;
        private double m_meanZ;
        private int m_nEntry; // total number of times fill called
        private double m_rmsX;
        private double m_rmsY;
        private double m_rmsZ;
        private double m_sumWeight; // Sum of all weights
        private double m_sumWeightSquared; // Sum of the squares of the weights
        /**
         * Creates a variable-width histogram.
         * Example: <tt>xEdges = (0.2, 1.0, 5.0, 6.0), yEdges = (-5, 0, 7), zEdges = (-5, 0, 7)</tt> yields 3*2*2 in-range bins.
         * @param title The histogram title.
         * @param xEdges the bin boundaries the x-axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @param yEdges the bin boundaries the y-axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @param zEdges the bin boundaries the z-axis shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @throws ArgumentException if <tt>xEdges.Length < 1 || yEdges.Length < 1|| zEdges.Length < 1</tt>.
         */

        public Histogram3D(string title, double[] xEdges, double[] yEdges, double[] zEdges)
            : this(title, new VariableAxis(xEdges), new VariableAxis(yEdges), new VariableAxis(zEdges))
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
         * @param zBins The number of bins on the Z axis.
         * @param zMin The minimum value on the Z axis.
         * @param zMax The maximum value on the Z axis.
         */

        public Histogram3D(string title, int xBins, double xMin, double xMax,
                           int yBins, double yMin, double yMax,
                           int zBins, double zMin, double zMax)
            : this(
                title, new FixedAxis(xBins, xMin, xMax), new FixedAxis(yBins, yMin, yMax),
                new FixedAxis(zBins, zMin, zMax))
        {
        }

        /**
         * Creates a histogram with the given axis binning.
         * 
         * @param title The histogram title.
         * @param xAxis The x-axis description to be used for binning.
         * @param yAxis The y-axis description to be used for binning.
         * @param zAxis The z-axis description to be used for binning.
         */

        public Histogram3D(string title, IAxis xAxis, IAxis yAxis, IAxis zAxis)
            : base(title)
        {
            m_xAxis = xAxis;
            m_yAxis = yAxis;
            m_zAxis = zAxis;
            int xBins = xAxis.bins();
            int yBins = yAxis.bins();
            int zBins = zAxis.bins();

            m_entries = new int[xBins + 2,yBins + 2,zBins + 2];
            m_heights = new double[xBins + 2,yBins + 2,zBins + 2];
            m_errors = new double[xBins + 2,yBins + 2,zBins + 2];
        }

        #region IHistogram3D Members

        public override int allEntries()
        {
            return m_nEntry;
        }

        public override int binEntries(int indexX, int indexY, int indexZ)
        {
            return m_entries[mapX(indexX), mapY(indexY), mapZ(indexZ)];
        }

        public override double binError(int indexX, int indexY, int indexZ)
        {
            return Math.Sqrt(m_errors[mapX(indexX), mapY(indexY), mapZ(indexZ)]);
        }

        public override double binHeight(int indexX, int indexY, int indexZ)
        {
            return m_heights[mapX(indexX), mapY(indexY), mapZ(indexZ)];
        }

        public override double equivalentBinEntries()
        {
            return m_sumWeight*m_sumWeight/m_sumWeightSquared;
        }

        public new void fill(double x, double y, double z)
        {
            int xBin = mapX(m_xAxis.coordToIndex(x));
            int yBin = mapY(m_yAxis.coordToIndex(y));
            int zBin = mapZ(m_zAxis.coordToIndex(z));
            m_entries[xBin, yBin, zBin]++;
            m_heights[xBin, yBin, zBin]++;
            m_errors[xBin, yBin, zBin]++;
            m_nEntry++;
            m_sumWeight++;
            m_sumWeightSquared++;
            m_meanX += x;
            m_rmsX += x;
            m_meanY += y;
            m_rmsY += y;
            m_meanZ += z;
            m_rmsZ += z;
        }

        public override void fill(double x, double y, double z, double weight)
        {
            int xBin = mapX(m_xAxis.coordToIndex(x));
            int yBin = mapY(m_yAxis.coordToIndex(y));
            int zBin = mapZ(m_zAxis.coordToIndex(z));
            m_entries[xBin, yBin, zBin]++;
            m_heights[xBin, yBin, zBin] += weight;
            m_errors[xBin, yBin, zBin] += weight*weight;
            m_nEntry++;
            m_sumWeight += weight;
            m_sumWeightSquared += weight*weight;
            m_meanX += x*weight;
            m_rmsX += x*weight*weight;
            m_meanY += y*weight;
            m_rmsY += y*weight*weight;
            m_meanZ += z*weight;
            m_rmsZ += z*weight*weight;
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

        public override double meanX()
        {
            return m_meanX/m_sumWeight;
        }

        public override double meanY()
        {
            return m_meanY/m_sumWeight;
        }

        public override double meanZ()
        {
            return m_meanZ/m_sumWeight;
        }

        public override void reset()
        {
            for (int i = 0; i < m_entries.GetLength(0); i++)
            {
                for (int j = 0; j < m_entries.GetLength(1); j++)
                {
                    for (int k = 0; j < m_entries.GetLength(2); k++)
                    {
                        m_entries[i, j, k] = 0;
                        m_heights[i, j, k] = 0;
                        m_errors[i, j, k] = 0;
                    }
                }
            }
            m_nEntry = 0;
            m_sumWeight = 0;
            m_sumWeightSquared = 0;
            m_meanX = 0;
            m_rmsX = 0;
            m_meanY = 0;
            m_rmsY = 0;
            m_meanZ = 0;
            m_rmsZ = 0;
        }

        public override double rmsX()
        {
            return Math.Sqrt(m_rmsX/m_sumWeight - m_meanX*m_meanX/m_sumWeight/m_sumWeight);
        }

        public override double rmsY()
        {
            return Math.Sqrt(m_rmsY/m_sumWeight - m_meanY*m_meanY/m_sumWeight/m_sumWeight);
        }

        public override double rmsZ()
        {
            return Math.Sqrt(m_rmsZ/m_sumWeight - m_meanZ*m_meanZ/m_sumWeight/m_sumWeight);
        }

        public override double sumAllBinHeights()
        {
            return m_sumWeight;
        }

        #endregion

        public override IHistogram2D internalSliceXY(string title, int indexZ1, int indexZ2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexZ2 < indexZ1)
            {
                throw new ArgumentException("Invalid bin range");
            }

            int xBins = m_xAxis.bins() + 2;
            int yBins = m_yAxis.bins() + 2;
            int[,] sliceEntries = new int[xBins,yBins];
            double[,] sliceHeights = new double[xBins,yBins];
            double[,] sliceErrors = new double[xBins,yBins];

            for (int i = 0; i < xBins; i++)
            {
                for (int j = 0; j < yBins; j++)
                {
                    for (int k = indexZ1; k <= indexZ2; k++)
                    {
                        sliceEntries[i, j] += m_entries[i, j, k];
                        sliceHeights[i, j] += m_heights[i, j, k];
                        sliceErrors[i, j] += m_errors[i, j, k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, m_xAxis, m_yAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
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

        public override IHistogram2D internalSliceXZ(string title, int indexY1, int indexY2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexY2 < indexY1)
            {
                throw new ArgumentException("Invalid bin range");
            }

            int xBins = m_xAxis.bins() + 2;
            int zBins = m_zAxis.bins() + 2;
            int[,] sliceEntries = new int[xBins,zBins];
            double[,] sliceHeights = new double[xBins,zBins];
            double[,] sliceErrors = new double[xBins,zBins];

            for (int i = 0; i < xBins; i++)
            {
                for (int j = indexY1; j <= indexY2; j++)
                {
                    for (int k = 0; i < zBins; k++)
                    {
                        sliceEntries[i, k] += m_entries[i, j, k];
                        sliceHeights[i, k] += m_heights[i, j, k];
                        sliceErrors[i, k] += m_errors[i, j, k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, m_xAxis, m_zAxis);
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

        public override IHistogram2D internalSliceYZ(string title, int indexX1, int indexX2)
        {
            // Attention: our internal definition of bins has been choosen
            // so that this works properly even if the indeces passed in include
            // the underflow or overflow bins
            if (indexX2 < indexX1)
            {
                throw new ArgumentException("Invalid bin range");
            }

            int yBins = m_yAxis.bins() + 2;
            int zBins = m_zAxis.bins() + 2;
            int[,] sliceEntries = new int[yBins,zBins];
            double[,] sliceHeights = new double[yBins,zBins];
            double[,] sliceErrors = new double[yBins,zBins];

            for (int i = indexX1; i <= indexX2; i++)
            {
                for (int j = 0; j < yBins; j++)
                {
                    for (int k = 0; k < zBins; k++)
                    {
                        sliceEntries[j, k] += m_entries[i, j, k];
                        sliceHeights[j, k] += m_heights[i, j, k];
                        sliceErrors[j, k] += m_errors[i, j, k];
                    }
                }
            }
            Histogram2D result = new Histogram2D(title, m_yAxis, m_zAxis);
            result.setContents(sliceEntries, sliceHeights, sliceErrors);
            return result;
        }
    }
}
