#region

using System;
using HC.Core.Io.Serialization.Interfaces;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public class FuncRow2D : ASerializable, IComparable<FuncRow2D>
    {
        #region Properties

        public double Fx { get; set; }
        public double X { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Leave empty for serilization tasks
        /// </summary>
        public FuncRow2D()
        {
        }

        public FuncRow2D(double dblX, double dblFx)
        {
            X = dblX;
            Fx = dblFx;
        }

        #endregion

        #region IComparable<FuncRow2D> Members

        /// <summary>
        /// Sort by X values
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(FuncRow2D o)
        {
            double difference = X - o.X;
            if (difference < 0)
            {
                return -1;
            }
            if (difference > 0)
            {
                return 1;
            }
            return 0;
        }

        #endregion
    }
}
