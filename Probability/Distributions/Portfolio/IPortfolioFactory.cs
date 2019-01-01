#region

using HC.Analytics.Probability.Distributions.LossDistributions;

#endregion

namespace HC.Analytics.Probability.Distributions.Portfolio
{
    public interface IPortfolioFactory
    {
        #region Methods

        IPortfolio BuildPortfolio(
            IRepository repository,
            IDistribution baseDistribution);

        IPortfolio BuildPortfolio(
            double[] dblChromosomeArray,
            IRepository repository,
            IDistribution baseDistribution);

        #endregion
    }
}
