#region

using System;
using System.Threading.Tasks;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.MixedSolvers.DummyObjectiveFunctions;
using HC.Core;
using HC.Core.Events;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.PopulationClasses
{
    /// <summary>
    ///   For very large problems the Genetic Algorithm requires a 
    ///   large number of iterations to converge to its local optimal. 
    ///   In order to enhance the converge speed we generate a 
    ///   set of “improved solutions”. 
    ///   These solutions will be input to the Genetic Algorithm which 
    ///   will be re-sampling more good solutions.
    /// </summary>
    [Serializable]
    public class RandomInitialPopulation : IInitialPopulation
    {
        #region EnumEvolutionarySolver enum

        public enum Enums
        {
            ProblemName,
            Individual,
            Iteration,
            TimeSecs,
            InitialPopulationStats,
            Fitness,
            PopulationSize,
            Percentage
        }

        #endregion

        #region Properties

        public bool DoLocalSearch { get; set; }

        #endregion

        #region Events

        /// <summary>
        ///   Event fired when an exception is thrown when calculating
        ///   the inicital population
        /// </summary>
        public event ExceptionOccurredEventHandler ExceptionOccurred;

        /// <summary>
        ///   Progress bar event
        /// </summary>
        public event UpdateInitialPopProgressEventHandler UpdateProgress;

        #endregion

        #region Members

        private readonly HeuristicProblem m_heuristicProblem;
        private readonly SelfDescribingTsEvent m_initialPopulationStats;
        private readonly object m_lockObject1 = new object();
        private readonly object m_lockObject2 = new object();
        private int m_intInitialPoolIndividualReady;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        public RandomInitialPopulation(
            HeuristicProblem heuristicProblem)
        {
            DoLocalSearch = true;
            m_heuristicProblem = heuristicProblem;
            var strClassName =
                "IntitalPopulationStats_" + GetType().Name;
            m_initialPopulationStats = new SelfDescribingTsEvent(
                strClassName);
            m_initialPopulationStats.SetStrValue(
                Enums.ProblemName,
                heuristicProblem.ProblemName);
            PublishGridStats();
        }

        #endregion

        #region Public

        /// <summary/>
        ///   Gets the initial population. Do an extensive local search in 
        ///   order to start the GA with good invididuals and reduce the number
        ///   of GA iterations.
        ///   <returns>
        ///     Population
        ///   </returns>
        public void GetInitialPopulation()
        {
            try
            {
                m_heuristicProblem.Solver.InvokeOnInitialPopulationStarted();
                string strMessage =
                    m_heuristicProblem.Solver.GetSolverName() +
                    Environment.NewLine + "Generating random initial population. Please wait...";
                PrintToScreen.WriteLine(strMessage);
                UpdateProgressBar(-1, strMessage);
                lock (m_lockObject2)
                {
                    //
                    // generate random population
                    //
                    if (m_heuristicProblem.Threads == 1)
                    {
                        for (int i = 0; i < m_heuristicProblem.PopulationSize; i++)
                        {
                            IterateIndivudual();
                        }

                    }
                    else
                    {
                        Parallel.For(
                            0,
                            m_heuristicProblem.PopulationSize,
                             new ParallelOptions { MaxDegreeOfParallelism = m_heuristicProblem.Threads > 0 ? m_heuristicProblem.Threads : 50 },
                            i => IterateIndivudual()
                            );
                    }
                    m_heuristicProblem.Population.LoadPopulation();
                    // evaluate random population in parallel
                    strMessage = "Finish evaluating random initial pool.";
                    PrintToScreen.WriteLine(strMessage);
                    UpdateProgressBar(-1, strMessage);
                    m_heuristicProblem.Solver.InvokeOnInitialPopulationCompleted();
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void IterateIndivudual()
        {
            Individual currentIndividual =
                m_heuristicProblem.IndividualFactory.BuildRandomIndividual();

            if (currentIndividual.IsEvaluated ||
                currentIndividual.IsReadOnly())
            {
                throw new HCException("Invalid individual state");
            }

            EvaluateIndividual(currentIndividual);
        }

        #endregion

        #region Private

        /// <summary>
        ///   Evaluate individual
        /// </summary>
        /// <param name = "individual">
        ///   Evaluate individual
        /// </param>
        /// <returns></returns>
        private void EvaluateIndividual(Individual individual)
        {
            var dateLog = DateTime.Now;

            individual.Evaluate(DoLocalSearch,
                                true,
                                true,
                                m_heuristicProblem);

            lock (m_lockObject1)
            {
                m_intInitialPoolIndividualReady++;
            }

            var intPercentage = (m_intInitialPoolIndividualReady*100)/
                                m_heuristicProblem.PopulationSize;

            intPercentage =
                OptimisationConstants.INT_LOAD_DATA_PERCENTAGE +
                intPercentage*OptimisationConstants.INT_INITAL_POOL_PERCENTAGE/100;

            var strMessage = "Finish evaluating initial pool. Individual " +
                             m_intInitialPoolIndividualReady + " of " +
                             m_heuristicProblem.PopulationSize;
            strMessage = strMessage + ". Percentage = " + intPercentage + "%";
            Verboser.WriteLine(strMessage);
            UpdateProgressBar(intPercentage, strMessage);
            Logger.Log(strMessage);

            //
            // publish grid stats
            //
            m_initialPopulationStats.SetStrValue(
                Enums.Individual,
                individual.ToString());
            m_initialPopulationStats.SetDblValue(
                Enums.Fitness,
                individual.Fitness);
            m_initialPopulationStats.SetDblValue(
                Enums.TimeSecs,
                (DateTime.Now - dateLog).TotalSeconds);
            m_initialPopulationStats.SetIntValue(
                Enums.Iteration,
                m_intInitialPoolIndividualReady);
            m_initialPopulationStats.SetIntValue(
                Enums.PopulationSize,
                m_heuristicProblem.PopulationSize);
            m_initialPopulationStats.SetIntValue(
                Enums.Percentage,
                intPercentage);
            m_heuristicProblem.Solver.InvokeOnIndividualEvaluated(individual);
            PublishGridStats();
        }

        #endregion

        #region Protected

        private void PublishGridStats()
        {
            try
            {
                if (!m_heuristicProblem.DoPublish)
                {
                    return;
                }

                if (!(m_heuristicProblem.ObjectiveFunction is ObjectiveFunctionDummy))
                {
                    var strProblemName =
                        m_initialPopulationStats.GetStrValue(
                            Enums.ProblemName);
                    m_initialPopulationStats.Time = DateTime.Now;
                    LiveGuiPublisherEvent.PublishGrid(
                        m_heuristicProblem.GuiNodeName,
                        strProblemName,
                        Enums.InitialPopulationStats.ToString(),
                        Guid.NewGuid().ToString(),
                        m_initialPopulationStats,
                        2);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        ///   Progress bar method
        /// </summary>
        /// <param name = "intProgress">
        ///   Progress value
        /// </param>
        /// <param name = "strMessage">
        ///   Progress message
        /// </param>
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
    }
}
