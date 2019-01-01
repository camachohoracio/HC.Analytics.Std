#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    [Serializable]
    public abstract class AbstractLocalSearchSimple : AbstractLocalSearch
    {
        #region Members

        /// <summary>
        ///   Number of iterations to be computed by the neighbourhood algorithm
        /// </summary>
        protected const int SIMPLE_NEIGHBOURHOOD = 2;

        private readonly ILocalSearch m_localSearchNearestNeigh;

        #endregion

        #region Constructors

        public AbstractLocalSearchSimple(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_localSearchNearestNeigh = CreateLocalSearchNearN();
        }

        #endregion

        public override void DoLocalSearch(
            Individual individual)
        {
            m_localSearchNearestNeigh.DoLocalSearch(individual);
        }

        #region AbstractMethods

        public abstract ILocalSearch CreateLocalSearchNearN();

        #endregion
    }
}
