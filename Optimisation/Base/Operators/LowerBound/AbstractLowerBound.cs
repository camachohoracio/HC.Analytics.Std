#region

using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LowerBound
{
    public abstract class AbstractLowerBound : ILowerBound
    {
        #region Members

        protected HeuristicProblem m_heuristicProblem;
        private List<VariableContribution> m_lowerBoundList;

        #endregion

        #region ILowerBound Members

        public abstract Individual GetLowerBound();

        #endregion

        #region Constructor

        public AbstractLowerBound(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
        }

        #endregion

        /// <summary>
        ///   Get a list of variables ranked by likelihood of being
        ///   included in the solution. The likelihood is measured
        ///   by the rate of risk/reward
        /// </summary>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "repository">
        ///   Repository
        /// </param>
        /// <returns>
        ///   Lower bound list
        /// </returns>
        protected List<VariableContribution> GetLowerBoundList()
        {
            if (m_lowerBoundList == null)
            {
                var allRanksList =
                    new List<List<VariableContribution>>(
                        m_heuristicProblem.Constraints.ListConstraints.Count);

                foreach (IHeuristicConstraint constraint in
                    m_heuristicProblem.Constraints.ListConstraints)
                {
                    List<VariableContribution> currentRiskRank =
                        constraint.GetRankList();
                    var currentReturnRank = -1;

                    List<VariableContribution> currentRank =
                        new List<VariableContribution>(currentReturnRank);
                    for (var i = 0; i < currentRiskRank.Count; i++)
                    {
                        var index = currentRank.IndexOf(currentRiskRank[i]);
                        currentRank.ElementAt(index).Contribution /=
                            currentRiskRank[i].Contribution == 0.0
                                ? 1.0
                                : currentRiskRank[i].Contribution;
                    }
                    allRanksList.Add(currentRank);
                }


                var resultRankList = new List<VariableContribution>();
                for (var i = 0;
                     i < m_heuristicProblem.ObjectiveFunction.VariableCount;
                     i++)
                {
                    resultRankList.Add(new VariableContribution(i, 0));
                }

                var intRepositorySize =
                    m_heuristicProblem.ObjectiveFunction.VariableCount;

                foreach (List<VariableContribution> rankList in allRanksList)
                {
                    rankList.Sort();
                    for (var i = 0; i < intRepositorySize; i++)
                    {
                        resultRankList[rankList[i].Index].Contribution += intRepositorySize - i;
                    }
                }

                m_lowerBoundList = resultRankList;
            }

            return m_lowerBoundList;
        }
    }
}
