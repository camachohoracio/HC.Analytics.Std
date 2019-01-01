#region

using System;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkTokenCheap : IDisposable
    {
        private const double TR = 0.3;
        private double m_matched;

        public double GetStringMetric(TokenWrapper T, TokenWrapper U)
        {
            double score = GetRawMetric(T, U);
            return Winkler(score, T, U);
        }

        public double GetRawMetric(TokenWrapper T, TokenWrapper U)
        {
            int tSize, uSize;
            //if (T == "")
            //{
            //    return 0.0;
            //}
            if (T.Equals(U))
            {
                m_matched = T.Length;
                return 1.0;
            }
            else
            {
                tSize = T.Length;
                uSize = U.Length;
                if (tSize == 0 || uSize == 0)
                {
                    return 0.0;
                }
                // let T be the largest token
                if (tSize < uSize)
                {
                    TokenWrapper tmp1 = T;
                    T = U;
                    U = tmp1;
                    int tmp2 = tSize;
                    tSize = uSize;
                    uSize = tmp2;
                }
            }
            double score = GetScore(T, U);
            m_matched = score;
            double score1 = score / tSize;
            double score2 = score / uSize;
            return (2.0*score1*score2)/(score1 + score2);
        }

        private double Winkler(
            double score, 
            TokenWrapper T, 
            TokenWrapper U)
        {
            int bound = Math.Min(4, Math.Min(T.Length, U.Length));
            int prefix;
            for (prefix = 0; prefix < bound; prefix++)
            {
                if (T[prefix] != U[prefix])
                {
                    break;
                }
            }
            score = score + (prefix*0.1*(1.0 - score));
            return score;
        }


        public double GetMatched()
        {
            return m_matched;
        }

        private double GetScore(
            TokenWrapper T, 
            TokenWrapper U)
        {
            int bound = (int) (1.0/TR);
            int tLength = T.Length;
            double totalScore = 0.0;
            for (int t = 0; t < tLength; t++)
            {
                char chT = T[t];
                double lastTr = -1;
                int uLength = U.Length;
                double maxCharScore = -1;
                for (int u = Math.Max(0, t - bound), flag = 0;
                     u < Math.Min(t + bound + 1, uLength) && flag == 0;
                     u++)
                {
                    double tr2 = (Math.Abs(t - u));
                    if ((lastTr >= 0.0) && (lastTr < tr2))
                    {
                        flag = 1;
                    }
                    else
                    {
                        char chU = U[u];
                        double charScore = 0.0;
                        if (chT == chU)
                        {
                            charScore = 1.0;
                        }
                        if (charScore > 0.0)
                        {
                            if (charScore == 1.0)
                            {
                                lastTr = tr2;
                            }
                            charScore = charScore - (TR*tr2);
                            if (charScore == 1.0)
                            {
                                flag = 1;
                            }
                            if (charScore > maxCharScore)
                            {
                                maxCharScore = charScore;
                            }
                        }
                    }
                }
                if (maxCharScore > 0)
                {
                    totalScore += maxCharScore;
                }
            }
            return totalScore;
        }

        public void Dispose()
        {
        }
    }
}
