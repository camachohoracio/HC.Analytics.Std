namespace HC.Analytics.Mathematics
{
    /// <summary>
    /// Inequality type
    /// </summary>
    public enum InequalityType
    {
        LESS_OR_EQUAL,
        GREATER_OR_EQUAL,
        LESS_THAN,
        GREATER_THAN,
        EQUALS,
        LIKE
    }

    public static class MathConstants
    {
        /// <summary>
        /// Round factor
        /// </summary>
        public const double DBL_ROUNDING_FACTOR = 1E-10;

        public const double BIG = 4.503599627370496e15;
        public const double BIG_INV = 2.22044604925031308085e-16;
        public const double DBL_BETA_DELTA = 0.01;
        public const int INT_FUNCTION_POINTS = 60;
        public const int INT_GAMMA_CASHED_SIZE = 5000000;
        public const int INT_PARAMETER_INTEGER_UPPER_BOUND = 500;
        public const int INT_PARAMETER_UPPER_BOUND = 5;
        public const double MACHEP = 1.11022302462515654042E-16;
        public const double MACHINE_EPSILON = 5E-16;
        public const double MAX_REAL_NUMBER = 1E300;
        public const double MAXLOG = 7.09782712893383996732E2;
        public const double MIN_REAL_NUMBER = 1E-300;
        public const int ROUND_DECIMALS = 15;
        public const double ROUND_ERROR = 1E-11;
    }
}
