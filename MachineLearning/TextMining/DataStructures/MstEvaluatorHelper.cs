using System;
using System.Collections.Generic;
using System.IO;
using HC.Analytics.MachineLearning.MstCluster;
using HC.Core.Helpers;
using HC.Core.Io;
using HC.Core.Text;

namespace HC.Analytics.MachineLearning.TextMining.DataStructures
{
    class MstEvaluatorHelper
    {
        public void EvaluateTree(
            string m_strDbName,
            string m_strFileName,
            Dictionary<int, MstEdge> m_htEdge,
            Dictionary<string, double> m_htScores,
            DataWrapper m_dataArray)
        {
            StreamWriter streamWriter0 = new StreamWriter("Evaluation_" + m_strDbName + ".txt");
            streamWriter0.WriteLine("Precision\tRecall\tF1\tTime(seconds)");

            int goalMatch = GetGoalToMatch(
                m_dataArray),
                correctMatched = 0,
                totalMatched = 0;

            // get result list
            List<ScoreObject> resultList = GetResultList(
                m_htEdge,
                m_htScores);

            // save the sorted score list
            resultList.Sort();
            resultList.Reverse();

            FileInfo fi = new FileInfo(m_strFileName);
            if (!DirectoryHelper.Exists(
                fi.DirectoryName,
                false))
            {
                DirectoryHelper.CreateDirectory(
                    fi.DirectoryName);
            }

            StreamWriter streamWriter = new StreamWriter(m_strFileName);
            //streamWriter.WriteLine("Redundant record\tOriginal record\tRelevance");
            foreach (ScoreObject scoreObject in resultList)
            {
                double score = scoreObject.GetScore();
                int current = scoreObject.GetPosition();
                MstEdge currentMSTEdge = m_htEdge[current];
                int before = currentMSTEdge.GetBefore();
                TokenWrapper keyT = m_dataArray.Data[current].Columns[0][0];
                TokenWrapper keyU = m_dataArray.Data[before].Columns[0][0];
                if (keyT.Equals(keyU))
                {
                    correctMatched++;
                }
                streamWriter.WriteLine(keyT + "\t" + keyU + "\t" + score);
                totalMatched++;
            }
            streamWriter.Close();
            double precision = correctMatched / ((double)totalMatched);
            double recall = correctMatched / ((double)goalMatch);
            double F1 = (2.0 * precision * recall) / (precision + recall);

            streamWriter0.WriteLine(precision + "\t" + recall + "\t" + F1 + "\t" + 0);
            streamWriter0.Close();

            string strMessage = "DataManager detected " + totalMatched +
                                " redundant records. \nCheck file: ' "
                                + m_strFileName + "' for details";
            PrintToScreen.WriteLine(strMessage);

            // to do :
            //HC.Utils.MessageBoxWrapper.Show(strMessage);
        }

        private int GetGoalToMatch(
            DataWrapper dataArray)
        {
            int N = dataArray.Length;
            var hasht = 
                new Dictionary<TokenWrapper, int>(
                    new TokenWrapperComparer());
            for (int i = 0; i < N; i++)
            {
                TokenWrapper currentKey = dataArray.Data[i].Columns[0][0];
                if (!hasht.ContainsKey(currentKey))
                {
                    hasht.Add(currentKey, 1);
                }
                else
                {
                    int freq = hasht[currentKey];
                    hasht[currentKey] = freq + 1;
                }
            }
            int goalMatch = 0;
            foreach (KeyValuePair<TokenWrapper, int> de in hasht)
            {
                int freq = de.Value;
                goalMatch += freq - 1;
            }
            return goalMatch;
        }

        private List<ScoreObject> GetResultList(
            Dictionary<int, MstEdge> m_htEdge,
            Dictionary<string, double> m_htScores)
        {
            List<ScoreObject> resultList = new List<ScoreObject>();
            foreach (KeyValuePair<int, MstEdge> de in m_htEdge)
            {
                MstEdge currentMSTEdge = de.Value;
                int current = de.Key;
                int before = currentMSTEdge.GetBefore();
                if (before > -1)
                {
                    int xNew = Math.Min(current, before),
                        yNew = Math.Max(current, before);
                    string position = xNew + "-" + yNew;
                    double score = m_htScores[position];
                    resultList.Add(new ScoreObject(current, score));
                }
            }
            return resultList;
        }

        public List<MstDistanceObj> GetRootList(
            Dictionary<int, MstEdge> m_htEdge,
            Dictionary<string, double> m_htScores)
        {
            // get result list
            List<ScoreObject> resultList = GetResultList(
                m_htEdge,
                m_htScores);
            List<MstDistanceObj> rootList = new List<MstDistanceObj>();
            // save the sorted score list
            resultList.Sort();
            resultList.Reverse();
            // iterate result list
            foreach (ScoreObject scoreObject in resultList)
            {
                double score = scoreObject.GetScore();
                int current = scoreObject.GetPosition();
                MstEdge currentMSTEdge = m_htEdge[current];
                int parent = currentMSTEdge.GetParent();
                MstDistanceObj scoreObject3 =
                    new MstDistanceObj(current, parent, score);
                rootList.Add(scoreObject3);
            }
            return rootList;
        }


        private void SetScoreMap(
            Dictionary<string, double> scoreMap0,
            Dictionary<string, double> m_htScores)
        {
            m_htScores = scoreMap0;
        }

    }
}

