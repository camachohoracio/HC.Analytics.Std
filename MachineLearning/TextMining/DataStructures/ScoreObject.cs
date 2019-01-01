#region

using System;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.DataStructures
{
    internal class ScoreObject : IComparable
    {
        private readonly int m_intPosition;
        private double m_dblScore;

        public ScoreObject(int intPosition0, double dblScore0)
        {
            m_intPosition = intPosition0;
            m_dblScore = dblScore0;
        }

        #region IComparable Members

        public int CompareTo(Object obj)
        {
            ScoreObject compare = (ScoreObject) obj;
            int intResult = m_dblScore.CompareTo(compare.GetScore());
            if (intResult == 0)
            {
                intResult = m_dblScore.CompareTo(compare.GetScore());
            }
            return intResult;
        }

        #endregion

        public int GetPosition()
        {
            return m_intPosition;
        }

        public double GetScore()
        {
            return m_dblScore;
        }

        public void SetScore(double newScore)
        {
            m_dblScore = newScore;
        }
    }
}
