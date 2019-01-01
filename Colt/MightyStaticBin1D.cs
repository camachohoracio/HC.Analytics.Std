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
     * Static and the same as its superclass, except that it can do more: Additionally computes moments of arbitrary integer order, harmonic mean, geometric mean, etc.
     * 
     * Constructors need to be told what functionality is required for the given use case.
     * Only maintains aggregate measures (incrementally) - the added elements themselves are not kept.
     * 
     * @author wolfgang.hoschek@cern.ch
     * @version 0.9, 03-Jul-99
     */

    [Serializable]
    public class MightyStaticBin1D : StaticBin1D
    {
        public bool m_hasSumOfInversions;
        public bool m_hasSumOfLogarithms;
        public double m_sumOfInversions; // Sum( 1/x[i] )
        public double m_sumOfLogarithms; // Sum( Log(x[i]) )

        public double[] m_sumOfPowers; // Sum( x[i]^3 ) .. Sum( x[i]^max_k )
        /**
         * Constructs and returns an empty bin with limited functionality but good performance; equivalent to <tt>MightyStaticBin1D(false,false,4)</tt>.
         */

        public MightyStaticBin1D()
            : this(false, false, 4)
        {
        }

        /**
         * Constructs and returns an empty bin with the given capabilities.
         *
         * @param hasSumOfLogarithms  Tells whether {@link #sumOfLogarithms()} can return meaningful results.
         *        Set this parameter to <tt>false</tt> if measures of sum of logarithms, geometric mean and product are not required.
         * <p>
         * @param hasSumOfInversions  Tells whether {@link #sumOfInversions()} can return meaningful results.
         *        Set this parameter to <tt>false</tt> if measures of sum of inversions, harmonic mean and sumOfPowers(-1) are not required.
         * <p>
         * @param maxOrderForSumOfPowers  The maximum order <tt>k</tt> for which {@link #sumOfPowers(int)} can return meaningful results.
         *        Set this parameter to at least 3 if the skew is required, to at least 4 if the kurtosis is required.
         *        In general, if moments are required set this parameter at least as large as the largest required moment.
         *        This method always substitutes <tt>Math.Max(2,maxOrderForSumOfPowers)</tt> for the parameter passed in.
         *        Thus, <tt>sumOfPowers(0..2)</tt> always returns meaningful results.
         *
         * @see #hasSumOfPowers(int)
         * @see #moment(int,double)
         */

        public MightyStaticBin1D(bool hasSumOfLogarithms, bool hasSumOfInversions, int maxOrderForSumOfPowers)
        {
            setMaxOrderForSumOfPowers(maxOrderForSumOfPowers);
            m_hasSumOfLogarithms = hasSumOfLogarithms;
            m_hasSumOfInversions = hasSumOfInversions;
            Clear();
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
            base.addAllOfFromTo(list, from, to);

            if (m_sumOfPowers != null)
            {
                //int max_k = min_k + sumOfPowers.Length-1;
                Descriptive.incrementalUpdateSumsOfPowers(list, from, to, 3, getMaxOrderForSumOfPowers(), m_sumOfPowers);
            }

            if (m_hasSumOfInversions)
            {
                m_sumOfInversions += Descriptive.sumOfInversions(list, from, to);
            }

            if (m_hasSumOfLogarithms)
            {
                m_sumOfLogarithms += Descriptive.sumOfLogarithms(list, from, to);
            }
        }

        /**
         * Resets the values of all measures.
         */

        public new void clearAllMeasures()
        {
            base.clearAllMeasures();

            m_sumOfLogarithms = 0.0;
            m_sumOfInversions = 0.0;

            if (m_sumOfPowers != null)
            {
                for (int i = m_sumOfPowers.Length; --i >= 0;)
                {
                    m_sumOfPowers[i] = 0.0;
                }
            }
        }

        /**
         * Returns a deep copy of the receiver.
         *
         * @return a deep copy of the receiver.
         */

        public new Object Clone()
        {
            MightyStaticBin1D Clone = (MightyStaticBin1D) base.Clone();
            if (m_sumOfPowers != null)
            {
                Clone.m_sumOfPowers = (double[]) Clone.m_sumOfPowers.Clone();
            }
            return Clone;
        }

        /**
         * Computes the deviations from the receiver's measures to another bin's measures.
         * @param other the other bin to Compare with
         * @return a summary of the deviations.
         */

        public new string compareWith(AbstractBin1D other)
        {
            StringBuilder buf = new StringBuilder(base.compareWith(other));
            if (other is MightyStaticBin1D)
            {
                MightyStaticBin1D m = (MightyStaticBin1D) other;
                if (hasSumOfLogarithms() && m.hasSumOfLogarithms())
                {
                    buf.Append("geometric mean: " + relError(geometricMean(), m.geometricMean()) + " %\n");
                }
                if (hasSumOfInversions() && m.hasSumOfInversions())
                {
                    buf.Append("harmonic mean: " + relError(harmonicMean(), m.harmonicMean()) + " %\n");
                }
                if (hasSumOfPowers(3) && m.hasSumOfPowers(3))
                {
                    buf.Append("skew: " + relError(skew(), m.skew()) + " %\n");
                }
                if (hasSumOfPowers(4) && m.hasSumOfPowers(4))
                {
                    buf.Append("kurtosis: " + relError(kurtosis(), m.kurtosis()) + " %\n");
                }
                buf.Append(Environment.NewLine);
            }
            return buf.ToString();
        }

        /**
         * Returns the geometric mean, which is <tt>Product( x[i] )<sup>1.0/Size()</sup></tt>.
         *
         * This method tries to avoid overflows at the expense of an equivalent but somewhat inefficient definition:
         * <tt>geoMean = exp( Sum( Log(x[i]) ) / Size())</tt>.
         * Note that for a geometric mean to be meaningful, the minimum of the data sequence must not be less or equal to zero.
         * @return the geometric mean; <tt>double.NaN</tt> if <tt>!hasSumOfLogarithms()</tt>.
         */

        public double geometricMean()
        {
            return Descriptive.geometricMean(Size(), sumOfLogarithms());
        }

        /**
         * Returns the maximum order <tt>k</tt> for which sums of powers are retrievable, as specified upon instance construction.
         * @see #hasSumOfPowers(int)
         * @see #sumOfPowers(int)
         */

        public int getMaxOrderForSumOfPowers()
        {
            /* order 0..2 is always recorded.
               order 0 is Size()
               order 1 is sum()
               order 2 is sum_xx()
            */
            if (m_sumOfPowers == null)
            {
                return 2;
            }

            return 2 + m_sumOfPowers.Length;
        }

        /**
         * Returns the minimum order <tt>k</tt> for which sums of powers are retrievable, as specified upon instance construction.
         * @see #hasSumOfPowers(int)
         * @see #sumOfPowers(int)
         */

        public int getMinOrderForSumOfPowers()
        {
            int minOrder = 0;
            if (hasSumOfInversions())
            {
                minOrder = -1;
            }
            return minOrder;
        }

        /**
         * Returns the harmonic mean, which is <tt>Size() / Sum( 1/x[i] )</tt>.
         * Remember: If the receiver contains at least one element of <tt>0.0</tt>, the harmonic mean is <tt>0.0</tt>.
         * @return the harmonic mean; <tt>double.NaN</tt> if <tt>!hasSumOfInversions()</tt>.
         * @see #hasSumOfInversions()
         */

        public double harmonicMean()
        {
            return Descriptive.harmonicMean(Size(), sumOfInversions());
        }

        /**
         * Returns whether <tt>sumOfInversions()</tt> can return meaningful results.
         * @return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
         * See the constructors for proper parametrization.
         */

        public bool hasSumOfInversions()
        {
            return m_hasSumOfInversions;
        }

        /**
         * Tells whether <tt>sumOfLogarithms()</tt> can return meaningful results.
         * @return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
         * See the constructors for proper parametrization.
         */

        public bool hasSumOfLogarithms()
        {
            return m_hasSumOfLogarithms;
        }

        /**
         * Tells whether <tt>sumOfPowers(k)</tt> can return meaningful results.
         * Defined as <tt>hasSumOfPowers(k) <==> getMinOrderForSumOfPowers() <= k && k <= getMaxOrderForSumOfPowers()</tt>.
         * A return value of <tt>true</tt> implies that <tt>hasSumOfPowers(k-1) .. hasSumOfPowers(0)</tt> will also return <tt>true</tt>.
         * See the constructors for proper parametrization.
         * <p>
         * <b>Details</b>: 
         * <tt>hasSumOfPowers(0..2)</tt> will always yield <tt>true</tt>.
         * <tt>hasSumOfPowers(-1) <==> hasSumOfInversions()</tt>.
         *
         * @return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
         * @see #getMinOrderForSumOfPowers()
         * @see #getMaxOrderForSumOfPowers()
         */

        public bool hasSumOfPowers(int k)
        {
            return getMinOrderForSumOfPowers() <= k && k <= getMaxOrderForSumOfPowers();
        }

        /**
         * Returns the kurtosis (aka excess), which is <tt>-3 + moment(4,mean()) / standardDeviation()<sup>4</sup></tt>.
         * @return the kurtosis; <tt>double.NaN</tt> if <tt>!hasSumOfPowers(4)</tt>.
         * @see #hasSumOfPowers(int)
         */

        public double kurtosis()
        {
            return Descriptive.kurtosis(moment(4, mean()), standardDeviation());
        }

        /**
         * Returns the moment of <tt>k</tt>-th order with value <tt>c</tt>,
         * which is <tt>Sum( (x[i]-c)<sup>k</sup> ) / Size()</tt>.
         *
         * @param k the order; must be greater than or equal to zero.
         * @param c any number.
         * @throws ArgumentException if <tt>k < 0</tt>.
         * @return <tt>double.NaN</tt> if <tt>!hasSumOfPower(k)</tt>.
         */

        public double moment(int k, double c)
        {
            if (k < 0)
            {
                throw new ArgumentException("k must be >= 0");
            }
            //checkOrder(k);
            if (!hasSumOfPowers(k))
            {
                return double.NaN;
            }

            int maxOrder = Math.Min(k, getMaxOrderForSumOfPowers());
            DoubleArrayList sumOfPows = new DoubleArrayList(maxOrder + 1);
            sumOfPows.Add(Size());
            sumOfPows.Add(sum());
            sumOfPows.Add(sumOfSquares());
            for (int i = 3; i <= maxOrder; i++)
            {
                sumOfPows.Add(sumOfPowers(i));
            }

            return Descriptive.moment(k, c, Size(), sumOfPows.elements());
        }

        /**
         * Returns the product, which is <tt>Prod( x[i] )</tt>.
         * In other words: <tt>x[0]*x[1]*...*x[Size()-1]</tt>.
         * @return the product; <tt>double.NaN</tt> if <tt>!hasSumOfLogarithms()</tt>.
         * @see #hasSumOfLogarithms()
         */

        public double product()
        {
            return Descriptive.product(Size(), sumOfLogarithms());
        }

        /**
         * Sets the range of orders in which sums of powers are to be computed.
         * In other words, <tt>sumOfPower(k)</tt> will return <tt>Sum( x[i]^k )</tt> if <tt>min_k <= k <= max_k || 0 <= k <= 2</tt>
         * and throw an exception otherwise.
         * @see #isLegalOrder(int)
         * @see #sumOfPowers(int)
         * @see #getRangeForSumOfPowers()
         */

        public void setMaxOrderForSumOfPowers(int max_k)
        {
            //if (max_k < ) throw new ArgumentException();

            if (max_k <= 2)
            {
                m_sumOfPowers = null;
            }
            else
            {
                m_sumOfPowers = new double[max_k - 2];
            }
        }

        /**
         * Returns the skew, which is <tt>moment(3,mean()) / standardDeviation()<sup>3</sup></tt>.
         * @return the skew; <tt>double.NaN</tt> if <tt>!hasSumOfPowers(3)</tt>.
         * @see #hasSumOfPowers(int)
         */

        public double skew()
        {
            return Descriptive.skew(moment(3, mean()), standardDeviation());
        }

        /**
         * Returns the sum of inversions, which is <tt>Sum( 1 / x[i] )</tt>.
         * @return the sum of inversions; <tt>double.NaN</tt> if <tt>!hasSumOfInversions()</tt>.
         * @see #hasSumOfInversions()
         */

        public double sumOfInversions()
        {
            if (!m_hasSumOfInversions)
            {
                return double.NaN;
            }
            //if (! hasSumOfInversions) throw new IllegalOperationException("You must specify upon instance construction that the sum of inversions shall be computed.");
            return m_sumOfInversions;
        }

        /**
         * Returns the sum of logarithms, which is <tt>Sum( Log(x[i]) )</tt>.
         * @return the sum of logarithms; <tt>double.NaN</tt> if <tt>!hasSumOfLogarithms()</tt>.
         * @see #hasSumOfLogarithms()
         */

        public double sumOfLogarithms()
        {
            if (!m_hasSumOfLogarithms)
            {
                return double.NaN;
            }
            //if (! hasSumOfLogarithms) throw new IllegalOperationException("You must specify upon instance construction that the sum of logarithms shall be computed.");
            return m_sumOfLogarithms;
        }

        /**
         * Returns the <tt>k-th</tt> order sum of powers, which is <tt>Sum( x[i]<sup>k</sup> )</tt>.
         * @param k the order of the powers.
         * @return the sum of powers; <tt>double.NaN</tt> if <tt>!hasSumOfPowers(k)</tt>.
         * @see #hasSumOfPowers(int)
         */

        public double sumOfPowers(int k)
        {
            if (!hasSumOfPowers(k))
            {
                return double.NaN;
            }
            //checkOrder(k);	
            if (k == -1)
            {
                return sumOfInversions();
            }
            if (k == 0)
            {
                return Size();
            }
            if (k == 1)
            {
                return sum();
            }
            if (k == 2)
            {
                return sumOfSquares();
            }

            return m_sumOfPowers[k - 3];
        }

        /**
         * Returns a string representation of the receiver.
         */

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder(base.ToString());

            if (hasSumOfLogarithms())
            {
                buf.Append("Geometric mean: " + geometricMean());
                buf.Append("\nProduct: " + product() + Environment.NewLine);
            }

            if (hasSumOfInversions())
            {
                buf.Append("Harmonic mean: " + harmonicMean());
                buf.Append("\nSum of inversions: " + sumOfInversions() + Environment.NewLine);
            }

            int maxOrder = getMaxOrderForSumOfPowers();
            int maxPrintOrder = Math.Min(6, maxOrder); // don't print tons of measures
            if (maxOrder > 2)
            {
                if (maxOrder >= 3)
                {
                    buf.Append("Skew: " + skew() + Environment.NewLine);
                }
                if (maxOrder >= 4)
                {
                    buf.Append("Kurtosis: " + kurtosis() + Environment.NewLine);
                }
                for (int i = 3; i <= maxPrintOrder; i++)
                {
                    buf.Append("Sum of powers(" + i + "): " + sumOfPowers(i) + Environment.NewLine);
                }
                for (int k = 0; k <= maxPrintOrder; k++)
                {
                    buf.Append("Moment(" + k + ",0): " + moment(k, 0) + Environment.NewLine);
                }
                for (int k = 0; k <= maxPrintOrder; k++)
                {
                    buf.Append("Moment(" + k + ",mean()): " + moment(k, mean()) + Environment.NewLine);
                }
            }
            return buf.ToString();
        }

        /**
         * @throws IllegalOperationException if <tt>! isLegalOrder(k)</tt>.
         */

        public void xcheckOrder(int k)
        {
            //if (! isLegalOrder(k)) return double.NaN;
            //if (! xisLegalOrder(k)) throw new IllegalOperationException("Illegal order of sum of powers: k="+k+". Upon instance construction legal range was fixed to be "+getMinOrderForSumOfPowers()+" <= k <= "+getMaxOrderForSumOfPowers());
        }

        /**
         * Returns whether two bins are equal; 
         * They are equal if the other object is of the same class or a subclass of this class and both have the same size, minimum, maximum, sum, sumOfSquares, sumOfInversions and sumOfLogarithms.
         */

        public bool xequals(Object object_)
        {
            if (!(object_ is MightyStaticBin1D))
            {
                return false;
            }
            MightyStaticBin1D other = (MightyStaticBin1D) object_;
            return base.Equals(other) && sumOfInversions() == other.sumOfInversions() &&
                   sumOfLogarithms() == other.sumOfLogarithms();
        }

        /**
         * Tells whether <tt>sumOfPowers(fromK) .. sumOfPowers(toK)</tt> can return meaningful results.
         * @return <tt>false</tt> if the bin was constructed with insufficient parametrization, <tt>true</tt> otherwise.
         * See the constructors for proper parametrization.
         * @throws ArgumentException if <tt>fromK > toK</tt>.
         */

        public bool xhasSumOfPowers(int fromK, int toK)
        {
            if (fromK > toK)
            {
                throw new ArgumentException("fromK must be less or equal to toK");
            }
            return getMinOrderForSumOfPowers() <= fromK && toK <= getMaxOrderForSumOfPowers();
        }

        /**
         * Returns <tt>getMinOrderForSumOfPowers() <= k && k <= getMaxOrderForSumOfPowers()</tt>.
         */

        public bool xisLegalOrder(int k)
        {
            return getMinOrderForSumOfPowers() <= k && k <= getMaxOrderForSumOfPowers();
        }
    }
}
