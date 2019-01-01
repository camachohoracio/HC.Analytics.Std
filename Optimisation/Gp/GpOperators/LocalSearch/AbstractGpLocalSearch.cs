#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LocalSearch;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Threading;

#endregion

namespace HC.Analytics.Optimisation.Gp.GpOperators.LocalSearch
{
    public abstract class AbstractGpLocalSearch : AbstractLocalSearch
    {
        #region OnCompletedGeneration

        #region Delegates

        public delegate void CompletedGeneration(
            HeuristicProblem heuristicProblem);

        #endregion

        public event CompletedGeneration OnCompletedGeneration;

        #endregion

        #region Constants

        protected const int EXPENSIVE_LOCAL_SEARCH_THREADS = 1;
        protected const int LOCAL_SEARCH_GENERATIONS = 100;
        protected const int SOLVER_ITERATIONS = 10;
        protected const int SOLVER_POPULATION_SIZE = 10;
        protected const int SOLVER_THREADS = 0;

        #endregion

        #region Members

        protected readonly GpOperatorsContainer m_gpOperatorsContainer;
        protected readonly Dictionary<string, object> m_individualValidator;
        protected readonly object m_lockObject = new object();
        protected int m_intThreadCounter;

        #endregion

        #region Constructors

        protected AbstractGpLocalSearch(
            HeuristicProblem heuristicProblem,
            GpOperatorsContainer gpOperatorsContainer) :
                base(heuristicProblem)
        {
            m_gpOperatorsContainer = gpOperatorsContainer;
            m_individualValidator = new Dictionary<string, object>();
        }

        #endregion

        #region Public

        /// <summary>
        ///   Construct an integer problem for the variable
        ///   nodes contained in the individual's tree
        /// </summary>
        /// <param name = "individual"></param>
        public override void DoLocalSearch(Individual individual)
        {
            if (individual.IsEvaluated)
            {
                //Debugger.Break();
                throw new HCException("Error. Individual already evaluated.");
            }

            var rng = HeuristicProblem.CreateRandomGenerator();
            List<AbstractGpNode> varNodeList;
            AbstractGpNode newRootNode;

            lock (m_lockObject)
            {
                //
                // check if local search conditions are met
                // also select an individual from the population
                //
                if (!CheckDoLocalSearch(
                    rng,
                    out individual))
                {
                    return;
                }

                newRootNode = individual.Root.Clone(
                    null,
                    m_heuristicProblem);

                varNodeList = GetVariableNodeList(
                    newRootNode);

                if (varNodeList.Count < 2)
                {
                    //
                    // not worth local search if the number of variables
                    // is less than two
                    //
                    return;
                }

                m_intThreadCounter++;
                ResetIterations(m_heuristicProblem, null);
            }
            DoLocalSearch1(individual);
        }

        public void DoLocalSearch1(Individual individual)
        {
            var lowerBoundRootNode =
                individual.Root.Clone(
                    null,
                    m_heuristicProblem);

            //
            // build local search problem
            //
            var localSearchHeuristicProblem =
                BuildLocalSearchHeuristicProblem(
                    individual);

            // return if problem is null
            if (localSearchHeuristicProblem == null)
            {
                return;
            }

            //
            // reset iterations every time the solver improves
            //
            localSearchHeuristicProblem.Solver.OnImprovementFound +=
                ResetIterations;
            // publish results each completed generation
            localSearchHeuristicProblem.Solver.OnCompletedGeneration +=
                InvokeOnCompletedGeneration;


            //
            // solve new optimisation problem as a new process
            //
            var worker = new ThreadWorker();
            worker.WaitForExit = true;
            worker.OnExecute +=
                localSearchHeuristicProblem.Solver.Solve;
            worker.Work();


            var newIndividual = ValidateLocalSearch(
                individual,
                localSearchHeuristicProblem,
                lowerBoundRootNode);


            //
            // release local search thread
            //
            lock (m_lockObject)
            {
                m_intThreadCounter--;
                //
                // reset number of iterations
                //
                m_intLocaSearchIterations = 0;
                //
                // add new individual to the banned list
                //
                var strIndividualDescription = newIndividual.ToString();
                if (!m_individualValidator.ContainsKey(strIndividualDescription))
                {
                    m_individualValidator.Add(
                        strIndividualDescription,
                        null);
                }
            }
        }

        private Individual ValidateLocalSearch(
            Individual individual,
            HeuristicProblem localSearchHeuristicProblem,
            AbstractGpNode lowerBoundRootNode)
        {
            //
            // retrieve the best individual from the population
            //
            var bestIndividual =
                localSearchHeuristicProblem.
                    Population.GetIndividualFromPopulation(
                        localSearchHeuristicProblem,
                        0).Clone(m_heuristicProblem);

            var dblBestFitness = bestIndividual.Fitness;

            double[] dblBestChromosomeArr = null;
            if (bestIndividual.ContainsChromosomeDbl())
            {
                dblBestChromosomeArr =
                    bestIndividual.GetChromosomeCopyDbl();
            }

            int[] intBestChromosomeArr = null;
            if (bestIndividual.ContainsChromosomeInt())
            {
                intBestChromosomeArr =
                    bestIndividual.GetChromosomeCopyInt();
            }

            var newIndividual =
                ((AbstractGpVarObjFunction) localSearchHeuristicProblem.
                                                ObjectiveFunction).CreateIndividualForParentProblem(
                                                    lowerBoundRootNode,
                                                    m_gpOperatorsContainer,
                                                    m_heuristicProblem,
                                                    intBestChromosomeArr,
                                                    dblBestChromosomeArr);

            var dblNewFitness = newIndividual.Evaluate(
                false,
                false,
                true,
                m_heuristicProblem);

            //
            // Validate fitness value.
            // The value should be the same in both optimisation problems
            //
            if (Math.Abs(dblNewFitness - dblBestFitness) >
                MathConstants.ROUND_ERROR)
            {
                //Debugger.Break();
                throw new HCException("Error. Local search error.");
            }

            //
            // validate fitness value and cluster individual
            // the fitness of the individual should be at least of its
            // lower bound.
            //
            if (dblNewFitness - individual.Fitness <
                -MathConstants.ROUND_ERROR)
            {
                throw new HCException("Error. Local search error.");
            }
            return newIndividual;
        }

        public HeuristicProblem BuildLocalSearchHeuristicProblem(
            Individual individual)
        {
            HeuristicProblem localSearchHeuristicProblem;

            var lowerBoundRootNode =
                individual.Root.Clone(
                    null,
                    m_heuristicProblem);

            var varNodeList =
                GetVariableNodeList(
                    lowerBoundRootNode);

            //
            // get lower bound chromosomes
            //
            int[] intChromosomeArr;
            double[] dblChromosomeArr;

            GetLowerBoundChromosomes(
                varNodeList,
                out intChromosomeArr,
                out dblChromosomeArr);

            //
            // return if both chromosomes are null
            //

            if (intChromosomeArr == null &&
                dblChromosomeArr == null)
            {
                return null;
            }

            var intChromosomeLengthInteger =
                (intChromosomeArr == null
                     ? 0
                     : intChromosomeArr.Length);

            var intChromosomeLenghtContinuous =
                (dblChromosomeArr == null
                     ? 0
                     : dblChromosomeArr.Length);

            //
            // create new optimisation problem
            //
            var gpStratVarProblemFactory =
                GetProblemFactory(
                    varNodeList,
                    lowerBoundRootNode,
                    intChromosomeLengthInteger,
                    intChromosomeLenghtContinuous);

            localSearchHeuristicProblem = gpStratVarProblemFactory.BuildProblem();

            var lowerBoundIndividual = new Individual(
                dblChromosomeArr,
                intChromosomeArr,
                null,
                0,
                localSearchHeuristicProblem);

            //
            // validate fitness value
            //
            var dblNewFitness = lowerBoundIndividual.Evaluate(
                false,
                false,
                true,
                localSearchHeuristicProblem);

            if (Math.Abs(individual.Fitness - dblNewFitness) >
                MathConstants.ROUND_ERROR)
            {
                throw new HCException("Local sarch exception");
            }

            //
            // set solver params
            //
            localSearchHeuristicProblem.PopulationSize = 100;
            localSearchHeuristicProblem.Iterations = 1000000;
            localSearchHeuristicProblem.Convergence = 1000000;
            return localSearchHeuristicProblem;
        }

        private void ResetIterations(
            HeuristicProblem heuristicProblem,
            Individual bestIndividual)
        {
            //
            // reset number of iterations
            //
            m_intLocaSearchIterations = 0;
        }

        private Individual SelectIndividualFromPopulation()
        {
            //
            // choose an individual from the large population
            // make sure the individual hasn't been chosen for
            // local search before
            //
            for (var i = 0; i < m_heuristicProblem.Population.LargePopulationSize; i++)
            {
                var individual =
                    m_heuristicProblem.Population.
                        GetIndividualFromLargePopulation(
                            m_heuristicProblem,
                            i);
                var strIndividualDescription = individual.ToString();

                if (!m_individualValidator.ContainsKey(strIndividualDescription))
                {
                    lock (m_individualValidator)
                    {
                        m_individualValidator.Add(
                            strIndividualDescription,
                            null);
                    }
                    return individual;
                }
            }
            return null;
        }

        #endregion

        #region Abstract Methods

        public abstract List<AbstractGpNode> GetVariableNodeList(
            AbstractGpNode rootNode);

        protected abstract void GetLowerBoundChromosomes(
            List<AbstractGpNode> timeSeriesNodeList,
            out int[] intChromosomeArr,
            out double[] dblChromosomeArr);

        protected abstract MixedHeurPrblmFctGeneric GetProblemFactory(
            List<AbstractGpNode> varNodeList,
            AbstractGpNode newRootNode,
            int intChromosomeLengthInteger,
            int intChromosomeLengthContinuous);

        protected abstract AbstractGpVarObjFunction BuildObjectiveFunction(
            int intVariableCount,
            AbstractGpNode root,
            GpOperatorsContainer gpOperatorsContainer,
            HeuristicProblem baseHeuristicProblem,
            List<AbstractGpNode> varNodeList);

        #endregion

        #region Private

        private bool CheckDoLocalSearch(
            RngWrapper rng,
            out Individual individual)
        {
            individual = null;

            if (!CheckIterations())
            {
                return false;
            }
            //
            // check number of generations
            //
            if (m_intLocaSearchIterations/
                ((double) m_heuristicProblem.PopulationSize) <
                LOCAL_SEARCH_GENERATIONS)
            {
                return false;
            }

            //
            // Ramdomly set an extensive local search
            //
            if (rng.NextDouble() <= OptimisationConstants.DBL_EXTENSIVE_LOCAL_SEARCH ||
                m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    0) == null)
            {
                return false;
            }

            // validate threads
            if (EXPENSIVE_LOCAL_SEARCH_THREADS <= m_intThreadCounter)
            {
                return false;
            }

            individual = SelectIndividualFromPopulation();
            if (individual == null)
            {
                return false;
            }

            return true;
        }

        private bool CheckIterations()
        {
            m_intLocaSearchIterations++;
            var bestIndividual =
                m_heuristicProblem.Population.GetIndividualFromPopulation(
                    m_heuristicProblem,
                    0);

            if (bestIndividual != null)
            {
                if (bestIndividual.Fitness > m_dblBestFitness)
                {
                    m_dblBestFitness =
                        bestIndividual.Fitness;
                    m_intLocaSearchIterations = 0;
                    return false;
                }
            }
            return bestIndividual != null;
        }

        private void InvokeOnCompletedGeneration(
            HeuristicProblem heuristicProblem)
        {
            if (OnCompletedGeneration != null &&
                OnCompletedGeneration.GetInvocationList().Length > 0)
            {
                OnCompletedGeneration.Invoke(heuristicProblem);
            }
        }

        #endregion
    }
}
