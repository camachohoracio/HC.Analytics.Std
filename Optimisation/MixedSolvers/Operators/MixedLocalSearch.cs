#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Binary.Operators.LocalSearch;
using HC.Analytics.Optimisation.Continuous.Operators.LocalSearch;
using HC.Analytics.Optimisation.Integer.Operators.LocalSearch;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.Operators
{
    [Serializable]
    public class MixedLocalSearch : AbstractLocalSearch
    {
        #region Members

        private readonly List<ILocalSearch> m_localSearchList;

        #endregion

        #region Constructors

        public MixedLocalSearch(
            HeuristicProblem heuristicProblem,
            List<HeuristicProblem> heuristicProblems) :
                base(heuristicProblem)
        {
            m_localSearchList = new List<ILocalSearch>();

            foreach (HeuristicProblem problem in heuristicProblems)
            {
                if (problem != null)
                {
                    ILocalSearch currentLocalSearch = null;
                    switch (problem.EnumOptimimisationPoblemType)
                    {
                        case EnumOptimimisationPoblemType.INTEGER:
                            currentLocalSearch = new LocalSearchStdInt(problem);
                            break;
                        case EnumOptimimisationPoblemType.BINARY:
                            currentLocalSearch = new LocalSearchStdBln(problem);
                            break;
                        case EnumOptimimisationPoblemType.CONTINUOUS:
                            currentLocalSearch = new LocalSearchStdDbl(problem);
                            break;
                        case EnumOptimimisationPoblemType.GENETIC_PROGRAMMING:
                            // do nothing
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    if (currentLocalSearch != null)
                    {
                        m_localSearchList.Add(currentLocalSearch);
                    }
                }
            }
        }

        #endregion

        #region Public

        public override void DoLocalSearch(Individual individual)
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            var localSearchList = new List<ILocalSearch>(m_localSearchList);
            rng.ShuffleList(localSearchList);

            foreach (ILocalSearch localSearch in localSearchList)
            {
                localSearch.DoLocalSearch(individual);
            }
        }

        #endregion
    }
}
