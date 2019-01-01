#region

using System;
using System.Collections.Generic;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Constraints
{
    public abstract class AbstractLinearConstraint : IHeuristicConstraint
    {
        #region Properties

        public double[] Coefficients
        {
            get { return m_dblCoefficients; }
        }

        public int[] Indexes
        {
            get { return m_intIndexes; }
        }

        public double Boundary { get; set; }

        /// <summary>
        ///   The type of inequality
        /// </summary>
        public InequalityType Inequality { get; set; }

        #endregion

        #region Members

        private double[] m_dblCoefficients;
        private double[] m_dblScaleArr;
        protected HeuristicProblem m_heuristicProblem;
        private int[] m_intIndexes;

        #endregion

        #region Constructors

        public AbstractLinearConstraint(
            double[] dblCoefficients,
            int[] intIndexes,
            InequalityType inequality,
            double dblBoundary)
        {
        }

        public AbstractLinearConstraint(
            double[] dblCoefficients,
            double[] dblScaleArr,
            int[] intIndexes,
            InequalityType inequality,
            double dblBoundary,
            HeuristicProblem heuristicProblem)
        {
            SetState(
                dblCoefficients,
                dblScaleArr,
                intIndexes,
                inequality,
                dblBoundary,
                heuristicProblem);
        }

        #endregion

        #region IHeuristicConstraint Members

        public List<VariableContribution> GetRankList()
        {
            throw new NotImplementedException();
        }

        public bool CheckConstraint(Individual individual)
        {
            var dblSum = EvaluateConstraint(individual);
            return MathHelper.CheckInequality(Inequality, dblSum, Boundary);
        }

        public double EvaluateConstraint(Individual individual)
        {
            if (individual.IndividualList != null &&
                individual.IndividualList.Count > 0)
            {
                individual = individual.GetIndividual(
                    m_heuristicProblem.ProblemName);
            }

            double dblSum = 0;
            for (var i = 0; i < m_dblCoefficients.Length; i++)
            {
                dblSum +=
                    GetChromosomeValue(individual, i)*m_dblCoefficients[i]*m_dblScaleArr[i];
            }

            return dblSum;
        }

        #endregion

        #region Private

        private void SetState(
            double[] dblCoefficients,
            double[] dblScaleArr,
            int[] intIndexes,
            InequalityType inequality,
            double dblBoundary,
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_dblCoefficients = dblCoefficients;
            m_intIndexes = intIndexes;
            m_dblScaleArr = dblScaleArr;
            Inequality = inequality;
            Boundary = dblBoundary;

            if (m_dblScaleArr == null)
            {
                m_dblScaleArr = new double[dblCoefficients.Length];
                for (var i = 0; i < dblCoefficients.Length; i++)
                {
                    m_dblScaleArr[i] = 1.0;
                }
            }
        }

        #endregion

        #region AbstractMethods

        protected abstract double GetChromosomeValue(
            Individual individual,
            int intIndex);

        protected abstract void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract double[] GetChromosomeCopy(
            Individual individual);

        protected abstract double GetMaxChromosomeValue(int intIndex);

        #endregion

        public virtual void Dispose()
        {
            m_dblCoefficients = null;
            m_dblScaleArr = null;
            m_intIndexes = null;
            m_heuristicProblem = null;
        }
    }
}
