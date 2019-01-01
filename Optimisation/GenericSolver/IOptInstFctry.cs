using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.DynamicCompilation;

namespace HC.Analytics.Optimisation.GenericSolver
{
    public interface IOptInstFctry
    {
        HeuristicProblem HeuristicProblem { get; set; }
        bool AmIUnitTesting { get; set; }

        bool ContainsStatsCache(
            OptChromosomeWrapper chromosomeWrapper,
            out OptStatsCache optStatsCache);

        OptStatsCache GetOptStatsCache(
            OptChromosomeWrapper chromosomeWrapper,
            ASelfDescribingClass constantsToOptimise);
    }
}
