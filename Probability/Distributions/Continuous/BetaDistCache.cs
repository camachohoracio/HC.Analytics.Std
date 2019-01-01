#region

using System;
using System.Collections.Generic;
using System.Threading;
using HC.Analytics.Mathematics.Functions;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Helpers;
using HC.Core.Io;
using HC.Core.Io.Serialization;

//using HC.Core.Io;

#endregion

namespace HC.Analytics.Probability.Distributions.Continuous
{
    public class BetaDistCache : BetaDist
    {
        #region Constructors

        public BetaDistCache(
            double dblAlpha,
            double dblBeta,
            RngWrapper rng) : base(
                dblAlpha,
                dblBeta,
                rng)
        {
        }

        #endregion

        #region Memebers

        //
        // The parameters: m_intInitialIndex and  m_intInitialIndex2 are used for 
        // retrieving probabilities from the beta distribution array.
        //
        // Recall: The beta distribution is subdivided in three parts: a continuous part, 
        // a continuous-integer part and an integer part.
        // We used two bounds to split the array (INT_PARAMETER_UPPER_BOUND and 
        // INT_PARAMETER_INTEGER_UPPER_BOUND)
        //
        // If beta is > than continuous bound and alpha < than the continuous bound then 
        // look into the second part of the beta array.+
        private const int m_intInitialIndex =
            (int) ((Constants.INT_PARAMETER_UPPER_BOUND)/
                   Constants.DBL_BETA_DELTA);

        // If alpha and beta are both > than the integer bound then we look into the third part of the array
        private const int m_intInitialIndex2 = m_intInitialIndex +
                                               Constants.INT_PARAMETER_INTEGER_UPPER_BOUND;

        // beta distribution

        // own instance
        //private static BetaDist m_ownInstance = null;
        private static bool m_blnCalculateRealProb;
        private static bool m_blnLoadingData;

        private static Dictionary<double, double> m_cashedGamma =
            new Dictionary<double, double>(
                Constants.INT_GAMMA_CASHED_SIZE + 100);

        private static double[][][] m_dblBetaDistributionArr;

        private static object m_lockObject = new object();
        private static object m_lockObject2 = new object();

        #endregion

        #region Properties

        public static double[][][] BetaDistributionArr
        {
            get { return m_dblBetaDistributionArr; }
            set { m_dblBetaDistributionArr = value; }
        }

        /// <summary>
        /// Get / Set flag which defines if the real distribution is calculated
        /// </summary>
        public static bool CalculateRealProb
        {
            get { return m_blnCalculateRealProb; }
            set { m_blnCalculateRealProb = value; }
        }

        #endregion

        /// <summary>
        /// Create instance
        /// </summary>
        public static void Initialize()
        {
            if (!m_blnLoadingData)
            {
                LoadPrecompiledBeta();
            }
        }

        private static void LoadPrecompiledBeta()
        {
            if (!m_blnLoadingData && !m_blnCalculateRealProb)
            {
                m_blnLoadingData = true;
                ThreadStart job = LoadPrecompiledBeta2;
                Thread thread = new Thread(job);
                thread.Start();
            }
        }

        private static void LoadPrecompiledBeta2()
        {
            string strTargetDir = ""; // Config.GetBetaDataPath();
            string strFileName = strTargetDir + @"\" +
                                 Constants.STR_PRECOMPILED_DATA_FILE_NAME + "_" +
                                 Constants.INT_PARAMETER_UPPER_BOUND + "_" +
                                 Constants.DBL_BETA_DELTA;
            if (!DirectoryHelper.Exists(
                strTargetDir,
                false))
            {
                DirectoryHelper.CreateDirectory(strTargetDir);
            }
            if (!FileHelper.Exists(
                strFileName,
                false))
            {
                double[][][] betaDistribution = GenerateBetaData();
                Serializer.Serialize(strFileName, betaDistribution);
                m_dblBetaDistributionArr = betaDistribution;
            }
            else
            {
                // deserialize data
                PrintToScreen.WriteLine("Loading beta distribution data. Please wait...");
                DateTime startDate = DateTime.Now;
                m_dblBetaDistributionArr = DeSerialize(strFileName);
                TimeSpan ts = DateTime.Now.Subtract(startDate);
                PrintToScreen.WriteLine("Finished loading beta distribution data. Time: " + ts.TotalSeconds);
            }
        }

        //private static void Serialize(string strFileName, double[][][] betaDistribution) 
        //{
        //    if (FileHelper.Exists(strFileName))
        //    {
        //        FileHelper.Delete(strFileName);
        //    }
        //    PrintToScreen.WriteLine("Serializing data. Please wait...");
        //    Stream str = File.OpenWrite(strFileName);
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    formatter.Serialize(str, betaDistribution);
        //    str.Close();
        //    PrintToScreen.WriteLine("Finished serializing data.");
        //}

        private static double[][][] DeSerialize(string strFileName)
        {
            double[][][] betaDistribution;
            try
            {
                betaDistribution = Serializer.DeserializeFile<double[][][]>(strFileName);
            }
            catch
            {
                betaDistribution = GenerateBetaData();
                Serializer.Serialize(strFileName, betaDistribution);
            }
            return betaDistribution;
        }

        private static double[][][] GenerateBetaData()
        {
            double dblDelta = Constants.DBL_BETA_DELTA,
                   dblUpperBound = Constants.INT_PARAMETER_UPPER_BOUND,
                   dblAlpha,
                   dblBeta,
                   dblX;
            int intParameterDimensions = (int) (Math.Round(dblUpperBound/dblDelta)),
                intProbDimensions = (int) (Math.Round(1.0/dblDelta, 0)),
                intTotalCount = intParameterDimensions +
                                2*Constants.INT_PARAMETER_INTEGER_UPPER_BOUND,
                i,
                betaCount = 0;
            double[][][] betaDistribution = new double[intTotalCount][][];
            //
            // First part of the array
            // Consider values alpha and beta in [0,5]
            //
            for (i = 0; i < intParameterDimensions; i++)
            {
                betaDistribution[i] = new double[intParameterDimensions - i][];
                dblAlpha = (i + 1)*dblDelta;
                for (int j = i; j < intParameterDimensions; j++)
                {
                    dblBeta = (j + 1)*dblDelta;
                    betaDistribution[i][j - i] = new double[intProbDimensions];
                    // validation loop
                    for (int l = 0; l < intProbDimensions; l++)
                    {
                        betaDistribution[i][j - i][l] = -1;
                    }
                    for (int k = 0; k < intProbDimensions; k++)
                    {
                        dblX = (k + 1)*dblDelta;
                        betaDistribution[i][j - i][k] =
                            IncompleteBetaFunct.IncompleteBeta(dblX, dblAlpha, dblBeta);
                        betaCount++;
                    }
                }
                PrintToScreen.WriteLine("Generating beta data " + i +
                                        " of " + intTotalCount);
            }

            int arrayLength = Constants.INT_PARAMETER_INTEGER_UPPER_BOUND;
            int intSubSecondArrLength = intParameterDimensions + Constants.INT_PARAMETER_INTEGER_UPPER_BOUND;
            //
            // Second part of the array
            //
            int j2 = 0;
            int intUpperBound2 = Constants.INT_PARAMETER_UPPER_BOUND + 1;
            for (; i < intTotalCount; i++)
            {
                if (i >= intSubSecondArrLength)
                {
                    dblAlpha = i - intSubSecondArrLength + intUpperBound2;
                    if (i > 2*(intParameterDimensions))
                    {
                        arrayLength--;
                    }
                    j2 = i - 2*(intParameterDimensions);
                }
                else
                {
                    dblAlpha = dblDelta*(i - intParameterDimensions + 1);
                    j2 = 0;
                }

                betaDistribution[i] = new double[arrayLength][];

                for (int j = 0; j < arrayLength; j++, j2++)
                {
                    dblBeta = (j2 + intUpperBound2);
                    betaDistribution[i][j] = new double[intProbDimensions];
                    // validation loop
                    for (int l = 0; l < intProbDimensions; l++)
                    {
                        betaDistribution[i][j][l] = -1;
                    }

                    for (int k = 0; k < intProbDimensions; k++)
                    {
                        dblX = (k + 1)*dblDelta;
                        betaDistribution[i][j][k] =
                            IncompleteBetaFunct.IncompleteBeta(dblX, dblAlpha, dblBeta);
                        betaCount++;
                    }
                }
                PrintToScreen.WriteLine("Generating beta data " + i +
                                        " of " + intTotalCount);
            }

            // validation loop
            for (i = 0; i < betaDistribution.Length; i++)
            {
                for (int j = 0; j < betaDistribution[i].Length; j++)
                {
                    for (int k = 0; k < betaDistribution[i][j].Length; k++)
                    {
                        if (betaDistribution[i][j][k] < 0)
                        {
                            throw new HCException("Beta value is negative");
                        }
                    }
                }
            }

            PrintToScreen.WriteLine("Finished generating beta values." +
                                    " Total values generated: " + betaCount);

            //TestBetaData();

            return betaDistribution;
        }


        /// <summary>
        /// Retrieve probabilites from the beta distribution.
        /// Convert alpha, beta and loss threshold into indexes of the array.
        /// </summary>
        /// <param name="dblCurrentLoss">
        /// Loss threshold
        /// </param>
        /// <param name="dblAlpha">
        /// Alpha parameter for Beta distribution
        /// </param>
        /// <param name="dblBeta">
        /// Beta parameter for beta distribution
        /// </param>
        /// <returns></returns>
        public double DistributionFunction(
            double dblCurrentLoss)
        {
            if (!m_blnCalculateRealProb &&
                m_dblBetaDistributionArr == null)
            {
                Initialize();
            }

            if (m_blnCalculateRealProb)
            {
                return base.Cdf(dblCurrentLoss);
            }
            if (m_dblBetaDistributionArr == null)
            {
                return Cdf(dblCurrentLoss);
            }
            if (dblCurrentLoss < 0.01)
            {
                return Cdf(dblCurrentLoss);
            }
            if (Alpha < 0.01 && Beta > 5)
            {
                return Cdf(dblCurrentLoss);
            }


            //
            // check if the beta distribution is not inverted
            // i.e. p(alpha, beta, loss) = 1-p(beta, alpha, 1-loss)
            //
            bool blnInvertedBeta = false;

            int intAlphaIndex,
                intBetaIndex,
                intLossIndex;

            //
            // Alpha is set to the lowest value
            //
            if (Alpha > Beta)
            {
                double dblTmp = Alpha;
                Alpha = Beta;
                Beta = dblTmp;
                dblCurrentLoss = 1.0 - dblCurrentLoss;
                // the distribution is inverted
                blnInvertedBeta = true;
            }

            // keep original values of alpha and beta for future validation
            double dblAlpha0 = Alpha,
                   dblBeta0 = Beta,
                   dblCurrentLoss0 = dblCurrentLoss;

            //
            // normalize first part of the array
            //
            if (Math.Round(Beta, 0) ==
                Constants.INT_PARAMETER_UPPER_BOUND &&
                Beta > Constants.INT_PARAMETER_UPPER_BOUND)
            {
                // alpha is normalized accordingly
                Alpha = ((Constants.INT_PARAMETER_UPPER_BOUND)*
                         Alpha)/Beta;
                Beta = Constants.INT_PARAMETER_UPPER_BOUND;
            }
            if (Beta <= Constants.INT_PARAMETER_UPPER_BOUND)
            {
                // Round alpha and beta
                Alpha = Math.Round(Alpha, 2);
                Beta = Math.Round(Beta, 2);
                Alpha = Alpha == 0
                            ?
                                Constants.DBL_BETA_DELTA
                            :
                                Alpha;
                Beta = Beta == 0
                           ?
                               Constants.DBL_BETA_DELTA
                           :
                               Beta;
                // get indexes
                intAlphaIndex =
                    (int) (Math.Round(Alpha/
                                      Constants.DBL_BETA_DELTA, 0)) - 1;
                intBetaIndex =
                    (int) (Math.Round(Beta/
                                      Constants.DBL_BETA_DELTA, 0)) - 1;
                intBetaIndex = intBetaIndex - intAlphaIndex;
            }
            else
            {
                //
                // normalize second part of the array
                //
                // normalize alpha if beta is > integer upper bound
                // and normalize beta
                if (Beta > Constants.INT_PARAMETER_INTEGER_UPPER_BOUND + Constants.INT_PARAMETER_UPPER_BOUND)
                {
                    Alpha =
                        Constants.INT_PARAMETER_INTEGER_UPPER_BOUND
                        *(Alpha/Beta);
                    // truncate beta
                    Beta = Constants.INT_PARAMETER_INTEGER_UPPER_BOUND;
                }
                else
                {
                    // Round beta to its closest integer value
                    Beta = Math.Round(Beta, 0);
                }
                //
                // get beta index (set to an integer value)
                //
                intBetaIndex =
                    ((int) Beta) -
                    (Constants.INT_PARAMETER_UPPER_BOUND + 1);
                //
                // Round alpha
                //
                if (Alpha >
                    Constants.INT_PARAMETER_UPPER_BOUND
                    && Math.Round(Alpha, 0) ==
                       Constants.INT_PARAMETER_UPPER_BOUND)
                {
                    Alpha = Math.Round(Alpha, 0);
                }
                if (Alpha <= Constants.INT_PARAMETER_UPPER_BOUND)
                {
                    // alpha is in [0,5]
                    Alpha = Math.Round(Alpha, 2);
                    Alpha = Alpha == 0
                                ?
                                    Constants.DBL_BETA_DELTA
                                :
                                    Alpha;
                    intAlphaIndex =
                        m_intInitialIndex +
                        (int) (Math.Round(Alpha/
                                          Constants.DBL_BETA_DELTA, 0)) - 1;
                }
                else
                {
                    // alpha is set to an integer value
                    Alpha = Math.Round(Alpha, 0);
                    int intAlphaIndex0 = ((int) Alpha) -
                                         (Constants.INT_PARAMETER_UPPER_BOUND + 1);
                    intAlphaIndex =
                        m_intInitialIndex2 + intAlphaIndex0;
                    intBetaIndex = intBetaIndex - intAlphaIndex0;
                }
            }
            //
            // Round loss threshold
            //
            dblCurrentLoss = Math.Round(dblCurrentLoss, 2);
            dblCurrentLoss = dblCurrentLoss == 0
                                 ?
                                     Constants.DBL_BETA_DELTA
                                 :
                                     dblCurrentLoss;
            intLossIndex =
                (int) (Math.Round(dblCurrentLoss/
                                  Constants.DBL_BETA_DELTA)) - 1;
            //
            // do interpolation
            //
            if (double.IsNaN(Alpha))
            {
                return 0.0;
            }
            double dblProb = m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex][intLossIndex],
                   dblProb1 = 0,
                   dblCurrentDelta = 0;

            //
            // interpolate alpha
            //
            if (Alpha > dblAlpha0 && intAlphaIndex > 0)
            {
                if (m_dblBetaDistributionArr[intAlphaIndex - 1].Length > intBetaIndex)
                {
                    dblProb1 = m_dblBetaDistributionArr[intAlphaIndex - 1][intBetaIndex][intLossIndex];
                    dblCurrentDelta = Alpha - dblAlpha0;
                }
                else
                {
                    dblCurrentDelta = 0.0;
                }
            }
            else if (intAlphaIndex < (m_intInitialIndex + 2.0*Constants.INT_PARAMETER_INTEGER_UPPER_BOUND - 1))
            {
                if (m_dblBetaDistributionArr[intAlphaIndex + 1].Length < intBetaIndex)
                {
                    dblProb1 = m_dblBetaDistributionArr[intAlphaIndex + 1][intBetaIndex][intLossIndex];
                    dblCurrentDelta = dblAlpha0 - Alpha;
                }
                else
                {
                    dblCurrentDelta = 0;
                }
            }
            double dblNormalizer = 0;
            dblNormalizer = Alpha <= Constants.INT_PARAMETER_UPPER_BOUND ? Constants.DBL_BETA_DELTA : 1;
            double dblInterpolationValue = (dblProb1 - dblProb)*dblCurrentDelta/dblNormalizer;

            // return beta value if the interpolation value is too large
            if (Math.Abs(dblInterpolationValue) > 0.01)
            {
                dblProb = Cdf(dblCurrentLoss0);
                if (blnInvertedBeta)
                {
                    return 1.0 - dblProb;
                }
                return dblProb;
            }

            dblProb += dblInterpolationValue;

            //
            // interpolate beta
            //
            if (Beta > dblBeta0 && intBetaIndex > 0)
            {
                dblProb1 = m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex - 1][intLossIndex];
                dblCurrentDelta = Beta - dblBeta0;
            }
            else if (intBetaIndex < m_dblBetaDistributionArr[intAlphaIndex].Length - 1)
            {
                dblProb1 = m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex + 1][intLossIndex];
                dblCurrentDelta = dblBeta0 - Beta;
            }
            if (Beta <= Constants.INT_PARAMETER_UPPER_BOUND)
            {
                dblNormalizer = Constants.DBL_BETA_DELTA;
            }
            else
            {
                dblNormalizer = 1;
            }
            dblInterpolationValue = (dblProb1 - dblProb)*dblCurrentDelta/dblNormalizer;

            // return beta value if the interpolation value is too large
            if (Math.Abs(dblInterpolationValue) > 0.01)
            {
                dblProb = Cdf(dblCurrentLoss0);
                if (blnInvertedBeta)
                {
                    return 1.0 - dblProb;
                }
                return dblProb;
            }

            dblProb += dblInterpolationValue;

            //
            // interpolate probability
            //
            if (dblCurrentLoss > dblCurrentLoss0)
            {
                if (intLossIndex > 0)
                {
                    dblProb1 = m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex][intLossIndex - 1];
                }
                else
                {
                    dblProb1 = 0;
                }
                dblCurrentDelta = dblCurrentLoss - dblCurrentLoss0;
            }
            else if (intLossIndex < m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex].Length - 1)
            {
                dblProb1 = m_dblBetaDistributionArr[intAlphaIndex][intBetaIndex][intLossIndex + 1];
                dblCurrentDelta = dblCurrentLoss0 - dblCurrentLoss;
            }
            dblInterpolationValue = (dblProb1 - dblProb)*dblCurrentDelta/
                                    Constants.DBL_BETA_DELTA;

            // return beta value if the interpolation value is too large
            if (Math.Abs(dblInterpolationValue) > 0.01)
            {
                dblProb = Cdf(dblCurrentLoss0);
                if (blnInvertedBeta)
                {
                    return 1.0 - dblProb;
                }
                return dblProb;
            }

            dblProb += dblInterpolationValue;

            // check if the distribution is inverted
            if (blnInvertedBeta)
            {
                return 1.0 - dblProb;
            }

            // return beta value if the prob is negative
            if (dblProb < 0.0 || dblProb > 1.0)
            {
                return dblProb = Cdf(dblCurrentLoss0);
            }
            if (dblProb < 0)
            {
                throw new HCException("Negative probability.");
            }
            return dblProb;
        }
    }
}
