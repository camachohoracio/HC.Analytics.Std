#region

using System;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    /// <summary>
    /// Sets a X vector and an F(x) pair of values
    /// </summary>
    [Serializable]
    public class FunctionRow
    {
        #region Properties

        public double Fx { get; set; }
        public double[] XArray { get; set; }

        #endregion

        public FunctionRow(double dblFx, double[] dblXArray)
        {
            Fx = dblFx;
            XArray = dblXArray;
        }
    }
}
