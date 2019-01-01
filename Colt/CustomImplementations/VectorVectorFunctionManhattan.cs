#region

using System;
using HC.Analytics.Colt.CustomImplementations.tmp;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class VectorVectorFunctionManhattan : VectorVectorFunction
    {
        private DoubleDoubleFunction fun = new DoubleDoubleFunctionCanberra();

        private Functions m_F;

        public VectorVectorFunctionManhattan(Functions F)
        {
            m_F = F;
        }

        #region VectorVectorFunction Members

        public double Apply(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            return a.aggregate(b, Functions.m_plus,
                               Functions.chain(Functions.m_absm, Functions.m_minus));
        }

        #endregion
    }
}
