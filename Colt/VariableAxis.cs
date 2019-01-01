#region

using System;
using System.Text;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram;
    /**
    Variable-width axis; A reference implementation of IAxis.

    @author Wolfgang Hoschek, Tony Johnson, and others.
    @version 1.0, 23/03/2000
    */

    [Serializable]
    public class VariableAxis : IAxis
    {
        public int bins_;
        public double[] edges;
        public double min;
        /**
         * Constructs and returns an axis with the given bin edges.
         * Example: <tt>edges = (0.2, 1.0, 5.0)</tt> yields an axis with 2 in-range bins <tt>[0.2,1.0), [1.0,5.0)</tt> and 2 extra bins <tt>[-inf,0.2), [5.0,inf]</tt>.
         * @param edges the bin boundaries the partition shall have;
         *        must be sorted ascending and must not contain multiple identical elements.
         * @throws ArgumentException if <tt>edges.Length < 1</tt>.
         */

        public VariableAxis(double[] edges)
        {
            if (edges.Length < 1)
            {
                throw new ArgumentException();
            }

            // check if really sorted and has no multiple identical elements
            for (int i = 0; i < edges.Length - 1; i++)
            {
                if (edges[i + 1] <= edges[i])
                {
                    throw new ArgumentException(
                        "edges must be sorted ascending and must not contain multiple identical values");
                }
            }

            min = edges[0];
            bins_ = edges.Length - 1;
            edges = (double[]) edges.Clone();
        }

        public override double binCentre(int index)
        {
            return (binLowerEdge(index) + binUpperEdge(index))/2;
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
            return edges[index];
        }

        public override int bins()
        {
            return bins_;
        }

        public override double binUpperEdge(int index)
        {
            if (index == Constants.UNDERFLOW)
            {
                return lowerEdge();
            }
            if (index == Constants.OVERFLOW)
            {
                return Double.PositiveInfinity;
            }
            return edges[index + 1];
        }

        public override double binWidth(int index)
        {
            return binUpperEdge(index) - binLowerEdge(index);
        }

        public override int coordToIndex(double coord)
        {
            if (coord < min)
            {
                return Constants.UNDERFLOW;
            }

            int index = Array.BinarySearch(edges, coord);
            //int index = new DoubleArrayList(edges).binarySearch(coord); // just for debugging
            if (index < 0)
            {
                index = -index - 1 - 1; // not found
            }
            //else index++; // found

            if (index >= bins_)
            {
                return Constants.OVERFLOW;
            }

            return index;
        }

        public override double lowerEdge()
        {
            return min;
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(double[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        public override double upperEdge()
        {
            return edges[edges.Length - 1];
        }
    }
}
