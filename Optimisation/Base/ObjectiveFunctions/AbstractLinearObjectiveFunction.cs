#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public abstract class AbstractLinearObjectiveFunction : IHeuristicObjectiveFunction
    {
        #region Properties

        public double[] ReturnArray
        {
            get { return m_dblReturnArray; }
        }

        public int[] Indexes
        {
            get { return m_intIndexes; }
        }

        public string ObjectiveName { get; set; }
        
        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { return ObjectiveFunctionType.STD_OBJECTIVE_FUNCT; }
        }

        #endregion

        #region Members

        private double[] m_dblReturnArray;
        private double[] m_dblScaleArr;
        private int[] m_intIndexes;

        #endregion

        #region Constructors

        public AbstractLinearObjectiveFunction(
            double[] dblReturnArray,
            double[] dblScaleArr,
            HeuristicProblem heuristicProblem) :
                this(dblReturnArray,
                     dblScaleArr,
                     null,
                     heuristicProblem)
        {
        }

        public AbstractLinearObjectiveFunction(
            double[] dblReturnArray,
            double[] dblScaleArr,
            int[] intIndexes,
            HeuristicProblem heuristicProblem)
        {
            SetState(
                dblReturnArray,
                dblScaleArr,
                intIndexes,
                heuristicProblem);
        }

        #endregion

        #region IHeuristicObjectiveFunction Members

        public double Evaluate()
        {
            throw new NotImplementedException();
        }

        public int VariableCount
        {
            get { return m_dblReturnArray.Length; }
            set { throw new NotImplementedException(); }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        public virtual double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            double dblTotal = 0;
            for (var i = 0; i < VariableCount; i++)
            {
                dblTotal += GetChromosomeValue(individual, i)*
                            m_dblReturnArray[i]*
                            m_dblScaleArr[i];
            }
            return dblTotal;
        }

        #endregion

        private void SetState(
            double[] dblReturnArray,
            double[] dblScaleArr,
            int[] intIndexes,
            HeuristicProblem heuristicProblem)
        {
            m_dblReturnArray = dblReturnArray;
            m_intIndexes = intIndexes;
            m_dblScaleArr = dblScaleArr;
            if (m_dblScaleArr == null)
            {
                m_dblScaleArr = new double[VariableCount];
                for (var i = 0; i < VariableCount; i++)
                {
                    m_dblScaleArr[i] = 1.0;
                }
            }
        }

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

        public void Dispose()
        {
            m_dblReturnArray = null;
            m_dblScaleArr = null;
            m_intIndexes= null;
        }

        ~AbstractLinearObjectiveFunction()
        {
            Dispose();
        }

    }
}
