#region

using System;
using System.Collections.Generic;
using HC.Analytics.Probability.Distributions.LossDistributions;

#endregion

namespace HC.Analytics.Probability.Distributions.Portfolio
{
    /// <summary>
    /// Call event once the distribution is loaded.
    /// </summary>
    /// <param name="strMessage">
    /// Message.
    /// </param>
    public delegate void LoadDistributionReadyEventHandler(string strMessage);

    /// <summary>
    /// Repository interface
    /// </summary>
    public interface IRepository : IDisposable
    {
        #region Properties

        /// <summary>
        /// Number of distributions stored in memory
        /// </summary>
        int Capacity { get; set; }

        /// <summary>
        /// Distribution factory
        /// </summary>
        IDistributionFactory DistributionFactory { get; set; }

        /// <summary>
        /// Portfolio factory
        /// </summary>
        IPortfolioFactory PortfolioFactory { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event called once the distribution is loaded
        /// </summary>
        event LoadDistributionReadyEventHandler LoadReady;

        #endregion

        #region Methods

        /// <summary>
        /// Add new job to the distribution
        /// </summary>
        /// <param name="jobAnalysis">
        /// Job used for connection with SQL
        /// </param>
        void AddDistributionJob(IJob jobAnalysis);

        /// <summary>
        /// Add a new distribution to the repository
        /// </summary>
        /// <param name="distribution">
        /// Distribution
        /// </param>
        void AddDistribution(IDistribution distribution);

        /// <summary>
        /// Delete distribution from current repository
        /// </summary>
        /// <param name="intDistributionIndex">
        /// Distribution index
        /// </param>
        void DeleteDistribution(int intDistributionIndex);

        /// <summary>
        /// Get a distribution from the whoe repository
        /// </summary>
        /// <returns></returns>
        IDistribution GetDistributionTable();

        /// <summary>
        /// Count the number of distributions stored in 
        /// memory
        /// </summary>
        /// <returns>
        /// Number of distributions stored in memory
        /// </returns>
        int CountActiveDistributions();

        /// <summary>
        /// Calculates marginal risks for each distribution stored in the
        /// repository
        /// </summary>
        /// <param name="dblReturnPeriod">
        /// Return period (years)
        /// </param>
        /// <param name="enumEpType">
        /// EP type
        /// </param>
        /// <param name="intCapacity">
        /// Capacity. Number of distributions stored in memory
        /// </param>
        /// <returns>
        /// Marginal risks
        /// </returns>
        List<RiskVariable> GetMarginalRiskList(
            double dblReturnPeriod,
            EnumEpType enumEpType,
            int intCapacity);

        /// <summary>
        /// Load distribution table into memory
        /// </summary>
        void LoadDistributionData();

        /// <summary>
        /// Get distribution from a given index
        /// </summary>
        /// <param name="intIndex">
        /// Distribution index
        /// </param>
        /// <returns>
        /// Distribution
        /// </returns>
        IDistribution GetDistribution(int intIndex);

        /// <summary>
        /// Get number of distribution included in the repository.
        /// </summary>
        /// <returns></returns>
        int GetSize();

        #endregion
    }
}
