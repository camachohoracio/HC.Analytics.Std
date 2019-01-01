#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch
{
    [Serializable]
    public class GpStratLocalSrch : AbstractLocalSearch
    {
        #region Members

        private readonly List<ILocalSearch> m_gpLocalSearchList;

        #endregion

        #region Constructors

        public GpStratLocalSrch(
            HeuristicProblem heuristicProblem,
            List<ILocalSearch> localSearchList) :
                base(heuristicProblem)
        {
            m_gpLocalSearchList = new List<ILocalSearch>(localSearchList);

            //m_gpLocalSearchList.Add(new GpStratVarLocalSrch(
            //                            heuristicProblem,
            //                            gpOperatorsContainer,
            //                            intTimeWindow));
        }

        #endregion

        #region Public

        public override void DoLocalSearch(
            Individual individual)
        {
            var rng = HeuristicProblem.CreateRandomGenerator();

            rng.ShuffleList(m_gpLocalSearchList);
            foreach (ILocalSearch localSearch in m_gpLocalSearchList)
            {
                localSearch.DoLocalSearch(individual);
            }
        }

        #endregion
    }
}
