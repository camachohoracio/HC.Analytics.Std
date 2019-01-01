#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses
{
    /// <summary>
    ///   Worker for evaluating individuals.
    ///   Note: This class is non threadsafe.
    /// </summary>
    public class IndividualWorker : IDisposable
    {
        #region Events

        #region Delegates

        /// <summary>
        ///   Event called when exception ocurs while evaluating individual
        /// </summary>
        /// <param name = "e">
        ///   HCException
        /// </param>
        public delegate void IndividualExceptionOccurredEventHandler(
            HCException e);

        /// <summary>
        ///   Event called when individual is finish with evaluation
        /// </summary>
        public delegate void WorkderReadyEventHandler(Individual individual);

        /// <summary>
        ///   Event called when individual is finish with evaluation
        /// </summary>
        public delegate void WorkerFinishedEventHandler();

        #endregion

        /// <summary>
        ///   Event called when exception ocurs while evaluating individual
        /// </summary>
        public event IndividualExceptionOccurredEventHandler ExceptionOccurred;

        /// <summary>
        ///   Event called when individual is finish with evaluation
        /// </summary>
        public event WorkderReadyEventHandler WorkerReadyEventHandler;

        /// <summary>
        ///   Event called when individual is finish with evaluation
        /// </summary>
        public event WorkerFinishedEventHandler OnWorkerFinishedEventHandler;

        #endregion

        #region Members

        private HeuristicExecutionHelper m_heuristicExecutionHelper;
        private HeuristicProblem m_heuristicProblem;

        /// <summary>
        ///   IIndividual to be evaluated
        /// </summary>
        private Individual m_individual;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constrcutor
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual to be evalauted
        /// </param>
        public IndividualWorker(
            Individual individual,
            HeuristicExecutionHelper heuristicExecutionHelper,
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            m_individual = individual;
            m_heuristicExecutionHelper = heuristicExecutionHelper;
        }

        #endregion

        #region Private

        /// <summary>
        ///   Free memory when object is disposed
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///   Do work
        /// </summary>
        private void DoWork()
        {
            try
            {
                m_individual.Evaluate(m_heuristicProblem);
                InvokeWokerReadyEventHandler(m_individual);

                while (m_heuristicExecutionHelper.RunThread)
                {
                    IterateIndividual();
                }
            }
            catch (HCException e2)
            {
                //Logger.GetLogger().Write(e2);
                //Debugger.Break();
                OnExceptionOccurred(e2);
            }
            InvokeWokerFinishedEventHandler();
        }

        public void IterateIndividual()
        {
            try
            {
                if (m_heuristicExecutionHelper.RunThread)
                {
                    if (m_heuristicExecutionHelper.BatchRunSize > 0)
                    {
                        var individualList = new List<Individual>();
                        for (int i = 0; i < m_heuristicExecutionHelper.BatchRunSize; i++)
                        {
                            Individual newIndividual =
                                m_heuristicProblem.Reproduction.DoReproduction();
                            individualList.Add(newIndividual);
                        }
                        m_heuristicExecutionHelper.RunBatchIndividuals(individualList);
                    }
                    else
                    {
                        Individual newIndividual =
                            m_heuristicProblem.Reproduction.DoReproduction();
                        newIndividual.Evaluate(m_heuristicProblem);
                        InvokeWokerReadyEventHandler(newIndividual);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Public

        /// <summary>
        ///   Do Work
        /// </summary>
        public void Work()
        {
            DoWork();

            //ThreadWorker worker = new ThreadWorker();
            //worker.WaitForExit = true;
            //worker.m_onDelegateThreadExecute += DoWork;
            //worker.Work();
        }

        /// <summary>
        ///   Call this method when worker is finish
        /// </summary>
        public void InvokeWokerReadyEventHandler(Individual individual)
        {
            if (WorkerReadyEventHandler != null)
            {
                if (WorkerReadyEventHandler.GetInvocationList().Length > 0)
                {
                    WorkerReadyEventHandler.Invoke(individual);
                }
            }
        }

        /// <summary>
        ///   Call this method when worker is finish
        /// </summary>
        public void InvokeWokerFinishedEventHandler()
        {
            if (OnWorkerFinishedEventHandler != null)
            {
                if (OnWorkerFinishedEventHandler.GetInvocationList().Length > 0)
                {
                    OnWorkerFinishedEventHandler.Invoke();
                }
            }
        }

        #endregion

        #region Protected

        /// <summary>
        ///   Call this method when worker evaluation throws an exception
        /// </summary>
        /// <param name = "e"></param>
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

        #endregion

        ~IndividualWorker()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_heuristicExecutionHelper = null;
            m_heuristicProblem = null;
            m_individual = null;
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }
    }
}
