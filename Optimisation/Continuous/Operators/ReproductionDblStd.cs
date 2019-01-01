#region

using System;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Reproduction;
using HC.Analytics.Optimisation.Base.Operators.Selection;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Continuous.Operators.Crossover;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators
{
    [Serializable]
    public class ReproductionDblStd : AbstractReproduction
    {
        #region Members

        private readonly ICrossover m_deCrossover;
        private readonly ICrossover m_edaCrossover;
        private readonly ISelection m_mixedSelection;

        #endregion

        #region Constructor

        public ReproductionDblStd(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_mixedSelection = new MixedSelection(heuristicProblem);
            m_deCrossover = new DeCrossoverDbl(heuristicProblem);
            m_edaCrossover = new EdaCrossoverDbl(heuristicProblem);
        }

        #endregion

        public override Individual DoReproduction()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            //
            // decide the type of reproduction. Reproduction types are
            // DE or EDA
            //
            if (rng.NextDouble() <=
                Constants.DBL_DE_REPRODUCTION)
            {
                var parents = SelectParents();
                return m_deCrossover.DoCrossover(rng, parents);
            }
            return m_edaCrossover.DoCrossover(rng, null);
        }

        public override void ClusterInstance(Individual individual)
        {
        }

        private Individual[] SelectParents()
        {
            var parents = new Individual[4];

            parents[0] = m_mixedSelection.DoSelection();
            parents[1] = m_mixedSelection.DoSelection();
            parents[2] = m_mixedSelection.DoSelection();
            parents[3] = m_mixedSelection.DoSelection();

            return parents;
        }
    }
}
