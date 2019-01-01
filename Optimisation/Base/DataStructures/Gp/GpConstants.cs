#region

using System;
using System.Text;
using System.Xml.Serialization;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpConstants
    {

        #region Constants

        public const double FULL_TREE_PROB = 0.5;
        public const double OPERATOR_PROB = 0.5;
        public const double RANDOM_INDIVIDUAL = 0.05;

        #endregion

        #region Properties

        [XmlElement("GpConstantName", typeof (string))]
        public string GpConstantName { get; set; }

        [XmlElement("Value", typeof (object))]
        public object Value { get; set; }

        #endregion

        #region Constructors

        public GpConstants()
        {
        }

        public GpConstants(object value, string name)
        {
            Value = value;
            GpConstantName = name;
        }

        #endregion

        #region Public

        public void SetValue(object value)
        {
            Value = value;
        }

        public virtual object GetValue()
        {
            return Value;
        }

        public void ToStringB(StringBuilder sb)
        {
            sb.Append(GpConstantName);
        }

        public override string ToString()
        {
            return GpConstantName;
        }

        #endregion

        //public const double EXPIRY_TRADE_DAYS_THRESHOLD = 20;
        public const double MIN_EXPIRY_TRADE_DAYS_THRESHOLD = 3;
        public const double MAX_EXPIRY_TRADE_DAYS_THRESHOLD = 30;
    }
}
