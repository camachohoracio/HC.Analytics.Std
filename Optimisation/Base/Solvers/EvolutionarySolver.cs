#region

using System;
using System.Text;
using HC.Analytics.Optimisation.Base.Operators;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers
{
    /// <summary>
    ///   The "AbstractEvolutionarySolver" class represents an
    ///   Evolutionary-compuation type of solver. These type of algorithms 
    ///   emulate real-world evolution by generating offspring at each generation.
    ///   Each generation tends to be better than its predecesors (parents).
    ///   Naming conventions:
    ///   - IIndividual = A solution representation.
    ///   - Chromosome = Each individual contains an array which represents the 
    ///   solution details.
    ///   - Population = Set of individuals.
    ///   - Fitness = Each individual is evaluated (by using the objective function).
    ///   - Generation = Each iteration carried out by the algorithm is called a generation.
    ///   - Reproduction = The individual in the population set are used 
    ///   for the creation of new individuals. The individuals with the best fitness are
    ///   most likely to create offsprings for the next generation.
    ///   - Repair = Each individual is "fixed" in order to ensure it satisfies the 
    ///   set of constraints.
    /// </summary>
    [Serializable]
    public class EvolutionarySolver : AbstractHeuristicSolver
    {
        #region Properties

        public IOptiGuiHelper OptiGuiHelper { get; private set; }
        public HeuristicExecutionHelper HeuristicExecutionHelper { get; set; }

        #endregion

        #region Events & delegates

        #region Delegates

        public delegate void IndividualEvaluatedDel(Individual individual);

        public delegate void ImprovementFoundDel(
            HeuristicProblem heuristicProblem,
            Individual bestIndividual);

        public delegate void ProblemBeatDel(
            HeuristicProblem heuristicProblem);

        public delegate void SolverFinishDel();

        public delegate void UpdateProgressDel(string strMessage, int intProgress);

        #endregion

        public event ImprovementFoundDel OnImprovementFound;

        public event ProblemBeatDel OnCompletedGeneration;

        public event SolverFinishDel OnSolverFinish;

        public event UpdateProgressDel OnUpdateProgress;

        public event IndividualEvaluatedDel OnIndividualEvaluated;

        public event SolverFinishDel OnInitialPopulationCompleted;
        public event SolverFinishDel OnInitialPopulationStarted;

        #endregion

        #region Members

        /// <summary>
        ///   object used to lock threads at the beginning/end of each iteration
        /// </summary>
        private readonly object m_lockObject = new object();

        /// <summary>
        ///   Flag which indicates if the solver should keep running or stop
        /// </summary>
        public bool IterateSolver { get; private set; }


        private DateTime m_prevCheckTime;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "heuristicProblem"></param>
        public EvolutionarySolver(
            HeuristicProblem heuristicProblem)
            : base(
                heuristicProblem)
        {
            try
            {
                SetStatus(heuristicProblem);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
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
                ValidateSolver();

                // initialize solver
                //
                InitializeSolver();

                //
                // update stats
                //
                PublishProblemStats();

                //
                // run solver
                //
                RunSolver();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Private

        private void PublishProblemStats()
        {
            try
            {
                HeuristicProblem.ProblemStats.SetStrValue(
                    EnumHeuristicProblem.ProblemName,
                    HeuristicProblem.ProblemName);
                HeuristicProblem.ProblemStats.SetIntValue(
                    EnumHeuristicProblem.PopulationSize,
                    HeuristicProblem.PopulationSize);
                HeuristicProblem.ProblemStats.SetIntValue(
                    EnumHeuristicProblem.Threads,
                    HeuristicProblem.Threads);
                HeuristicProblem.ProblemStats.SetBlnValue(
                    EnumHeuristicProblem.DoLocalSearch,
                    HeuristicProblem.DoLocalSearch);
                HeuristicProblem.ProblemStats.SetDblValue(
                    EnumHeuristicProblem.LocalSearchProb,
                    HeuristicProblem.LocalSearchProb);
                HeuristicProblem.ProblemStats.SetStrValue(
                    EnumHeuristicProblem.ProblemType,
                    HeuristicProblem.EnumOptimimisationPoblemType.ToString());
                HeuristicProblem.ProblemStats.SetDblValue(
                    EnumHeuristicProblem.RepairProb,
                    HeuristicProblem.RepairProb);
                HeuristicProblem.PublishGridStats();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void StopSolver()
        {
            IterateSolver = false;
        }

        public void InvokeOnUpdateProgress(string strMessage, int intProgress)
        {
            try
            {
                if (OnUpdateProgress != null &&
                    OnUpdateProgress.GetInvocationList().Length > 0)
                {
                    OnUpdateProgress.Invoke(strMessage, intProgress);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void ValidateSolver()
        {
            try
            {


                //
                // Solver validation
                //
                if (HeuristicProblem.ObjectiveFunction.VariableCount == 1)
                {
                    throw new HCException("The repository contains only one variable.");
                }

                if (HeuristicProblem.ContainsIntegerVariables() &&
                    !HeuristicProblem.ValidateIntegerProblem())
                {
                    throw new HCException("Variable ranges missing for integer problem.");
                }

                if (HeuristicProblem.PopulationSize == 0)
                {
                    throw new HCException("Error. Population size is zero.");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void SetStatus(HeuristicProblem heuristicProblem)
        {
            try
            {
                SetSolverName(heuristicProblem.ProblemName);
                HeuristicProblem = heuristicProblem;
                HeuristicExecutionHelper = new HeuristicExecutionHelper(
                    HeuristicProblem);

                //
                // register events
                //
                if (HeuristicProblem.LocalSearch != null)
                {
                    HeuristicProblem.LocalSearch.ImprovementFoundEvent +=
                        InvokeOnImprovementFoundEvent;
                }


                //
                // register initial population events
                //
                HeuristicProblem.InitialPopulation.UpdateProgress +=
                    UpdateProgressBar;
                HeuristicProblem.InitialPopulation.ExceptionOccurred +=
                    OnExceptionOccurred;


                InitialCompletionPercentage =
                    OptimisationConstants.INT_LOAD_DATA_PERCENTAGE +
                    OptimisationConstants.INT_INITAL_POOL_PERCENTAGE +
                    OptimisationConstants.INT_INITAL_POPULATION_PERCENTAGE;
                PercentageCompletionValue = OptimisationConstants.INT_SOLVER_PERCENTAGE;

                if (heuristicProblem.DoPublish)
                {
                    OptiGuiHelper = new OptiGuiHelper(this);
                }
                else
                {
                    OptiGuiHelper = new DummyOptiGuiHelper(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        /// <summary>
        ///   GetTask solver
        /// </summary>
        private void RunSolver()
        {
            try
            {
                HeuristicExecutionHelper.ExecuteSolver();

                //
                // get final results
                //
                GetFinalResults();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        ///   Calculate final results
        /// </summary>
        private void GetFinalResults()
        {
            try
            {
                //
                // display solver summary
                //
                var sb = new StringBuilder();
                var totalTime = ((DateTime.Now - StartTime)).TotalSeconds;
                sb.Append(Environment.NewLine + GetSolverName());
                sb.Append(Environment.NewLine + "Total time (secs): " + totalTime);
                sb.Append(Environment.NewLine + "Iterations: " + CurrentIteration);
                sb.Append(Environment.NewLine + "Solutions explored: " + SolutionsExplored);
                sb.Append(Environment.NewLine + "Best solution: " +
                          HeuristicProblem.Population.GetIndividualFromPopulation(
                              HeuristicProblem,
                              0));
                sb.Append(Environment.NewLine + "Objective function = " +
                          HeuristicProblem.Population.GetIndividualFromPopulation(
                              HeuristicProblem,
                              0).Fitness);

                var strMessage = sb.ToString();
                Logger.Log(strMessage);
                PrintToScreen.WriteLine(strMessage);
                UpdateProgressBar(-1, strMessage);
                InvokeSolverFinished(ResultList);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        /// <summary>
        ///   Iterate solver
        /// </summary>
        public void IterateIndividual(Individual individual)
        {
            try
            {
                if (IterateSolver)
                {
                    lock (m_lockObject)
                    {
                        m_intIterationCounter++;
                        SolutionsExplored++;

                        //
                        // note that we dont want to check for stopping conditions too often
                        // this could kill performance
                        //
                        if ((DateTime.Now - m_prevCheckTime).TotalSeconds > 1)
                        {
                            m_prevCheckTime = DateTime.Now;
                            if (!CheckStoppingConditions(individual))
                            {
                                return;
                            }
                        }

                        //
                        // check if a complete iteration has been completed
                        //
                        if (m_intIterationCounter >= HeuristicProblem.PopulationSize)
                        {
                            m_intIterationCounter = 0;
                            IterateGeneration();
                        }

                        InvokeOnIndividualEvaluated(individual);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void InvokeOnIndividualEvaluated(Individual individual)
        {
            try
            {
                if (OnIndividualEvaluated != null)
                {
                    OnIndividualEvaluated(individual);
                }

                OptiGuiHelper.UpdateStats();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void IterateGeneration()
        {
            try
            {
                //
                // upgrade solver operators
                //
                OperatorHelper.UpgradeSolverOperators(
                    HeuristicProblem);

                if (!CheckStoppingConditions(null))
                {
                    return;
                }

                InvokeOnCompletedGeneration();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private bool CheckStoppingConditions(Individual individual)
        {
            try
            {
                bool blnImprovementFound = CheckConvergence(individual);
                double intPercentage = SetProgress(blnImprovementFound);

                //
                // check if solver is finalised
                //
                if (intPercentage >= (PercentageCompletionValue + InitialCompletionPercentage) &&
                    IterateSolver)
                {
                    IterateSolver = false;
                    HeuristicExecutionHelper.FinishSolver();
                    InvokeOnSoverFinishes();
                    OptiGuiHelper.SetFinishStats();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        private double SetProgress(bool blnImprovementFound)
        {
            try
            {
                // go to next iteration
                CurrentIteration++;

                //
                // calculate percentage completion
                //
                int intPercentage = (MaxConvergence * 100) /
                                    HeuristicProblem.Convergence;
                var intPercentage2 = (CurrentIteration * 100) /
                                     HeuristicProblem.Iterations;
                intPercentage = Math.Max(intPercentage, intPercentage2);
                intPercentage = InitialCompletionPercentage +
                                ((intPercentage * PercentageCompletionValue) / 100);

                OptiGuiHelper.SetProgress(
                    blnImprovementFound,
                    intPercentage);
                return intPercentage;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        private bool CheckConvergence(Individual individual)
        {
            try
            {
                var bestIndividual = HeuristicProblem.Population.GetIndividualFromPopulation(
                    HeuristicProblem,
                    0);
                bool blnExistsIndividual = false;
                if (individual == null)
                {
                    individual = bestIndividual;
                }
                else
                {
                    blnExistsIndividual = true;
                }
                bool blnImprovementFound = false;
                if (BestFitness < individual.Fitness)
                {
                    if (blnExistsIndividual)
                    {
                        IterateGeneration();
                    }
                    BestFitness = individual.Fitness;
                    CurrentConvergence = 0;
                    blnImprovementFound = true;
                    SaveBestIndividual();
                    InvokeOnImprovementFound(individual);
                }
                else
                {
                    CurrentConvergence++;
                }

                OptiGuiHelper.UpdateConvergence(bestIndividual);


                if (MaxConvergence < CurrentConvergence)
                {
                    MaxConvergence = CurrentConvergence;
                }
                return blnImprovementFound;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return false;
        }

        private void LoadInitialPopulation()
        {
            try
            {
                HeuristicProblem.InitialPopulation.GetInitialPopulation();


                //
                // load population
                //
                IterateGeneration();

                if (OnCompletedGeneration != null)
                {
                    OnCompletedGeneration.Invoke(HeuristicProblem);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        private void InvokeOnCompletedGeneration()
        {
            try
            {
                if (OnCompletedGeneration != null)
                {
                    if (OnCompletedGeneration.GetInvocationList().Length > 0)
                    {
                        OnCompletedGeneration.Invoke(
                            HeuristicProblem);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        ///   Initialize solver
        /// </summary>
        private void InitializeSolver()
        {
            try
            {
                ResetSolverStatus();
                LoadInitialPopulation();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void ResetSolverStatus()
        {
            try
            {
                IterateSolver = true;
                MaxConvergence = -1;
                StartTime = DateTime.Now;
                BestFitness = -double.MaxValue;
                CurrentIteration = 0;
                SolutionsExplored = 0;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        ///   Call this method each time the local search 
        ///   operator finds an improvement
        /// </summary>
        private void InvokeOnImprovementFoundEvent()
        {
            // reset convergence
            CurrentConvergence = 0;
        }

        private void InvokeOnImprovementFound(Individual bestIndividual)
        {
            try
            {
                if (OnImprovementFound != null)
                {
                    if (OnImprovementFound.GetInvocationList().Length > 0)
                    {
                        OnImprovementFound.Invoke(HeuristicProblem, bestIndividual);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void InvokeOnSoverFinishes()
        {
            try
            {
                if (OnSolverFinish != null)
                {
                    if (OnSolverFinish.GetInvocationList().Length > 0)
                    {
                        OnSolverFinish.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
                if (OptiGuiHelper != null)
                {
                    OptiGuiHelper.Dispose();
                    OptiGuiHelper = null;
                }
                if (HeuristicExecutionHelper != null)
                {
                    HeuristicExecutionHelper.Dispose();
                    HeuristicExecutionHelper = null;
                }
                EventHandlerHelper.RemoveAllEventHandlers(this);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void InvokeOnInitialPopulationCompleted()
        {
            try
            {
                if (OnInitialPopulationCompleted != null)
                {
                    OnInitialPopulationCompleted.Invoke();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void InvokeOnInitialPopulationStarted()
        {
            try
            {
                if (OnInitialPopulationStarted != null)
                {
                    OnInitialPopulationStarted.Invoke();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion
    }
}
