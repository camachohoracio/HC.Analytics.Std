namespace HC.Analytics.Optimisation.Continuous
{
    public static class Constants
    {
        /// <summary>
        ///   DE mutation factor
        /// </summary>
        public const double DBL_DE_MUATION_FACTOR = 0.8;

        /// <summary>
        ///   Probability of reproducing an individual 
        ///   via differential evolution
        /// </summary>
        public const double DBL_DE_REPRODUCTION = 0.9;

        /// <summary>
        ///   DE convergence value
        /// </summary>
        public const int INT_DE_CONVERGENCE = 40000;

        public const int INT_DE_SMALL_CONVERGENCE = 40;

        public const int INT_NM_CONVERGENCE = 4000;

        /// <summary>
        ///   Differential evolution small convergence.
        ///   Value used for small problems
        /// </summary>
        public const int INT_SMALL_PROBLEM_DE = 100;

        /// <summary>
        ///   Simplex model name
        /// </summary>
        public const string STR_SIMPLEX_MODEL_FILENAME =
            "SimplexModel.mod";
    }
}
