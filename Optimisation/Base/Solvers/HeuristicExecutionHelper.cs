#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers
{
    [Serializable]
    public class HeuristicExecutionHelper : IDisposable
    {
        #region Properties

        public bool RunThread { get; private set; }
        public int BatchRunSize { get; set; }

        public delegate void RunBatchIndividualsDel(List<Individual> individualList);

        public event RunBatchIndividualsDel OnRunBatchIndividuals;
        
        #endregion

        #region Members

        private HeuristicProblem m_heuristicProblem;

        /// <summary>
        ///   Thread workers. 
        ///   Each individual worker generates a new offspring in a loop
        /// </summary>
        private IndividualWorker[] m_workerArr;

        #endregion

        #region Constructors

        public HeuristicExecutionHelper(HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            RunThread = true;
        }

        #endregion

        #region Public Methods

        public void ExecuteSolver()
        {
            try
            {
                if (m_heuristicProblem.Threads > 1)
                {
                    // run individual threads
                    RunIndividualThreads();
                }
                else if (m_heuristicProblem.Threads == 1)
                {
                    // run one single thread
                    RunOneThread();
                }
                else if (m_heuristicProblem.Threads == 0)
                {
                    // run all threads at once
                    RunAllThreads();
                }
                //
                // load population for last iteration
                //
                m_heuristicProblem.Population.LoadPopulation();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        #region Private methods for one single threads execution

        private void RunOneThread()
        {
            var individualWorker = CreateIndividualWorker();

            for (var i = 0;
                 i <
                 m_heuristicProblem.Iterations*
                 m_heuristicProblem.PopulationSize;
                 i++)
            {
                if (RunThread)
                {
                    individualWorker.IterateIndividual();
                }
                else
                {
                    break;
                }
            }

            FinishThreads();
        }

        #endregion

        #region Private methods for all threads execution

        private void RunAllThreads()
        {
            RunAllThreads0();
            FinishThreads();
        }

        private void RunAllThreads0()
        {
            var individualWorker = CreateIndividualWorker();
            // load the initial threads
            Parallel.For(0,
                         m_heuristicProblem.Iterations*
                         m_heuristicProblem.PopulationSize, delegate(int i)
                                                                //for (int i = 0; i <
                                                                //HeuristicProblem.Threads; i++)
                                                                {
                                                                    try
                                                                    {
                                                                        if (RunThread)
                                                                        {
                                                                            individualWorker.IterateIndividual();
                                                                        }
                                                                    }
                                                                    catch(Exception ex)
                                                                    {
                                                                        Logger.Log(ex);
                                                                    }
                                                                });
        }

        #endregion

        #region Private methods for threaded execution

        public void FinishSolver()
        {
            // run individual threads
            FinishThreads();
        }

        private void RunIndividualThreads()
        {
            CreateIndividualWorkers();

            // load the initial threads
            Parallel.For(0, m_heuristicProblem.Threads, i => m_workerArr[i].Work());

            FinishThreads();
        }

        private void CreateIndividualWorkers()
        {
            m_workerArr = new IndividualWorker[
                m_heuristicProblem.Threads];

            for (var i = 0;
                 i <
                 m_heuristicProblem.Threads;
                 i++)
            {
                var worker = CreateIndividualWorker();
                m_workerArr[i] = worker;
            }
        }

        private IndividualWorker CreateIndividualWorker()
        {
            try
            {
                Individual newIndividual =
                    m_heuristicProblem.Reproduction.DoReproduction();

                var worker = new IndividualWorker(
                    newIndividual,
                    this,
                    m_heuristicProblem);

                worker.WorkerReadyEventHandler +=
                    m_heuristicProblem.Solver.IterateIndividual;

                return worker;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        /// <summary>
        ///   Finalize threads
        /// </summary>
        private void FinishThreads()
        {
            RunThread = false;
        }

        #endregion

        internal void RunBatchIndividuals(List<Individual> individualList)
        {
            if(OnRunBatchIndividuals != null)
            {
                OnRunBatchIndividuals(individualList);
                for (int i = 0; i < individualList.Count; i++)
                {
                    individualList[i].AckEvaluate(m_heuristicProblem);
                    List<Individual> innerIndividualList = individualList[i].IndividualList;
                    for (int j = 0; j < innerIndividualList.Count; j++)
                    {
                        innerIndividualList[j].SetFitnessValue(individualList[i].Fitness);
                    }

                    m_workerArr[0].InvokeWokerReadyEventHandler(individualList[i]);
                }
            }
            else
            {
                throw new HCException("Event not subscribed to batch exectution");
            }
        }

        ~HeuristicExecutionHelper()
        {
            Dispose();
        }

        public void Dispose()
        {
            RunThread = false;
            if(m_workerArr != null)
            {
                for (int i = 0; i < m_workerArr.Length; i++)
                {
                    m_workerArr[i].Dispose();
                }
                m_workerArr = null;
            }
            m_heuristicProblem = null;
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }
    }
}
