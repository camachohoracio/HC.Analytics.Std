#region

using System;
using System.Text;

#endregion

namespace HC.Analytics.Colt
{
    ////package bin;

    /**
     * Abstract base class for all arbitrary-dimensional bins consumes <tt>double</tt> elements.
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
    public abstract class AbstractBin : PersistentObject
    {
        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Returns <tt>center(0)</tt>.
         */

        public double center()
        {
            return center(0);
        }

        /**
         * Returns a custom definable "center" measure; override this method if necessary.
         * Returns the absolute or relative center of this bin.
         * For example, the center of gravity.
         * 
         * The <i>real</i> absolute center can be obtained as follow:
         * <tt>partition(i).Min(j) * bin(j).offset() + bin(j).center(i)</tt>,
         * where <tt>i</tt> is the dimension.
         * and <tt>j</tt> is the index of this bin.
         *
         * <p>This default implementation always returns 0.5.
         *
         * @param dimension the dimension to be considered (zero based).
         */

        public double center(int dimension)
        {
            return 0.5;
        }

        /**
         * Removes all elements from the receiver.
         * The receiver will be empty after this call returns.
         */
        public abstract void Clear();
        /**
         * Returns whether two objects are equal;
         * This default implementation returns true if the other object is a bin 
         * and has the same size, value, error and center.
         */

        public override bool Equals(Object otherObj)
        {
            if (!(otherObj is AbstractBin))
            {
                return false;
            }
            AbstractBin other = (AbstractBin) otherObj;
            return Size() == other.Size() && value() == other.value() && error() == other.error() &&
                   center() == other.center();
        }

        /**
         * Returns <tt>error(0)</tt>.
         */

        public double error()
        {
            return error(0);
        }

        /**
         * Returns a custom definable error measure; override this method if necessary.
         * This default implementation always returns <tt>0</tt>.
         *
         * @param dimension the dimension to be considered.
         */

        public double error(int dimension)
        {
            return 0;
        }

        /**
         * Returns whether a client can obtain all elements added to the receiver.
         * In other words, tells whether the receiver internally preserves all added elements.
         * If the receiver is rebinnable, the elements can be obtained via <tt>elements()</tt> methods.
         */
        public abstract bool isRebinnable();
        /**
         * Returns <tt>offset(0)</tt>.
         */

        public double offset()
        {
            return offset(0);
        }

        /**
         * Returns the relative or absolute position for the center of the bin; override this method if necessary.
         * Returns 1.0 if a relative center is stored in the bin.
         * Returns 0.0 if an absolute center is stored in the bin.
         *
         * <p>This default implementation always returns 1.0 (relative).
         *
         * @param dimension the index of the considered dimension (zero based);
         */

        public double offset(int dimension)
        {
            return 1.0;
        }

        /**
         * Returns the number of elements contained.
         *
         * @returns the number of elements contained.
         */
        public abstract int Size();
        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(base.ToString());
            buf.Append("\n-------------");
            /*
            buf.Append("\nValue: "+value());
            buf.Append("\nError: "+error());
            buf.Append("\nRMS: "+rms()+Environment.NewLine);
            */
            buf.Append(Environment.NewLine);
            return buf.ToString();
        }

        /**
         * Trims the capacity of the receiver to be the receiver's current size.
         * Releases any superfluos internal memory.
         * An application can use this operation to minimize the storage of the receiver.
         *
         * This default implementation does nothing.
         */

        public void trimToSize()
        {
        }

        /**
         * Returns <tt>value(0)</tt>.
         */

        public double value()
        {
            return value(0);
        }

        /**
         * Returns a custom definable "value" measure; override this method if necessary.
         * <p>This default implementation always returns 0.0.
         *
         * @param dimension the dimension to be considered.
         */

        public double value(int dimension)
        {
            return 0;
        }
    }
}
