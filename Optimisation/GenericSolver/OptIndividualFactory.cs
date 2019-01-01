#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers.Operators;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptIndividualFactory : MixedIndividualFactoryGeneric
    {
        #region Members

        private readonly OptChromosomeFactory m_optChromosomeFactory;

        #endregion

        public OptIndividualFactory(
            HeuristicProblem heuristicProblem, 
            List<HeuristicProblem> heuristicProblemList,
            OptChromosomeFactory optChromosomeFactory): 
                base(heuristicProblem, heuristicProblemList)
        {
            m_optChromosomeFactory = optChromosomeFactory;
        }

        public override Individual BuildRandomIndividual()
        {
            var individual = base.BuildRandomIndividual();
            RepairChromosomeHelper.FixIndividual(
                individual,
                m_heuristicProblem,
                m_optChromosomeFactory);
            return individual;
        }
    }
}

