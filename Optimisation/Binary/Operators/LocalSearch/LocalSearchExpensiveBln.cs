#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators.LocalSearch
{
    [Serializable]
    public class LocalSearchExpensiveBln : AbstractNearNeigLocalSearch0
    {
        #region Members

        private int m_intIterations = Constants.INT_LOCAL_SEARCH_ITERAIONS_BINARY;

        #endregion

        #region Constructors

        public LocalSearchExpensiveBln(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            Logger.Log("Expensive bln local search");
        }

        #endregion

        public override void DoLocalSearch(
            Individual individual0)
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

            var intCurrentIteration = 0;

            foreach (VariableContribution variableContribution
                in indexListRanked)
            {
                var intIndex = variableContribution.Index;
                if (intCurrentIteration < m_intIterations)
                {
                    IterateLocalSearch(
                        individual0,
                        ref bestReturn,
                        indexList,
                        intIndex,
                        blnGoForward);
                }
                else
                {
                    break;
                }
                intCurrentIteration++;
            }

            individual0.Evaluate(
                false,
                false,
                false,
                m_heuristicProblem);

            if (bestReturn > individual0.Fitness)
            {
                throw new HCException("Error. Local search.");
            }
        }


        /// <summary>
        ///   Iterate extensive local search
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
        /// <param name = "oneList">
        ///   List with variables included in the solution
        /// </param>
        /// <param name = "intIndexZero">
        ///   List with variables not included in the solution
        /// </param>
        /// <param name = "cc">
        ///   Cluster class
        /// </param>
        /// <param name = "intIterations">
        ///   Number of iterations
        /// </param>
        private void IterateLocalSearch(
            Individual individual0,
            ref double bestReturn,
            List<int> indexList,
            int intIndex,
            bool blnGoForward)
        {
            double currentReturn = 0;
            bool blnImprovementFound;
            var blnGlobalImprovement = false;
            var localIndividual = individual0;
            if (individual0.IndividualList != null &&
                individual0.IndividualList.Count > 0)
            {
                localIndividual = individual0.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }

            //
            // swap the zero index by one
            //
            localIndividual.SetChromosomeValueBln(intIndex, blnGoForward);
            int intBestIndex;
            //
            // iterate each of the one list and swap its values.
            // Then get the index which provides the best
            // fitness.
            //
            if (!m_heuristicProblem.CheckConstraints(individual0))
            {
                blnImprovementFound = false;
                intBestIndex = -1;
                var intCurrentIteration = 0;
                foreach (int intIndexOne in indexList)
                {
                    if (intCurrentIteration < m_intIterations)
                    {
                        localIndividual.SetChromosomeValueBln(intIndexOne, !blnGoForward);

                        if (m_heuristicProblem.CheckConstraints(individual0))
                        {
                            currentReturn =
                                m_heuristicProblem.ObjectiveFunction.Evaluate(
                                individual0,
                                m_heuristicProblem);
                            if (currentReturn > bestReturn)
                            {
                                intBestIndex = intIndexOne;
                                bestReturn = currentReturn;
                                blnImprovementFound = true;
                                blnGlobalImprovement = true;
                            }
                        }
                        // back to original state
                        localIndividual.SetChromosomeValueBln(intIndexOne, blnGoForward);
                    }
                    else
                    {
                        break;
                    }
                    intCurrentIteration++;
                }

                if (blnImprovementFound)
                {
                    // remove the one index from the list
                    // and keep the zero index in the solution
                    localIndividual.SetChromosomeValueBln(intBestIndex, !blnGoForward);
                    indexList.Remove(intBestIndex);
                }
                else
                {
                    // back to original state
                    localIndividual.SetChromosomeValueBln(intIndex, !blnGoForward);
                }
            }
            else
            {
                //
                // check if return has been improved
                //
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
                    localIndividual.SetChromosomeValueBln(intIndex, !blnGoForward);
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
