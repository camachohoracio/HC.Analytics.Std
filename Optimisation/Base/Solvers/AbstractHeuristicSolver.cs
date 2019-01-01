#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers
{
    /// <summary>
    ///   Abstract solver.
    ///   Generic form of a solver
    /// </summary>
    [Serializable]
    public abstract class AbstractHeuristicSolver : ISolver
    {
        #region Members

        /// <summary>
        ///   Solver's best individual
        /// </summary>
        protected Individual m_bestIndividual;

        /// <summary>
        ///   Best fitness found so far
        /// </summary>
        public double BestFitness { get; protected set; }

        /// <summary>
        ///   Final results percentage of completion
        /// </summary>
        protected int m_intFinalResultsPercentage;

        protected int m_intIterationCounter;


        /// <summary>
        ///   Number of results calculated
        /// </summary>
        protected int m_intResultCounter;

        protected string m_strSolverName;

        #endregion

        #region Events

        public event FinishSolverDel OnSolverFinished;
        public event SolverExceptionOccurredDel ExceptionOccurred;
        public event UpdateSolverProgressDel UpdateProgress;

        #endregion

        #region Properties

        /// <summary>
        ///   Current level of convergence.
        ///   This value is reset if an improvement is found
        /// </summary>
        public int CurrentConvergence { get; protected set; }
        
        /// <summary>
        ///   Solver start time
        /// </summary>
        public DateTime StartTime { get; protected set; }

        /// <summary>
        ///   Solutions explored
        /// </summary>
        public int SolutionsExplored { get; protected set; }

        /// <summary>
        ///   Current iteration
        /// </summary>
        public int CurrentIteration { get; protected set; }
        
        /// <summary>
        ///   Maximum level of convergence. The covergence
        ///   value is reset to zero if an improvement is
        ///   found
        /// </summary>
        public int MaxConvergence { get; protected set; }

        /// <summary>
        ///   Number of results explored by the solver
        ///   finished
        /// </summary>
        public List<ResultRow> ResultList { get; set; }

        /// <summary>
        ///   Number of results to display once the solver is
        /// </summary>
        public int ResultsSize { get; set; }

        /// <summary>
        ///   Flag which states if the final results are to be analysed
        /// </summary>
        public bool EvaluateFinalResults { get; set; }

        /// <summary>
        ///   Completion percentage
        /// </summary>
        public int InitialCompletionPercentage { get; set; }

        /// <summary>
        ///   Total percentage value
        /// </summary>
        public int PercentageCompletionValue { get; set; }

        public HeuristicProblem HeuristicProblem { get; protected set; }

        #endregion

        #region Constructors

        public AbstractHeuristicSolver(
            HeuristicProblem heuristicProblem)
        {
            HeuristicProblem = heuristicProblem;
            EvaluateFinalResults = true;
            m_intFinalResultsPercentage = OptimisationConstants.INT_FINAL_RESULTS_PERCENTAGE;
            ResultsSize = Config.GetResultSize();
        }

        #endregion

        #region Private

        #endregion

        #region Protected

        /// <summary>
        ///   Save best individual distcription into a text file
        /// </summary>
        protected void SaveBestIndividual()
        {
        }

        /// <summary>
        ///   Finalize solver
        /// </summary>
        /// <param name = "resultList">
        ///   List with results
        /// </param>
        protected void InvokeSolverFinished(List<ResultRow> resultList)
        {
            if (OnSolverFinished != null)
            {
                if (OnSolverFinished.GetInvocationList().Length > 0)
                {
                    OnSolverFinished.Invoke(resultList);
                }
            }
        }

        /// <summary>
        ///   Event called when an exception is thrown
        /// </summary>
        /// <param name = "e">
        ///   HCException
        /// </param>
        protected void OnExceptionOccurred(HCException e)
        {
            if (ExceptionOccurred != null)
            {
                if (ExceptionOccurred.GetInvocationList().Length > 0)
                {
                    ExceptionOccurred(e);
                }
            }
        }

        /// <summary>
        ///   Update progress bar
        /// </summary>
        /// <param name = "intProgress">
        ///   Progress value
        /// </param>
        /// <param name = "strMessage"></param>
        protected void UpdateProgressBar(int intProgress, string strMessage)
        {
            if (UpdateProgress != null)
            {
                if (UpdateProgress.GetInvocationList().Length > 0)
                {
                    UpdateProgress(intProgress, strMessage);
                }
            }
        }

        #endregion

        #region Abstract

        /// <summary>
        ///   Solve
        /// </summary>
        public abstract void Solve();

        /// <summary>
        ///   Get the name of the current solver
        /// </summary>
        /// <returns></returns>
        public string GetSolverName()
        {
            return (m_strSolverName ?? string.Empty);
        }

        public void SetSolverName(string strSolverName)
        {
            m_strSolverName = strSolverName;
        }

        #endregion

        public virtual void Dispose()
        {
            if(ResultList != null)
            {
                ResultList.Clear();
            }
            m_bestIndividual = null;
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }
    }
}
