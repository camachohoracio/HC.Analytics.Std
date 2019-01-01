#region

using System;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkPrefix
    {
        public static double GetStringMetric(
            TokenWrapper T, 
            TokenWrapper U)
        {
            int intGoal = Math.Min(2, Math.Min(T.Token.Length, U.Token.Length));
            if (intGoal == 0)
            {
                return 0.0;
            }
            //int matched = 0;
            for (int i = 0; i < intGoal; i++)
            {
                char tChar = T[i];
                char uChar = U[i];
                if (tChar == uChar)
                {
                    return 1.0;
                }
            }
            return 0.0;
        }
    }
}
