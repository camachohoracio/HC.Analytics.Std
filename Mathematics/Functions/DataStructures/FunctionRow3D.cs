#region

using System;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public class FunctionRow3D : IComparable<FunctionRow3D>
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool IsNewRow { get; set; }

        #endregion

        #region Constructors

        public FunctionRow3D(
            double dblX,
            double dblY,
            double dblZ)
        {
            X = dblX;
            Y = dblY;
            Z = dblZ;
        }

        #endregion

        #region IComparable<FunctionRow3D> Members

        /// <summary>
        /// Sort by X and then by Y values
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(FunctionRow3D o)
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

            difference = Y - o.Y;
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

        public FunctionRow3D Copy()
        {
            return new FunctionRow3D(
                X, Y, Z);
        }
    }
}
