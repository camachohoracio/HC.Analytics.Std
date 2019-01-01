#region

using System.IO;
using System.Text;

#endregion

namespace HC.Analytics.Optimisation.Gp
{
    public static class GpHelper
    {
        public static void AddTabToParameter(
            string[] strParams,
            int intIndex)
        {
            var sb = new StringBuilder();
            using (var sr = new StringReader(strParams[intIndex]))
            {
                string strLine;
                while ((strLine = sr.ReadLine()) != null)
                {
                    sb.AppendLine("\t" + strLine);
                }
            }
            //
            // set new stirng to params
            //
            strParams[intIndex] = sb.ToString();
        }
    }
}
