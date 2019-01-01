#region

using System;
using System.Xml.Serialization;
using System.Text;
using HC.Core.DynamicCompilation;
using HC.Core.Io.Serialization.Interfaces;
using HC.Core.Logging;
using HC.Core.Reflection;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public abstract class ATsEvent : ASerializable, ITsEvent
    {
        #region Properties

        public virtual DateTime Time { get; set; }

        [XmlIgnore]
        public TsDataRequest TsDataRequest { get; set; }

        #endregion

        #region Members


        [NonSerialized] private IReflector m_reflector;

        #endregion

        #region Public

        public ATsEvent ParseCsvString(
            string strLine)
        {
            LoadBinder();
            object item = m_reflector.CreateInstance();
            TsEventHelper.ParseCsvString(
                strLine,
                item,
                m_reflector.GetPropertyNames());
            return (ATsEvent)item;
        }

        public virtual string ToCsvString()
        {
            var strCsvString =
                TsEventHelper.ToCsvString(
                    this,
                    GetType());
            return strCsvString;
        }

        public object GetHardPropertyValue(string strFieldName)
        {
            LoadBinder();
            return m_reflector.GetPropertyValue(
                this,
                strFieldName);
        }

        private void LoadBinder()
        {
            if (m_reflector == null)
            {
                m_reflector = ReflectorCache.GetReflector(GetType());
            }
        }

        public override string ToString()
        {
            try
            {
                LoadBinder();
                var sb = new StringBuilder();
                bool blnIsTitle = true;
                foreach (string strPropertyName in m_reflector.GetPropertyNames())
                {
                    if (!blnIsTitle)
                    {
                        sb.Append(",\n");
                    }
                    else
                    {
                        blnIsTitle = false;
                    }
                    sb.Append(strPropertyName + " = " +
                              m_reflector.GetPropertyValue(this, strPropertyName));
                }
                return sb.ToString();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return string.Empty;
        }

        public virtual void Dispose()
        {
            if (TsDataRequest != null)
            {
                TsDataRequest.Dispose();
                TsDataRequest = null;
            }
            m_reflector = null;
        }

        #endregion
    }
}
