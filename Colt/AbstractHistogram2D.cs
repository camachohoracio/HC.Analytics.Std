#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram;
    ////import IHistogram1D;
    ////import IHistogram2D;
    /**
    Abstract base class extracting and implementing most of the redundancy of the interface.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public abstract class AbstractHistogram2D : Histogram, IHistogram2D
    {
        public IAxis m_xAxis, m_yAxis;

        public AbstractHistogram2D(string title)
            : base(title)
        {
        }

        #region IHistogram2D Members

        public override int allEntries()
        {
            int n = 0;
            for (int i = m_xAxis.bins(); --i >= -2;)
            {
                for (int j = m_yAxis.bins(); --j >= -2;)
                {
                    n += binEntries(i, j);
                }
            }
            return n;
        }

        public abstract int binEntries(int indexX, int indexY);

        public int binEntriesX(int indexX)
        {
            return projectionX().binEntries(indexX);
        }

        public int binEntriesY(int indexY)
        {
            return projectionY().binEntries(indexY);
        }

        public abstract double binError(int indexX, int indexY);
        public abstract double binHeight(int indexX, int indexY);

        public double binHeightX(int indexX)
        {
            return projectionX().binHeight(indexX);
        }

        public double binHeightY(int indexY)
        {
            return projectionY().binHeight(indexY);
        }

        public override int dimensions()
        {
            return 2;
        }

        public override int entries()
        {
            int n = 0;
            for (int i = 0; i < m_xAxis.bins(); i++)
            {
                for (int j = 0; j < m_yAxis.bins(); j++)
                {
                    n += binEntries(i, j);
                }
            }
            return n;
        }

        public override int extraEntries()
        {
            return allEntries() - entries();
        }

        public void fill(double x, double y)
        {
            fill(x, y, 1);
        }

        public abstract void fill(double x, double y, double weight);
        public abstract double meanX();
        public abstract double meanY();
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

        public int[] minMaxBins()
        {
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;
            int minBinX = -1;
            int minBinY = -1;
            int maxBinX = -1;
            int maxBinY = -1;
            for (int i = m_xAxis.bins(); --i >= 0;)
            {
                for (int j = m_yAxis.bins(); --j >= 0;)
                {
                    double value = binHeight(i, j);
                    if (value < minValue)
                    {
                        minValue = value;
                        minBinX = i;
                        minBinY = j;
                    }
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxBinX = i;
                        maxBinY = j;
                    }
                }
            }
            int[] result = {minBinX, minBinY, maxBinX, maxBinY};
            return result;
        }

        public IHistogram1D projectionX()
        {
            string newTitle = title() + " (projectionX)";
            //return internalSliceX(newTitle,yAxis.under,yAxis.over);
            return internalSliceX(newTitle, mapY(Constants.UNDERFLOW), mapY(Constants.OVERFLOW));
        }

        public IHistogram1D projectionY()
        {
            string newTitle = title() + " (projectionY)";
            //return internalSliceY(newTitle,xAxis.under,xAxis.over);
            return internalSliceY(newTitle, mapX(Constants.UNDERFLOW), mapX(Constants.OVERFLOW));
        }

        public abstract double rmsX();
        public abstract double rmsY();

        public IHistogram1D sliceX(int indexY)
        {
            //int start = yAxis.map(indexY);
            int start = mapY(indexY);
            string newTitle = title() + " (sliceX [" + indexY + "])";
            return internalSliceX(newTitle, start, start);
        }

        public IHistogram1D sliceX(int indexY1, int indexY2)
        {
            //int start = yAxis.map(indexY1);
            //int stop = yAxis.map(indexY2);
            int start = mapY(indexY1);
            int stop = mapY(indexY2);
            string newTitle = title() + " (sliceX [" + indexY1 + ":" + indexY2 + "])";
            return internalSliceX(newTitle, start, stop);
        }

        public IHistogram1D sliceY(int indexX)
        {
            //int start = xAxis.map(indexX);
            int start = mapX(indexX);
            string newTitle = title() + " (sliceY [" + indexX + "])";
            return internalSliceY(newTitle, start, start);
        }

        public IHistogram1D sliceY(int indexX1, int indexX2)
        {
            //int start = xAxis.map(indexX1);
            //int stop = xAxis.map(indexX2);
            int start = mapX(indexX1);
            int stop = mapX(indexX2);
            string newTitle = title() + " (slicey [" + indexX1 + ":" + indexX2 + "])";
            return internalSliceY(newTitle, start, stop);
        }

        public override double sumAllBinHeights()
        {
            double n = 0;
            for (int i = m_xAxis.bins(); --i >= -2;)
            {
                for (int j = m_yAxis.bins(); --j >= -2;)
                {
                    n += binHeight(i, j);
                }
            }
            return n;
        }

        public override double sumBinHeights()
        {
            double n = 0;
            for (int i = 0; i < m_xAxis.bins(); i++)
            {
                for (int j = 0; j < m_yAxis.bins(); j++)
                {
                    n += binHeight(i, j);
                }
            }
            return n;
        }

        public override double sumExtraBinHeights()
        {
            return sumAllBinHeights() - sumBinHeights();
        }

        public IAxis xAxis()
        {
            return m_xAxis;
        }

        public IAxis yAxis()
        {
            return m_yAxis;
        }

        #endregion

        public abstract IHistogram1D internalSliceX(string title, int indexY1, int indexY2);
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
        public abstract IHistogram1D internalSliceY(string title, int indexX1, int indexX2);
        /**
             * Package private method to map from the external representation of bin
             * number to our internal representation of bin number
             */

        public int mapX(int index)
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

        /**
             * Package private method to map from the external representation of bin
             * number to our internal representation of bin number
             */

        public int mapY(int index)
        {
            int bins = m_yAxis.bins() + 2;
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
