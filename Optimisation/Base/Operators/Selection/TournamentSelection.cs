#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    [Serializable]
    public class TournamentSelection : AbstractSelection
    {
        #region Memebers

        private const int M_INT_K = 2; //tournament size

        #endregion

        #region Constructor

        public TournamentSelection(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override Individual DoSelection()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            var intPopSize = m_heuristicProblem.PopulationSize;

            int best = rng.NextInt(0, intPopSize - 1);
            double bestRank =
                m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    best).Fitness;

            for (var i = 1; i < M_INT_K; i++)
            {
                int competitor =
                    rng.NextInt(0, intPopSize - 1);
                if (m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    competitor).Fitness <
                    bestRank)
                {
                    best = competitor;
                    bestRank = m_heuristicProblem.Population.GetIndividualFromPopulation(
                        m_heuristicProblem,
                        competitor).Fitness;
                }
            }
            return m_heuristicProblem.Population.GetIndividualFromPopulation(
                m_heuristicProblem,
                best);
        }
    }
}
