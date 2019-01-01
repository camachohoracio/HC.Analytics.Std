#region

using System;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.DataStructures
{
    internal class Candidates : IComparable
    {
        private readonly double dblScoreClass;
        private readonly int m_intTColumnClass;
        private readonly int m_intTPosClass;
        private readonly int m_intUColumnClass;
        private readonly int m_intUPosClass;

        public Candidates(
            int intTPos,
            int intUPos,
            int intTColumn,
            int intUColumn,
            double dblScore)
        {
            m_intTPosClass = intTPos;
            m_intUPosClass = intUPos;
            m_intTColumnClass = intTColumn;
            m_intUColumnClass = intUColumn;
            dblScoreClass = dblScore;
        }

        public Candidates(
            int intTPos,
            int intUPos,
            double dblScore)
        {
            m_intTPosClass = intTPos;
            m_intUPosClass = intUPos;
            dblScoreClass = dblScore;
        }

        #region IComparable Members

        public int CompareTo(Object obj)
        {
            Candidates Compare = (Candidates) obj;
            int intResult = dblScoreClass.CompareTo(Compare.GetScore());
            if (intResult == 0)
            {
                int intTrClass = -Math.Abs(m_intTPosClass - m_intUPosClass);
                int intTrCompare = -Math.Abs(Compare.GetTPos() - Compare.GetUPos());
                intResult = intTrClass.CompareTo(intTrCompare);
            }
            return intResult;
        }

        #endregion

        public int GetTPos()
        {
            return m_intTPosClass;
        }

        public int GetUPos()
        {
            return m_intUPosClass;
        }

        public int GetTCol()
        {
            return m_intTColumnClass;
        }

        public int GetUCol()
        {
            return m_intUColumnClass;
        }

        public double GetScore()
        {
            return dblScoreClass;
        }
    }
}
