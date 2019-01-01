namespace HC.Analytics.Optimisation.Base
{
    public enum EnumOptimimisationPoblemType
    {
        BINARY,
        INTEGER,
        CONTINUOUS,
        MIXED,
        GENETIC_PROGRAMMING
    }

    /// <summary>
    ///   Objective function type
    /// </summary>
    public enum ObjectiveFunctionType
    {
        STD_OBJECTIVE_FUNCT,
        MULTI_OBJECTIVE_FUNCT,
        MIXED
    }

    /// <summary>
    ///   Evaluation state
    /// </summary>
    public enum EvaluationStateType
    {
        INITIAL_STATE,
        SUCCESS_EVALUATION,
        FAILURE_EVALUATION
    }

    public static class OptimisationConstants
    {
        public const double LOCAL_SEARCH_POPULATON_FACTOR = 0.3;

        //
        // progress bar percentages
        // values must sum up to one
        //

        // converge factor
        /// <summary>
        ///   Crossover probability
        /// </summary>
        public const double CROSSOVER_PROB = 0.25;

        /// <summary>
        ///   Cheap vector metric alpha parameter
        /// </summary>
        public const double DBL_CHEAP_METRIC_ALPHA = 0.7;

        /// <summary>
        ///   Cheap similarity threshold
        /// </summary>
        public const double DBL_CHEAP_THRESHOLD = 0.1;

        public const double DBL_CONVERGENCE_FACTOR = 0.9;

        //
        // clustering constants
        //
        /// <summary>
        ///   Expensive similarity threshold
        /// </summary>
        public const double DBL_EXPENSIVE_THRESHOLD = 0.8;

        /// <summary>
        ///   Extensive local search probability
        /// </summary>
        public const double DBL_EXTENSIVE_LOCAL_SEARCH = 0.30;

        /// <summary>
        ///   Probability of force mutation operator
        /// </summary>
        public const double DBL_FORCE_MUATION = 0.9;

        // Guided mutation constants

        /// <summary>
        ///   Guided mutation beta
        /// </summary>
        public const double DBL_GM_BETA = 0.9;

        /// <summary>
        ///   Guided mutation alpha
        /// </summary>
        public const double DBL_GM_LAMBDA = 0.5;

        /// <summary>
        ///   Local search probability
        /// </summary>
        public const double DBL_LOCAL_SEARCH = 0.25;

        /// <summary>
        ///   Mutation factor
        /// </summary>
        public const double DBL_MUATION_FACTOR = 0.001;

        /// <summary>
        ///   Probability of mutation for a continuous problem
        /// </summary>
        public const double DBL_MUTATION_CONTINUOUS = 0.8;


        /// <summary>
        ///   Select guided mutation operation
        /// </summary>
        public const double DBL_USE_GM = 0.6;

        /// <summary>
        ///   Number of Local search iterations to be executed before
        ///   expensive local search kicks in
        /// </summary>
        public const int EXPENSIVE_LOCAL_SERCH_ITERATIONS = 10;

        public const double GUIDED_MUTATION_REPRODUCTION_PROB = 0.1;

        /// <summary>
        ///   Number of variables to be evaluated by the 
        ///   cheap vector metric
        /// </summary>
        public const int INT_CHEAP_METRIC_WINDOW_SIZE = 1;

        /// <summary>
        ///   Parent size
        /// </summary>
        //public const int INT_LARGE_POPULATION_SIZE = 100;
        public const int INT_FINAL_RESULTS_PERCENTAGE = 5;

        /// <summary>
        ///   GA convergence value
        /// </summary>
        public const int INT_GA_CONVERGENCE = 40000;

        public const int INT_GA_SMALL_CONVERGENCE = 50;

        /// <summary>
        ///   Guided mutation population size
        /// </summary>
        public const int INT_GM_POPULATION = 10;

        public const int INT_INITAL_POOL_PERCENTAGE = 3;
        public const int INT_POPULATION_SIZE = 100;

        /// <summary>
        ///   GA convergence value used for small problems
        /// </summary>
        public const int INT_SMALL_PROBLEM_GA = 30;

        public const int INT_SOLVER_PERCENTAGE = 85;
        public const int INT_THREADS = 0;

        public const double MUTATION_PROBABILITY = 0.5;

        /// <summary>
        ///   Average number of chromosomes to be mutated
        /// </summary>
        public const double MUTATION_RATE = 0.13;


        /// <summary>
        ///   Maximum likelihood to be considered in order to 
        ///   go into one direcion in the search
        /// </summary>
        public const double SEARCH_DIRECTION_LIKELIHOOD = 0.8;

        public const double STD_REPRODUCTION_PROB = 0.9;
        public static int INT_INITAL_POPULATION_PERCENTAGE = 5;
        public static int INT_LOAD_DATA_PERCENTAGE = 2;
    }
}
