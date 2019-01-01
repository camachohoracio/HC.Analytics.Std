#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public class OptConstraint : IHeuristicConstraint
    {
        #region Members

        private readonly OptChromosomeFactory m_chromosomeFactory;
        private readonly IOptInstFctry m_optInstFctry;
        private readonly string m_strParameterName1;
        private readonly string m_strParameterName2;

        #endregion

        #region Properties

        public HeuristicProblem HeuristicProblem { get; set; }
        public InequalityType Inequality { get; set; }
        public double Boundary { get; set; }
        public string ParameterName1 { get; private set; }
        public string ParameterName2 { get; private set; }

        #endregion

        #region Constructors

        public OptConstraint(
            IOptInstFctry optInstFctry,
            OptChromosomeFactory optChromosomeFactory,
            string strParameterName1,
            string strParaenterName2,
            InequalityType inequalityType,
            double dblBoundary)
        {
            ParameterName1 = strParameterName1;
            ParameterName2 = strParaenterName2;
            m_chromosomeFactory = optChromosomeFactory;
            m_optInstFctry = optInstFctry;
            m_strParameterName1 = strParameterName1;
            m_strParameterName2 = strParaenterName2;
            Inequality = inequalityType;
            Boundary = dblBoundary;
        }

        #endregion

        #region Public

        public List<VariableContribution> GetRankList()
        {
            throw new NotImplementedException();
        }

        public bool CheckConstraint(
            Individual individual)
        {
            var dblValue1 = GetConstraintValue(
                individual,
                m_strParameterName1);
            var dblValue2 = Boundary;

            if (!string.IsNullOrEmpty(m_strParameterName2))
            {
                dblValue2 = GetConstraintValue(
                    individual,
                    m_strParameterName2);
            }

            var blnIneq = MathHelper.CheckInequality(
                Inequality,
                dblValue1,
                dblValue2);

            return blnIneq;
        }

        public double EvaluateConstraint(
            Individual individual)
        {
            var dblValue = GetConstraintValue(
                individual,
                m_strParameterName1);
            return dblValue;
        }

        private double GetConstraintValue(
            Individual individual,
            string strParameterName)
        {
            var optStatsCache =
                OptChromosomeFactory.GetOptStatsCache(
                    individual,
                    m_optInstFctry,
                    m_chromosomeFactory);

            //
            // compute the constraint value
            //
            var dblValue = optStatsCache.StatsMap.GetDblValue(
                strParameterName);

            if (double.IsInfinity(dblValue) ||
                double.IsNaN(dblValue))
            {
                dblValue = 0;
            }
            return dblValue;
        }

        public override string ToString()
        {
            return m_strParameterName1 +
                   (string.IsNullOrEmpty(m_strParameterName2)
                        ? string.Empty
                        : ("_" + m_strParameterName2)) + "_constraint";
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}
