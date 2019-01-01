using System;

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl2_Weka
{

    public class Utils
    {


        /**
          * Checks if the given array contains the flag "-Char". Stops
          * searching at the first marker "--". If the flag is found,
          * it is replaced with the empty string.
          *
          * @param flag the character indicating the flag.
          * @param strings the array of strings containing all the options.
          * @return true if the flag was found
          * @exception Exception if an illegal option was found
          */
        public static bool getFlag(char flag, string[] options)
        {
            return getFlag("" + flag, options);
        }

        /**
         * Checks if the given array contains the flag "-string". Stops
         * searching at the first marker "--". If the flag is found,
         * it is replaced with the empty string.
         *
         * @param flag the string indicating the flag.
         * @param strings the array of strings containing all the options.
         * @return true if the flag was found
         * @exception Exception if an illegal option was found
         */
        public static bool getFlag(string flag, string[] options)
        {

            if (options == null)
            {
                return false;
            }
            for (int i = 0; i < options.Length; i++)
            {
                if ((options[i].Length > 1) && (options[i][0] == '-'))
                {
                    try
                    {
                        double dummy = double.Parse(options[i]);
                    }
                    catch (Exception e)
                    {
                        if (options[i].Equals("-" + flag))
                        {
                            options[i] = "";
                            return true;
                        }
                        if (options[i][1] == '-')
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }


        /**
         * Normalizes the doubles in the array by their sum.
         *
         * @param doubles the array of double
         * @exception IllegalArgumentException if sum is Zero or NaN
         */
        public static void normalize(double[] doubles)
        {

            double sum = 0;
            for (int i = 0; i < doubles.Length; i++)
            {
                sum += doubles[i];
            }
            normalize(doubles, sum);
        }

        /**
         * Normalizes the doubles in the array using the given value.
         *
         * @param doubles the array of double
         * @param sum the value by which the doubles are to be normalized
         * @exception IllegalArgumentException if sum is zero or NaN
         */
        public static void normalize(double[] doubles, double sum)
        {

            if (double.IsNaN(sum))
            {
                throw new Exception("Can't normalize array. Sum is NaN.");
            }
            if (sum == 0)
            {
                // Maybe this should just be a return.
                throw new Exception("Can't normalize array. Sum is zero.");
            }
            for (int i = 0; i < doubles.Length; i++)
            {
                doubles[i] /= sum;
            }
        }
    }

}
