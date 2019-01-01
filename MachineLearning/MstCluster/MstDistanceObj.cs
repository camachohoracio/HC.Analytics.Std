#region

using System;

#endregion

namespace HC.Analytics.MachineLearning.MstCluster
{
    public class MstDistanceObj : IComparable
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public double Score { get; private set; }

        public MstDistanceObj(
            int x,
            int y,
            double score)
        {
            X = x;
            Y = y;
            Score = score;
        }

        #region IComparable Members

        public int CompareTo(Object obj)
        {
            MstDistanceObj compare = (MstDistanceObj) obj;
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

        public int GetX()
        {
            return X;
        }

        public int GetY()
        {
            return Y;
        }

        public double GetScore()
        {
            return Score;
        }

        public void SetScore(double score)
        {
            Score = score;
        }
    }
}
