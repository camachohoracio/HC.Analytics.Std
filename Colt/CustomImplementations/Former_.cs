#region

using System;

#endregion

namespace HC.Analytics.Colt.CustomImplementations
{
    [Serializable]
    public class Former_ : Former
    {
        private readonly string format_;

        public Former_(string format)
        {
            format_ = format;
        }

        //private corejava.PrintfFormat f = (format!=null ? new corejava.PrintfFormat(format) : null);

        #region Former Members

        public string form(double value)
        {
            //private FormatStringBuffer f = (format!=null ? new corejava.FormatStringBuffer(format) : null);
            Format f =
                (format_ != null ? new Format(format_) : null);
            if (f == null || value == Double.PositiveInfinity || value == Double.NegativeInfinity ||
                double.IsNaN(value))
            {
                // value != value <==> Double.IsNaN(value)
                // Work around bug in corejava.Format.form() for inf, -inf, NaN
                return value.ToString();
            }
            //return f.format(value).ToString();
            return f.format(value);
            //return f.sprintf(value);
        }

        #endregion
    }
}
