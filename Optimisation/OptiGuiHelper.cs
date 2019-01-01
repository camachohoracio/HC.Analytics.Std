#region

using System;
using System.Text;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Analytics.TimeSeries.TsStats;
using HC.Core;
using HC.Core.Events;
using HC.Core.Logging;
using HC.Core.Threading.ProducerConsumerQueues;

#endregion

namespace HC.Analytics.Optimisation
{
    public class OptiGuiHelper : IDisposable, IOptiGuiHelper
    {
        public SelfDescribingTsEvent SolverStats { get; set; }

        #region Members

        private EvolutionarySolver m_evolutionarySolver;
        private EfficientWorkerManager<object> m_efficientQueueChart;
        private RollingWindowStdDev m_individualStats;
        private DateTime m_lastUpdateTime;
        private readonly string m_strSolverName;
        private int m_intPreviousPercentage;
        private DateTime m_prevProgress;

        #endregion

        #region Constructors

        public OptiGuiHelper(
            EvolutionarySolver evolutionarySolver)
        {
            try
            {
                m_evolutionarySolver = evolutionarySolver;
                m_individualStats = new RollingWindowStdDev(100);
                //
                // get solver stats
                //
                m_strSolverName = evolutionarySolver.GetSolverName();
                string strClassName =
                    "name_" +
                    (evolutionarySolver.HeuristicProblem.ProblemName + "_" +
                     (m_strSolverName ?? string.Empty))
                        .Replace(";", "_")
                        .Replace(",", "_")
                        .Replace(".", "_")
                        .Replace(":", "_")
                        .Replace("-", "_");
                SolverStats = new SelfDescribingTsEvent(
                    strClassName);
                SolverStats.SetStrValue(
                    EnumEvolutionarySolver.ProblemName,
                    evolutionarySolver.HeuristicProblem.ProblemName);
                SolverStats.SetStrValue(
                    EnumEvolutionarySolver.SolverName,
                    m_strSolverName);
                SolverStats.SetIntValue(
                    EnumEvolutionarySolver.MaxConvergence,
                    evolutionarySolver.MaxConvergence);

                m_efficientQueueChart = new EfficientWorkerManager<object>(1)
                                            {
                                                WaitMillSec = 10*1000
                                            };
                m_efficientQueueChart.SetAutoDisposeTasks(true);
                m_efficientQueueChart.OnWork += obj => PublishGridStats();
                m_efficientQueueChart.AddItem("PublishQueue", null);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

        public void PublishGridStats()
        {
            try
            {
                string strProblemName;
                if(!SolverStats.TryGetStrValue(
                        EnumEvolutionarySolver.ProblemName,
                        out strProblemName) ||
                    string.IsNullOrEmpty(strProblemName))
                {
                    return;
                }
                SolverStats.Time = DateTime.Now;
                
                LiveGuiPublisherEvent.PublishGrid(
                    m_evolutionarySolver.HeuristicProblem.GuiNodeName,
                    strProblemName,
                    EnumEvolutionarySolver.SolverStats.ToString(),
                    SolverStats.GetClassName(),
                    SolverStats,
                    2,
                    true);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void UpdateStats()
        {
            //
            // update stats of how often an individual is evaluated
            //
            DateTime now = DateTime.Now;
            if (m_lastUpdateTime.Year == now.Year)
            {
                m_individualStats.Update((now - m_lastUpdateTime).TotalSeconds);
                SolverStats.SetDblValue(
                    EnumEvolutionarySolver.IndividualsPerSecond,
                    1.0 / m_individualStats.Mean);
            }
            m_lastUpdateTime = now;
        }

        public void SetFinishStats()
        {
            try
            {
                SolverStats.SetBlnValue(
                    EnumEvolutionarySolver.StopSolver,
                    true);
                m_efficientQueueChart.AddItem("PublishQueue", null);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public int SetProgress(
            bool blnImprovementFound,
            int intPercentage)
        {
            try
            {
                Individual bestIndividual = null;
                if (blnImprovementFound)
                {
                    if (m_evolutionarySolver.HeuristicProblem.Verbose)
                    {
                        if ((DateTime.Now - m_prevProgress).TotalSeconds > 2)
                        {
                            bestIndividual = m_evolutionarySolver.HeuristicProblem.Population.
                                GetIndividualFromPopulation(
                                    m_evolutionarySolver.HeuristicProblem,
                                    0);
                            var sb = new StringBuilder();
                            sb.Append(Environment.NewLine + "-------------------------------------" +
                                      Environment.NewLine);
                            sb.Append("(*) " + m_strSolverName + ". Iteration " +
                                      m_evolutionarySolver.CurrentIteration + " of ");
                            sb.Append(m_evolutionarySolver.HeuristicProblem.Iterations);
                            sb.Append(", percentage: " + intPercentage);
                            sb.Append(Environment.NewLine + "Best Individual Description:");
                            sb.Append(Environment.NewLine +
                                      m_evolutionarySolver.HeuristicProblem.Population.GetIndividualFromPopulation(
                                          m_evolutionarySolver.HeuristicProblem,
                                          0));
                            sb.Append(Environment.NewLine + Environment.NewLine + "Objective = " +
                                      bestIndividual.Fitness);
                            sb.Append(Environment.NewLine + Environment.NewLine +
                                      m_evolutionarySolver.HeuristicProblem.Population.ToStringPopulationStats());
                            sb.Append(Environment.NewLine + "-------------------------------------" +
                                      Environment.NewLine);

                            var strMessage = sb.ToString();
                            PublishLog(strMessage);
                            m_evolutionarySolver.InvokeOnUpdateProgress(strMessage, intPercentage);
                            m_prevProgress = DateTime.Now;
                        }
                    }
                }
                if (m_intPreviousPercentage != intPercentage)
                {
                    if (m_evolutionarySolver.HeuristicProblem.Verbose)
                    {
                        if ((DateTime.Now - m_prevProgress).TotalSeconds > 2)
                        {
                            bestIndividual = m_evolutionarySolver.HeuristicProblem.Population.
                                GetIndividualFromPopulation(
                                    m_evolutionarySolver.HeuristicProblem,
                                    0);
                            m_intPreviousPercentage = intPercentage;
                            var sb = new StringBuilder();
                            sb.Append(Environment.NewLine + "-------------------------------------" +
                                      Environment.NewLine);
                            sb.Append(m_strSolverName + ", " + intPercentage + "% completed...");
                            sb.Append(Environment.NewLine + "Iteration " + m_evolutionarySolver.CurrentIteration +
                                      " of ");
                            sb.Append(m_evolutionarySolver.HeuristicProblem.Iterations);
                            sb.Append(Environment.NewLine + Environment.NewLine + "Objective = ");
                            sb.Append(bestIndividual.Fitness);
                            sb.Append(Environment.NewLine + Environment.NewLine +
                                      m_evolutionarySolver.HeuristicProblem.Population.ToStringPopulationStats());
                            sb.Append(Environment.NewLine + "-------------------------------------" +
                                      Environment.NewLine);
                            string strMessage = sb.ToString();
                            PublishLog(strMessage);
                            m_evolutionarySolver.InvokeOnUpdateProgress(strMessage, intPercentage);
                            m_prevProgress = DateTime.Now;
                        }
                    }
                }

                if (bestIndividual != null)
                {
                    SolverStats.SetIntValue(
                        EnumEvolutionarySolver.Percentage,
                        intPercentage);
                    UpdateSolverStats(
                        bestIndividual);
                    m_efficientQueueChart.AddItem("PublishQueue", null);
                }

                return intPercentage;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public void PublishLog(
            string strLog)
        {
            LiveGuiPublisherEvent.PublishLog(
                m_evolutionarySolver.HeuristicProblem.GuiNodeName,
                m_evolutionarySolver.HeuristicProblem.ProblemName,
                EnumEvolutionarySolver.Log.ToString(),
                Guid.NewGuid().ToString(),
                strLog);
        }

        private void UpdateSolverStats(
            Individual bestIndividual)
        {
            try
            {
                string strPopulationStats = m_evolutionarySolver.HeuristicProblem.Population.ToStringPopulationStats();
                double dblTotalTimeSecs = ((DateTime.Now - m_evolutionarySolver.StartTime)).TotalSeconds;
                SolverStats.SetDblValue(
                    EnumEvolutionarySolver.MaxIterations,
                    m_evolutionarySolver.HeuristicProblem.Iterations);
                SolverStats.SetDblValue(
                    EnumEvolutionarySolver.Objective,
                    bestIndividual.Fitness);
                SolverStats.SetStrValue(
                    EnumEvolutionarySolver.BestIndividual,
                    bestIndividual.ToString());
                SolverStats.SetStrValue(
                    EnumEvolutionarySolver.PopulationStats,
                    strPopulationStats);
                SolverStats.SetDblValue(
                    EnumEvolutionarySolver.TotalTimeMins,
                    dblTotalTimeSecs/60.0);
                SolverStats.SetIntValue(
                    EnumEvolutionarySolver.SolutionsExplored,
                    m_evolutionarySolver.SolutionsExplored);
                SolverStats.SetIntValue(
                    EnumEvolutionarySolver.Iterations,
                    m_evolutionarySolver.CurrentIteration);
                SolverStats.SetDateValue(
                    EnumEvolutionarySolver.StartTime,
                    m_evolutionarySolver.StartTime);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public void UpdateConvergence(Individual bestIndividual)
        {
            try
            {
                SolverStats.SetIntValue(
                    EnumEvolutionarySolver.CurrentConvergence,
                    m_evolutionarySolver.CurrentConvergence);
                UpdateSolverStats(
                    bestIndividual);
                m_efficientQueueChart.AddItem("PublishQueue", null);
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
        }

        ~OptiGuiHelper()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_evolutionarySolver = null;
            if(SolverStats != null)
            {
                SolverStats.Dispose();
            }
            if(m_efficientQueueChart != null)
            {
                m_efficientQueueChart.Dispose();
                m_efficientQueueChart = null;
            }
            if(m_individualStats != null)
            {
                m_individualStats.Dispose();
                m_individualStats = null;
            }
            EventHandlerHelper.RemoveAllEventHandlers(this);
        }
    }
}
