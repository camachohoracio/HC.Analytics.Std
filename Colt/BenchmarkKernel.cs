#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package Appendbench;

    /**
     * Not yet documented.
     * 
     * @author wolfgang.hoschek@cern.ch
     * @version 1.0, 10-Nov-99
     */

    [Serializable]
    public class BenchmarkKernel
    {
        /**
         * Benchmark constructor comment.
         */
        /**
         * Executes procedure repeatadly until more than minSeconds have elapsed.
         */

        public static float run(
            double minSeconds,
            TimerProcedure procedure)
        {
            long iter = 0;
            long minMillis = (long) (minSeconds*1000);

            long begin = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
            long limit = begin + minMillis;
            while ((DateTime.MinValue - DateTime.Now).TotalMilliseconds < limit)
            {
                procedure.init();
                procedure.Apply(null);
                iter++;
            }
            long end = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
            if (minSeconds/iter < 0.1)
            {
                // unreliable timing due to very fast iteration;
                // reading, starting and stopping timer distorts measurement
                // do it again with minimal timer overhead
                //PrintToScreen.WriteLine("iter="+iter+", minSeconds/iter="+minSeconds/iter);
                begin = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
                for (long i = iter; --i >= 0;)
                {
                    procedure.init();
                    procedure.Apply(null);
                }
                end = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
            }

            long begin2 = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
            int dummy = 1; // prevent compiler from optimizing away the loop
            for (long i = iter; --i >= 0;)
            {
                dummy *= (int) i;
                procedure.init();
            }
            long end2 = (long) (DateTime.MinValue - DateTime.Now).TotalMilliseconds;
            long elapsed = (end - begin) - (end2 - begin2);
            //if (dummy != 0) throw new HCException("dummy != 0");

            return elapsed/1000.0f/iter;
        }

        /**
         * Returns a string with the system's properties (vendor, version, operating system, etc.)
         */

        public static string systemInfo()
        {
            string[] properties = {
                                      "java.vm.vendor",
                                      "java.vm.version",
                                      "java.vm.name",
                                      "os.name",
                                      "os.version",
                                      "os.arch",
                                      "java.version",
                                      "java.vendor",
                                      "java.vendor.url"
                                      /*
		"java.vm.specification.version",
		"java.vm.specification.vendor",
		"java.vm.specification.name",
		"java.specification.version",
		"java.specification.vendor",
		"java.specification.name"
		*/
                                  };

            // build string matrix
            ObjectMatrix2D matrix = new DenseObjectMatrix2D(properties.Length, 2);
            matrix.viewColumn(0).assign(properties);

            // retrieve property values
            for (int i = 0; i < properties.Length; i++)
            {
                string value = (properties[i]);
                if (value == null)
                {
                    value = "?"; // prop not available
                }
                matrix.set(i, 1, value);
            }

            // format matrix
            Formatter formatter = new Formatter();
            formatter.setPrintShape(false);
            return formatter.ToString(matrix);
        }
    }
}
