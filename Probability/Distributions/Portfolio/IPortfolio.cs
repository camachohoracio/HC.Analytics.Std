#region

using HC.Analytics.Probability.Distributions.LossDistributions;

#endregion

namespace HC.Analytics.Probability.Distributions.Portfolio
{
    public interface IPortfolio
    {
        #region Properties

        double[] DistributionValidationArray { get; set; }

        IDistribution BaseDistribution { get; set; }

        IDistribution Distribution { get; set; }

        #endregion

        #region Interface Methods

        string ToStringPortfolioSummary();

        string ToStringMarginalRisks(
            EnumEpType enumEpType,
            double dblReturnPeriod);

        IDistribution GetDistribution();

        void RemoveAll();

        void AddDistributionIndexQueue(int intDistributionIndex);

        void AddDistributionIndexQueue(int intDistributionIndex, double dblWeight);

        void RemoveDistributionIndexQueue(int intDistributionIndex);

        void RemoveDistributionIndexQueue(
            int intDistributionIndex,
            double dblWeight);
        
        void RemoveFromBasePortfolio(
            IDistribution baseDistribution,
            IDistribution deletedDistribution);

        void AddToBasePortfolio(
            IDistribution baseDistribution,
            IDistribution addDistribution);

        bool AddNewDistributions();

        void RemoveDistributions(int intDistributionIndex);

        IPortfolio Clone();

        void RunAnalysis();

        #endregion
    }
}
