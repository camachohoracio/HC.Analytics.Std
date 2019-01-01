#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright ï¿½ 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package cern.jet.random.engine;

    //import java.util.DateTime;
    /**
     * Quick medium quality uniform pseudo-random number generator.
     *
     * Produces uniformly distributed <tt>int</tt>'s and <tt>long</tt>'s in the closed intervals <tt>[Integer.MIN_VALUE,Integer.MAX_VALUE]</tt> and <tt>[Long.MIN_VALUE,Long.MAX_VALUE]</tt>, respectively, 
     * as well as <tt>float</tt>'s and <tt>double</tt>'s in the open unit intervals <tt>(0.0f,1.0f)</tt> and <tt>(0.0,1.0)</tt>, respectively.
     * <p>
     * The seed can be any integer satisfying <tt>0 &lt; 4*seed+1 &lt; 2<sup>32</sup></tt>.
     * In other words, there must hold <tt>seed &gt;= 0 && seed &lt; 1073741823</tt>.
     * <p>
     * <b>Quality:</b> This generator follows the multiplicative congruential method of the form                    
     * <dt>
     * <tt>z(i+1) = a * z(i) (mod m)</tt> with
     * <tt>a=663608941 (=0X278DDE6DL), m=2<sup>32</sup></tt>.
     * <dt>
     * <tt>z(i)</tt> assumes all different values <tt>0 &lt; 4*seed+1 &lt; m</tt> during a full period of 2<sup>30</sup>.
     *
     * <p>
     * <b>Performance:</b> TO_DO
     * <p>
     * <b>Implementation:</b> TO_DO
     * <p>
     * Note that this implementation is <b>not synchronized</b>.                                  
     * <p>
     * 
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 09/24/99
     * @see MersenneTwister
     * @see java.util.RngWrapper
     */

    [Serializable]
    public class DRand : RandomEngine
    {
        public static int DEFAULT_SEED = 1;
        private int current;
        /**
         * Constructs and returns a random number generator with a default seed, which is a <b>constant</b>.
         */

        public DRand()
            : this(DEFAULT_SEED)
        {
        }

        /**
         * Constructs and returns a random number generator with the given seed.
         * @param seed should not be 0, in such a case <tt>DRand.DEFAULT_SEED</tt> is substituted.
         */

        public DRand(int seed)
        {
            setSeed(seed);
        }

        /**
         * Constructs and returns a random number generator seeded with the given date.
         *
         * @param d typically <tt>new java.util.DateTime()</tt>
         */

        public DRand(DateTime d)
            : this((int) d.TimeOfDay.TotalMilliseconds)
        {
        }

        /**
         * Returns a 32 bit uniformly distributed random number in the closed interval <tt>[Integer.MIN_VALUE,Integer.MAX_VALUE]</tt> (including <tt>Integer.MIN_VALUE</tt> and <tt>Integer.MAX_VALUE</tt>).
         */

        public override int NextInt()
        {
            current *= 0x278DDE6D; /* z(i+1)=a*z(i) (mod 2**32) */
            // a == 0x278DDE6D == 663608941

            return current;
        }

        /**
         * Sets the receiver's seed. 
         * This method resets the receiver's entire internal state.
         * The following condition must hold: <tt>seed &gt;= 0 && seed &lt; (2<sup>32</sup>-1) / 4</tt>.
         * @param seed if the above condition does not hold, a modified seed that meets the condition is silently substituted.
         */

        public void setSeed(int seed)
        {
            if (seed < 0)
            {
                seed = -seed;
            }
            int limit = (int) ((Math.Pow(2, 32) - 1)/4); // --> 536870911
            if (seed >= limit)
            {
                seed = seed >> 3;
            }

            current = 4*seed + 1;
        }
    }
}
