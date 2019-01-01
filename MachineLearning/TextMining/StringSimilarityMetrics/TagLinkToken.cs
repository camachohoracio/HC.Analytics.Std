#region

using System;
using System.Collections.Generic;
using HC.Analytics.MachineLearning.TextMining.DataStructures;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics
{
    public class TagLinkToken
    {
        private const double TR = 0.3;
        private double m_matched;

        public double getStringMetric(string strT, string strU)
        {
            double score = getRawMetric(strT, strU);
            return winkler(score, strT, strU);
        }

        public double getRawMetric(string strT, string strU)
        {
            int tSize, uSize;
            if (strT == "")
            {
                return 0.0;
            }
            if (strT.Equals(strU))
            {
                m_matched = strT.Length;
                return 1.0;
            }
            else
            {
                tSize = strT.Length;
                uSize = strU.Length;
                if (tSize == 0 || uSize == 0)
                {
                    return 0.0;
                }
                // let T be the largest token
                if (tSize < uSize)
                {
                    string tmp1 = strT;
                    strT = strU;
                    strU = tmp1;
                    int tmp2 = tSize;
                    tSize = uSize;
                    uSize = tmp2;
                }
            }
            List<Candidates> candidateList = algorithm1(strT, strU);
            candidateList.Sort();
            candidateList.Reverse();
            double dblScore = getScore(candidateList),
                   dblScore1 = dblScore/(tSize),
                   dblScore2 = dblScore/(uSize);

            //score = (score1+score2) / 2.0;
            dblScore = (2.0*dblScore1*dblScore2)/(dblScore1 + dblScore2);
            return dblScore;
        }

        private double winkler(double dblScore, string strT, string strU)
        {
            int bound = Math.Min(4, Math.Min(strT.Length, strU.Length));
            int prefix;
            for (prefix = 0; prefix < bound; prefix++)
            {
                if (strT[prefix] != strU[prefix])
                {
                    break;
                }
            }
            dblScore = dblScore + (prefix*0.1*(1.0 - dblScore));
            return dblScore;
        }


        private double getScore(List<Candidates> candidateList)
        {
            m_matched = 0;
            double scoreValue = 0;
            Dictionary<int, int> tMap = new Dictionary<int, int>();
            Dictionary<int, int> uMap = new Dictionary<int, int>();
            foreach (Candidates actualCandidates in candidateList)
            {
                int actualTPos = actualCandidates.GetTPos(),
                    actualUPos = actualCandidates.GetUPos();
                if ((!tMap.ContainsKey(actualTPos)) &&
                    (!uMap.ContainsKey(actualUPos)))
                {
                    double actualScore = actualCandidates.GetScore();
                    scoreValue += actualScore;
                    tMap.Add(actualTPos, actualTPos);
                    uMap.Add(actualUPos, actualUPos);
                    m_matched += actualScore;
                }
            }
            return scoreValue;
        }

        public double getMatched()
        {
            return m_matched;
        }

        private List<Candidates> algorithm1(string T, string U)
        {
            List<Candidates> candidateList = new List<Candidates>();
            int bound = (int) (1.0/TR);
            int tLength = T.Length;
            for (int t = 0; t < tLength; t++)
            {
                char chT = T[t];
                double lastTr = -1;
                int uLength = U.Length;
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
                            candidateList.Add(new Candidates(t, u, charScore));
                        }
                    }
                }
            }
            return candidateList;
        }
    }
}
