#region

using HC.Analytics.Probability.Distributions.LossDistributions;
using HC.Analytics.Probability.Distributions.Portfolio;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public interface IHeuristicStatisticalObjectiveFunction : IHeuristicObjectiveFunction
    {
        #region Properties

        /// <summary>
        ///   Base distribution
        /// </summary>
        IDistribution BaseDistribution { get; set; }

        /// <summary>
        ///   Distribution repository
        /// </summary>
        IRepository Repository { get; set; }

        /// <summary>
        ///   EP type (Either OEP or AEP)
        /// </summary>
        EnumEpType EnumEpType { get; set; }

        #endregion
    }
}
