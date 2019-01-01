using System;

namespace HC.Analytics.TimeSeries.Technicals
{
    [Serializable]
    public abstract class ATechIndFunction : TsFunction
    {
        #region Constructor

        public ATechIndFunction(
            string strFunctionName,
            string strXLabel,
            string strYLabel) :
                base(strFunctionName,
                     strXLabel,
                     strYLabel,
                     null)
        {
        }

        public ATechIndFunction()
        {
        }

        #endregion

        #region Abstract

        public abstract void Update(TsRow2D row2D);
        public abstract void Reset();

        #endregion
    }
}
