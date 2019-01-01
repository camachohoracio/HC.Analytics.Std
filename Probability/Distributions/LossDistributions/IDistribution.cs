#region

using System;
using HC.Analytics.Probability.Distributions.LossDistributions.Ep;
using HC.Core.Resources;

//using HC.Utils.Pooling.Resources;

#endregion

namespace HC.Analytics.Probability.Distributions.LossDistributions
{
    /// <summary>
    /// Distribution interface.
    /// Holds distribution data and all the operations
    /// related with distributions.
    /// </summary>
    public interface IDistribution : IComparable<IDistribution>, IResource
    {
        #region Methods

        /// <summary>
        /// Remove distribution
        /// </summary>
        /// <param name="distribution">
        /// The distribution to remove
        /// </param>
        /// <remarks>
        /// Assume distributions are sorted by event ID
        /// </remarks>
        void RemoveDistribution(IDistribution distribution);

        /// <summary>
        /// Sort distributions by loaded date
        /// </summary>
        /// <param name="o">
        /// </param>
        /// <returns></returns>
        new int CompareTo(IDistribution o);

        /// <summary>
        /// Merge two distributions with a default weight of 1.0
        /// </summary>
        /// <param name="distribution">
        /// Distribution to be merged
        /// </param>
        void MergeDistribution(
            IDistribution distribution);

        /// <summary>
        /// Merge two distributions
        /// </summary>
        /// <param name="distribution">
        /// Distribution to be merged
        /// </param>
        /// <param name="dblWeight">
        /// Merge weight. One means that the two 
        /// distribution will be merged. O.5 means
        /// that half of the distribution will be 
        /// merged.
        /// </param>
        void MergeDistribution(
            IDistribution distribution,
            double dblWeight);

        /// <summary>
        /// Merge distribution
        /// </summary>
        /// <param name="distribution">
        /// Distribution to be merged
        /// </param>
        /// <param name="dblWeight">
        /// Merge weight</param>
        /// <param name="dblStdDevWeight">
        /// Std. Deviation Weight
        /// </param>
        void MergeDistribution(
            IDistribution distribution,
            double dblWeight,
            double dblStdDevWeight);

        /// <summary>
        /// Calculates the AAL (Average Annual Loss)
        /// </summary>
        /// <returns>AAL</returns>
        double GetAal();

        /// <summary>
        /// Calculates the EP curve.
        /// </summary>
        /// <param name="enumEpType">
        /// The EP type. Either AEP or OEP.
        /// </param>
        /// <returns>Array of EP pints.</returns>
        EpRow[] GetEpCurve(
            EnumEpType enumEpType);

        /// <summary>
        /// Calculates an Exceedance probability point.
        /// </summary>
        /// <param name="dblLossThreshold">
        /// Loss threshold.
        /// </param>
        /// <param name="enumEpType">
        /// Type of EP (Either AEP or OEP)
        /// </param>
        /// <returns>EP value</returns>
        double GetEp(
            double dblLossThreshold,
            EnumEpType enumEpType);

        /// <summary>
        /// Calculates the RPL (Return Period Loss) from a
        /// given return period (Years).
        /// </summary>
        /// <param name="dblReturnPeriod">
        /// Return period (Years)
        /// </param>
        /// <param name="enumEpType">
        /// Type of EP (AEP or OEP)
        /// </param>
        /// <returns>RPL value</returns>
        double GetPnl(
            double dblReturnPeriod,
            EnumEpType enumEpType);

        /// <summary>
        /// Calculate RPL (Return Period Loss) given a 
        /// return period (years).
        /// </summary>
        /// <param name="dblReturnPeriod">
        /// return period (years)
        /// </param>
        /// <param name="enumEpType">
        /// EP type (Either AEP or OEP)
        /// </param>
        /// <param name="dblPrecision">
        /// RPL precision.
        /// </param>
        /// <param name="intIterations">
        /// Binary search iterations
        /// </param>
        /// <returns>
        /// RPL value
        /// </returns>
        double GetPnl(double dblReturnPeriod,
                      EnumEpType enumEpType,
                      double dblPrecision,
                      int intIterations);

        /// <summary>
        /// Get Standard Deviation value.
        /// </summary>
        /// <returns>
        /// Std. Dev. Value
        /// </returns>
        double GetStdDev();

        /// <summary>
        /// Get the correlation between the current and 
        /// the provided distributions.
        /// </summary>
        /// <param name="distribution">
        /// Distribution
        /// </param>
        /// <returns>
        /// Correlation value
        /// </returns>
        double GetCorrelation(IDistribution distribution);

        /// <summary>
        /// Calculates the TCE (Tail Conditional Expectation) probability
        /// given a loss threshold and a return period (Years)
        /// </summary>
        /// <param name="dblLossThreshold">
        /// Loss threshold
        /// </param>
        /// <param name="dblReturnPeriod">
        /// Return period (years)
        /// </param>
        /// <returns>
        /// TCE value
        /// </returns>
        double GetTvar(double dblReturnPeriod, EnumEpType enumEpType);

        /// <summary>
        /// Check is the current distribution contains data stored 
        /// in the memory. 
        /// 
        /// In some cases, the header and descriptions are in memory, but
        /// the distribution table may not.
        /// </summary>
        /// <returns>
        /// true if the disitrubution is in memory, 
        /// false otherwise.
        /// </returns>
        bool ContainsDistribution();


        /// <summary>
        /// Write current distribution into a CSV file.
        /// </summary>
        /// <param name="strFileName">
        /// File name.
        /// </param>
        void WriteToCsv(string strFileName);

        /// <summary>
        /// Remove distribution table from memory
        /// </summary>
        void RemoveDistributionData();

        /// <summary>
        /// Clone current disitrubion
        /// </summary>
        /// <returns></returns>
        IDistribution Clone();

        #endregion
    }
}
