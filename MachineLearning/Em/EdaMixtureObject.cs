#region

using System.Collections;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    public class EdaMixtureObject
    {
        #region Members

        private readonly int m_intIteration;
        private readonly ArrayList m_populationList;

        #endregion

        public EdaMixtureObject(ArrayList populationList, int iteration)
        {
            m_populationList = populationList;
            m_intIteration = iteration;
        }

        public ArrayList getPopulationList()
        {
            return m_populationList;
        }

        public int getIteration()
        {
            return m_intIteration;
        }
    }
}
