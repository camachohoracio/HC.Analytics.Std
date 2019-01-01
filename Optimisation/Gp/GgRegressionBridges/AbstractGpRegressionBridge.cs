#region

using System;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Gp.GgRegressionBridges
{
    public abstract class AbstractGpRegressionBridge : AbstractGpBridge
    {
        #region Members

        private double m_dblMaxX;
        private double m_dblMaxY;
        private double m_dblMinX;
        private double m_dblMinY;
        private double[] m_dblXArr;
        private double[] m_dblY2Arr;
        private double[] m_dblYArr;
        //private readonly EfficientMemoryBuffer<string, double> m_solutions;

        #endregion

        #region Properties

        public double DblMinX
        {
            get { return m_dblMinX; }
            set { m_dblMinX = value; }
        }

        public double DblMaxX
        {
            get { return m_dblMaxX; }
            set { m_dblMaxX = value; }
        }

        public double DblMinY
        {
            get { return m_dblMinY; }
            set { m_dblMinY = value; }
        }

        public double DblMaxY
        {
            get { return m_dblMaxY; }
            set { m_dblMaxY = value; }
        }

        public double[] DblXArr
        {
            get { return m_dblXArr; }
            set { m_dblXArr = value; }
        }

        public double[] DblYArr
        {
            get { return m_dblYArr; }
            set { m_dblYArr = value; }
        }

        public double[] DblY2Arr
        {
            get { return m_dblY2Arr; }
            set { m_dblY2Arr = value; }
        }

        #endregion

        #region Constructors

        protected AbstractGpRegressionBridge(
            GpOperatorsContainer gpOperatorsContainer)
            : base(gpOperatorsContainer)
        {
            //m_solutions =
            //    new EfficientMemoryBuffer<string, double>(50000);
        }

        #endregion

        protected void CreateTics()
        {
            var ticLength = (m_dblMaxX - m_dblMinX)/m_intNumbTestCases;
            m_dblXArr = new double[m_intNumbTestCases];
            m_dblYArr = new double[m_intNumbTestCases];
            m_dblXArr[0] = m_dblMinX;
            m_dblYArr[0] = EvaluateFunction(m_dblXArr[0]);
            m_dblMinY = m_dblYArr[0];
            m_dblMaxY = m_dblYArr[0];
            for (var i = 1; i < m_intNumbTestCases; i++)
            {
                m_dblXArr[i] = m_dblMinX + ticLength*i;
                m_dblYArr[i] = EvaluateFunction(m_dblXArr[i]);
                if (m_dblYArr[i] < m_dblMinY)
                {
                    m_dblMinY = m_dblYArr[i];
                }
                if (m_dblYArr[i] > m_dblMaxY)
                {
                    m_dblMaxY = m_dblYArr[i];
                }
            }
        }

        protected abstract double EvaluateFunction(double x);

        //protected double CalculateError(double[] Yind)
        //{
        //    double error = 0;
        //    for (int i = 0; i < m_intNumbTestCases; i++)
        //    {
        //        error = error + Math.Abs(Yind[i] - m_dblYArr[i]);
        //    }
        //    return error;
        //}

        public override double GetRegressionFit(Individual gpIndividual,
            HeuristicProblem heuristicProblem)
        {
            string strName = gpIndividual.ToString();
            double dblCurrFitness;
            //if (!m_solutions.TryGetValue(
            //    strName,
            //    out dblCurrFitness))
            //{
                dblCurrFitness = GetRegressionFit1(gpIndividual);
                //m_solutions.AddItem(strName, dblCurrFitness);
                return dblCurrFitness;
            //}
            return dblCurrFitness;
        }

        private double GetRegressionFit1(Individual gpIndividual)
        {
            m_dblY2Arr = new double[m_intNumbTestCases];
            var dblY2Arr = new double[m_intNumbTestCases];

            // evaluate each of the points in the individuals
            for (var i = 0; i < m_intNumbTestCases; i++)
            {
                //AbstractGpVariable gpVariable = 
                //    GetParameterValue(m_dblXArr[i]);
                var gpVariable =
                    GetVariable((double) i);
                dblY2Arr[i] = gpIndividual.EvaluateTree(gpVariable);
            }
            m_dblY2Arr = dblY2Arr;

            /*Added by me for experimentation 31st jan 2010 */
            // get the maximum error in the curve
            var error = Math.Abs(dblY2Arr[0] - DblYArr[0]); //= CalculateError(m_dblY2Arr);
            for (var i = 0; i < dblY2Arr.Length; i++)
            {
                var error2 = Math.Abs(dblY2Arr[i] - DblYArr[i]);
                if (Double.IsNaN(error2))
                {
                    return -10000000;
                }
                error = Math.Max(error2, error);
            }

            if (Double.IsNaN(error))
            {
                return -10000000;
            }
            return -error;
        }
    }
}
