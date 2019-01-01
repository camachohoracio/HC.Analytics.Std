#region

using System;
using System.Xml.Serialization;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public abstract class AbstractGpVariable
    {
        #region Properties

        [XmlElement("Value", typeof (object))]
        public object Value { get; set; }

        [XmlElement("VariableName", typeof (string))]
        public string VariableName { get; set; }

        #endregion

        #region Constructors

        public AbstractGpVariable() { }

        public AbstractGpVariable(string name)
        {
            VariableName = name;
        }

        #endregion

        public void SetValue(object value)
        {
            Value = value;
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return VariableName;
        }


        public abstract AbstractGpVariable Clone();
    }
}
