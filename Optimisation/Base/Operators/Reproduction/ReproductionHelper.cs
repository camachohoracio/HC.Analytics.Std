namespace HC.Analytics.Optimisation.Base.Operators.Reproduction
{
    /// <summary>
    ///   Reproduction helper class
    /// </summary>
    public static class ReproductionHelper
    {
        #region Public

        /// <summary>
        /// Cluster instance
        /// </summary>
        /// <param name="individual">
        /// IIndividual
        /// </param>
        /// <param name="convergenceList">
        /// Convergence list
        /// </param>
        //public static void ClusterInstance(
        //    IIndividual individual,
        //    IIndividual[] convergenceList)
        //{
        //    double dblWeight = individual.Fitness;
        //    IIndividual newVector = 
        //        individual.HeuristicOptimizationProblem_.IndividualFactory_.BuildIndividual(
        //            individual.GetChromosomeCopy());

        //    //if (convergenceList.ContainsKey(newVector))
        //    //{
        //    //    // current element is already clustered
        //    //    return;
        //    //}

        //    //lock (convergenceList)
        //    //{
        //    //    if (convergenceList.Count > 1000)
        //    //    {
        //    //        double dblWorstFitness = convergenceList.Keys[convergenceList.Count - 1].Fitness;
        //    //        if (dblWorstFitness < dblWeight)
        //    //        {
        //    //            lock (convergenceList)
        //    //            {
        //    //                convergenceList.Keys[convergenceList.Count - 1] = newVector;
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        lock (convergenceList)
        //    //        {
        //    //            convergenceList.Add(newVector, null);
        //    //        }
        //    //    }
        //    //}
        //}

        #endregion
    }
}
