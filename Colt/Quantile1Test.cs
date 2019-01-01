#region

using System;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    //package quantile;

    //import DynamicBin1D;
    //import bin.QuantileBin1D;

    //import java.text.DecimalFormat;
    //import DateTime;

    ////import RngWrapper;
    ////import DRand;

    /**
     * A class to test the QuantileBin1D code.
     * The command line is "java Quantile1Test numExamples N"
     * where numExamples is the number of random (Gaussian) numbers to
     * be presented to the QuantileBin1D.add method, and N is
     * the absolute maximum number of examples the QuantileBin1D is setup
     * to receive in the constructor.  N can be set to "L", which will use
     * long.MaxValue, or to "I", which will use int.MaxValue, or to 
     * any positive long value.
     */

    [Serializable]
    public class Quantile1Test
    {
        public static void main(string[] argv)
        {
            /*
             * Get the number of examples from the first argument
             */
            int numExamples = 0;
            try
            {
                numExamples = int.Parse(argv[0]);
            }
            catch (HCException e)
            {
                PrintToScreen.WriteLine("Unable to parse input line count argument");
                PrintToScreen.WriteLine(e.Message);
                ////System.exit(1);
            }
            PrintToScreen.WriteLine("Got numExamples=" + numExamples);

            /*
             * Get N from the second argument
             */
            long N = 0;
            try
            {
                if (argv[1].Equals("L"))
                {
                    N = long.MaxValue;
                }
                else if (argv[1].Equals("I"))
                {
                    N = int.MaxValue;
                }
                else
                {
                    N = long.Parse(argv[1]);
                }
            }
            catch (HCException e)
            {
                PrintToScreen.WriteLine("Error parsing flag for N");
                PrintToScreen.WriteLine(e.Message);
                //System.exit(1);
            }
            PrintToScreen.WriteLine("Got N=" + N);

            /*
             * Set up the QuantileBin1D object
             */
            RngWrapper rand = new RngWrapper();
            QuantileBin1D qAccum = new QuantileBin1D(false,
                                                     N,
                                                     1.0e-4,
                                                     1.0e-3,
                                                     200,
                                                     rand,
                                                     false,
                                                     false,
                                                     2);

            DynamicBin1D dbin = new DynamicBin1D();

            /*
             * Use a new random number generator to generate numExamples
             * random gaussians, and add them to the QuantileBin1D
             */
            RngWrapper dataRand = new RngWrapper(7757);
            for (int i = 1; i <= numExamples; i++)
            {
                double gauss = dataRand.NextDouble();
                qAccum.Add(gauss);
                dbin.Add(gauss);
            }

            /*
             * print out the percentiles
             */
            //DecimalFormat fmt = new DecimalFormat("0.00");
            PrintToScreen.WriteLine();
            //int step = 1;
            int step = 10;
            for (int i = 1; i < 100;)
            {
                double percent = (i)*0.01;
                double quantile = qAccum.quantile(percent);
                PrintToScreen.WriteLine("" + "  " + quantile + ",  " + dbin.quantile(percent) + ",  " +
                                  (dbin.quantile(percent) - quantile));
                i = i + step;
            }
        }
    }
}
