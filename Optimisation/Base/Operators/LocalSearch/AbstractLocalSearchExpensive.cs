#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    [Serializable]
    public abstract class AbstractLocalSearchExpensive : AbstractLocalSearch
    {
        #region Members

        /// <summary>
        ///   Number of iterations to be computed by the neighbourhood algorithm
        /// </summary>
        protected const int EXPENSIVE_NEIGHBOURHOOD = 4;

        private readonly ILocalSearch m_localSearchNerestNeighbourDbl;
        private readonly AbstractLocalSearchNm m_nmLocalSearch;

        #endregion

        #region Constructors

        public AbstractLocalSearchExpensive(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_nmLocalSearch = CreateLocalSearchNM();
            m_localSearchNerestNeighbourDbl = CreateLocalSearchNearN();
        }

        #endregion

        #region Public Methods

        public override void DoLocalSearch(
            Individual individual)
        {
            if (m_nmLocalSearch.ValidateNmSolver(m_intLocaSearchIterations))
            {
                m_intLocaSearchIterations = 0;
                m_nmLocalSearch.DoLocalSearch(individual);
            }
            else
            {
                m_localSearchNerestNeighbourDbl.DoLocalSearch(
                    individual);
            }


            m_intLocaSearchIterations++;
        }

        #endregion

        #region AbstractMethods

        public abstract ILocalSearch CreateLocalSearchNearN();
        public abstract AbstractLocalSearchNm CreateLocalSearchNM();

        #endregion
    }
}
