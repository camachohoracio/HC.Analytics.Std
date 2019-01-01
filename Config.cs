#region

using System.Collections.Generic;
using HC.Core.ConfigClasses;

#endregion

namespace HC.Analytics
{
    public static class Config
    {
        public static int GetResultSize()
        {
            return HCConfig.GetConstant<int>(
                "ResultsSize",
                typeof(Config));
        }

        public static IEnumerable<string> GetCrdwList()
        {
            return HCConfig.GetConfigList(
                "CrdwTable",
                typeof(Config));
        }

        public static List<string> GetDurbingWatsonList()
        {
            return HCConfig.GetConfigList(
                "DurbinWatsonTable",
                typeof(Config));
        }

        public static string GetOptDir()
        {
            return HCConfig.GetConstant<string>(
                "OptDir",
                typeof(Config));
        }
    }
}
