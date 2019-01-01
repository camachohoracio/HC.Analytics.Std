#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    /// <summary>
    ///   Combines multiple selection operators 
    ///   according to a likelihood value
    /// </summary>
    [Serializable]
    public class MixedSelection : AbstractSelection
    {
        #region Members

        private readonly ISelection m_randomSelection;
        private readonly ISelection m_rankSelection;
        private readonly ISelection m_tournamentSelection;

        #endregion

        #region Constructor

        public MixedSelection(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_tournamentSelection = new TournamentSelection(
                m_heuristicProblem);
            m_randomSelection = new RandomSelection(
                m_heuristicProblem);
            m_rankSelection = new RankSelection(
                m_heuristicProblem);
        }

        #endregion

        public override Individual DoSelection()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            ISelection selection = null;

            if (rng.NextDouble() > 0.8)
            {
                selection = m_randomSelection;
            }
            else if (rng.NextDouble() > 0.5)
            {
                selection = m_tournamentSelection;
            }
            else
            {
                selection = m_rankSelection;
            }

            return selection.DoSelection();
        }
    }
}
