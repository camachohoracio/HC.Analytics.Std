using System;

namespace HC.Analytics.MachineLearning.MstCluster
{
    [Serializable]
    public class MSTParentObject
    {
        private readonly int m_intParent;

        public MSTParentObject(int intParent0)
        {
            m_intParent = intParent0;
        }

        public int GetParent()
        {
            return m_intParent;
        }
    }
}
