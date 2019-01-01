#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    ////package bin;

    ////import DoubleArrayList;
    //////import Descriptive;
    /**
     * 1-dimensional non-rebinnable bin consuming <tt>double</tt> elements;
     * Efficiently computes basic statistics of data sequences.
     * First see the <a href="package-summary.html">//package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
     * <p>
     * The data streamed into a <tt>SimpleBin1D</tt> is not preserved!
     * As a consequence infinitely many elements can be added to this bin.
     * As a further consequence this bin cannot compute more than basic statistics.
     * It is also not rebinnable.
     * If these drawbacks matter, consider to use a {@link DynamicBin1D}, 
     * which overcomes them at the expense of increased memory requirements.
     * <p>
     * This class is fully thread safe (all public methods are ).
     * Thus, you can have one or more threads adding to the bin as well as one or more threads reading and viewing the statistics of the bin <i>while it is filled</i>.
     * For high performance, Add data in large chunks (buffers) via method <tt>addAllOf</tt> rather than piecewise via method <tt>Add</tt>.
     * <p>
     * <b>Implementation</b>:
     * Incremental maintainance. Performance linear in the number of elements added.
     * 
     * @author wolfgang.hoschek@cern.ch
     * @version 0.9, 03-Jul-99
     */

    [Serializable]
    public class StaticBin1D : AbstractBin1D
    {
        /**
         * The number of elements consumed by incremental parameter maintainance.
         */
        public static double[] m_arguments = new double[20];
        public double m_max; // Max( x[i] )
        public double m_min; // Min( x[i] )
        public int m_size;
        public double m_sum; // Sum( x[i] )
        public double m_sum_xx; // Sum( x[i]*x[i] )

        /** 
         * Function arguments used by method addAllOf(...)
         * For memory tuning only. Avoids allocating a new array of arguments each time addAllOf(...) is called.
         *
         * Each bin does not need its own set of argument vars since they are declared as "static".
         * addAllOf(...) of this class uses only 4 entries.
         * Subclasses computing additional incremental statistics may need more arguments.
         * So, to be on the safe side we allocate space for 20 args.
         * Be sure you access this arguments only in  blocks like
         *  (arguments) { do it }
         *
         * By the way, the whole fuss would be unnecessary if Java would know INOUT parameters (call by reference).
         */
        /**
         * Constructs and returns an empty bin.
         */

        public StaticBin1D()
        {
            Clear();
        }

        /**
         * Adds the specified element to the receiver.
         *
         * @param element element to be appended.
         */

        public override void Add(double element)
        {
            // prototyping implementation; inefficient; TODO
            addAllOf(new DoubleArrayList(new[] {element}));
            /*
            sumSquares += element * element;
            if (done == 0) { // initial setup
                Min = element;
                Max = element;
            }
            else {
                if (element < Min) Min = element;
                if (element > Max) Max = element;

                double oldMean = mean;
                mean += (element - mean)/(done+1);
                sumsq += (element-mean)*(element-oldMean); // cool, huh?
            }
            done++;
            */
        }

        /**
         * Adds the part of the specified list between indexes <tt>from</tt> (inclusive) and <tt>to</tt> (inclusive) to the receiver.
         *
         * @param list the list of which elements shall be added.
         * @param from the index of the first element to be added (inclusive).
         * @param to the index of the last element to be added (inclusive).
         * @throws HCException if <tt>list.Size()&gt;0 && (from&lt;0 || from&gt;to || to&gt;=list.Size())</tt>.
         */

        public new void addAllOfFromTo(DoubleArrayList list, int from, int to)
        {
            //if (arguments == null) setUpCache();
            // prepare arguments
            m_arguments[0] = m_min;
            m_arguments[1] = m_max;
            m_arguments[2] = m_sum;
            m_arguments[3] = m_sum_xx;

            Descriptive.incrementalUpdate(list, from, to, m_arguments);

            // store the new parameters back
            m_min = m_arguments[0];
            m_max = m_arguments[1];
            m_sum = m_arguments[2];
            m_sum_xx = m_arguments[3];

            m_size += to - from + 1;
        }

        /**
         * Removes all elements from the receiver.
         * The receiver will be empty after this call returns.
         */

        public override void Clear()
        {
            clearAllMeasures();
            m_size = 0;
        }

        /**
         * Resets the values of all measures.
         */

        public void clearAllMeasures()
        {
            m_min = Double.PositiveInfinity;
            m_max = Double.NegativeInfinity;
            m_sum = 0.0;
            m_sum_xx = 0.0;
        }

        /**
         * Returns <tt>false</tt>.
         * Returns whether a client can obtain all elements added to the receiver.
         * In other words, tells whether the receiver internally preserves all added elements.
         * If the receiver is rebinnable, the elements can be obtained via <tt>elements()</tt> methods.
         *
         */

        public override bool isRebinnable()
        {
            return false;
        }

        /**
         * Returns the maximum.
         */

        public override double max()
        {
            return m_max;
        }

        /**
         * Returns the minimum.
         */

        public override double min()
        {
            return m_min;
        }

        /**
         * Returns the number of elements contained in the receiver.
         *
         * @returns  the number of elements contained in the receiver.
         */

        public override int Size()
        {
            return m_size;
        }

        /**
         * Returns the sum of all elements, which is <tt>Sum( x[i] )</tt>.
         */

        public override double sum()
        {
            return m_sum;
        }

        /**
         * Returns the sum of squares, which is <tt>Sum( x[i] * x[i] )</tt>.
         */

        public override double sumOfSquares()
        {
            return m_sum_xx;
        }
    }
}
