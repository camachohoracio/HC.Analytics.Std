#region

using System;

#endregion

namespace HC.Analytics.Colt
{
    /*
    Copyright � 1999 CERN - European Organization for Nuclear Research.
    Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
    is hereby granted without fee, provided that the above copyright notice appear in all copies and 
    that both that copyright notice and this permission notice appear in supporting documentation. 
    CERN makes no representations about the suitability of this software for any purpose. 
    It is provided "as is" without expressed or implied warranty.
    */
    //package Appendbench;

    ////import DoubleMatrix2D;

    [Serializable]
    public abstract class Double2DProcedure : TimerProcedure
    {
        public DoubleMatrix2D m_A;
        public DoubleMatrix2D m_B;
        public DoubleMatrix2D m_C;
        public DoubleMatrix2D m_D;
        /**
         * The number of operations a single call to "apply" involves.
         */

        #region TimerProcedure Members

        public abstract void Apply(Timer element);
        public abstract void init();

        #endregion

        public double operations()
        {
            return m_A.Rows()*m_A.Columns()/1.0E6;
        }

        /**
         * Sets the matrices to operate upon.
         */

        public void setParameters(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            m_A = A;
            m_B = B;
            m_C = A.Copy();
            m_D = B.Copy();
        }
    }
}
