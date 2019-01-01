#region

using System;
using System.Collections.Generic;
using System.IO;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.ConfigClasses;
using HC.Core.Events;
using HC.Core.Io.TailFilesClasses;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public abstract class AOptEngine : IDisposable
    {
        #region Properties

        public AOptPrblmFctry OptPrblmFctry { get; set; }

        public bool EnablePlots { get; set; }
        public HeuristicProblem HeuristicProblem { get; set; }
        public AOptParams OptParams { get; protected set; }

        #endregion

        #region Members

        protected static readonly object m_lockObject = new object();
        protected OptChromosomeFactory m_optChromosomeFactory;
        protected CsvWriter m_csvWriter;
        protected List<IHeuristicObjectiveFunction> m_objectiveFunctions;

        #endregion

        protected void SetCsvLogger(string strFileName)
        {
            var fi = new FileInfo(strFileName);
            var strTradingEngineName = fi.Name.Replace(fi.Extension, string.Empty);
            var strLogFileName = strTradingEngineName + "_optimisation.csv";
            strLogFileName = Path.Combine(
                HCConfig.ResultsPath,
                strLogFileName);
            m_csvWriter = new CsvWriter(strLogFileName);
        }

        #region Dispose

        public void SolveMultiObjective()
        {
            //
            // construct lower bound for current problem
            //
            SendMessageEvent.OnSendMessage("Computing lower bound...", -1);
            GetLowerBoundMixed(HeuristicProblem);
            // solve problem
            HeuristicProblem.Solver.Solve();
        }

        protected abstract void GetLowerBoundMixed(HeuristicProblem heuristicProblem);

        protected void BuildProblem(
            AOptPrblmFctry optPrblmFctry,
            string strProblemName,
            string strGuiNodeName)
        {

            HeuristicProblem =
                optPrblmFctry.BuildProblem(
                strProblemName,
                strGuiNodeName);

            //
            // get objective function list
            //
            if (HeuristicProblem.ObjectiveFunction is
                HeuristicMultiObjectiveFunction)
            {
                m_objectiveFunctions =
                    ((HeuristicMultiObjectiveFunction)HeuristicProblem.
                                                           ObjectiveFunction).ObjectiveFunctions;
            }
            else
            {
                m_objectiveFunctions =
                    new List<IHeuristicObjectiveFunction>
                        {
                            HeuristicProblem.ObjectiveFunction
                        };
            }

            ((OptObjFunct)m_objectiveFunctions[0]).
                OnObjectiveAfterEvaluatedEvent +=
                OnObjectiveAfterAfterEvaluateEventHandler;
        }

        ~AOptEngine()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_csvWriter != null)
            {
                m_csvWriter.Close();
                m_csvWriter = null;
            }
        }


        private void LogResults(
            OptStatsCache optStatsCache)
        {
            if (m_csvWriter == null)
            {
                return;
            }

            //
            // log strategy params
            //
            foreach (KeyValuePair<string, string> keyValuePair in
                (optStatsCache.StrategyParams).GetStringValues())
            {
                var strHeader = keyValuePair.Key;
                var strValue = keyValuePair.Value.Trim()
                    .Replace('\n', ' ')
                    .Replace('\r', ' ')
                    .Replace('\t', ' ');
                m_csvWriter.update(DateTime.Now, "params_" + strHeader, strValue);
            }

            //
            // log stats map
            //
            if (optStatsCache.StatsMap != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in
                    (optStatsCache.StatsMap).GetStringValues())
                {
                    var strHeader = keyValuePair.Key;
                    var strValue = keyValuePair.Value.Trim()
                        .Replace('\n', ' ')
                        .Replace('\r', ' ')
                        .Replace('\t', ' ');
                    m_csvWriter.update(DateTime.Now, "stats_" + strHeader, strValue);
                }
            }
        }

        protected void OnObjectiveAfterAfterEvaluateEventHandler(
            Individual individual,
            OptObjFunct optObjFunction,
            double dblLogTime,
            OptStatsCache optStatsCache)
        {
            lock (m_lockObject)
            {
                LogResults(optStatsCache);
            }
        }

        #endregion
    }
}

