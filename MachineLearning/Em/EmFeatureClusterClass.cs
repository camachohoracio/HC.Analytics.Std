#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HC.Core.Events;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using QLNet;

#endregion

namespace HC.Analytics.MachineLearning.Em
{
    public class EmFeatureClusterClass
    {
        #region Constants

        private const int EM_ITERATIONS = 200;

        #endregion

        #region Members

        private Dictionary<string, FeatureObject> m_featuresDict;

        #endregion

        #region Constructors

        public Dictionary<string, int> DoEmCluster(
            Dictionary<string, double[]> features,
            int intClusters)
        {
            //
            // iterate cluster method until we get the desired number of clusters
            // partition by the largest cluster
            //
            int intClusterLeftToDo = intClusters;
            Dictionary<string, double[]> currentFeatures =
                features;

            // this object contains the group of features by cluster
            List<Dictionary<string, double[]>> overalClusters =
                new List<Dictionary<string, double[]>>();

            //
            // do while having missing clusters
            //
            while (intClusterLeftToDo > 0 && currentFeatures.Count > 0)
            {
                Dictionary<string, int> currentClusters = DoClusterIteration(
                    currentFeatures,
                    intClusterLeftToDo);

                //
                // get list of features for current cluster
                //
                List<Dictionary<string,double[]>> clusters =
                    new List<Dictionary<string, double[]>>();
                LoadFeatures(currentFeatures, currentClusters, clusters);

                if(clusters.Count + overalClusters.Count >= intClusters)
                {
                    //
                    // merge clusters and overall clusters
                    //
                    foreach (Dictionary<string, double[]> dictionary in clusters)
                    {
                        overalClusters.Add(dictionary);
                    }
                    break;
                }
                //
                // get the smallest feature dictionary
                //
                Dictionary<string, double[]> selectedDict = GetSmallestFeatureDict(
                    clusters);
                overalClusters.Add(selectedDict);
                // set clusters left to do
                intClusterLeftToDo--;

                //
                // set current features as a list with th erest of the features
                //
                currentFeatures = GetCurrentFeatures(clusters);
            }
            //
            // get final clusters
            //
            Dictionary<string, int> finalClusters =
                GetFinalClusters(overalClusters);
            return finalClusters;

        }

        #endregion

        #region Private

        private static Dictionary<string,double[]> GetCurrentFeatures(
            IEnumerable<Dictionary<string,double[]>> clusters)
        {
            Dictionary<string,double[]> currentFeatures = new Dictionary<string, double[]>();
            foreach (Dictionary<string, double[]> dictionary in clusters)
            {
                foreach (KeyValuePair<string, double[]> keyValuePair in dictionary)
                {
                    currentFeatures.Add(keyValuePair.Key,
                        keyValuePair.Value);
                }
            }
            return currentFeatures;
        }

        private static Dictionary<string,int> GetFinalClusters(
            IEnumerable<Dictionary<string,double[]>> overalClusters)
        {
            Dictionary<string,int> finalClusters=
                new Dictionary<string, int>();
            int intClusterCounter = 0;
            foreach (Dictionary<string, double[]> overalCluster in overalClusters)
            {
                foreach (string strFeatureName in overalCluster.Keys)
                {
                    finalClusters.Add(
                        strFeatureName,
                        intClusterCounter);
                }
                intClusterCounter++;
            }
            return finalClusters;
        }

        private static Dictionary<string,double[]> GetSmallestFeatureDict(
            List<Dictionary<string,double[]>> overalClusters)
        {
            int intMinDictSize = int.MaxValue;
            Dictionary<string, double[]> selectedDict = null;
            foreach (Dictionary<string, double[]> currentFeatureDict in overalClusters)
            {
                if(currentFeatureDict.Count<intMinDictSize)
                {
                    intMinDictSize = currentFeatureDict.Count;
                    selectedDict = currentFeatureDict;
                }
            }
            //
            // remove this cluster from the list
            //
            overalClusters.Remove(selectedDict);
            return selectedDict;
        }

        private static void LoadFeatures(
            Dictionary<string,double[]> features,
            Dictionary<string, int> currentClusters,
            List<Dictionary<string,double[]>> overalClusters)
        {
            Dictionary<int,int> clusterLookup =
                new Dictionary<int, int>();
            Dictionary<int, Dictionary<string,double[]>> featureMap =
                new Dictionary<int, Dictionary<string, double[]>>();
            foreach (KeyValuePair<string, int> keyValuePair in currentClusters)
            {
                if(!clusterLookup.ContainsKey(
                    keyValuePair.Value))
                {
                    featureMap.Add(clusterLookup.Count,
                        new Dictionary<string, double[]>());
                    clusterLookup.Add(
                        keyValuePair.Value,
                        clusterLookup.Count);
                }
                //
                // add current feature
                //
                featureMap[
                    clusterLookup[keyValuePair.Value]].Add(
                    keyValuePair.Key,
                    features[keyValuePair.Key]);
            }

            foreach (KeyValuePair<int, Dictionary<string, double[]>> keyValuePair in featureMap)
            {
                overalClusters.Add(keyValuePair.Value);
            }
        }

        private Dictionary<string,int> DoClusterIteration(
            Dictionary<string,double[]> features,
            int intClusters)
        {
            Initialize();

            //
            // get features
            //
            foreach (KeyValuePair<string, double[]> keyValuePair in features)
            {
                FeatureObject featureObject =
                    new FeatureObject(
                        (double[])keyValuePair.Value.Clone());
                m_featuresDict.Add(
                    keyValuePair.Key,
                    featureObject);
            }

            int intColumns = features.Values.First().Length;

            NormaliseFeatures(intColumns);

            SendMessageEvent.OnSendMessage("Loading cluster data...");
            double[,] dblFeatureArr = new double[
                m_featuresDict.Count,
                intColumns];

            int intIndex = 0;
            foreach (FeatureObject featureObject in m_featuresDict.Values)
            {
                for (int j = 0; j < intColumns; j++)
                {
                    dblFeatureArr[intIndex, j] = featureObject.NormalizedFeature[j];
                }
                intIndex++;
            }

            SendMessageEvent.OnSendMessage("Running EM cluster...");
            int intNumGaussians = intClusters;
            var emClusterArr = new EmCluster[EM_ITERATIONS];
            Parallel.For(
                0, EM_ITERATIONS, delegate(int i)
                                      {
                                          emClusterArr[i] = new EmCluster();
                                          emClusterArr[i].DoCluster(
                                              intNumGaussians,
                                              dblFeatureArr);
                                      }
                );
            EmCluster emCluster = null;
            double dblMaxLikelihood = -double.MaxValue;
            int intMaxClusterCount = -int.MaxValue;

            //
            // select cluster with the highest likelihood function
            // also select the cluster with the closest number of gaussians
            // as requested.
            //
            for (int i = 0; i < EM_ITERATIONS; i++)
            {
                int intClusterCount = GetClusterCount(emClusterArr[i]);
                if(intMaxClusterCount <= intClusterCount)
                {
                    intMaxClusterCount = intClusterCount;
                    if(intMaxClusterCount <= intClusterCount)
                    {
                        intMaxClusterCount = intClusterCount;
                        if(emClusterArr[i].Likelihood > dblMaxLikelihood)
                        {
                            dblMaxLikelihood = emClusterArr[i].Likelihood;
                            emCluster = emClusterArr[i];
                        }
                    }
                }
            }
            if(emCluster != null)
            {
                PrintToScreen.WriteLine(emCluster.ToString());
                return GetEmResults(emCluster);
            }
            throw new HCException("Null EM cluster");
        }

        private int GetClusterCount(
            EmCluster emCluster)
        {
            SendMessageEvent.OnSendMessage("Loading results...");
            //
            // assign clusters
            //
            List<int> clusterResults =
                new List<int>();

            foreach (KeyValuePair<string, FeatureObject> kvp in m_featuresDict)
            {
                FeatureObject featureObject = kvp.Value;
                int intClusterClass =
                    emCluster.ClusterInstance(featureObject.NormalizedFeature);
                if(!clusterResults.Contains(intClusterClass))
                {
                    clusterResults.Add(intClusterClass);
                }
            }
            SendMessageEvent.OnSendMessage("");
            return clusterResults.Count;
        }

        private Dictionary<string,int> GetEmResults(
            EmCluster emCluster)
        {
            SendMessageEvent.OnSendMessage("Loading results...");
            //
            // asign clusters
            //
            Dictionary<string,int> clusterResults =
                new Dictionary<string, int>();

            foreach (KeyValuePair<string, FeatureObject> kvp in m_featuresDict)
            {
                FeatureObject featureObject =
                    kvp.Value;
                string strFeatureName = kvp.Key;
                int intClusterClass =
                    emCluster.ClusterInstance(featureObject.NormalizedFeature);
                clusterResults.Add(strFeatureName,intClusterClass);
            }
            SendMessageEvent.OnSendMessage("");
            return clusterResults;
        }

        private void NormaliseFeatures(int intColCount)
        {
            IncrementalStatistics[] stat = new IncrementalStatistics[
                intColCount];
            IncrementalStatistics[] stat2 = new IncrementalStatistics[
                intColCount];

            for (int i = 0; i < intColCount; i++)
            {
                stat[i] = new IncrementalStatistics();
                stat2[i] = new IncrementalStatistics();
            }

            //
            // update stats for each data column
            //
            foreach (FeatureObject featureObject in m_featuresDict.Values)
            {
                double[] feature = featureObject.Feature;

                for (int i = 0; i < feature.Length; i++)
                {
                    stat[i].add(feature[i]);
                }
            }

            //
            // center featues around the mean
            //
            foreach (FeatureObject featureObject in m_featuresDict.Values)
            {
                double[] feature = featureObject.Feature;
                double[] normFeature = (double[]) feature.Clone();
                for (int i = 0; i < feature.Length; i++)
                {
                    double dblStdDev = stat[i].standardDeviation();
                    dblStdDev = double.IsNaN(dblStdDev) ? 0 : dblStdDev;

                    double dblFeatureValue = dblStdDev == 0
                                                 ? 0
                                                 : ((feature[i] - stat[i].mean())/
                                                    dblStdDev);
                    if(double.IsNaN(dblFeatureValue))
                    {
                        throw new HCException("NaN feature value");
                    }

                    normFeature[i] = dblFeatureValue;

                    stat2[i].add(normFeature[i]);
                }
                featureObject.NormalizedFeature =
                    normFeature;
            }

            //
            // remove negative values
            //
            foreach (FeatureObject featureObject in m_featuresDict.Values)
            {
                double[] feature = featureObject.NormalizedFeature;
                for (int i = 0; i < feature.Length; i++)
                {
                    double dblLow = stat2[i].min();
                    double dblHigh = stat2[i].max();
                    double dblLength = dblHigh - dblLow;
                    double dblFeatureValue = (feature[i] - dblLow)/dblLength;

                    if(dblLength == 0)
                    {
                        dblFeatureValue = 0;
                    }
                    else
                    {
                        feature[i] = dblFeatureValue;
                    }
                    if(double.IsNaN(dblFeatureValue))
                    {
                        throw new HCException("NaN feature value");
                    }
                }
            }
        }

        private void Initialize()
        {
            m_featuresDict =
                new Dictionary<string, FeatureObject>();
        }

        #endregion

    }
}

