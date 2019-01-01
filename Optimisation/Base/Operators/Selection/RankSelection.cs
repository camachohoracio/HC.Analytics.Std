#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    /// <summary>
    ///   Select individuals form the ranked population.
    ///   The lower the rank index, the better the individual
    /// </summary>
    [Serializable]
    public class RankSelection : AbstractSelection
    {
        #region Memebers

        /// <summary>
        ///   Default tournament size
        /// </summary>
        private const int TOURNAMENT_SIZE = 2;

        /// <summary>
        ///   Tournament size
        /// </summary>
        private readonly int m_intTournamentSize;

        #endregion

        #region Constructor

        public RankSelection(
            HeuristicProblem heuristicProblem) :
                this(heuristicProblem,
                     TOURNAMENT_SIZE)
        {
        }

        public RankSelection(
            HeuristicProblem heuristicProblem,
            int intTournamentSize) :
                base(heuristicProblem)
        {
            m_intTournamentSize = intTournamentSize;
        }

        #endregion

        public override Individual DoSelection()
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            var intNumb = rng.NextInt(0, m_heuristicProblem.PopulationSize - 1);

            for (var i = 1; i < m_intTournamentSize; i++)
            {
                intNumb = Math.Min(
                    intNumb,
                    rng.NextInt(0, m_heuristicProblem.PopulationSize - 1));
            }
            return m_heuristicProblem.Population.GetIndividualFromPopulation(
                m_heuristicProblem,
                intNumb);
        }
    }
}
