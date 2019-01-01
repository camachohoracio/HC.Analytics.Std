#region

using System;
using HC.Analytics.Colt.CustomImplementations;

#endregion

namespace HC.Analytics.Colt
{
    ////package bin;

    /** 
    Function objects computing dynamic bin aggregations; to be passed to generic methods.
    @see doublealgo.Formatter
    @see Statistic
    @author wolfgang.hoschek@cern.ch
    @version 1.0, 09/24/99
    */

    [Serializable]
    public class BinFunctions1D : Object
    {
        /**
        Little trick to allow for "aliasing", that is, renaming this class.
        Using the aliasing you can instead write
        <p>
        <tt>BinFunctions F = BinFunctions.functions; <br>
        someAlgo(Functions.Max);</tt>
        */
        public static BinFunctions1D functions = new BinFunctions1D();
        public static BinFunction1D geometricMean = new BinFunction1D10_();

        /**
         * Function that returns <tt>bin.Max()</tt>.
         */
        public static BinFunction1D max = new BinFunction1D_();

        /**
         * Function that returns <tt>bin.mean()</tt>.
         */
        public static BinFunction1D mean = new BinFunction1D2_();

        /**
         * Function that returns <tt>bin.median()</tt>.
         */
        public static BinFunction1D median = new BinFunction1D3_();

        /**
         * Function that returns <tt>bin.Min()</tt>.
         */
        public static BinFunction1D min = new BinFunction1D4_();

        /**
         * Function that returns <tt>bin.rms()</tt>.
         */
        public static BinFunction1D rms = new BinFunction1D5_();

        /**
         * Function that returns <tt>bin.Size()</tt>.
         */
        public static BinFunction1D size = new BinFunction1D6_();

        /**
         * Function that returns <tt>bin.standardDeviation()</tt>.
         */
        public static BinFunction1D stdDev = new BinFunction1D7_();

        /**
         * Function that returns <tt>bin.sum()</tt>.
         */
        public static BinFunction1D sum = new BinFunction1D8_();
        /**
         * Function that returns <tt>bin.sumOfLogarithms()</tt>.
         */
        public static BinFunction1D sumLog = new BinFunction1D9_();
        /**
         * Function that returns <tt>bin.geometricMean()</tt>.
         */


        /**
         * Makes this class non instantiable, but still let's others inherit from it.
         */
        /**
         * Function that returns <tt>bin.quantile(percentage)</tt>.
         * @param the percentage of the quantile (<tt>0 <= percentage <= 1</tt>).
         */

        public static BinFunction1D quantile(double percentage)
        {
            return new BinFunction1D11_(percentage);
        }
    }
}
