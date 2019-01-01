#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram;
    ////import IHistogram2D;
    ////import IHistogram3D;
    /**
    Abstract base class extracting and implementing most of the redundancy of the interface.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public abstract class AbstractHistogram3D : Histogram, IHistogram3D
    {
        public IAxis m_xAxis;
        public IAxis m_yAxis;
        public IAxis m_zAxis;

        public AbstractHistogram3D(string title)
            : base(title)
        {
        }

        #region IHistogram3D Members

        public override int allEntries()
        {
            int n = 0;
            for (int i = m_xAxis.bins(); --i >= -2;)
            {
                for (int j = m_yAxis.bins(); --j >= -2;)
                {
                    for (int k = m_zAxis.bins(); --k >= -2;)
                    {
                        n += binEntries(i, j, k);
                    }
                }
            }
            return n;
        }

        public override int dimensions()
        {
            return 3;
        }

        public override int entries()
        {
            int n = 0;
            for (int i = 0; i < m_xAxis.bins(); i++)
            {
                for (int j = 0; j < m_yAxis.bins(); j++)
                {
                    for (int k = 0; k < m_zAxis.bins(); k++)
                    {
                        n += binEntries(i, j, k);
                    }
                }
            }
            return n;
        }

        public override int extraEntries()
        {
            return allEntries() - entries();
        }

        public abstract int binEntries(int indexX, int indexY, int indexZ);
        public abstract double binError(int indexX, int indexY, int indexZ);
        public abstract double binHeight(int indexX, int indexY, int indexZ);

        public void fill(double x, double y, double z)
        {
            fill(x, y, z, 1);
        }

        public abstract void fill(double x, double y, double z, double weight);
        public abstract double meanX();
        public abstract double meanY();
        public abstract double meanZ();
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

        public int[] minMaxBins()
        {
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;
            int minBinX = -1;
            int minBinY = -1;
            int minBinZ = -1;
            int maxBinX = -1;
            int maxBinY = -1;
            int maxBinZ = -1;
            for (int i = m_xAxis.bins(); --i >= 0;)
            {
                for (int j = m_yAxis.bins(); --j >= 0;)
                {
                    for (int k = m_zAxis.bins(); --k >= 0;)
                    {
                        double value = binHeight(i, j, k);
                        if (value < minValue)
                        {
                            minValue = value;
                            minBinX = i;
                            minBinY = j;
                            minBinZ = k;
                        }
                        if (value > maxValue)
                        {
                            maxValue = value;
                            maxBinX = i;
                            maxBinY = j;
                            maxBinZ = k;
                        }
                    }
                }
            }
            int[] result = {minBinX, minBinY, minBinZ, maxBinX, maxBinY, maxBinZ};
            return result;
        }

        public IHistogram2D projectionXY()
        {
            string newTitle = title() + " (projectionXY)";
            return internalSliceXY(newTitle, mapZ(Constants.UNDERFLOW), mapZ(Constants.OVERFLOW));
        }

        public IHistogram2D projectionXZ()
        {
            string newTitle = title() + " (projectionXZ)";
            return internalSliceXZ(newTitle, mapY(Constants.UNDERFLOW), mapY(Constants.OVERFLOW));
        }

        public IHistogram2D projectionYZ()
        {
            string newTitle = title() + " (projectionYZ)";
            return internalSliceYZ(newTitle, mapX(Constants.UNDERFLOW), mapX(Constants.OVERFLOW));
        }

        public abstract double rmsX();
        public abstract double rmsY();
        public abstract double rmsZ();

        public IHistogram2D sliceXY(int indexZ)
        {
            return sliceXY(indexZ, indexZ);
        }

        public IHistogram2D sliceXY(int indexZ1, int indexZ2)
        {
            int start = mapZ(indexZ1);
            int stop = mapZ(indexZ2);
            string newTitle = title() + " (sliceXY [" + indexZ1 + ":" + indexZ2 + "])";
            return internalSliceXY(newTitle, start, stop);
        }

        public IHistogram2D sliceXZ(int indexY)
        {
            return sliceXZ(indexY, indexY);
        }

        public IHistogram2D sliceXZ(int indexY1, int indexY2)
        {
            int start = mapY(indexY1);
            int stop = mapY(indexY2);
            string newTitle = title() + " (sliceXZ [" + indexY1 + ":" + indexY2 + "])";
            return internalSliceXY(newTitle, start, stop);
        }

        public IHistogram2D sliceYZ(int indexX)
        {
            return sliceYZ(indexX, indexX);
        }

        public IHistogram2D sliceYZ(int indexX1, int indexX2)
        {
            int start = mapX(indexX1);
            int stop = mapX(indexX2);
            string newTitle = title() + " (sliceYZ [" + indexX1 + ":" + indexX2 + "])";
            return internalSliceYZ(newTitle, start, stop);
        }

        public override double sumAllBinHeights()
        {
            double n = 0;
            for (int i = m_xAxis.bins(); --i >= -2;)
            {
                for (int j = m_yAxis.bins(); --j >= -2;)
                {
                    for (int k = m_zAxis.bins(); --k >= -2;)
                    {
                        n += binHeight(i, j, k);
                    }
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
                    for (int k = 0; k < m_zAxis.bins(); k++)
                    {
                        n += binHeight(i, j, k);
                    }
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

        public IAxis zAxis()
        {
            return m_zAxis;
        }

        #endregion

        public abstract IHistogram2D internalSliceXY(string title, int indexZ1, int indexZ2);
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
        public abstract IHistogram2D internalSliceXZ(string title, int indexY1, int indexY2);
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
        public abstract IHistogram2D internalSliceYZ(string title, int indexX1, int indexX2);
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

        /**
         * Package private method to map from the external representation of bin
         * number to our internal representation of bin number
         */

        public int mapZ(int index)
        {
            int bins = m_zAxis.bins() + 2;
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
