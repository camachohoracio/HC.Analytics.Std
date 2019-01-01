#region

using HC.Core.DynamicCompilation;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptStatsCache
    {
        #region Properties

        public ASelfDescribingClass StrategyParams { get; set; }
        public ASelfDescribingClass StatsMap { get; set; }

        #endregion
    }
}
