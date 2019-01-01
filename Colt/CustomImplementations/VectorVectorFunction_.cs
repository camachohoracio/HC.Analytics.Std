#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class VectorVectorFunction_ : VectorVectorFunction
    {
        private Functions m_F;

        public VectorVectorFunction_(Functions F)
        {
            m_F = F;
        }

        #region VectorVectorFunction Members

        public double Apply(DoubleMatrix1D a, DoubleMatrix1D b)
        {
            return Math.Sqrt(a.aggregate(b, Functions.m_plus, Functions.chain(Functions.m_square, Functions.m_minus)));
        }

        #endregion
    }
}
