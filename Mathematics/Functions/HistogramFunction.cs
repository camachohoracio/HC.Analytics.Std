using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Mathematics.Functions.DataStructures;
using HC.Core.Exceptions;

namespace HC.Analytics.Mathematics.Functions
{
    public class HistogramFunction : AFunction2D
    {
        #region Properties

        public List<double[]> HistogramList
        {
            get { return m_histogramList; }
            set { m_histogramList = value; }
        }

        public string[] BinsNames
        {
            get { return m_binsNames; }
            set { m_binsNames = value; }
        }

        public List<string> HistogramNameList
        {
            get { return m_histogramNameList; }
            set { m_histogramNameList = value; }
        }

        #endregion

        #region Members

        private string[] m_binsNames;

        private List<double[]> m_histogramList;
        private List<string> m_histogramNameList;
        private Dictionary<string, double[]> m_histogramDictionary;
        #endregion

        #region Constructors

        public HistogramFunction(
            string[] binsNames,
            string strName)
        {
            m_binsNames = binsNames;
            m_histogramList = new List<double[]>();
            m_histogramNameList = new List<string>();
            m_histogramDictionary = 
                new Dictionary<string, double[]>();
            FunctionName = strName;
        }

        #endregion

        public void AddHistogram(
            string strName,
            double[] dblBinsValues)
        {
            if(m_binsNames.Length != dblBinsValues.Length)
            {
                //Debugger.Break();
                throw new HCException("Bin size does not match");
            }

            // check if name is in the list
            if(!m_histogramDictionary.ContainsKey(strName))
            {
                m_histogramList.Add(dblBinsValues);
                m_histogramNameList.Add(strName);
                m_histogramDictionary.Add(strName, dblBinsValues);
            }
            else
            {
                //
                // add to the existing histogram
                //
                double[] dblCurrentBins = m_histogramDictionary[strName];
                for (int i = 0; i < dblCurrentBins.Length; i++)
                {
                    dblCurrentBins[i] += dblBinsValues[i];
                }
            }
        }

        public override void SetFunctionLimits()
        {
            double dblMaxValue = -double.MaxValue;
            for (int i = 0; i < m_histogramList.Count; i++)
            {
                if(dblMaxValue < m_histogramList[i].Sum())
                {
                    dblMaxValue = m_histogramList[i].Sum();
                }
            }
            YMin = 0;
            YMax = dblMaxValue;
        }

        public override double EvaluateFunction(double dblX)
        {
            throw new NotImplementedException();
        }
    }
}

