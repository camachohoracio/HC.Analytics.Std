#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Matrix2DMatrix2DFunction3_ : Matrix2DMatrix2DFunction
    {
        #region Matrix2DMatrix2DFunction Members

        public double Apply(DoubleMatrix2D AA, DoubleMatrix2D BB)
        {
            return AA.zSum();
        }

        #endregion
    }
}
