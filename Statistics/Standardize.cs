namespace HC.Analytics.Statistics
{
    public class Standardize
    {
        //STANDARDIZATION
        // Standardization of an array of doubles to a mean of 0 and a standard deviation of 1
        public static double[] standardize(double[] aa)
        {
            double mean0 = Mean.GetSampleMean(aa);
            double sd0 = StdDev.GetSampleStdDev(aa);
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (aa[i] - mean0)/sd0;
            }

            return bb;
        }

        public static double[] standardise(double[] aa)
        {
            return standardize(aa);
        }

        // Standardization of an array of floats to a mean of 0 and a standard deviation of 1
        public static float[] standardize(float[] aa)
        {
            float mean0 = Mean.GetSampleMean(aa);
            float sd0 = (float) StdDev.GetSampleStdDev(aa);
            int n = aa.Length;
            float[] bb = new float[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (aa[i] - mean0)/sd0;
            }

            return bb;
        }

        public static float[] standardise(float[] aa)
        {
            return standardize(aa);
        }

        // Standardization of an array of longs to a mean of 0 and a standard deviation of 1
        // converts to double
        public static double[] standardize(long[] aa)
        {
            double mean0 = Mean.GetSampleMean(aa);
            double sd0 = StdDev.GetSampleStdDev(aa);
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (aa[i] - mean0)/sd0;
            }

            return bb;
        }

        public static double[] standardise(long[] aa)
        {
            return standardize(aa);
        }

        // Standardization of an array of ints to a mean of 0 and a standard deviation of 1
        // converts to double
        public static double[] standardize(int[] aa)
        {
            double mean0 = Mean.GetSampleMean(aa);
            double sd0 = StdDev.GetSampleStdDev(aa);
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = (aa[i] - mean0)/sd0;
            }

            return bb;
        }

        public static double[] standardise(int[] aa)
        {
            return standardize(aa);
        }

        // Standardization of an array of BigDecimals to a mean of 0 and a standard deviation of 1
        // converts to double
        public static double[] standardize(decimal[] aa)
        {
            double mean0 = Mean.GetSampleMean(aa);
            double sd0 = StdDev.GetSampleStdDev(aa);
            int n = aa.Length;
            double[] bb = new double[n];
            for (int i = 0; i < n; i++)
            {
                bb[i] = ((double) aa[i] - mean0)/sd0;
            }

            return bb;
        }

        public static double[] standardise(decimal[] aa)
        {
            return standardize(aa);
        }
    }
}
