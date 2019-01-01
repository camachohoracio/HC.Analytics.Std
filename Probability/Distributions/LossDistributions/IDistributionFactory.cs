namespace HC.Analytics.Probability.Distributions.LossDistributions
{

    #region Private

    /// <summary>
    /// Build a distribution
    /// </summary>
    public interface IDistributionFactory
    {
        /// <summary>
        /// Build  a distribution
        /// </summary>
        /// <param name="intCapacity">
        /// Capacity
        /// </param>
        /// <returns>
        /// Distribution
        /// </returns>
        IDistribution BuildDistribution();
    }

    #endregion
}
