#region

using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class Stemer
    {
        #region Members

        private static readonly PorterStemer m_stemmer = new PorterStemer();

        #endregion

        public static void DoStem(DataWrapper dataWrapper)
        {
            foreach (RowWrapper row in dataWrapper.Data)
            {
                foreach (TokenWrapper[] col in row.Columns)
                {
                    foreach (TokenWrapper token in col)
                    {
                        token.SetToken(m_stemmer.DoStem(
                            token.Token));
                    }
                }
            }
        }
    }
}

