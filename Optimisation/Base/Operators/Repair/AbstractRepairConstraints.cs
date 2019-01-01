#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Repair
{
    /// <summary>
    ///   Repair continuous solution
    /// </summary>
    public abstract class AbstractRepairConstraints : AbstractRepair
    {
        #region Members

        /// <summary>
        ///   Number of default iterations
        /// </summary>
        private const int REPAIR_ITERATIONS = 3;

        private readonly int m_intIterations;

        private readonly SearchDirectionOperator m_searchDirectionOperator;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        public AbstractRepairConstraints(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_intIterations = REPAIR_ITERATIONS;
            m_searchDirectionOperator = new SearchDirectionOperator(
                heuristicProblem);
        }

        #endregion

        #region Public

        /// <summary>
        ///   Repair solution
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "reproduction">
        ///   Reproduction
        /// </param>
        /// <param name = "constratints">
        ///   Constraints
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        public override bool DoRepair(
            Individual individual)
        {
            //
            // return if the constraints are satisfied
            //
            var blnSatisfyConstraint = m_heuristicProblem.CheckConstraints(
                individual);

            if (blnSatisfyConstraint)
            {
                return true;
            }

            var dblInitialChromosomeArr = GetChromosomeCopy(individual);

            var rng = HeuristicProblem.CreateRandomGenerator();
            //
            // get array with flags indicating going either 
            // forwards or backwards in the repair
            // 
            var blnForwardList = GetForwardArr(rng);

            // get candidate list
            var candidateList =
                GetCandidateList(
                    individual,
                    blnForwardList,
                    rng);

            // set the number of trials
            // after that the weights will be removed
            var intCurrentIteration = 0;

            Individual nestedIndividual;
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                nestedIndividual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }
            else
            {
                nestedIndividual = individual;
            }

            while (!blnSatisfyConstraint)
            {
                if (candidateList.Count > 0)
                {
                    // remove a continuous ELT
                    var intIndex = candidateList[0].Index;

                    IterateRepair(
                        nestedIndividual,
                        m_intIterations,
                        intIndex,
                        intCurrentIteration,
                        blnForwardList[intIndex],
                        rng);

                    blnSatisfyConstraint =
                        m_heuristicProblem.CheckConstraints(
                            individual);
                    //
                    // end loop if constraint is validated
                    //
                    if (blnSatisfyConstraint)
                    {
                        //
                        // set direcion counter
                        //
                        m_searchDirectionOperator.SetImprovementCounter(
                            intIndex,
                            blnForwardList[intIndex]);
                        break;
                    }
                    intCurrentIteration++;
                }

                if (candidateList.Count == 0)
                {
                    //
                    // There are no more variables to remove from current solution.
                    // Therefore, go back to original state
                    //
                    if (rng.NextDouble() > 0.2)
                    {
                        BackToInitialState(
                            nestedIndividual,
                            dblInitialChromosomeArr);
                    }

                    return false;
                }

                //
                // new number list.
                // Use guided convergence in order to 
                // move to anohter less-likely variable which will be used
                // by the repair operator.
                //
                candidateList =
                    GetCandidateList(
                        individual,
                        blnForwardList,
                        rng);
            }
            return true;
        }

        private bool[] GetForwardArr(
            RngWrapper rng)
        {
            var blnForwardArr = new bool[m_heuristicProblem.VariableCount];
            for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
            {
                var blnGoForward =
                    m_searchDirectionOperator.CheckGoForward(
                        i,
                        rng);
                blnForwardArr[i] = blnGoForward;
            }
            return blnForwardArr;
        }

        public override void AddRepairOperator(IRepair repair)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private void IterateRepair(
            Individual individual,
            int intIterations,
            int intIndex,
            int intCurrentIteration,
            bool blnGoForward,
            RngWrapper rng)
        {
            var dblCurrentWeight = GetWeight(
                individual,
                intIterations,
                intIndex,
                intCurrentIteration,
                blnGoForward,
                rng);

            if (blnGoForward)
            {
                // add weight
                AddChromosomeValue(
                    individual,
                    intIndex,
                    dblCurrentWeight);
            }
            else
            {
                // remove weight
                RemoveChromosomeValue(
                    individual,
                    intIndex,
                    dblCurrentWeight);
            }
        }

        private double GetWeight(
            Individual individual,
            int intIterations,
            int intIndex,
            int intCurrentIteration,
            bool blnGoForward,
            RngWrapper rng)
        {
            try
            {
                double dblCurrentWeight;
                //
                // Load weight.
                // after a set of trials, load the whole weight
                //
                if (intCurrentIteration >= intIterations)
                {
                    dblCurrentWeight =
                        GetChromosomeValue(individual, intIndex);
                    var dblMaxWeight = GetMaxChromosomeValue(intIndex);

                    if (blnGoForward)
                    {
                        dblCurrentWeight = dblMaxWeight - dblCurrentWeight;
                    }
                }
                else
                {
                    //
                    // set same random weight for forward or backward
                    //
                    var dblCurrentChromosomeValue =
                        GetChromosomeValue(individual, intIndex);
                    dblCurrentWeight = rng.NextDouble()*
                                       dblCurrentChromosomeValue;
                    var dblMaxChromosomeValue = GetMaxChromosomeValue(intIndex);
                    if (dblCurrentWeight + dblCurrentChromosomeValue > dblMaxChromosomeValue)
                    {
                        dblCurrentWeight = dblMaxChromosomeValue - dblCurrentChromosomeValue;
                    }
                }
                //
                // rounding error
                //
                if (dblCurrentWeight < MathConstants.DBL_ROUNDING_FACTOR)
                {
                    dblCurrentWeight = Math.Min(
                        MathConstants.DBL_ROUNDING_FACTOR,
                        GetChromosomeValue(individual, intIndex));
                }
                return dblCurrentWeight;
            }
            catch (HCException e)
            {
                //Debugger.Break();
                throw;
            }
        }

        /// <summary>
        ///   Get candidate list of variables by removing variables
        ///   until the constraint is satisfied
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "reproduction">
        ///   Reproduciton
        /// </param>
        /// <returns>
        ///   Candidate list
        /// </returns>
        private List<VariableContribution> GetCandidateList(
            Individual individual,
            bool[] blnForwardList,
            RngWrapper rng)
        {
            var blnUseGm = true;
            if (rng.NextDouble() >
                OptimisationConstants.DBL_USE_GM)
            {
                blnUseGm = false;
            }

            var numberList =
                new List<VariableContribution>(
                    m_heuristicProblem.VariableCount + 1);

            for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
            {
                var blnAddVariable = false;
                if (blnForwardList[i])
                {
                    //
                    // validate values to be added
                    //
                    blnAddVariable = ValidateAddVariable(i, individual);
                }
                else
                {
                    //
                    // validate values to be removed
                    //
                    blnAddVariable = ValidateRemoveVariable(i, individual);
                }

                if (blnAddVariable)
                {
                    numberList.Add(
                        new VariableContribution(i,
                                                 !blnUseGm
                                                     ? rng.NextDouble()
                                                     : m_heuristicProblem.GuidedConvergence.GetGcProb(i) -
                                                       rng.NextDouble()));
                }
            }
            numberList.Sort();
            // reverse the list because we are interested in 
            // removing less probable variables
            numberList.Reverse();
            return numberList;
        }

        /// <summary>
        ///   Return current solution to its best state found so far
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "dblBestChromosomeArray">
        ///   Best chromosome
        /// </param>
        private void BackToInitialState(
            Individual individual,
            double[] dbInitialChromosomeArray)
        {
            for (var i = 0;
                 i <
                 m_heuristicProblem.VariableCount;
                 i++)
            {
                var dblCurrentWeight = dbInitialChromosomeArray[i] -
                                       GetChromosomeValue(individual, i);
                if (dblCurrentWeight > 0)
                {
                    AddChromosomeValue(
                        individual,
                        i,
                        dblCurrentWeight);
                }
                else
                {
                    RemoveChromosomeValue(
                        individual,
                        i,
                        Math.Abs(dblCurrentWeight));
                }
            }
        }

        #endregion

        #region AbstractMethods

        protected abstract double GetChromosomeValue(
            Individual individual,
            int intIndex);

        protected abstract void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract double[] GetChromosomeCopy(
            Individual individual);

        protected abstract double GetMaxChromosomeValue(int intIndex);

        protected abstract bool ValidateAddVariable(int intIndex, Individual individual);

        protected abstract bool ValidateRemoveVariable(int intIndex, Individual individual);

        #endregion
    }
}
