#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class VectorVectorFunction1_ : VectorVectorFunction
    {
        private Functions m_F;

        public VectorVectorFunction1_(Functions F)
        {
            m_F = F;
        }

        #region VectorVectorFunction Members

        public double Apply(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            return a.aggregate(b, Functions.m_plus, Functions.chain(Functions.m_absm, Functions.m_minus))/
                   a.aggregate(b, Functions.m_plus, Functions.m_plus);
        }

        #endregion
    }
}
