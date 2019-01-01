#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptObjFunct : IHeuristicObjectiveFunction
    {
        public event OnObjectiveCallDel OnObjectiveAfterEvaluatedEvent;

        #region Properties

        public string ObjectiveName { get; set; }
        
        public IOptInstFctry InstFctry { get; private set; }
        
        public double FunctionMultiplier { get; private set; }

        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { return ObjectiveFunctionType.STD_OBJECTIVE_FUNCT; }
        }

        public AOptParams OptParams { get; private set; }

        #endregion

        #region Members

        private readonly OptChromosomeFactory m_chromosomeFactory;
        private readonly int m_intParamsCount;

        #endregion

        #region Constructors

        public OptObjFunct(
            IOptInstFctry instFctry,
            OptChromosomeFactory chromosomeFactory,
            string strParameterName,
            double dblFunctionMultiplier,
            AOptParams optParams)
        {
            ObjectiveName = strParameterName;
            OptParams = optParams;
            m_chromosomeFactory = chromosomeFactory;
            InstFctry = instFctry;
            FunctionMultiplier = dblFunctionMultiplier;
            m_intParamsCount =
                OptParams.IntParams.Count +
                OptParams.DblParams.Count +
                OptParams.BlnParams.Count;
        }

        #endregion

        #region Public

        public List<VariableContribution> GetRankList()
        {
            throw new NotImplementedException();
        }

        public ResultRow GetResultObject(
            Individual solution,
            bool blnGetFinalResults)
        {
            throw new NotImplementedException();
        }

        public double Evaluate()
        {
            throw new NotImplementedException();
        }

        public int VariableCount
        {
            get { return m_intParamsCount; }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            var dateLog = DateTime.Now;

            var optStatsCache = OptChromosomeFactory.GetOptStatsCache(
                individual,
                InstFctry,
                m_chromosomeFactory);

            //
            // compute objective
            //
            var dblValue = optStatsCache.StatsMap.GetDblValue(
                ObjectiveName);

            //
            // set default values, in cases where
            // the param is not found
            //
            if (double.IsInfinity(dblValue) ||
                double.IsNaN(dblValue))
            {
                dblValue = 0;
            }

            var dblLogTime = (DateTime.Now - dateLog).TotalSeconds;

            Logger.Log("Finish evaluating individual. Time = " +
                       dblLogTime + " sec." + Environment.NewLine +
                       individual);

            InvokeOnObjectiveAfterEvaluatedEventHandler(
                individual,
                dblLogTime,
                optStatsCache);

            return FunctionMultiplier * dblValue;
        }

        #endregion

        #region Private

        private void InvokeOnObjectiveAfterEvaluatedEventHandler(
            Individual individual,
            double dblLogTime,
            OptStatsCache optStatsCache)
        {
            if (OnObjectiveAfterEvaluatedEvent != null &&
                OnObjectiveAfterEvaluatedEvent.GetInvocationList().Count() > 0)
            {
                OnObjectiveAfterEvaluatedEvent.Invoke(
                    individual,
                    this,
                    dblLogTime,
                    optStatsCache);
            }
        }

        public override string ToString()
        {
            return ObjectiveName + "_objective";
        }

        ~OptObjFunct()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_chromosomeFactory != null)
            {
                m_chromosomeFactory.Dispose();
            }
        }

        #endregion
    }
}
