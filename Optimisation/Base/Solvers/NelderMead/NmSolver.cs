#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers.NelderMead.DataStructures;
using HC.Analytics.Optimisation.Integer.Operators.LocalSearch.NmLocalSearch;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers.NelderMead
{
    /// <summary>
    ///   Nelder - Mead algorithm.
    /// 
    ///   Solver used for continuous optimisation. Finds a local solution.
    /// </summary>
    public class NmSolver : AbstractHeuristicSolver
    {
        #region Members

        private readonly int m_intNmDimensions;
        private object m_lockObject1 = new object();
        private AbstractNmPopulationGenerator m_nmPopulationGenerator;
        private double m_dblLowerBound;
        private List<Individual> m_individualQueue;
        private int m_intPopulationReady;
        private int m_intQueueSize;
        private AbstractNmVertex m_midVertex;
        private AbstractNmVertex[] m_vertexArray;
        private readonly bool m_blnIsIntegerProblem;
        private DateTime m_prevLogTime;
        
        #endregion

        #region Events

        #region Delegates

        public delegate void NmImprovementFoundDel(NmSolver nmSolver, Individual bestSolution);

        #endregion

        public event NmImprovementFoundDel OnImprovementFound;

        #endregion

        #region Constructors

        public NmSolver(
            HeuristicProblem heuristicProblem,
            AbstractNmPopulationGenerator abstractNmPopulationGenerator)
            : base(heuristicProblem)
        {
            m_blnIsIntegerProblem = abstractNmPopulationGenerator is NmPopulationGeneratorInt;
            m_strSolverName = "Nelder-Mead";
            Verboser.WriteLine("Running nmSolver...");
            m_nmPopulationGenerator = abstractNmPopulationGenerator;
            //
            // the number of dimensions is n+1
            //
            m_intNmDimensions =
                HeuristicProblem.VariableCount + 1;
        }

        #endregion

        #region Public

        /// <summary>
        ///   Solve
        /// </summary>
        public override void Solve()
        {
            try
            {
                Initialize();
                RunNm();
                //
                // consolidate population
                //
                HeuristicProblem.Population.LoadPopulation();

                Verboser.WriteLine("Finish running nmSolver. " + CurrentIteration + " iterations");
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        ///   Get best solution
        /// </summary>
        /// <returns></returns>
        public Individual GetBestSolution()
        {
            Array.Sort(m_vertexArray);
            Array.Reverse(m_vertexArray);
            return m_vertexArray[0].Individual_.Clone(HeuristicProblem);
        }

        /// <summary>
        ///   Get longest edge to best solution
        /// </summary>
        /// <returns></returns>
        public double GetLongestEdgeToBest()
        {
            double dblDistanceMax = 0.0;
            for (var i = 1; i < m_intNmDimensions; i++)
            {
                double dblDistance = m_vertexArray[i].DistanceTo(m_vertexArray[0]);
                if (dblDistanceMax < dblDistance)
                {
                    dblDistanceMax = dblDistance;
                }
            }
            return dblDistanceMax;
        }

        /// <summary>
        ///   Iterate NM algorithm
        /// </summary>
        /// <returns></returns>
        public NmIterationType IterateNmAlgorithm()
        {
            for (var i = 0;
                 i <
                 HeuristicProblem.VariableCount;
                 i++)
            {
                m_midVertex.SetVertexValue(i, 0.0);
                for (var j = 0;
                     j <
                     HeuristicProblem.VariableCount;
                     j++)
                {
                    var dblChromosomeValue =
                        m_midVertex.GetVertexValue(i) +
                        m_vertexArray[j].GetVertexValue(i);

                    m_midVertex.SetVertexValue(i, dblChromosomeValue);
                }
                var dblValue = m_midVertex.GetVertexValue(i) / HeuristicProblem.VariableCount;
                m_midVertex.SetVertexValue(i, dblValue);
            }

            AbstractNmVertex refV = m_vertexArray[
                HeuristicProblem.VariableCount].Combine(-1.0, m_midVertex);
            EvaluateVertex(refV);
            if (refV.Value < m_vertexArray[0].Value)
            {
                AbstractNmVertex expV = m_vertexArray[HeuristicProblem.VariableCount].Combine(-2.0, m_midVertex);
                EvaluateVertex(expV);
                if (expV.Value < refV.Value)
                {
                    m_vertexArray[HeuristicProblem.VariableCount] = expV;

                    return NmIterationType.EXPANSION;
                }
                m_vertexArray[HeuristicProblem.VariableCount] = refV;

                return NmIterationType.REFLECTION;
            }
            if (refV.Value >= m_vertexArray[HeuristicProblem.VariableCount].Value)
            {
                AbstractNmVertex icV = m_vertexArray[HeuristicProblem.VariableCount].Combine(0.5, m_midVertex);
                EvaluateVertex(icV);
                if (icV.Value < m_vertexArray[HeuristicProblem.VariableCount].Value)
                {
                    m_vertexArray[HeuristicProblem.VariableCount] = icV;

                    return NmIterationType.SRINK;
                }
                Shrink(EvaluationStateType.INITIAL_STATE);
                return NmIterationType.INSIDE_CONTRACTION;
            }

            if (refV.Value > m_vertexArray[HeuristicProblem.VariableCount - 1].Value)
            {
                AbstractNmVertex ocV = m_vertexArray[HeuristicProblem.VariableCount].Combine(-0.5, m_midVertex);
                EvaluateVertex(ocV);
                if (ocV.Value < refV.Value)
                {
                    m_vertexArray[HeuristicProblem.VariableCount] = ocV;

                    return NmIterationType.SRINK;
                }
                Shrink(EvaluationStateType.INITIAL_STATE);
                return NmIterationType.OUTSIDE_CONTRACTION;
            }
            m_vertexArray[HeuristicProblem.VariableCount] = refV;

            return NmIterationType.REFLECTION;
        }

        /// <summary>
        ///   To string results
        /// </summary>
        public void CheckResults()
        {
            var dblMaxValue = -double.MaxValue;
            var intMaxIndex = -1;
            for (var i = 0; i < m_intNmDimensions; i++)
            {
                if (dblMaxValue < -m_vertexArray[i].Value)
                {
                    dblMaxValue = -m_vertexArray[i].Value;
                    intMaxIndex = i;
                }
            }

            if (m_dblLowerBound < m_vertexArray[intMaxIndex].Value)
            {
                throw new HCException("NM weight is lower than the initial condition");
            }
            //
            // get individual from large population since
            // we are clustering in the large population directly
            //
            Individual bestIndividual =
                HeuristicProblem.Population.GetIndividualFromLargePopulation(
                    HeuristicProblem,
                    0);

            if (bestIndividual == null || 
                (bestIndividual.Fitness <
                    -m_vertexArray[intMaxIndex].Value))
            {
                Individual bestSolution = GetBestSolution();
                HeuristicProblem.Population.ClusterBestSolution(
                    bestSolution,
                    HeuristicProblem);

                LogProgress(bestSolution);
                InvokeImprovementFound(bestSolution);
            }
        }

        private void LogProgress(Individual bestVector)
        {
            if ((DateTime.Now - m_prevLogTime).TotalSeconds > 2)
            {
                string strMessage = GetSolverName() +
                                    ". Iteration " +
                                    CurrentIteration +
                                    " of " +
                                    HeuristicProblem.Iterations +
                                    ". Best objective function = " +
                                    bestVector.Fitness +
                                    ", " +
                                    Environment.NewLine +
                                    bestVector;

                PrintToScreen.WriteLine(strMessage);
                UpdateProgressBar(-1, strMessage);
                m_prevLogTime = DateTime.Now;
            }
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
                m_lockObject1 = null;
                m_nmPopulationGenerator = null;
                if (m_individualQueue != null)
                {
                    m_individualQueue.Clear();
                }
                m_midVertex.Dispose();
                if (m_vertexArray != null)
                {
                    for (int i = 0; i < m_vertexArray.Length; i++)
                    {
                        m_vertexArray[i].Dispose();
                    }
                }
                EventHandlerHelper.RemoveAllEventHandlers(this);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Private

        private void LoadGraphItems()
        {
            m_vertexArray = m_nmPopulationGenerator.GenerateVertexArray();
            //
            // set an arbitrary individual for the mid vertex
            //
            m_midVertex = m_nmPopulationGenerator.CreateNmVertex(
                HeuristicProblem.VariableCount,
                m_vertexArray[0].Individual_.Clone(HeuristicProblem));
        }

        /// <summary>
        ///   GetTask algorithm
        /// </summary>
        private double RunNm()
        {
            double dblLongestEdgeToBest = double.NaN;
            try
            {
                dblLongestEdgeToBest = GetLongestEdgeToBest();
                while (dblLongestEdgeToBest > 1E-8)
                {
                    // sort results
                    Array.Sort(m_vertexArray);
                    Array.Reverse(m_vertexArray);
                    CheckResults();
                    // stoping condition
                    dblLongestEdgeToBest = GetLongestEdgeToBest();
                    IterateNmAlgorithm();
                    CurrentIteration++;
                    if (CurrentIteration >= HeuristicProblem.Iterations)
                    {
                        Console.WriteLine("NM solver closed due iterations [" +
                            CurrentIteration + "]");
                        break;
                    }
                    if (!HeuristicProblem.Solver.IterateSolver)
                    {
                        Console.WriteLine("NM solver closed due to end of solver");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Console.WriteLine(ex);
            }
            return dblLongestEdgeToBest;
        }

        /// <summary>
        ///   Get individual from a given vertex
        /// </summary>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <param name = "intVertedIndex">
        ///   Vertex index
        /// </param>
        /// <returns>
        ///   IIndividual
        /// </returns>
        private Individual GetIndividual(
            AbstractNmVertex vertex,
            int intVertedIndex)
        {
            if (!m_blnIsIntegerProblem)
            {
                for (var i = 0;
                     i < HeuristicProblem.VariableCount;
                     i++)
                {
                    if (vertex.GetVertexValue(i) > 1.0 ||
                        vertex.GetVertexValue(i) < 0.0)
                    {
                        // penalise weights out of the [0,1] range
                        vertex.Value = double.MaxValue;
                        return null;
                    }
                }
            }
            vertex.Individual_.IndividualId = intVertedIndex;
            return vertex.Individual_;
        }

        /// <summary>
        ///   Evaluate vertex
        /// </summary>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Vertex value
        /// </returns>
        private void EvaluateVertex(AbstractNmVertex vertex)
        {
            var individual = GetIndividual(vertex, -1);
            if (individual == null)
            {
                return;
            }

            individual.Evaluate(false,
                                false,
                                false,
                                HeuristicProblem);
            LoadVetexFitness(
                individual,
                vertex);
        }

        /// <summary>
        ///   Interpolate individual
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <param name = "vertex">
        ///   Vertex
        /// </param>
        /// <returns>
        ///   Intepolated individual
        /// </returns>
        private void LoadVetexFitness(
            Individual individual,
            AbstractNmVertex vertex)
        {
            //
            // Penalise individuals out of bounds.
            // Assign large negative fitness value
            //
            if (!HeuristicProblem.CheckConstraints(individual))
            {
                // 
                vertex.Value = double.MaxValue;
                return;
            }
            vertex.Value = -individual.Fitness;
        }

        /// <summary>
        ///   Initialize algorithm
        /// </summary>
        private void Initialize()
        {
            LoadGraphItems();
            //
            // get lower bound value
            //
            m_dblLowerBound = (from n in m_vertexArray select n.Value).Min();
            CurrentIteration = 0;
        }

        /// <summary>
        ///   Get a queue of individuals
        /// </summary>
        private void GetIndividualQueueList()
        {
            m_individualQueue = new List<Individual>(HeuristicProblem.VariableCount);
            for (var i = 1; i < m_intNmDimensions; i++)
            {
                m_vertexArray[i] = m_vertexArray[i].Combine(0.5, m_vertexArray[0]);


                var currentIndividual = GetIndividual(m_vertexArray[i], i);
                if (currentIndividual != null)
                {
                    currentIndividual.OnIndividualReady +=
                        Shrink;

                    m_individualQueue.Add(currentIndividual);
                }
            }
            m_intQueueSize = m_individualQueue.Count;
        }

        /// <summary>
        ///   Shrink vector
        /// </summary>
        /// <param name = "state">
        ///   IIndividual state
        /// </param>
        private void Shrink(EvaluationStateType state)
        {
            lock (m_lockObject1)
            {
                if (state == EvaluationStateType.INITIAL_STATE)
                {
                    GetIndividualQueueList();
                    EvaluatePopulation(m_individualQueue);
                }
                else
                {
                    if (state == EvaluationStateType.SUCCESS_EVALUATION)
                    {
                        m_intPopulationReady++;
                    }
                    else
                    {
                        throw new HCException("Error. IIndividual not evaluated.");
                    }
                    // counts the number of individuals which finished with been evaluated
                    if (m_intPopulationReady == m_intQueueSize)
                    {
                        // interpretate each individual
                        foreach (Individual individual in m_individualQueue)
                        {
                            var intVertexId = individual.IndividualId;
                            LoadVetexFitness(individual, m_vertexArray[intVertexId]);
                        }
                        // initialize number of individuals ready
                        m_intPopulationReady = 0;
                        // sort results
                        Array.Sort(m_vertexArray);
                        Array.Reverse(m_vertexArray);
                        // run another iteration
                        RunNm();
                    }
                }
            }
        }

        /// <summary>
        ///   Evaluate population
        /// </summary>
        /// <param name = "individualQueue">
        ///   IIndividual queue
        /// </param>
        private void EvaluatePopulation(List<Individual> individualQueue)
        {
            for (var i = 0; i < individualQueue.Count; i++)
            //Parallel.For(0, individualQueue.Count, delegate(int i)
            {
                EvaluateIndividual(individualQueue[i]);
            }
            //);
        }

        /// <summary>
        ///   Evaluate individual
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns></returns>
        private void EvaluateIndividual(Individual individual)
        {
            individual.Evaluate(false,
                                true,
                                true,
                                HeuristicProblem);
        }

        #endregion

        #region Protected

        /// <summary>
        ///   Call this method when a improvent has been found
        /// </summary>
        protected void InvokeImprovementFound(Individual bestSolution)
        {
            if (OnImprovementFound != null)
            {
                if (OnImprovementFound.GetInvocationList().Length > 0)
                {
                    OnImprovementFound(this, bestSolution);
                }
            }
        }

        #endregion
    }
}
