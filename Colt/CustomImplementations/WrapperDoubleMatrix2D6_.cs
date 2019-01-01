#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class WrapperDoubleMatrix2D6_ : WrapperDoubleMatrix2D
    {
        private readonly int[] m_cix;
        private readonly int[] m_rix;
        private WrapperDoubleMatrix2D m_wrapperDoubleMatrix2D;

        public WrapperDoubleMatrix2D6_(
            WrapperDoubleMatrix2D wrapperDoubleMatrix2D,
            int[] rix,
            int[] cix) :
                base(wrapperDoubleMatrix2D)
        {
            m_rix = rix;
            m_cix = cix;
            m_wrapperDoubleMatrix2D = wrapperDoubleMatrix2D;
        }

        public override double getQuick(int i, int j)
        {
            return content.get(m_rix[i], m_cix[j]);
        }

        public override void setQuick(int i, int j, double value)
        {
            content.set(m_rix[i], m_cix[j], value);
        }
    }
}
