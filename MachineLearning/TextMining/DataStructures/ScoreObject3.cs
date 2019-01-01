#region

using System;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.DataStructures
{
    public class ScoreObject3 : IComparable
    {
        private readonly TokenWrapper m_strX;
        private readonly TokenWrapper m_strY;
        private double m_dblScore;

        public ScoreObject3(TokenWrapper strX, TokenWrapper strY, double dblScore)
        {
            m_strX = strX;
            m_strY = strY;
            m_dblScore = dblScore;
        }

        #region IComparable Members

        public int CompareTo(Object obj)
        {
            ScoreObject3 compare = (ScoreObject3) obj;
            double dblScore = GetScore();
            double dblScoreCompare = compare.GetScore();
            int intResult = dblScore.CompareTo(dblScoreCompare);
            if (intResult == 0)
            {
                intResult = dblScore.CompareTo(dblScoreCompare);
            }
            return intResult;
        }

        #endregion

        public TokenWrapper GetX()
        {
            return m_strX;
        }

        public TokenWrapper GetY()
        {
            return m_strY;
        }

        public double GetScore()
        {
            return m_dblScore;
        }

        public void SetScore(double dblScore)
        {
            m_dblScore = dblScore;
        }
    }
}
