#region

using System;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.DblInt.ObjectiveFunctions
{
    public class LinearObjectiveFunctionDblInt : IHeuristicObjectiveFunction
    {
        #region Properties

        public string ObjectiveName { get; set; }

        public ObjectiveFunctionType ObjectiveFunctionType
        {
            get { return ObjectiveFunctionType.STD_OBJECTIVE_FUNCT; }
        }

        #endregion

        #region Members

        private double[] m_dblReturnArrayContinuous;
        private double[] m_dblReturnArrayInteger;
        private double[] m_dblScaleArrContinuous;
        private double[] m_dblScaleArrInteger;

        #endregion

        #region Constructors

        public LinearObjectiveFunctionDblInt(
            double[] dblReturnArrayInteger,
            double[] dblReturnArrayContinuous,
            double[] dblScaleArrInteger,
            double[] dblScaleArrContinuous)
        {
            m_dblReturnArrayInteger = dblReturnArrayInteger;
            m_dblReturnArrayContinuous = dblReturnArrayContinuous;
            m_dblScaleArrInteger = dblScaleArrInteger;
            m_dblScaleArrContinuous = dblScaleArrContinuous;
        }

        #endregion

        #region Public Methods

        public double Evaluate(Individual individual,
            HeuristicProblem heuristicProblem)
        {
            double dblTotal = 0;

            //
            // integer part
            //
            if (individual.ContainsChromosomeInt())
            {
                for (var i = 0; i < m_dblReturnArrayInteger.Length; i++)
                {
                    dblTotal += individual.GetChromosomeValueInt(i)*
                                m_dblReturnArrayInteger[i]*
                                m_dblScaleArrInteger[i];
                }
            }

            //
            // continuous part
            //
            if (individual.ContainsChromosomeDbl())
            {
                for (var i = 0; i < m_dblReturnArrayContinuous.Length; i++)
                {
                    dblTotal += individual.GetChromosomeValueDbl(i)*
                                m_dblReturnArrayContinuous[i]*
                                m_dblScaleArrContinuous[i];
                }
            }

            return dblTotal;
        }

        #endregion

        #region IHeuristicObjectiveFunction Members

        public double Evaluate()
        {
            throw new NotImplementedException();
        }

        public int VariableCount
        {
            get { return m_dblReturnArrayInteger.Length + m_dblReturnArrayContinuous.Length; }
        }

        public string GetVariableDescription(int intIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        ~LinearObjectiveFunctionDblInt()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_dblReturnArrayContinuous = null;
            m_dblReturnArrayInteger = null;
            m_dblScaleArrContinuous = null;
            m_dblScaleArrInteger = null;
        }
    }
}
