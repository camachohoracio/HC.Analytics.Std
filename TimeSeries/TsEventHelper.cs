#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HC.Core.DynamicCompilation;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Io;
using HC.Core.Logging;
using HC.Core.Reflection;
using HC.Core.Time;

#endregion

namespace HC.Analytics.TimeSeries
{
    public static class TsEventHelper
    {

        public static SelfDescribingClassFactory CreateSelfDescribingClassFactory(
            string strClassName,
            string strNameSpace)
        {
            var selfDescribingClassFactory =
                SelfDescribingClassFactory.CreateFactory(
                    strClassName,
                    strNameSpace);
            //
            // make class factory a time series event
            //
            selfDescribingClassFactory.AddUsingStatement("HC.Analytics.TimeSeries.Events");
            selfDescribingClassFactory.AddReferencedAssembly(
                FileHelper.GetAssemblyFullFileName(typeof (ITsEvents)));
            selfDescribingClassFactory.AddReferencedAssembly(
                FileHelper.GetAssemblyFullFileName(typeof (HCException)));
            selfDescribingClassFactory.AddInterface(typeof (ITsEvent).Name);
            selfDescribingClassFactory.AddProperty("Time", typeof (DateTime));

            var methodSb = new StringBuilder();
            methodSb.AppendLine("string strCsvString =");
            methodSb.AppendLine("TsEventHelper.ToCsvString(this);");
            methodSb.AppendLine("return strCsvString;");

            selfDescribingClassFactory.AddMethod(
                true,
                false,
                "ToCsvString",
                typeof (string),
                null,
                methodSb.ToString());

            return selfDescribingClassFactory;
        }

        public static string[] GetFields(Type type)
        {
            var propertyInfos = GetPropertyInfos(type);
            return propertyInfos;
        }

        public static string[] GetPropertyInfos(Type type)
        {
            var binder = ReflectorCache.GetReflector(type);
            var propertyInfos =
                (from n in binder.GetPropertyNames()
                 orderby n
                 select n).ToArray();
            return propertyInfos;
        }

        public static string ToCsvString(
            ITsEvent tsEvent)
        {
            return ToCsvString(tsEvent, tsEvent.GetType());
        }

        public static string ToCsvString(
            object obj,
            Type type)
        {
            try
            {
                var binder = ReflectorCache.GetReflector(type);
                var oArr = (from n in binder.GetPropertyNames()
                            select binder.GetPropertyValue(
                                obj,
                                n)).ToArray();

                return ToCsvString(oArr);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return string.Empty;
        }

        public static string ToCsvString(
            object obj,
            Type type,
            string[] fields)
        {
            var oArr = new object[fields.Length];
            var binder = ReflectorCache.GetReflector(type);
            for (var i = 0; i < fields.Length; i++)
            {
                var strField = fields[i];
                oArr[i] =
                    binder.GetPropertyValue(
                        obj,
                        strField);
            }
            return ToCsvString(oArr);
        }

        public static string ToCsvString(object[] oArr)
        {
            var sb = new StringBuilder();
            if (oArr[0] is DateTime)
            {
                sb.Append(
                    DateHelper.ToDateTimeString((DateTime) oArr[0]));
            }
            else
            {
                if (oArr[0] != null)
                {
                    sb.Append(oArr[0].ToString()
                                  .Replace(",", "_")
                                  .Replace("\n", "")
                                  .Replace("\t", "")
                                  .Trim());
                }
            }

            for (var i = 1; i < oArr.Length; i++)
            {
                sb.Append(",");
                if (oArr[i] is DateTime)
                {
                    sb.Append(
                        DateHelper.ToDateTimeString((DateTime) oArr[i]));
                }
                else
                {
                    if (oArr[i] != null)
                    {
                        //
                        // remove invalid characters
                        //
                        sb.Append(oArr[i].ToString()
                                      .Replace(",", "_")
                                      .Replace("\n", "")
                                      .Replace("\t", "")
                                      .Trim());
                    }
                    else
                    {
                        //
                        // add an empty dummy value
                        //
                        sb.Append(string.Empty);
                    }
                }
            }
            return sb.ToString();
        }

        public static void ParseCsvString<T>(
            string strLine,
            T tEvent,
            IEnumerable<string> strTitles)
        {
            var strTokens = strLine.Split(',');
            var intIndex = 0;

            var expressionBinder = ReflectorCache.GetReflector(tEvent.GetType());

            foreach (string strTitle in strTitles)
            {
                if (!expressionBinder.ContainsProperty(strTitle))
                {
                    intIndex++;
                    continue;
                }
                if (!expressionBinder.CanWriteProperty(strTitle))
                {
                    continue;
                }
                if (intIndex > strTokens.Length - 1)
                {
                    break;
                }
                var strToken = strTokens[intIndex];
                var type = expressionBinder.GetPropertyType(strTitle);
                if (type == typeof (DateTime))
                {
                    expressionBinder.SetPropertyValue(
                        tEvent,
                        strTitle,
                        DateHelper.ParseDateTimeString(strToken));
                }
                else
                {
                    expressionBinder.SetPropertyValue(
                        tEvent,
                        strTitle,
                        ParserHelper.ParseString(
                            strToken,
                            type));
                }
                intIndex++;
            }
        }
    }
}
