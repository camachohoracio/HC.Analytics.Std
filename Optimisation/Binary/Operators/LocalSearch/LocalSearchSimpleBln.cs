#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchSimpleBln : AbstractNearNeigLocalSearch0
    {
        #region Constructors

        public LocalSearchSimpleBln(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            Logger.Log("simpple bln local search");
        }

        #endregion

        public override void DoLocalSearch(Individual individual0)
        {
            var rng = HeuristicProblem.CreateRandomGenerator();
            var localIndividual = individual0;
            if (individual0.IndividualList != null &&
                individual0.IndividualList.Count > 0)
            {
                localIndividual = individual0.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }

            var bestReturn =
                m_heuristicProblem.ObjectiveFunction.Evaluate(
                individual0,
                m_heuristicProblem);
            var intChromosomeLength = m_heuristicProblem.VariableCount;

            //
            // check if go forward
            //
            var blnGoForward = m_searchDirectionOperator.CheckGoForward(
                rng);

            List<int> indexList;
            List<VariableContribution> indexListRanked;

            LocalSearchHelperBln.GetRankLists(
                localIndividual,
                rng,
                intChromosomeLength,
                blnGoForward,
                m_heuristicProblem,
                out indexList,
                out indexListRanked);


            var intOneCount = Math.Min(indexList.Count, indexListRanked.Count);
            var intTrialCount = 1;
            for (var intIndex = 0; intIndex < intOneCount; intIndex++)
            {
                IterateLocalSearch(
                    individual0,
                    ref bestReturn,
                    indexList,
                    indexListRanked,
                    intIndex,
                    blnGoForward);

                if (intTrialCount >=
                    Constants.INT_LOCAL_SEARCH_ITERAIONS_BINARY)
                {
                    break;
                }
                //}
                intTrialCount++;
            }
        }


        /// <summary>
        ///   Iterate local search
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <param name = "bestReturn">
        ///   Best return
        /// </param>
        /// <param name = "oneListRanked">
        ///   List with variables in current solution
        /// </param>
        /// <param name = "zeroListRanked">
        ///   List with variables not included in the solution but
        ///   ranked by likelihood
        /// </param>
        /// <param name = "intIndex">
        ///   IIndividual index
        /// </param>
        /// <param name = "cc">
        ///   Cluster
        /// </param>
        private void IterateLocalSearch(
            Individual individual0,
            ref double bestReturn,
            List<int> indexList,
            List<VariableContribution> indexListRanked,
            int intIndex,
            bool blnGoForward)
        {
            var localIndividual = individual0;
            if (individual0.IndividualList != null &&
                individual0.IndividualList.Count > 0)
            {
                localIndividual = individual0.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }

            bool blnOriginalState;
            int intCurrentIndex;
            int intCurrentIndexRanked;
            double currentReturn;

            intCurrentIndex = indexList[intIndex];
            intCurrentIndexRanked = indexListRanked[intIndex].Index;

            localIndividual.SetChromosomeValueBln(intCurrentIndexRanked, blnGoForward);

            var blnGlobalImprovement = false;

            if (!m_heuristicProblem.CheckConstraints(individual0))
            {
                blnOriginalState = false;
                localIndividual.SetChromosomeValueBln(intCurrentIndex, !blnGoForward);

                if (m_heuristicProblem.CheckConstraints(individual0))
                {
                    currentReturn =
                        m_heuristicProblem.ObjectiveFunction.Evaluate(
                            individual0,
                            m_heuristicProblem);

                    if (currentReturn > bestReturn)
                    {
                        bestReturn = currentReturn;
                        blnGlobalImprovement = true;
                    }
                    else
                    {
                        blnOriginalState = true;
                    }
                }
                else
                {
                    blnOriginalState = true;
                }
                if (blnOriginalState)
                {
                    // back to original state
                    localIndividual.SetChromosomeValueBln(intCurrentIndexRanked, !blnGoForward);
                    localIndividual.SetChromosomeValueBln(intCurrentIndex, blnGoForward);
                }
            }
            else
            {
                currentReturn =
                    m_heuristicProblem.ObjectiveFunction.Evaluate(
                        individual0,
                    m_heuristicProblem);

                if (currentReturn > bestReturn)
                {
                    bestReturn = currentReturn;
                    blnGlobalImprovement = true;
                }
                else
                {
                    // back to original state
                    localIndividual.SetChromosomeValueBln(intCurrentIndexRanked, !blnGoForward);
                }
            }
            //
            // set global improvement
            //
            if (blnGlobalImprovement)
            {
                m_searchDirectionOperator.SetImprovementCounter(
                    intIndex,
                    blnGoForward);
            }
        }
    }
}
