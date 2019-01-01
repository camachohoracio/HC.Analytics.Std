namespace HC.Analytics.MachineLearning.TextMining
{
    public static class TextMiningConstants
    {
        //
        // Merger constants
        //
        public const double DBL_CHEAP_LINK_THRESHOLD = 0.9;
        public const double DBL_CHEAP_LINK_TOKEN_ALPHA = 0.7;
        public const double DBL_MERGER_CHEAP_THRESHOLD_1 = 0.1;
        public const double DBL_MERGER_CHEAP_THRESHOLD_2 = 0.2;
        public const double DBL_MERGER_CHEAP_THRESHOLD_3 = 0.55;
        public const double DBL_MERGER_THRESHOLD = 0.6;
        public const double PURGER_CHEAP_THRESHOLD_1 = 0.1;
        public const double PURGER_CHEAP_THRESHOLD_2 = 0.2;
        public const double PURGER_CHEAP_THRESHOLD_3 = 0.55;


        //
        // Searcher constants
        //
        public const double DBL_SEARCHER_CHEAP_THRESHOLD_1 = 0.1;
        public const double DBL_SEARCHER_CHEAP_THRESHOLD_2 = 0.2;
        public const double DBL_SEARCHER_CHEAP_THRESHOLD_3 = 0.55;


        //
        // CheapLink constrants
        //
        public const int CHEAP_LINK_TOKEN_WINDOW = 1;
        public const int CHEAP_LINK_WINDOW = 3;

        /// <summary>
        /// define the number each record will search for a match 
        /// </summary>
        public const int INT_MERGER_SEARCH_LENGTH = 1000;

    }
}
