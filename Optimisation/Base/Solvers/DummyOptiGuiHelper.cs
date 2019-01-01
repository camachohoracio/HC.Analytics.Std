using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Core;

namespace HC.Analytics.Optimisation.Base.Solvers
{
    public class DummyOptiGuiHelper : IOptiGuiHelper
    {
        public DummyOptiGuiHelper(EvolutionarySolver evolutionarySolver)
        {
        }

        public SelfDescribingTsEvent SolverStats { get; set; }

        public void PublishLog(string s)
        {
        }

        public void UpdateStats()
        {
        }

        public void SetFinishStats()
        {
        }

        public int SetProgress(bool blnImprovementFound, int intPercentage)
        {
            return -1;
        }

        public void UpdateConvergence(Individual bestIndividual)
        {
        }

        public void Dispose()
        {
        }

        public void PublishGridStats()
        {
        }
    }
}