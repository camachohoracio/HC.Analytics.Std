#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries.TsStats.TrainerWrappers
{
    public class NnetTrainerWrapper : ATrainerWrapper
    {
        protected int m_intHiddenNeurons = NnetTrainer.NUM_HIDDEN_NEURONS;
        private List<alglib.multilayerperceptron> m_ensemble;
        private double m_dblSampleSizeFactor;

        #region Constructors

        public NnetTrainerWrapper(
            List<double> xData,
            List<double> yData,
            int intThreads = 40,
            int intEnsembleSize = ENSEMBLE_SIZE)
            : this(
                (from n in xData select new[] { n }).ToList(),
                yData,
                intThreads,
                intEnsembleSize)
        {
        }

        public NnetTrainerWrapper(
            List<double> xData,
            List<double> yData,
            int intNumClasses,
            bool blnVerbose)
            : this(
                (from n in xData select new[] { n }).ToList(),
                yData,
                intNumClasses,
                blnVerbose)
        {
        }

        public NnetTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intThreads = 40,
            int intHiddenNeurons = NnetTrainer.NUM_HIDDEN_NEURONS,
            int intEnsembleSize = ENSEMBLE_SIZE)
            : this(
            xData,
            yData,
            1, // 1 means it is a regression
            false,
            intThreads,
            intHiddenNeurons,
            intEnsembleSize)
        {
        }

        public NnetTrainerWrapper(
            List<double[]> xData,
            List<double> yData,
            int intNumClasses,
            bool blnVerbose = true,
            int intThreads = 40,
            int intHiddenNeurons = NnetTrainer.NUM_HIDDEN_NEURONS,
            int intEnsembleSize = ENSEMBLE_SIZE,
            double dblSampleSizeFactor = 1)
            : base(
                xData,
                yData,
                intNumClasses,
                blnVerbose,
                intThreads,
                intEnsembleSize)
        {
            m_intHiddenNeurons = intHiddenNeurons;
            m_dblSampleSizeFactor = dblSampleSizeFactor;
            LoadEnsemble();
        }

        #endregion

        public override double Forecast(
            double[] xVar)
        {
            try
            {
                if (m_intNumClasses == 1)
                {
                    List<double> predictions = NnetTrainer.GetNnetPredictionsEnsemble(
                        xVar,
                        m_ensemble);

                    return predictions.Average();
                }
                List<int> classEnsemble = NnetTrainer.GetNnetClassificationsEnsemble(
                    m_intNumClasses,
                    xVar,
                    m_ensemble);
                int intVotedClass =
                    TimeSeriesHelper.GetMostVotedClass(classEnsemble);
                return intVotedClass;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return double.NaN;
        }

        private void LoadEnsemble()
        {
            try
            {
                if (m_ensemble != null)
                {
                    return;
                }
                m_ensemble = NnetTrainer.TrainNnetSet(
                    m_xData,
                    m_yData,
                    m_intEnsembleSize,
                    m_intNumClasses,
                    m_dblSampleSizeFactor,
                    false,
                    m_blnVerbose,
                    m_intThreads,
                    m_intHiddenNeurons);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public double GetPrcCorrectlyClassified(List<double[]> xData, List<double> yData)
        {
            int intCorrect = 0;
            for (int i = 0; i < xData.Count; i++)
            {
                double[] xRow = xData[i];
                double dblYForecast = Forecast(xRow) - 1;
                if (dblYForecast == yData[i])
                {
                    intCorrect++;
                }
            }
            return intCorrect * 1.0 / yData.Count;
        }
    }
}
