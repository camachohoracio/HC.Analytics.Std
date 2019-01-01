#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace HC.Analytics.TimeSeries.TsStats
{
    public class SmaDiffHc
    {
        #region Delegates

        public delegate void CrossingDel(object context, SmaDiffHc smaDiffHc);

        #endregion

        public event CrossingDel OnCrossing;

        #region Properties

        public double CurrDiff { get; set; }
        public SMAvgFunction SMAvgFunction1 { get; set; }
        public SMAvgFunction SMAvgFunction2 { get; set; }

        public bool IsPopulated
        {
            get { return SMAvgFunction1.IsInitialized() && SMAvgFunction2.IsInitialized(); }
        }

        public bool IsGoingDown
        {
            get
            {
                SMAvgFunction sMAvgFunctionSlow;
                SMAvgFunction sMAvgFunctionFast;
                if (m_intTimeWindow1 > m_intTimeWindow2)
                {
                    sMAvgFunctionSlow = SMAvgFunction1;
                    sMAvgFunctionFast = SMAvgFunction2;
                }
                else
                {
                    sMAvgFunctionSlow = SMAvgFunction2;
                    sMAvgFunctionFast = SMAvgFunction1;
                }
                return sMAvgFunctionSlow.FunctionData.Last().Fx >
                       sMAvgFunctionFast.FunctionData.Last().Fx;
            }
        }
        
        public object Context { get; private set; }

        #endregion

        #region Members

        private readonly int m_intTimeWindow1;
        private readonly int m_intTimeWindow2;
        private double m_dblDiff;

        #endregion

        #region Constructors

        public SmaDiffHc(
            int intTimeWindow1,
            int intTimeWindow2)
            : this(intTimeWindow1, intTimeWindow2, null) { }

        public SmaDiffHc(
            int intTimeWindow1,
            int intTimeWindow2,
            object context)
        {
            //if (intTimeWindow1 == intTimeWindow2)
            //{
            //    throw new Exception("Invalid time window");
            //}

            m_intTimeWindow1 = intTimeWindow1;
            m_intTimeWindow2 = intTimeWindow2;
            SMAvgFunction1 = new SMAvgFunction(
                "Function1",
                new List<TsRow2D>(),
                intTimeWindow1);
            SMAvgFunction2 = new SMAvgFunction(
                "Function2",
                new List<TsRow2D>(),
                intTimeWindow2);
            m_dblDiff = double.NaN;
            Context = context;
        }

        #endregion

        #region Public

        public void Update(
            DateTime dateTime,
            double dblValue)
        {
            if (double.IsNaN(dblValue) ||
                double.IsInfinity(dblValue))
            {
                throw new Exception("Invalid value");
            }

            var tsRow = new TsRow2D(dateTime, dblValue);
            SMAvgFunction1.Update(tsRow);
            SMAvgFunction2.Update(tsRow);

            if (!SMAvgFunction1.IsInitialized() &&
                !SMAvgFunction2.IsInitialized())
            {
                return;
            }

            //
            // check cross
            //
            if (double.IsNaN(m_dblDiff))
            {
                m_dblDiff = SMAvgFunction1.FunctionData.Last().Fx -
                            SMAvgFunction2.FunctionData.Last().Fx;
            }

            CurrDiff = SMAvgFunction1.FunctionData.Last().Fx -
                       SMAvgFunction2.FunctionData.Last().Fx;

            if (double.IsNaN(m_dblDiff) ||
                double.IsNaN(CurrDiff))
            {
                throw new Exception("Invalid diff value");
            }

            if (Math.Sign(CurrDiff) != Math.Sign(m_dblDiff))
            {
                //
                // the two functions have crossed
                //
                m_dblDiff = CurrDiff;
                CrossingDel h = OnCrossing;
                if (h != null)
                {
                    h(Context, this);
                }
            }
        }

        #endregion
    }
}
