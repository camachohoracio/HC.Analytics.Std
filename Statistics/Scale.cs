namespace HC.Analytics.Statistics
{
    public class Scale
    {
        // SCALING DATA
        // Scale an array of doubles to a new mean and new standard deviation
        public static double[] scale(double[] aa, double mean, double sd)
        {
            double[] bb = Standardize.standardize(aa);
            int n = aa.Length;
            for (int i = 0; i < n; i++)
            {
                bb[i] = bb[i]*sd + mean;
            }

            return bb;
        }

        // Scale an array of floats to a new mean and new standard deviation
        public static float[] scale(float[] aa, float mean, float sd)
        {
            float[] bb = Standardize.standardize(aa);
            int n = aa.Length;
            for (int i = 0; i < n; i++)
            {
                bb[i] = bb[i]*sd + mean;
            }

            return bb;
        }

        // Scale an array of longs to a new mean and new standard deviation
        public static double[] scale(long[] aa, double mean, double sd)
        {
            double[] bb = Standardize.standardize(aa);
            int n = aa.Length;
            for (int i = 0; i < n; i++)
            {
                bb[i] = bb[i]*sd + mean;
            }

            return bb;
        }

        // Scale an array of longs to a new mean and new standard deviation
        public static double[] scale(int[] aa, double mean, double sd)
        {
            double[] bb = Standardize.standardize(aa);
            int n = aa.Length;
            for (int i = 0; i < n; i++)
            {
                bb[i] = bb[i]*sd + mean;
            }

            return bb;
        }

        // Scale an array of BigDecimals to a new mean and new standard deviation
        public static double[] scale(decimal[] aa, double mean, double sd)
        {
            double[] bb = Standardize.standardize(aa);
            int n = aa.Length;
            for (int i = 0; i < n; i++)
            {
                bb[i] = bb[i]*sd + mean;
            }

            return bb;
        }
    }
}
