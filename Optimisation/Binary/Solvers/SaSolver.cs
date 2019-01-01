#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Helpers;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Binary.Solvers
{
    /// <summary>
    ///   Simulated Annealing solver.
    ///   Emmulates the cooling process of annealing.
    ///   At the beginning (when the temperature is high) the solver
    ///   jumps all over the search space. 
    ///   The solver starts to converge as the temperature cools down.
    /// </summary>
    public class SaSolver : AbstractHeuristicSolver
    {
        #region Members

        private readonly RngWrapper m_rngWrapper;

        /// <summary>
        ///   Current individual
        /// </summary>
        private Individual m_currentIndividual;

        /// <summary>
        ///   Alpha parameter
        /// </summary>
        private double m_dblAlpha = Constants.DBL_ALPHA;

        /// <summary>
        ///   Probability of moving to a neighbour
        /// </summary>
        private double m_dblNeighbourhood = Constants.DBL_NEIGHBOURHOOD;

        /// <summary>
        ///   Initial temperature
        /// </summary>
        private double m_dblTemperature = Constants.DBL_TEMPERATURE;

        /// <summary>
        ///   Neighbourhood size
        /// </summary>
        private int m_intNeighbourhood;

        /// <summary>
        ///   Number of iterations
        /// </summary>
        private int m_intTemperatureIterations = Constants.INT_TEMPERATURE_ITERATIONS;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "lowerBoundIndividual">
        ///   Lower bound
        /// </param>
        /// <param name = "repairIndividual">
        ///   Repair operator
        /// </param>
        /// <param name = "localSearch">
        ///   Local serch
        /// </param>
        /// <param name = "cc">
        ///   Cluster
        /// </param>
        public SaSolver(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
            m_strSolverName = "Simulated Annealing";
            m_rngWrapper = HeuristicProblem.CreateRandomGenerator();

            SetDefaultParams();
        }

        #endregion

        #region Public

        /// <summary>
        ///   Solve
        /// </summary>
        public override void Solve()
        {
            // initialize solver
            //
            StartTime = DateTime.Now;
            CurrentIteration = 0;
            MaxConvergence = -1;
            BestFitness = -double.MaxValue;
            //
            // create and evaluate initial individual
            //
            m_currentIndividual =
                HeuristicProblem.IndividualFactory.BuildRandomIndividual();

            Individual neighbourIndividual;
            m_bestIndividual = null;

            m_currentIndividual.Evaluate(false,
                                         true,
                                         true,
                                         HeuristicProblem);


            // do local search in order to improve the starting point
            HeuristicProblem.LocalSearch.DoLocalSearch(
                m_currentIndividual);


            // save results in result list
            HeuristicProblem.Population.AddIndividualToPopulation(m_currentIndividual);

            m_bestIndividual = m_currentIndividual;
            // save the best individual to a file
            SaveBestIndividual();


            SolutionsExplored++;

            var dblCurrentFitness = m_currentIndividual.Fitness;
            double dblNeighbourFitness;
            double dblProbability;
            var dblBestFitness = dblCurrentFitness;

            var intTemperatureIterations = 0;

            while (CurrentIteration <= HeuristicProblem.Iterations &&
                   CurrentConvergence <= HeuristicProblem.Convergence)
            {
                neighbourIndividual = GetNeighbourIndividual(m_currentIndividual);
                neighbourIndividual.Evaluate(false,
                                             true,
                                             true,
                                             HeuristicProblem);
                // cluster results
                HeuristicProblem.Population.AddIndividualToPopulation(m_currentIndividual);
                dblNeighbourFitness = neighbourIndividual.Fitness;
                SolutionsExplored++;


                if (dblNeighbourFitness > dblCurrentFitness)
                {
                    // there is an improvement
                    m_currentIndividual = neighbourIndividual;
                    dblCurrentFitness = dblNeighbourFitness;
                    if (dblCurrentFitness > dblBestFitness)
                    {
                        dblBestFitness = dblCurrentFitness;
                        m_bestIndividual = m_currentIndividual;

                        // save the best individual to a file
                        SaveBestIndividual();

                        //
                        // update temperature
                        //
                        m_dblTemperature = m_dblTemperature*m_dblAlpha;
                        CurrentConvergence = 0;
                    }
                }
                else
                {
                    // move to neighbour with a certain probability
                    CurrentConvergence++;


                    dblProbability = Math.Exp(
                        -(dblCurrentFitness - dblNeighbourFitness)/m_dblTemperature);
                    if (m_rngWrapper.NextDouble() <= dblProbability)
                    {
                        m_currentIndividual = neighbourIndividual;
                        dblCurrentFitness = dblNeighbourFitness;
                    }
                    //
                    // update temperature
                    //
                    if (intTemperatureIterations >= m_intTemperatureIterations)
                    {
                        m_dblTemperature = m_dblTemperature*m_dblAlpha;
                        intTemperatureIterations = 0;
                    }
                    intTemperatureIterations++;
                }


                if (BestFitness < m_bestIndividual.Fitness)
                {
                    BestFitness = m_bestIndividual.Fitness;
                    CurrentConvergence = 0;
                }
                else
                {
                    CurrentConvergence++;
                }
                if (MaxConvergence < CurrentConvergence)
                {
                    MaxConvergence = CurrentConvergence;
                }

                var intPercentage = (MaxConvergence*100)/HeuristicProblem.Convergence;
                var intPercentage2 = (CurrentIteration*100)/HeuristicProblem.Iterations;
                intPercentage = Math.Max(intPercentage, intPercentage2);
                if (intPercentage > 100)
                {
                    intPercentage = 100;
                }
                intPercentage = InitialCompletionPercentage + ((intPercentage*PercentageCompletionValue)/100);
                var strMessage = GetSolverName() + ". Iteration " + CurrentIteration + " of " +
                                 HeuristicProblem.Iterations + ". Best relative return found " +
                                 m_bestIndividual.Fitness + ", [" + intPercentage + "]%" +
                                 Environment.NewLine + m_bestIndividual;
                PrintToScreen.WriteLine(strMessage);
                UpdateProgressBar(intPercentage, strMessage);

                CurrentIteration++;
            }
            //
            // finalize solver
            //
            //GetFinalResults();
        }

        #endregion

        #region Private

        /// <summary>
        ///   Initialize algorithm
        /// </summary>
        private void SetDefaultParams()
        {
            if (HeuristicProblem.Iterations == 0)
            {
                HeuristicProblem.Iterations =
                    OptimizationHelper.GetHeuristicSolverIterations(
                        HeuristicProblem.VariableCount);
            }

            if (HeuristicProblem.ObjectiveFunction.VariableCount <=
                Constants.INT_SMALL_PROBLEM_SA)
            {
                HeuristicProblem.Convergence =
                    Constants.INT_SA_SMALL_CONVERGENCE;
            }
            else
            {
                HeuristicProblem.Convergence =
                    Constants.INT_SA_CONVERGENCE;
            }

            m_intNeighbourhood = (int) (m_dblNeighbourhood*(
                                                               HeuristicProblem.ObjectiveFunction.VariableCount)) +
                                 1;
        }

        /// <summary>
        ///   Get an individual in the neighbourhood
        /// </summary>
        /// <param name = "individual">
        ///   individual
        /// </param>
        /// <returns>
        ///   Neighbourhood individual
        /// </returns>
        private Individual GetNeighbourIndividual(Individual individual)
        {
            var dblNewChromosomeArray = individual.GetChromosomeCopyDbl();

            var numbers = new List<int>(
                HeuristicProblem.VariableCount);
            for (var i = 0;
                 i <
                 HeuristicProblem.ObjectiveFunction.VariableCount;
                 i++)
            {
                numbers.Add(i);
            }

            int intIndex;
            m_rngWrapper.ShuffleList(numbers);
            for (var i = 0; i < m_intNeighbourhood; i++)
            {
                intIndex = numbers[i];
                if (individual.GetChromosomeValueDbl(intIndex) >= 1.0 -
                    MathConstants.DBL_ROUNDING_FACTOR)
                {
                    dblNewChromosomeArray[intIndex] = 0;
                }
                else
                {
                    dblNewChromosomeArray[intIndex] = 1;
                }
            }
            return new Individual(
                dblNewChromosomeArray,
                HeuristicProblem);
        }

        #endregion
    }
}
