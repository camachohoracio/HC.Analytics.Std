#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Statistics.Regression;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    [Serializable]
    public class MnLogitTrainerWrapper : ATrainerWrapper
    {
        public double[] Weights { get; set; }

        #region Members

        private alglib.logitmodel m_logitmodel0;

        private alglib.logitmodel m_logitmodel
        {
            get
            {
                if (m_logitmodel0 == null &&
                    Weights != null)
                {
                    m_logitmodel0 = new alglib.logitmodel(
                        new alglib.logit.logitmodel
                        {
                            w = Weights
                        });
                }
                return m_logitmodel0;
            }
            set
            {
                m_logitmodel0 = value;
                Weights = value.innerobj.w;
            }
        }

        #endregion

        #region Constructors


        public MnLogitTrainerWrapper(
            List<double> xData,
            List<double> yData,
            int intNumClasses)
            : this(
                (from n in xData select new[] { n }).ToList(),
                yData,
                intNumClasses)
        {
        }

        public MnLogitTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses) :
            base(xData,
            yData,
            intNumClasses,
            true)
        {
            DoTraining();
        }

        public MnLogitTrainerWrapper(
            double[] data,
            int intNumClasses)
            : base(data, intNumClasses)
        {
            DoTraining();
        }

        #endregion


        private void DoTraining()
        {
            try
            {
                if (m_xData[0].Length >= m_xData.Count - 1)
                {
                    //
                    // TODO this is a real prblem, do not comment it!
                    //
                    throw new HCException("Number of variables [" + m_xData[0].Length +
                                               "] are too large in comparison to num of samples [" + m_xData.Count + "]");
                }

                if ((from n in m_yData
                     where n >= m_intNumClasses
                     select n).Any())
                {
                    throw new HCException("Zero class is not allowed");
                }

                var currLogitmodel = TrainLogit();
                m_logitmodel = currLogitmodel;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private alglib.logitmodel TrainLogit()
        {
            alglib.mnlreport rep;
            int intInfo;
            double[,] xy = RegressionHelperAlgLib.GetXy(
                m_xData,
                m_yData);
            int intNumSamples = m_xData.Count;
            int intNumVars = m_xData.First().Length;

            if(m_intNumClasses < 2)
            {
                throw new HCException("Invalid number of classes");
            }

            alglib.logitmodel currLogitmodel;
            alglib.mnltrainh(
                xy,
                intNumSamples,
                intNumVars,
                m_intNumClasses,
                out intInfo,
                out currLogitmodel,
                out rep);

            if (intInfo != 1)
            {
                throw new HCException("Invalid training [" + intInfo + "]");
            }
            return currLogitmodel;
        }

        public override double Forecast(double[] dblX)
        {
            try
            {
                if (m_logitmodel == null)
                {
                    return double.NaN;
                }

                var yResult = new double[m_intNumClasses];
                alglib.mnlprocess(m_logitmodel, dblX, ref yResult);

                int intClass = -1;
                double dblMaxProb = -double.MaxValue;
                for (int i = 0; i < m_intNumClasses; i++)
                {
                    if (yResult[i] > dblMaxProb)
                    {
                        dblMaxProb = yResult[i];
                        intClass = i;
                    }
                }
                return intClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

        public static double GetPrcCorrectlyClassified<T>(
            SortedDictionary<T, double[]> xMapSelected,
            SortedDictionary<T, double> yMap,
            int intNumClasses)
        {
            try
            {
                var mnLogitClassifier = new MnLogitTrainerWrapper(
                    xMapSelected.Values.ToList(),
                    yMap.Values.ToList(),
                    intNumClasses);
                return mnLogitClassifier.GetPrcCorrectlyClassified();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return 0;
        }

    }
}
