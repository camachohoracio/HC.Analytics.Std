#region

using System;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public class TsRow2D : ATsEvent, IComparable<TsRow2D>
    {
        #region Properties

        public double Fx { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///   Used for serialization
        /// </summary>
        public TsRow2D()
        {
        }

        public TsRow2D(DateTime dblX, double dblFx)
        {
            if (double.IsNaN(dblFx))
            {
                //Debugger.Break();
            }
            base.Time = dblX;
            Fx = dblFx;
        }

        #endregion

        #region IComparable<TsRow2D> Members

        /// <summary>
        ///   Sort by X values
        /// </summary>
        /// <param name = "o"></param>
        /// <returns></returns>
        public int CompareTo(TsRow2D o)
        {
            var difference = (base.Time - o.Time).TotalMilliseconds;
            if (difference < 0)
            {
                return -1;
            }
            if (difference > 0)
            {
                return 1;
            }

            difference = (Fx - o.Fx);
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

        #region Public Methods

        public TsRow2D Clone()
        {
            return new TsRow2D(
                Time,
                Fx);
        }

        public override string ToString()
        {
            return " Fx = " + Fx + ", x = " + Time;
        }

        #endregion
    }
}
