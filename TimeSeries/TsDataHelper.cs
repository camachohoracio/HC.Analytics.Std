#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries
{
    public static class TsDataHelper
    {
        public static string[] ParseSymboltring(string strSymbol)
        {
            try
            {
                if (string.IsNullOrEmpty(strSymbol))
                {
                    return new string[0];
                }
                var symbolArr = strSymbol.Split(@",; ".ToCharArray());
                return (from s in symbolArr
                        where !string.IsNullOrEmpty(s)
                        select s).ToArray();
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return new string[0];
        }

        public static string GetStringFromSymbols(List<string> ricsUniverseList)
        {
            return GetStringFromSymbols(ricsUniverseList.ToArray());
        }

        public static string GetStringFromSymbols(string[] ricsUniverseList)
        {
            if (ricsUniverseList == null || ricsUniverseList.Length == 0)
            {
                return null;
            }

            //
            // load client data symbols in bulk
            //
            var sb = new StringBuilder();
            sb.Append(ricsUniverseList[0]);

            for (var i = 1; i < ricsUniverseList.Length; i++)
            {
                sb.Append("," + ricsUniverseList[i]);
            }

            var strSymbols = sb.ToString();

            return strSymbols;
        }
    }
}

