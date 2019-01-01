#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{
    /// <summary>
    ///   Local search nearest neighbourhood algorithm.
    ///   Search values close to the provided solution.
    ///   Iterates an specified number of times. 
    ///   Uses binary search as the search operator
    /// </summary>
    [Serializable]
    public abstract class AbstractNearNeigLocalSearch : AbstractNearNeigLocalSearch0
    {
        #region Members

        /// <summary>
        ///   Mutation factor applied to chormosome
        /// </summary>
        protected const double MUTATION_FACTOR = 0.05;

        /// <summary>
        ///   Proportion of chromosomes to be mutated by the nearest neighbourhood
        /// </summary>
        private const double MUTATION_RATE_NN = 0.5;

        /// <summary>
        ///   Factor applied to chromosome in order to seach in the neigbourhood
        /// </summary>
        private const double NEIGHBOURHOOD_FACTOR = 0.1;

        #endregion

        #region Constructors

        public AbstractNearNeigLocalSearch(
            HeuristicProblem heuristicProblem,
            int intSearchIterations) :
                base(heuristicProblem)
        {
            m_intSearchIterations = intSearchIterations;
        }

        #endregion

        public override void DoLocalSearch(
            Individual individual)
        {
            var blnCheckConstraint = m_heuristicProblem.CheckConstraints(individual);
            if (!blnCheckConstraint)
            {
                //Debugger.Break();
                throw new HCException("Error. Local search not valid.");
            }

            var rng = HeuristicProblem.CreateRandomGenerator();
            //
            // get initial conditions
            //
            var dblInitialChromosomeArray = GetChromosomeCopy(individual);
            var dblInitialReturn =
                m_heuristicProblem.ObjectiveFunction.Evaluate(
                individual,
                m_heuristicProblem);

            //
            // get shuffled index list
            //
            var numberList = GetChromosomeIndexList(rng);

            // itereate each element of the list
            for (var i = 0;
                 i < Math.Min(numberList.Count, m_heuristicProblem.PopulationSize*0.3);
                 i++)
            {
                var intIndex = numberList[i];
                //
                // decide to go either forwards or backwards
                // for the knapsack problem, it should go forwards
                // however, for portfolio management it can go in either direction
                //
                var blnGoForward = m_searchDirectionOperator.CheckGoForward(
                    intIndex,
                    rng);

                //
                // Randomly mutate the weights in the chromosome in the opposite direction
                // this will allow the nearest neighbourhood algorithm to use the extra
                // available weight in order to explore in the neighbourhood.
                //
                if (rng.NextDouble() >= MUTATION_RATE_NN)
                {
                    MutateChromosomeWeight(
                        individual,
                        intIndex,
                        rng,
                        !blnGoForward);
                }

                IterateNearestNeighbour(
                    individual,
                    dblInitialChromosomeArray,
                    ref dblInitialReturn,
                    intIndex,
                    rng,
                    blnGoForward);
            }

            blnCheckConstraint = m_heuristicProblem.CheckConstraints(individual);
            if (!blnCheckConstraint)
            {
                //Debugger.Break();
                throw new HCException("Error. Local search not valid.");
            }
        }

        private List<int> GetChromosomeIndexList(RngWrapper rng)
        {
            var numberList = new List<int>(
                m_heuristicProblem.VariableCount + 1);
            for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
            {
                numberList.Add(i);
            }
            // shuffle list
            rng.ShuffleList(numberList);
            return numberList;
        }

        private double GetUpperBoundWeight(
            double dblWeight,
            RngWrapper rng,
            int intIndex)
        {
            //
            // randomly decide if an upper bound is to be used
            // this allows either big or smal jumps by
            // the binary search
            //
            var blnUseUpperBound = rng.NextBln();

            //
            // set upper bound weight
            //
            double dblUpperBoundWeight;


            var dblMaxChromosomeValue = GetMaxChromosomeValue(intIndex);

            if (blnUseUpperBound)
            {
                dblUpperBoundWeight = (1 + NEIGHBOURHOOD_FACTOR)*dblWeight;

                if (dblUpperBoundWeight > dblMaxChromosomeValue)
                {
                    dblUpperBoundWeight = dblMaxChromosomeValue;
                }
            }
            else
            {
                dblUpperBoundWeight = dblMaxChromosomeValue;
            }
            return dblUpperBoundWeight;
        }

        private static double GetLowerBoundWeight(
            double dblWeight,
            RngWrapper rng)
        {
            //
            // randomly decide if an upper bound is to be used
            //
            var blnUseLowerBound = rng.NextBln();

            //
            // set upper bound weight
            //
            double dblLowerBoundWeight;

            if (blnUseLowerBound)
            {
                dblLowerBoundWeight = (1.0 - NEIGHBOURHOOD_FACTOR)*dblWeight;
            }
            else
            {
                dblLowerBoundWeight = 0.0;
            }
            return dblLowerBoundWeight;
        }

        private void IterateNearestNeighbour(
            Individual individual,
            double[] dblInitialChromosomeArray,
            ref double dblBestReturn,
            int intIndex,
            RngWrapper rng,
            bool blnGoForward)
        {
            //double[] dblInitialChromosomeArray = GetChromosomeCopy(individual);
            //
            // get weight bounds
            //
            double dblUpperBoundWeight;
            double dblLowerBoundWeight;
            GetLowerUpperBounds(
                individual,
                intIndex,
                rng,
                blnGoForward,
                out dblUpperBoundWeight,
                out dblLowerBoundWeight);

            if (Math.Abs(dblUpperBoundWeight - dblLowerBoundWeight) <
                MathConstants.ROUND_ERROR)
            {
                //
                // Lower bound equals upper bound.
                // Therefore, not worth local search.
                //
                return;
            }

            // declare improvement flag
            var blnImprovement = false;
            var blnGlobalImprovement = false;

            //
            // iterate neighbourhood sarch
            //
            for (var intCounter = 0; intCounter < m_intSearchIterations; intCounter++)
            {
                // reset improvement
                blnImprovement = false;
                //
                // Calculate neighbour weight.
                // Similar to the binary sarch algorithm: 
                // Place the current weight at the middle of the two ranges
                //
                var dblNeighbourWeight =
                    dblLowerBoundWeight + (dblUpperBoundWeight - dblLowerBoundWeight)/2.0;


                var dblWeight = dblNeighbourWeight -
                                GetChromosomeValue(individual, intIndex);

                if (Math.Abs(dblWeight) >
                    MathConstants.DBL_ROUNDING_FACTOR)
                {
                    if (dblWeight > 0)
                    {
                        AddChromosomeValue(
                            individual,
                            intIndex,
                            dblWeight);
                    }
                    else
                    {
                        RemoveChromosomeValue(
                            individual,
                            intIndex,
                            Math.Abs(dblWeight));
                    }
                }

                //
                // check constraints
                //
                var blnCheckConstraint = m_heuristicProblem.CheckConstraints(individual);
                if (blnCheckConstraint)
                {
                    var dblCurrentReturn =
                        m_heuristicProblem.ObjectiveFunction.Evaluate(
                            individual,
                            m_heuristicProblem);
                    if (dblCurrentReturn > dblBestReturn)
                    {
                        //
                        // If improvement found then
                        // set the initial chromosome array to best neighbour found so far
                        //
                        dblInitialChromosomeArray = GetChromosomeCopy(individual);
                        dblBestReturn = dblCurrentReturn;
                        blnImprovement = true;
                        blnGlobalImprovement = true;
                    }
                    else
                    {
                        blnImprovement = false;
                    }
                }

                // move to next point in the neighbourhood
                // depending if forward is flagged
                //
                if (blnImprovement)
                {
                    if (blnGoForward)
                    {
                        dblLowerBoundWeight = dblNeighbourWeight;
                    }
                    else
                    {
                        dblUpperBoundWeight = dblNeighbourWeight;
                    }
                }
                else
                {
                    if (blnGoForward)
                    {
                        dblUpperBoundWeight = dblNeighbourWeight;
                    }
                    else
                    {
                        dblLowerBoundWeight = dblNeighbourWeight;
                    }
                }
            }
            //
            // go back to the last successful state if no 
            // improvement is achieved in the last stages
            //
            if (!blnImprovement)
            {
                BackToInitialState(
                    individual,
                    dblInitialChromosomeArray);
            }
            if (blnGlobalImprovement)
            {
                m_searchDirectionOperator.SetImprovementCounter(
                    intIndex,
                    blnGoForward);
            }

            var blnCheckConstraint0 = m_heuristicProblem.CheckConstraints(individual);
            if (!blnCheckConstraint0)
            {
                //Debugger.Break();
                throw new HCException("Error. Local search not valid.");
            }
        }

        private void GetLowerUpperBounds(
            Individual individual,
            int intIndex,
            RngWrapper rng,
            bool blnGoForward,
            out double dblUpperBoundWeight,
            out double dblLowerBoundWeight)
        {
            //
            // get upper and lower bound weights
            //
            if (blnGoForward)
            {
                dblUpperBoundWeight =
                    GetUpperBoundWeight(
                        GetChromosomeValue(individual, intIndex),
                        rng,
                        intIndex);

                dblLowerBoundWeight =
                    GetChromosomeValue(individual, intIndex);
            }
            else
            {
                dblUpperBoundWeight =
                    GetChromosomeValue(individual, intIndex);

                dblLowerBoundWeight =
                    GetLowerBoundWeight(
                        GetChromosomeValue(individual, intIndex),
                        rng);
            }
        }

        private void MutateChromosomeWeight(
            Individual individual,
            int intIndex,
            RngWrapper rng,
            bool blnGoForward)
        {
            var dblInitialChromosomeArray = GetChromosomeCopy(individual);

            var chromosomeIndexList =
                new List<int>(m_heuristicProblem.VariableCount + 1);
            //
            // get non-zero chromosome indexes
            //
            for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
            {
                if (dblInitialChromosomeArray[i] > 0 && i != intIndex)
                {
                    chromosomeIndexList.Add(i);
                }
            }
            // pick one chromosome randomly and remove weight
            if (chromosomeIndexList.Count > 0)
            {
                var intRngSelectedIndex = rng.NextInt(0, chromosomeIndexList.Count - 1);
                // delete weight from current solution
                var intSelectedIndex = chromosomeIndexList[intRngSelectedIndex];
                // get a chromosome weight in the neighbourhood

                var dblChromosomeWeight =
                    GetNearestNeighWeight(
                        GetChromosomeValue(individual, intSelectedIndex),
                        intIndex,
                        blnGoForward,
                        intSelectedIndex);

                //
                // add or remove weight to current chromosome
                //
                if (blnGoForward)
                {
                    AddChromosomeValue(
                        individual,
                        intSelectedIndex,
                        dblChromosomeWeight);
                }
                else
                {
                    RemoveChromosomeValue(
                        individual,
                        intSelectedIndex,
                        dblChromosomeWeight);
                }
            }
            //
            // check constraint. If constraint not satisfied, then return to 
            // origina state
            //
            var blnCheckConstraint = m_heuristicProblem.CheckConstraints(individual);
            if (!blnCheckConstraint)
            {
                BackToInitialState(
                    individual,
                    dblInitialChromosomeArray);
            }
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

        protected abstract double GetNearestNeighWeight(
            double dblChromosomeValue,
            int intIndex,
            bool blnGoForward,
            int intScaleIndex);

        #endregion
    }
}
