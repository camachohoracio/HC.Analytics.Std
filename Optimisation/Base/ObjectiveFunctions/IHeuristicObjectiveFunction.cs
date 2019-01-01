#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public interface IHeuristicObjectiveFunction : IObjectiveFunction
    {
        string ObjectiveName { get; set; }

        /// <summary>
        ///   Get real return. Ignore return weights to 
        ///   calculate the return
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        /// <returns></returns>
        double Evaluate(
            Individual individual,
            HeuristicProblem heuristicProblem);
    }
}
