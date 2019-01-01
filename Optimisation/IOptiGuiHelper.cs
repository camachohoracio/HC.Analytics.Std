using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Core;

namespace HC.Analytics.Optimisation
{
    public interface IOptiGuiHelper
    {
        SelfDescribingTsEvent SolverStats { get; set; }
        void PublishLog(string s);
        void UpdateStats();
        void SetFinishStats();
        int SetProgress(bool blnImprovementFound, int intPercentage);
        void UpdateConvergence(Individual bestIndividual);
        void Dispose();
        void PublishGridStats();
    }
}