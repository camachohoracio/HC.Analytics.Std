#region

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using HC.Analytics.TimeSeries.Technicals;
using HC.Core.Comunication.TopicBased.Contracts;
using HC.Core.DataStructures;
using HC.Core.DynamicCompilation;
using HC.Core.Io.Serialization.Interfaces;
using HC.Core.Resources;
using HC.Core.Time;
using NUnit.Framework;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.TimeSeries
{
    [Serializable]
    public class TsDataRequest : ASerializable, IDataRequest
    {
        #region Members

        private bool m_blnDoConsolidate;
        private bool m_blnIsContinuous;
        private bool m_blnUsePool;
        private bool m_blnUseService;
        private DateTime m_endTime;
        private string m_dataProviderType;
        private EnumTechIndType m_tsDataType;
        private IDataRequest m_innerFunctionDataRequest;
        private long m_lngPeriodSizeOffsetTicks;
        private long m_lngPeriodSizeTicks;
        private DateTime m_startTime;
        private string m_strColumn;
        private string m_strCountry;
        private string m_strCurrency;
        private string[] m_strCurrencyArr;
        private string m_strQuery;
        private string m_strRequestDescr;
        private string[] m_strSymbolArr;
        private string m_strSymbols;
        private SerializableDictionary<string, object> m_customParams;

        [NonSerialized] private Dictionary<string, PublishTsDelegate> m_tsPublishList;
        private int m_intPeriod;

        #endregion

        #region Properties

        public int Period
        {
            get { return m_intPeriod; }
            set
            {
                m_intPeriod = value;
                Reset();
            }
        }


        public TopicMessage TopicMessage { get; set; }

        public double PlaySpeed { get; set; }

        public string Symbols
        {
            get { return m_strSymbols; }
            set
            {
                m_strSymbols = value;
                Reset();
            }
        }

        
        public bool UseService
        {
            get { return m_blnUseService; }
            set
            {
                m_blnUseService = value;
                Reset();
            }
        }

        public string Country
        {
            get { return m_strCountry; }
            set
            {
                m_strCountry = value;
                Reset();
            }
        }

        public string Currency
        {
            get { return m_strCurrency; }
            set
            {
                m_strCurrency = value;
                Reset();
            }
        }

        public DateTime StartTime
        {
            get { return m_startTime; }
            set
            {
                m_startTime = value;
                Reset();
            }
        }

        public DateTime EndTime
        {
            get { return m_endTime; }
            set
            {
                m_endTime = value;
                Reset();
            }
        }

        public long PeriodSizeTicks
        {
            get { return m_lngPeriodSizeTicks; }
            set
            {
                m_lngPeriodSizeTicks = value;
                Reset();
            }
        }

        public long PeriodSizeOffsetTicks
        {
            get { return m_lngPeriodSizeOffsetTicks; }
            set
            {
                m_lngPeriodSizeOffsetTicks = value;
                Reset();
            }
        }

        public string DataProviderType
        {
            get { return m_dataProviderType; }
            set
            {
                m_dataProviderType = value;
                Reset();
            }
        }

        public bool DoConsolidate
        {
            get { return m_blnDoConsolidate; }
            set
            {
                m_blnDoConsolidate = value;
                Reset();
            }
        }

        public bool IsContinuous
        {
            get { return m_blnIsContinuous; }
            set
            {
                m_blnIsContinuous = value;
                Reset();
            }
        }

        public string Column
        {
            get { return m_strColumn; }
            set
            {
                m_strColumn = value;
                Reset();
            }
        }

        public string Query
        {
            get { return m_strQuery; }
            set
            {
                m_strQuery = value;
                Reset();
            }
        }

        public bool UsePool
        {
            get { return m_blnUsePool; }
            set
            {
                m_blnUsePool = value;
                Reset();
            }
        }

        public SerializableDictionary<string, object> CustomParams
        {
            get { return m_customParams; }
            set
            {
                m_customParams = value;
                Reset();
            }
        }

        public EnumTechIndType TsDataType
        {
            get { return m_tsDataType; }
            set
            {
                m_tsDataType = value;
                Reset();
            }
        }


        [XmlIgnore]
        public IDataRequest InnerFunctionDataRequest
        {
            get { return m_innerFunctionDataRequest; }
            set
            {
                if(value.Equals(this))
                {
                    throw new HCException("Inner request is the same as the parent!");
                }
                m_innerFunctionDataRequest = value;
                Reset();
            }
        }


        [XmlIgnore]
        public string[] SymbolArr
        {
            get
            {
                if (m_strSymbolArr == null)
                {
                    m_strSymbolArr = TsDataHelper.ParseSymboltring(
                        Symbols);
                }
                return m_strSymbolArr;
            }
            set
            {
                Symbols = TsDataHelper.GetStringFromSymbols(value);
                GetResourceName2();
            }
        }

        [XmlIgnore]
        public string[] CurrencyArr
        {
            get
            {
                if (m_strCurrencyArr == null)
                {
                    m_strCurrencyArr = TsDataHelper.ParseSymboltring(
                        Currency);
                }
                return m_strCurrencyArr;
            }
            set
            {
                Currency = TsDataHelper.GetStringFromSymbols(value);
                GetResourceName2();
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_strRequestDescr))
                {
                    GetResourceName2();
                }
                return m_strRequestDescr;
            }
        }

        #endregion

        #region Private

        [Test]
        public static void DoTest()
        {
            var customParams = new SerializableDictionary<string, object>();
            customParams["a"] = "b";
            var tsDataRequest1 = new TsDataRequest 
            {
                Column = "TestCol",
                Country = "TestCountry",
                Currency = "TestCurrency",
                CustomParams = customParams,
                DataProviderType = "TestDataProviderType",
                DoConsolidate = true,
                EndTime = DateTime.Now,
                IsContinuous = true,
                Period = 1,
                PeriodSizeOffsetTicks = 1,
                PeriodSizeTicks = 1,
                PlaySpeed = 1,
                Query = "TestQuery",
                StartTime = DateTime.Now,
                Symbols = "symbol1,symbol2",
                TsDataType = EnumTechIndType.KalmanDiff,
                UsePool = true,
                UseService = true
            };
            string strName1 = tsDataRequest1.GetResourceName2();
            var tsDataRequest2 = ParseRequest(strName1);
            string strName2 = tsDataRequest2.GetResourceName2();
            if (!strName1.Equals(strName2))
            {
                throw new HCException("Invalid parse");
            }
        }

        public string GetResourceName2()
        {
            var sb =
                new StringBuilder();
            sb.Append("%s%")
              .Append(
                    DateHelper.ToDateTimeString(StartTime))
              .Append("%e%")
              .Append(
                    DateHelper.ToDateTimeString(EndTime));

            if (PeriodSizeTicks > 0)
            {
                sb.Append("%pt%")
                  .Append(PeriodSizeTicks);
            }
            if (PeriodSizeOffsetTicks > 0)
            {
                sb.Append("%pot%")
                  .Append(PeriodSizeOffsetTicks);
            }
            
            sb.Append("%dt%")
              .Append(
                    DataProviderType == null ? string.Empty : DataProviderType)
              .Append("%c%")
              .Append(DoConsolidate);

            if (!string.IsNullOrEmpty(Symbols))
            {
                sb.Append("%si%")
                  .Append(
                    Symbols.Replace(@"/", "_"));
            }
            if (!string.IsNullOrEmpty(Column))
            {
                sb.Append("%col%")
                  .Append(Column);
            }
            if (!string.IsNullOrEmpty(Country))
            {
                sb.Append("%co%")
                  .Append(Country);
            }
            if (!string.IsNullOrEmpty(Currency))
            {
                sb.Append("%cu%")
                  .Append(Currency);
            }

            if (IsContinuous)
            {
                sb.Append("%ic%")
                  .Append("1");
            }
            if (TsDataType != EnumTechIndType.None)
            {
                sb.Append("%td%")
                    .Append(TsDataType);
            }

            if (InnerFunctionDataRequest != null)
            {
                sb.Append("%ir%")
                  .Append(InnerFunctionDataRequest.Name);
            }

            if (!string.IsNullOrEmpty(Query))
            {
                sb.Append("%q%")
                  .Append(Query);
            }

            if (Period != 0)
            {
                sb.Append("%ap%")
                  .Append(Period);
            }

            if (m_customParams != null &&
                m_customParams.Count > 0)
            {
                sb.Append("%cp%");
                foreach (KeyValuePair<string, object> keyValuePair in m_customParams)
                {
                    sb.Append(keyValuePair.Key + "=" +
                              keyValuePair.Value);
                }
            }
            m_strRequestDescr = sb.ToString();

            return m_strRequestDescr;
        }

        public static TsDataRequest ParseRequest(
            string strReq)
        {
            string[] toks = strReq.Split('%');
            var toksMap = new Dictionary<string, string>();
            for (int i = 1; i < toks.Length - 1; i++)
            {
                toksMap[toks[i]] = toks[++i];
            }
            var tsDataRequest = new TsDataRequest();
            string strVal;
            if (toksMap.TryGetValue("s", out strVal))
            {
                tsDataRequest.StartTime = DateHelper.ParseDateTimeString(strVal);
            }
            if (toksMap.TryGetValue("e", out strVal))
            {
                tsDataRequest.EndTime = DateHelper.ParseDateTimeString(strVal);
            }
            if (toksMap.TryGetValue("pt", out strVal))
            {
                tsDataRequest.PeriodSizeTicks = long.Parse(strVal);
            }
            if (toksMap.TryGetValue("pot", out strVal))
            {
                tsDataRequest.PeriodSizeOffsetTicks = long.Parse(strVal);
            }
            if (toksMap.TryGetValue("dt", out strVal))
            {
                tsDataRequest.DataProviderType = strVal;
            }
            if (toksMap.TryGetValue("c", out strVal))
            {
                tsDataRequest.DoConsolidate = bool.Parse(strVal);
            }
            if (toksMap.TryGetValue("si", out strVal))
            {
                tsDataRequest.Symbols = strVal;
            }
            if (toksMap.TryGetValue("col", out strVal))
            {
                tsDataRequest.Column = strVal;
            }
            if (toksMap.TryGetValue("co", out strVal))
            {
                tsDataRequest.Country = strVal;
            }
            if (toksMap.TryGetValue("cu", out strVal))
            {
                tsDataRequest.Currency = strVal;
            }
            if (toksMap.TryGetValue("ic", out strVal))
            {
                tsDataRequest.IsContinuous = strVal.Equals("1");
            }
            if (toksMap.TryGetValue("td", out strVal))
            {
                tsDataRequest.TsDataType = (EnumTechIndType)Enum.Parse(typeof(EnumTechIndType), strVal);
            }
            if (toksMap.TryGetValue("q", out strVal))
            {
                tsDataRequest.Query = strVal;
            }
            if (toksMap.TryGetValue("ap", out strVal))
            {
                tsDataRequest.Period = int.Parse(strVal);
            }
            if (toksMap.TryGetValue("cp", out strVal))
            {
                tsDataRequest.m_customParams = new SerializableDictionary<string, object>();
                string[] cpToks = strVal.Split('=');
                for (int j = 0; j < cpToks.Length; j++)
                {
                    tsDataRequest.m_customParams[cpToks[j]] = (cpToks[++j]);
                }
            }
            return tsDataRequest;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(m_strRequestDescr))
            {
                GetResourceName2();
            }

            return m_strRequestDescr;
        }

        public void Dispose()
        {
            m_dataProviderType = null;
            if (m_innerFunctionDataRequest != null)
            {
                m_innerFunctionDataRequest.Dispose();
                m_innerFunctionDataRequest = null;
            }
            m_strColumn = null;
            m_strCountry = null;
            m_strCurrency = null;
            m_strCurrencyArr = null;
            m_strQuery = null;
            m_strRequestDescr = null;
            m_strSymbolArr = null;
            m_strSymbols = null;

            if (m_customParams != null)
            {
                m_customParams.Clear();
                m_customParams = null;
            }
        }

        #endregion

        public override object Clone()
        {
            var item = (TsDataRequest)base.Clone();
            if(CustomParams != null)
            {
                item.CustomParams = new SerializableDictionary<string, object>(CustomParams);
            }
            return item;
        }

        #region Public

        public bool Equals(IDataRequest other)
        {
            if (other == null)
            {
                return false;
            }
            return Name.Equals(other.Name);
        }

        public int CompareTo(IDataRequest other)
        {
            return Name.CompareTo(other.Name);
        }

        public int Compare(IDataRequest x, IDataRequest y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public int Compare(object x, object y)
        {
            return Compare((IDataRequest) x,
                           (IDataRequest) y);
        }

        public bool Equals(IDataRequest x, IDataRequest y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(IDataRequest obj)
        {
            return obj.Name.GetHashCode();
        }

        public void ClearPublishList()
        {
            if (m_tsPublishList != null)
            {
                m_tsPublishList = new Dictionary<string, PublishTsDelegate>();
            }
        }

        public Dictionary<string,PublishTsDelegate> GetTsEventPublishList()
        {
            if (m_tsPublishList != null)
            {
                var map = 
                    new Dictionary<string, PublishTsDelegate>();
                foreach (var kvp in m_tsPublishList)
                {
                    map[kvp.Key] = kvp.Value;
                }
                return map;
            }
            return null;
        }

        private void Reset()
        {
            m_strRequestDescr = string.Empty;
            m_strSymbolArr = null;
            m_strCurrencyArr = null;
        }

        public void AddPublishTsDelegate(
            string strName,
            PublishTsDelegate publishTsDelegate)
        {
            if (publishTsDelegate == null)
            {
                return;
            }
            if (m_tsPublishList == null)
            {
                m_tsPublishList = new Dictionary<string, PublishTsDelegate>();
            }
            if (!m_tsPublishList.ContainsKey(strName))
            {
                m_tsPublishList.Add(strName, publishTsDelegate);
            }
        }

        public void InvokeTsPublishers(ITsEvent tsEvent)
        {
            if (m_tsPublishList != null &&
                m_tsPublishList.Count > 0)
            {
                foreach (PublishTsDelegate publishTsEvent in m_tsPublishList.Values)
                {
                    publishTsEvent.Invoke(tsEvent);
                }
            }
        }

        #endregion

    }
}
