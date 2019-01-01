namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories
{
    public interface IIndividualFactory
    {
        /// <summary>
        ///   Build a random individual
        /// </summary>
        /// <returns></returns>
        Individual BuildRandomIndividual();
    }
}
