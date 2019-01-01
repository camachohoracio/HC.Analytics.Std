#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Crossover
{
    public interface ICrossover
    {
        Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals);
    }
}
