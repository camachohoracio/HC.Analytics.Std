#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.Repair;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators
{
    /// <summary>
    ///   Repair solution:
    ///   Sets the current solution into the predefined bounds by 
    ///   excluding elts from the given portfolio
    /// </summary>
    [Serializable]
    public class RepairConstraintsBln : IRepair
    {
        #region Members

        private readonly HeuristicProblem m_heuristicProblem;

        private readonly RngWrapper m_rngWrapper;

        #endregion

        #region Public

        /// <summary>
        ///   Repair solution
        /// </summary>
        public RepairConstraintsBln(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_rngWrapper = HeuristicProblem.CreateRandomGenerator();
        }

        public HeuristicProblem HeuristicOptmizationProblem_
        {
            get { return m_heuristicProblem; }
        }

        /// <summary>
        ///   Do repair
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        public bool DoRepair(
            Individual parentIndividual)
        {
            // do binary repair
            DoRepairBinary(false,
                           parentIndividual);
            //if (!m_heuristicProblem.CheckConstraints(parentIndividual))
            //{
            //    throw new HCException("Error. Repair operation failure.");
            //}
            return true;
        }

        public void AddRepairOperator(IRepair repair)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Repair binary Distributions
        /// </summary>
        /// <param name = "blnSecondProcess">
        ///   Check if it is the second process
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "reproduction">
        ///   Reproduction
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        public void DoRepairBinary(bool blnSecondProcess,
                                   Individual individual)
        {
            try
            {
                return;

                Individual parentIndividual = individual;

                if (individual.IndividualList != null &&
                    individual.IndividualList.Count > 0)
                {
                    individual = individual.GetIndividual(
                        m_heuristicProblem.ProblemName);
                }
                var removeList = new List<int>(
                    m_heuristicProblem.VariableCount);
                var addList = new List<int>(
                    m_heuristicProblem.VariableCount);

                for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
                {
                    if (individual.GetChromosomeValueBln(i))
                    {
                        removeList.Add(i);
                    }
                    else
                    {
                        addList.Add(i);
                    }
                }
                m_rngWrapper.Shuffle(removeList);


                //int q;
                ////bool accept = true;
                //int intNumbersCount = numbers.Count;
                //var listQ = new List<int>(intNumbersCount + 1);
                //for (var i = 0; i < intNumbersCount; i++)
                //{
                //    int rn = m_rngWrapper.NextInt(
                //        0, numbers.Count - 1);
                //    q = numbers[rn];
                //    numbers.RemoveAt(rn);
                //    if (individual.GetChromosomeValueBln(q))
                //    {
                //        listQ.Add(q);
                //    }
                //}
                //List<int> removeList = null;

                //var blnBuildForward = false;

                // fix chromosome
                //foreach (int index in removeList)
                //{
                //    individual.SetChromosomeValueBln(index, 1);
                //}

                bool blnGreedyRepair = m_rngWrapper.NextDouble() >=
                                       Constants.DBL_GREEDY_REPAIR;

                //if (blnBuildForward)
                //{
                //    //
                //    // Add elts until the limit is exhausted
                //    //
                //    if (blnGreedyRepair)
                //    {
                //        BuildForwardsGreedy(listQ,
                //                            individual);

                //        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                //        {
                //            throw new HCException("Error. Repair operation failure.");
                //        }
                //    }
                //    else
                //    {
                //        BuildForwards(listQ,
                //                      individual);

                //        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                //        {
                //            throw new HCException("Error. Repair operation failure.");
                //        }
                //    }
                //}
                //else
                {
                    //if (m_heuristicProblem.CheckConstraints(parentIndividual))
                    //{
                    //    // add remaining items
                    //    if (blnGreedyRepair)
                    //    {
                    //        BuildForwardsGreedy(listQ,
                    //                            individual);

                    //        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                    //        {
                    //            throw new HCException("Error. Repair operation failure.");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        BuildForwards(listQ,
                    //                      individual);
                    //        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                    //        {
                    //            throw new HCException("Error. Repair operation failure.");
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //
                    //    // Remove elts until the portfolio is in bounds
                    //    //
                    if (blnGreedyRepair)
                    {
                        RepairBackwardsGreedy(removeList,
                                             individual,
                                             parentIndividual);
                        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                        {
                            foreach (int i in removeList) // restore individual
                            {
                                individual.SetChromosomeValueBln(i, true);
                            }

                            RepairForwrds(addList,
                                                 individual,
                                                 parentIndividual);
                            if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                            {
                                throw new HCException("Error. Repair operation failure.");
                            }
                        }
                    }
                    else
                    {
                        RepairBackwards(removeList,
                                       individual,
                                       parentIndividual);
                        if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                        {
                            foreach (int i in removeList) // restore individual
                            {
                                individual.SetChromosomeValueBln(i, true);
                            }

                            RepairForwrds(addList,
                                                 individual,
                                                 parentIndividual);
                            if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                            {
                                throw new HCException("Error. Repair operation failure.");
                            }
                        }
                    }
                    //}
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Private

        /// <summary>
        ///   Remove Elts until the portfolio within the predefined bounds
        /// </summary>
        /// <param name = "qList">
        ///   List with candidate variables
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        ///   <param name = "reproduction">
        ///     Reproduction operator
        ///   </param>
        ///   <param name = "constraints">
        ///     Constraints
        ///   </param>
        private void RepairBackwards(
            List<int> qList,
            Individual individual,
            Individual parentIndividual)
        {
            var buildList = new List<VariableContribution>();
            double dblContribution;
            foreach (int intIndex in qList)
            {
                if (m_heuristicProblem.Reproduction != null)
                {
                    dblContribution =
                        m_heuristicProblem.GuidedConvergence.GetGcProb(intIndex) -
                        m_rngWrapper.NextDouble();
                }
                else
                {
                    dblContribution = 1.0;
                }
                var currentVariable = new VariableContribution(
                    intIndex,
                    dblContribution);
                buildList.Add(currentVariable);
            }
            buildList.Sort();
            // reverse the list, we are interested on removing elts which are less likely
            buildList.Reverse();
            foreach (VariableContribution currentVariable in buildList)
            {
                var intIndex = currentVariable.Index;
                individual.SetChromosomeValueBln(intIndex, false);

                if (m_heuristicProblem.CheckConstraints(parentIndividual))
                {
                    break;
                }
            }
        }

        private void RepairForwrds(
            List<int> qList,
            Individual individual,
            Individual parentIndividual)
        {
            var buildList = new List<VariableContribution>();
            double dblContribution;
            foreach (int intIndex in qList)
            {
                if (m_heuristicProblem.Reproduction != null)
                {
                    dblContribution =
                        m_heuristicProblem.GuidedConvergence.GetGcProb(intIndex) -
                        m_rngWrapper.NextDouble();
                }
                else
                {
                    dblContribution = 1.0;
                }
                var currentVariable = new VariableContribution(
                    intIndex,
                    dblContribution);
                buildList.Add(currentVariable);
            }
            buildList.Sort();
            foreach (VariableContribution currentVariable in buildList)
            {
                var intIndex = currentVariable.Index;
                individual.SetChromosomeValueBln(intIndex, true);

                if (m_heuristicProblem.CheckConstraints(parentIndividual))
                {
                    break;
                }
            }
        }


        /// <summary>
        ///   Build backwards.
        ///   The solution is out of bounds and variables have to 
        ///   be removed from the current solution
        /// </summary>
        /// <param name = "qList">
        ///   Candidate list
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        private void RepairBackwardsGreedy(
            List<int> qList,
            Individual individual,
            Individual parentIndividual)
        {
            //foreach (int intIndex in qList)
            while (qList.Count > 0)
            {
                var intIndex1 = qList[0];
                var intIndex2 = -1;
                var dblReturn1 = CheckBackwardsIndex(intIndex1,
                                                     individual,
                                                     parentIndividual);
                double dblReturn2 = -1;
                if (qList.Count > 1)
                {
                    // go to next index
                    intIndex2 = qList[1];
                    dblReturn2 = CheckBackwardsIndex(intIndex2,
                                                     individual,
                                                     parentIndividual);
                }
                if (dblReturn1 > dblReturn2 && dblReturn1 >= 0 && dblReturn2 >= 0)
                {
                    individual.SetChromosomeValueBln(intIndex2, false);
                    qList.RemoveAt(1);
                }
                else if (dblReturn2 >= dblReturn1 && dblReturn2 >= 0)
                {
                    individual.SetChromosomeValueBln(intIndex1, false);
                    qList.RemoveAt(0);
                }
                else
                {
                    // neither of the two passed the test
                    individual.SetChromosomeValueBln(intIndex1, false);
                    qList.RemoveAt(0);
                    if (qList.Count > 0)
                    {
                        qList.RemoveAt(0);
                    }
                }
                if (m_heuristicProblem.CheckConstraints(parentIndividual))
                {
                    return;
                }
            }
        }

        /// <summary>
        ///   Add Elts to current portfolio until it is out of bounds
        /// </summary>
        /// <param name = "qList">
        ///   candidate list
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "reproduction">
        ///   reproduction
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        private void BuildForwards(
            List<int> qList,
            Individual individual,
            Individual parentIndividual)
        {
            var buildList = new List<VariableContribution>();
            double dblContribution;
            foreach (int intIndex in qList)
            {
                if (m_heuristicProblem.Reproduction != null)
                {
                    dblContribution =
                        m_heuristicProblem.GuidedConvergence.GetGcProb(intIndex) -
                        m_rngWrapper.NextDouble();
                }
                else
                {
                    dblContribution = 1.0;
                }
                var currentVariable = new VariableContribution(
                    intIndex,
                    dblContribution);
                buildList.Add(currentVariable);
            }
            buildList.Sort();
            var intTrial = 0;
            var intMaxTrials = 10;

            foreach (VariableContribution currentVariable in buildList)
            {
                var intIndex = currentVariable.Index;
                individual.SetChromosomeValueBln(intIndex, true);

                if (!m_heuristicProblem.CheckConstraints(parentIndividual))
                {
                    individual.SetChromosomeValueBln(intIndex, false);

                    intTrial++;
                    if (intTrial >= intMaxTrials)
                    {
                        return;
                    }
                }
                else
                {
                    intTrial = 0;
                }
            }
        }

        /// <summary>
        ///   Build forwards. Add variables until the 
        ///   constraint is not satisfied.
        ///   The variables are added in a greedy fashion.
        /// </summary>
        /// <param name = "qList">
        ///   Candidate list
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "constratint">
        ///   Constraint
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        private void BuildForwardsGreedy(
            List<int> qList,
            Individual individual,
            Individual parentIndividual)
        {
            var intTrials = 0;
            var intMaxTrials = 10;
            //foreach (int intIndex in qList)
            while (qList.Count > 0)
            {
                var intIndex1 = qList[0];
                var intIndex2 = -1;
                var dblReturn1 = CheckForwardsIndex(intIndex1,
                                                    individual,
                                                    parentIndividual);

                double dblReturn2 = -1;
                if (qList.Count > 1)
                {
                    // go to next index
                    intIndex2 = qList[1];
                    dblReturn2 = CheckForwardsIndex(intIndex2,
                                                    individual,
                                                    parentIndividual);
                }
                if (dblReturn1 > dblReturn2 && dblReturn1 != -1)
                {
                    individual.SetChromosomeValueBln(intIndex1, true);
                    qList.RemoveAt(0);
                    intTrials = 0;
                }
                else if (dblReturn2 >= dblReturn1 && dblReturn2 != -1)
                {
                    individual.SetChromosomeValueBln(intIndex2, true);
                    qList.RemoveAt(1);
                    intTrials = 0;
                }
                else
                {
                    // neither of the two passed the test
                    qList.RemoveAt(0);
                    if (qList.Count > 0)
                    {
                        qList.RemoveAt(0);
                    }
                    intTrials++;
                    if (intTrials >= intMaxTrials)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///   Check the fitness by adding an index to 
        ///   a solutions
        /// </summary>
        /// <param name = "intIndex">
        ///   index
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <returns>
        ///   Fitness value
        /// </returns>
        private double CheckForwardsIndex(
            int intIndex,
            Individual individual,
            Individual parentIndividual)
        {
            double dblFitness = -1;
            individual.SetChromosomeValueBln(intIndex, true);
            if (m_heuristicProblem.CheckConstraints(parentIndividual))
            {
                dblFitness =
                    m_heuristicProblem.ObjectiveFunction.Evaluate(
                    parentIndividual,
                    m_heuristicProblem);
            }
            // remove elt from current portfolio
            individual.SetChromosomeValueBln(intIndex, false);
            return dblFitness;
        }

        /// <summary>
        ///   Check the fitness by removing an index to
        ///   a given individual
        /// </summary>
        /// <param name = "intIndex">
        ///   Index
        /// </param>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <returns></returns>
        private double CheckBackwardsIndex(
            int intIndex,
            Individual individual,
            Individual parentIndividual)
        {
            individual.SetChromosomeValueBln(intIndex, false);
            double dblReturn = m_heuristicProblem.ObjectiveFunction.Evaluate(
                parentIndividual,
                m_heuristicProblem);
            individual.SetChromosomeValueBln(intIndex, true);
            return dblReturn;
        }

        #endregion

        ~RepairConstraintsBln()
        {
            Dispose();
        }

        public void Dispose()
        {
            
        }
    }
}
