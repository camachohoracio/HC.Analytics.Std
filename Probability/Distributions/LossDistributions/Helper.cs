namespace HC.Analytics.Probability.Distributions.LossDistributions
{
    public static class Helper
    {
        /// <summary>
        /// Get an array of threshold values
        /// </summary>
        /// <param name="dblMinLossThreshold">
        /// Min threshold
        /// </param>
        /// <param name="dblMaxLossThreshold">
        /// Max threshold
        /// </param>
        /// <param name="intPoints">
        /// Number of EP points
        /// </param>
        /// <returns></returns>
        public static double[] GetThresholdArray(
            double dblMinLossThreshold,
            double dblMaxLossThreshold,
            int intPoints)
        {
            double[] thresholdArray = new double[intPoints];
            double dblDelta = dblMaxLossThreshold/((double) intPoints - 1);
            double dblThreshold = dblMinLossThreshold;
            for (int i = 0; i < intPoints; i++)
            {
                thresholdArray[i] = dblThreshold;
                dblThreshold += dblDelta;
            }
            return thresholdArray;
        }
    }
}
