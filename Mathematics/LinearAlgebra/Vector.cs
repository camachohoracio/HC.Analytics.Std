#region

using System;

#endregion

namespace HC.Analytics.Mathematics.LinearAlgebra
{
    [Serializable]
    public class Vector : MatrixClass
    {
        #region Members

        protected readonly int m_intDimensions;

        #endregion

        #region Constructors

        public Vector(double[] dblArr)
            : base(dblArr)
        {
            m_intDimensions = dblArr.Length;
        }

        public Vector(int intDimensions)
            : base(intDimensions, 1)
        {
            m_intDimensions = intDimensions;
        }

        #endregion

        public int GetDimension()
        {
            return base.GetRowDimension();
        }

        public double Get(int intI)
        {
            return base.Get(intI, 0);
        }

        public new Vector Times(MatrixClass B)
        {
            if (B.GetRowDimension() != m_intDimensions)
            {
                throw new ArgumentException("Matrix inner dimensions must agree.");
            }
            double[] dblAArr = GetArr();
            double[,] dblBArr = B.GetArr();
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = 0;
                for (int j = 0; j <= i; j++)
                {
                    dblCArr[i] += dblBArr[i, j]*dblAArr[j];
                }
            }
            return new Vector(dblCArr);
        }

        public new Vector Times(double dblK)
        {
            double[] dblAArr = GetArr();
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = 0;
                for (int j = 0; j <= i; j++)
                {
                    dblCArr[i] += dblK*dblAArr[j];
                }
            }
            return new Vector(dblCArr);
        }

        public new Vector Plus(double dblK)
        {
            double[] dblAArr = GetArr();
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = dblAArr[i] + dblK;
            }
            return new Vector(dblCArr);
        }


        public Vector Plus(Vector B)
        {
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = Get(i) + B.Get(i);
            }
            return new Vector(dblCArr);
        }

        public Vector Minus(Vector B)
        {
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = Get(i) - B.Get(i);
            }
            return new Vector(dblCArr);
        }

        public new double[] GetArr()
        {
            double[] dblCArr = new double[m_intDimensions];
            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i] = m_dblArr[i, 0];
            }
            return dblCArr;
        }

        public void SetArr(double[] dblArr)
        {
            double[,] dblCArr = new double[
                m_intDimensions,
                1];

            for (int i = 0; i < m_intDimensions; i++)
            {
                dblCArr[i, 0] = dblArr[i];
            }

            m_dblArr = dblCArr;
            m_intRows = dblArr.Length;
            m_intColumns = 1;
        }
    }
}
