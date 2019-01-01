#region

using System.Collections.Generic;
using HC.Analytics.MachineLearning.TextMining.StringSimilarityMetrics;
using HC.Core.Helpers;
using HC.Core.Text;

#endregion

namespace HC.Analytics.MachineLearning.TextMining.PreProcessing
{
    public class Blocker
    {
        private readonly int m_intBlockingColumnCount;
        private string[][] m_dataArrayCoded;
        private Dictionary<string, List<int>>[] m_htCodedList;
        private int[] m_intBlockingColumnsArray;

        public Blocker(DataWrapper strDataArray)
        {
            // get the database with blocking keys
            GetCodedDataset(strDataArray);
            // select the most appropriate columns to block
            GetBlockingColumns();
            // count the number of columns
            m_intBlockingColumnCount = m_intBlockingColumnsArray.Length;
        }

        public void InitializeCodedListTable()
        {
            m_htCodedList = new Dictionary<string, List<int>>[m_intBlockingColumnCount];
            for (int i = 0; i < m_intBlockingColumnCount; i++)
            {
                m_htCodedList[i] = new Dictionary<string, List<int>>();
            }

            int N = m_dataArrayCoded.Length;
            List<int> indexList;
            for (int i = 0; i < N; i++)
            {
                for (int column = 0; column < m_intBlockingColumnCount; column++)
                {
                    int indexColumn = m_intBlockingColumnsArray[column];
                    string currentToken = m_dataArrayCoded[i][indexColumn];
                    if (!m_htCodedList[column].ContainsKey(currentToken))
                    {
                        indexList = new List<int>();
                        indexList.Add(i);
                        m_htCodedList[column].Add(currentToken, indexList);
                    }
                    else
                    {
                        indexList = m_htCodedList[column][currentToken];
                        indexList.Add(i);
                    }
                }
            }
        }

        public int[] GetBlockIndexes(int intRowIndex)
        {
            string[] tokenArray = m_dataArrayCoded[intRowIndex];
            return GetBlockIndexes(tokenArray, intRowIndex);
        }

        public int[] GetBlockIndexes(string[] strTokenArray, int intRowIndex)
        {
            List<int> indexList;
            List<int> resultList = new List<int>();
            Dictionary<int, object> tmpTable = new Dictionary<int, object>();
            for (int column = 0; column < m_intBlockingColumnCount; column++)
            {
                int index = m_intBlockingColumnsArray[column];
                string currentToken = strTokenArray[index];
                if (m_htCodedList[column].ContainsKey(currentToken))
                {
                    indexList = m_htCodedList[column][currentToken];
                    // iterate the list
                    foreach (int intCurrentIndex in indexList)
                    {
                        if (!tmpTable.ContainsKey(intCurrentIndex) && intCurrentIndex > intRowIndex)
                        {
                            tmpTable.Add(intCurrentIndex, null);
                            resultList.Add(intCurrentIndex);
                        }
                    }
                }
            }
            return resultList.ToArray();
        }

        private void GetCodedDataset(DataWrapper strDataArray)
        {
            // create a new coded dataset
            int N = strDataArray.Length,
                columnCount = strDataArray.Data[0].Columns.Length;
            m_dataArrayCoded = new string[N][];
            for (int i = 0; i < N; i++)
            {
                m_dataArrayCoded[i] = GetCodedRowArray(
                    strDataArray.Data[i].Columns, 
                    columnCount);
            }
        }

        public static string[] GetCodedRowArray(
            TokenWrapper[][] strRow,
            int intColumnCount)
        {
            string[] codedRow = new string[intColumnCount];
            for (int column = 0; column < intColumnCount; column++)
            {
                if (!string.IsNullOrEmpty(strRow[column][0].Token))
                {
                    string soundexCode =
                        Soundex.CalcSoundEx(strRow[column][0].Token);
                    codedRow[column] = soundexCode;
                }
                else
                {
                    codedRow[column] = "";
                }
            }
            return codedRow;
        }

        public bool CheckCodeMatch(int intTIdex, int intUIndex)
        {
            return CheckCodeMatch(m_dataArrayCoded[intTIdex],
                                  m_dataArrayCoded[intUIndex]);
        }

        public bool CheckCodeMatch(string[] tArray, int uIndex)
        {
            return CheckCodeMatch(tArray,
                                  m_dataArrayCoded[uIndex]);
        }

        public bool CheckCodeMatch(string[] strTArray, string[] strUArray)
        {
            for (int column = 0; column < m_intBlockingColumnCount; column++)
            {
                int index = m_intBlockingColumnsArray[column];
                if (strTArray[index].Equals(strUArray[index]))
                {
                    return true;
                }
            }
            return false;
        }

        private void GetBlockingColumns()
        {
            int N = m_dataArrayCoded.Length,
                columnCount = m_dataArrayCoded[0].Length;
            // for a small dataset return the same columns
            if (N <= 500)
            {
                m_intBlockingColumnsArray = new int[columnCount];
                for (int column = 0; column < columnCount; column++)
                {
                    m_intBlockingColumnsArray[column] = column;
                }
                return;
            }

            Dictionary<string, object>[] tokenTable = new Dictionary<string, object>[columnCount];
            for (int column = 0; column < columnCount; column++)
            {
                tokenTable[column] = new Dictionary<string, object>();
            }

            for (int i = 0; i < N; i++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    string currentToken = m_dataArrayCoded[i][column];
                    if (!tokenTable[column].ContainsKey(currentToken))
                    {
                        tokenTable[column].Add(currentToken, null);
                    }
                }
            }
            List<int> fieldList = new List<int>();
            for (int column = 0; column < columnCount; column++)
            {
                PrintToScreen.WriteLine("column " + column +
                                        ", block count " + tokenTable[column].Count);
                if (tokenTable[column].Count > 10)
                {
                    fieldList.Add(column);
                }
            }
            // if there is no field, then select the most adecuate column
            if (fieldList.Count == 0)
            {
                int maxCount = -1, maxCoumnIndex = -1;
                for (int column = 0; column < columnCount; column++)
                {
                    int currentCount = tokenTable[column].Count;
                    if (currentCount > maxCount)
                    {
                        maxCoumnIndex = column;
                    }
                }
                fieldList.Add(maxCoumnIndex);
            }
            m_intBlockingColumnsArray = fieldList.ToArray();
        }
    }
}
