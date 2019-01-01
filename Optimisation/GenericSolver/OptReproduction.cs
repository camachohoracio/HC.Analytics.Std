#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers.Operators;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptReproduction : MixedReproductionGeneric
    {
        #region Members

        private readonly OptChromosomeFactory m_optChromosomeFactory;

        #endregion

        public OptReproduction(
            HeuristicProblem heuristicProblem, 
            List<HeuristicProblem> heuristicProblems, 
            GpOperatorsContainer gpOperatorsContainer,
            OptChromosomeFactory optChromosomeFactory) : 
                base(heuristicProblem, 
                    heuristicProblems, 
                    gpOperatorsContainer)
        {
            m_optChromosomeFactory = optChromosomeFactory;
        }

        public override Individual DoReproduction()
        {
            var individual = base.DoReproduction();

            //
            // get inner individuals
            //
            RepairChromosomeHelper.FixIndividual(
                individual,
                m_heuristicProblem,
                m_optChromosomeFactory);
            return individual;
        }
    }
}

