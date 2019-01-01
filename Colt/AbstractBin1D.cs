#region

using System;
using System.Text;

#endregion

namespace HC.Analytics.Colt
{
    ////package bin;

    ////import DoubleArrayList;
    //////import Descriptive;
    /**
     * Abstract base class for all 1-dimensional bins consumes <tt>double</tt> elements.
     * First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
     * <p>
     * This class is fully thread safe (all public methods are ).
     * Thus, you can have one or more threads adding to the bin as well as one or more threads reading and viewing the statistics of the bin <i>while it is filled</i>.
     * For high performance, Add data in large chunks (buffers) via method <tt>addAllOf</tt> rather than piecewise via method <tt>Add</tt>.
     * 
     * @author wolfgang.hoschek@cern.ch
     * @version 0.9, 03-Jul-99
     */

    [Serializable]
    public abstract class AbstractBin1D : AbstractBin, DoubleBufferConsumer
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Adds the specified element to the receiver.
         *
         * @param element element to be appended.
         */
        /**
         * Adds all values of the specified list to the receiver.
         * @param list the list of which all values shall be added.
         */

        #region DoubleBufferConsumer Members

        public void addAllOf(DoubleArrayList list)
        {
            addAllOfFromTo(list, 0, list.Size() - 1);
        }

        #endregion

        public abstract void Add(double element);

        /**
         * Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
         * You may want to override this method for performance reasons.
         *
         * @param list the list of which elements shall be added.
         * @param from the index of the first element to be added (inclusive).
         * @param to the index of the last element to be added (inclusive).
         * @throws HCException if <tt>list.Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=list.Size())</tt>.
         */

        public void addAllOfFromTo(DoubleArrayList list, int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                Add(list.getQuick(i));
            }
        }

        /**
         * Constructs and returns a streaming buffer connected to the receiver.
         * Whenever the buffer is full it's contents are automatically flushed to <tt>this</tt>. 
         * (Addding elements via a buffer to a bin is significantly faster than adding them directly.)
         * @param capacity the number of elements the buffer shall be capable of holding before overflowing and flushing to the receiver.
         * @return a streaming buffer having the receiver as target.
         */

        public DoubleBuffer buffered(int capacity)
        {
            return new DoubleBuffer(this, capacity);
        }

        /**
         * Computes the deviations from the receiver's measures to another bin's measures.
         * @param other the other bin to Compare with
         * @return a summary of the deviations.
         */

        public string compareWith(AbstractBin1D other)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("\nDifferences [percent]");
            buf.Append("\nSize: " + relError(Size(), other.Size()) + " %");
            buf.Append("\nSum: " + relError(sum(), other.sum()) + " %");
            buf.Append("\nSumOfSquares: " + relError(sumOfSquares(), other.sumOfSquares()) + " %");
            buf.Append("\nMin: " + relError(min(), other.min()) + " %");
            buf.Append("\nMax: " + relError(max(), other.max()) + " %");
            buf.Append("\nMean: " + relError(mean(), other.mean()) + " %");
            buf.Append("\nRMS: " + relError(rms(), other.rms()) + " %");
            buf.Append("\nVariance: " + relError(variance(), other.variance()) + " %");
            buf.Append("\nStandard deviation: " + relError(standardDeviation(), other.standardDeviation()) + " %");
            buf.Append("\nStandard error: " + relError(standardError(), other.standardError()) + " %");
            buf.Append(Environment.NewLine);
            return buf.ToString();
        }

        /**
         * Returns whether two bins are equal; 
         * They are equal if the other object is of the same class or a subclass of this class and both have the same size, minimum, maximum, sum and sumOfSquares.
         */

        public override bool Equals(Object object_)
        {
            if (!(object_ is AbstractBin1D))
            {
                return false;
            }
            AbstractBin1D other = (AbstractBin1D) object_;
            return Size() == other.Size() && min() == other.min() && max() == other.max()
                   && sum() == other.sum() && sumOfSquares() == other.sumOfSquares();
        }

        /**
         * Returns the maximum.
         */
        public abstract double max();
        /**
         * Returns the arithmetic mean, which is <tt>Sum( x[i] ) / Size()</tt>.
         */

        public double mean()
        {
            return sum()/Size();
        }

        /**
         * Returns the minimum.
         */
        public abstract double min();
        /**
         * Computes the relative error (in percent) from one measure to another.
         */

        public double relError(double measure1, double measure2)
        {
            return 100*(1 - measure1/measure2);
        }

        /**
         * Returns the rms (Root Mean Square), which is <tt>Math.Sqrt( Sum( x[i]*x[i] ) / Size() )</tt>.
         */

        public double rms()
        {
            return Descriptive.rms(Size(), sumOfSquares());
        }

        /**
         * Returns the sample standard deviation, which is <tt>Math.Sqrt(variance())</tt>.
         */

        public double standardDeviation()
        {
            return Math.Sqrt(variance());
        }

        /**
         * Returns the sample standard error, which is <tt>Math.Sqrt(variance() / Size())</tt>
         */

        public double standardError()
        {
            return Descriptive.standardError(Size(), variance());
        }

        /**
         * Returns the sum of all elements, which is <tt>Sum( x[i] )</tt>.
         */
        public abstract double sum();
        /**
         * Returns the sum of squares, which is <tt>Sum( x[i] * x[i] )</tt>.
         */
        public abstract double sumOfSquares();
        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(ToString());
            buf.Append("\n-------------");
            buf.Append("\nSize: " + Size());
            buf.Append("\nSum: " + sum());
            buf.Append("\nSumOfSquares: " + sumOfSquares());
            buf.Append("\nMin: " + min());
            buf.Append("\nMax: " + max());
            buf.Append("\nMean: " + mean());
            buf.Append("\nRMS: " + rms());
            buf.Append("\nVariance: " + variance());
            buf.Append("\nStandard deviation: " + standardDeviation());
            buf.Append("\nStandard error: " + standardError());
            /*
            buf.Append("\nValue: "+value());
            buf.Append("\nError(0): "+error(0));
            */
            buf.Append(Environment.NewLine);
            return buf.ToString();
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current size.
         * Releases any superfluos internal memory.
         * An application can use this operation to minimize the storage of the receiver.
         * This default implementation does nothing.
         */

        public new void trimToSize()
        {
        }

        /**
         * Returns the sample variance, which is <tt>Sum( (x[i]-mean())<sup>2</sup> )  /  (Size()-1)</tt>.
         */

        public double variance()
        {
            return Descriptive.sampleVariance(Size(), sum(), sumOfSquares());
        }
    }
}
