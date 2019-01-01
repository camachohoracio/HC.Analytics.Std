#region

using System;
using HC.Analytics.Colt.CustomImplementations.tmp;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class VectorVectorFunctionCanberra : VectorVectorFunction
    {
        private readonly DoubleDoubleFunction fun = new DoubleDoubleFunctionCanberra();

        private Functions m_F;

        public VectorVectorFunctionCanberra(Functions F)
        {
            m_F = F;
        }

        #region VectorVectorFunction Members

        public double Apply(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            return a.aggregate(b, Functions.m_plus, fun);
        }

        #endregion
    }
}
