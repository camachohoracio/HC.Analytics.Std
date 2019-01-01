#region

using System;
using System.IO;
using HC.Analytics.Probability.Distributions.LossDistributions.Ep;
using HC.Core.Exceptions;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Probability.Distributions.LossDistributions
{
    /// <summary>
    /// Distribution utilities.
    /// Note: This class is not thread safe. Alpha and Beta 
    /// parameters are calculated in different threads.
    /// </summary>
    public static class DistributionUtils
    {
        #region Constants

        /// <summary>
        /// Smallest value allowed by distribution parameters
        /// </summary>
        private const double SMALL_VALUE = 1E-11;

        #endregion

        #region Public

        /// <summary>
        /// Calculate alpha parameter for a beta distribution.
        /// </summary>
        /// <param name="dblMeanLoss">Mean</param>
        /// <param name="dblCv">CV</param>
        /// <returns>Alpha value</returns>
        public static double GetAlpha(
            double dblMeanLoss,
            double dblCv)
        {
            // make sure the CV is not equal to zero
            if (dblCv == 0)
            {
                dblCv = SMALL_VALUE;
            }

            double dblAlpha =
                (Math.Pow(dblMeanLoss, 2)*
                 (1.0 - dblMeanLoss)/Math.Pow(dblCv, 2)) -
                dblMeanLoss;

            if (dblAlpha < 0)
            {
                return SMALL_VALUE;
            }

            if (dblAlpha < 0.0)
            {
                throw new HCException("Error. Alpha parameter is less or equal to zero.");
            }
            return dblAlpha;
        }

        /// <summary>
        /// Calculate Beta parameter for a beta distribution
        /// </summary>
        /// <param name="dblMeanLoss">Mean loss</param>
        /// <param name="dblAlpha">Alpha parameter</param>
        /// <returns></returns>
        public static double GetBeta(
            double dblMeanLoss,
            double dblAlpha)
        {
            double dblBeta = dblAlpha*(1.0 - dblMeanLoss)/dblMeanLoss;
            if (dblBeta == 0.0)
            {
                dblBeta = SMALL_VALUE;
            }
            if (dblBeta <= 0.0)
            {
                throw new HCException("Error. Beta parameter is less or equal to zero.");
            }
            return dblBeta;
        }

        /// <summary>
        /// Save EP curve to a CSV file
        /// </summary>
        /// <param name="epCurve">Array contianing EP points.</param>
        /// <param name="strFileName">Output file name.</param>
        public static void SaveEpCurveToCsv(EpRow[] epCurve, string strFileName)
        {
            using (StreamWriter sw = new StreamWriter(strFileName))
            {
                for (int i = 0; i < epCurve.Length; i++)
                {
                    sw.WriteLine(
                        epCurve[i].LossThreshold + "," + epCurve[i].ExceedenceProbability);
                }
                sw.Close();
            }
            PrintToScreen.WriteLine("Saved file: " + strFileName);
        }

        #endregion
    }
}
