namespace HC.Analytics.Optimisation.Binary
{
    public static class Constants
    {
        /// <summary>
        ///   Alpha parameter. Cooling factor.
        /// </summary>
        public const double DBL_ALPHA = 0.95;

        /// <summary>
        ///   Probability of doing a greedy repair
        /// </summary>
        public const double DBL_GREEDY_REPAIR = 0.7;


        /// <summary>
        ///   Probability of mooving to a neighbourhood location
        /// </summary>
        public const double DBL_NEIGHBOURHOOD = 0.1;

        /// <summary>
        ///   Intial temperature
        /// </summary>
        public const double DBL_TEMPERATURE = 100;

        /// <summary>
        ///   Local search iterations
        /// </summary>
        public const int INT_LOCAL_SEARCH_ITERAIONS_BINARY = 10;

        public const int INT_POPULATION_SIZE_BINARY = 40;

        /// <summary>
        ///   SA Convergence
        /// </summary>
        public const int INT_SA_CONVERGENCE = 100;

        /// <summary>
        ///   SA Convergence used for small problems
        /// </summary>
        public const int INT_SA_SMALL_CONVERGENCE = 50;


        /// <summary>
        ///   SA small convergence used for small problems
        /// </summary>
        public const int INT_SMALL_PROBLEM_SA = 30;

        /// <summary>
        ///   Tempreature iterations
        /// </summary>
        public const int INT_TEMPERATURE_ITERATIONS = 100;
    }
}
