#region

using System;
using System.Collections.Generic;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    [Serializable]
    public class FeatureObject
    {
        #region Members

        private readonly double[] m_dblFeature;
        private readonly List<int> m_indexList;
        private double[] m_dblNormalizedFeature;

        #endregion

        #region Properties

        public List<int> IndexList
        {
            get { return m_indexList; }
        }

        public double[] Feature
        {
            get { return m_dblFeature; }
        }

        public double[] NormalizedFeature
        {
            get { return m_dblNormalizedFeature; }
            set { m_dblNormalizedFeature = value; }
        }

        #endregion

        #region Constructor

        public FeatureObject(double[] dblFeature)
        {
            m_dblFeature = dblFeature;
            m_indexList = new List<int>();
        }

        #endregion

    }
}

