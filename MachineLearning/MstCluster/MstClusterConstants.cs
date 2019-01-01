namespace HC.Analytics.MachineLearning.MstCluster
{
    public static class MstClusterConstants
    {
        /// <summary>
        /// Difference between the maximum score and 
        /// the actual score
        /// </summary>
        public const double NODE_DEGREES_THRESHOLD = 0.15;
        public const double BRANCH_LENGHT_THRESHOLD = 0.7;
        public const double ADJACENT_NODE = 0.01;
        public const double BRANCH_SIZE_THRESHOLD = 0.1;
        public const double EDGE_THRESHOLD = 0.65;
        public const int PURGER_SEARCH_LENGTH = 1000;
    }
}

