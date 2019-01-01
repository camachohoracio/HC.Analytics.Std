namespace HC.Analytics.Probability.Distributions.LossDistributions
{
    /// <summary>
    /// Constant class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Error allowed by the binary search precision.
        /// The smaller the error, the greather the number of iterations 
        /// required by the binary serch algorithm
        /// </summary>
        public const double DBL_BINARY_SEARCH_PRECISION = 1E-3;

        /// <summary>
        /// The smallest Poisson distribution probability
        /// </summary>
        public const double DBL_LOWER_BOUND_POISSON_DISTR = 1E-28;

        /// <summary>
        /// Max expousure size. The constant is multiplied by the maximum
        /// exposure from a given distribution,
        /// Ensures that the maximum exposure is not exceeded by any OEP value
        /// </summary>
        public const double DBL_MAX_EXPOSURE_SIZE = 2;

        /// <summary>
        /// The smallest CEP value to be calculated by the EP curve
        /// </summary>
        public const double DBL_TARGET_CEP = 1E-10;

        /// <summary>
        /// Number of AEP points
        /// </summary>
        public const int INT_AEP_POINTS = 8192;

        /// <summary>
        /// Number of iterations carried out by binary search
        /// </summary>
        public const int INT_BINARY_SEARCH_ITERATIONS = 100;

        /// <summary>
        /// Number of EP points to be calculated in the second pass.
        /// </summary>
        public const int INT_EP_POINTS_SECOND_PASS = 1024;
    }
}
