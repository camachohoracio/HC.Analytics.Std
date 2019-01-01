#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.Mutation
{
    public static class DeMutationHelper
    {
        /// <summary>
        ///   In order to avoid convergence into a local optimal,
        ///   allow the mutation factor to take random values when
        ///   the mutation is very close to zero
        /// </summary>
        /// <param name = "rng"></param>
        /// <param name = "dblMutation"></param>
        /// <returns></returns>
        public static double ValidateMutationFactor(
            RngWrapper rng,
            double dblMutation)
        {
            // validate mutation factor
            if (Math.Abs(dblMutation) <=
                MathConstants.DBL_ROUNDING_FACTOR)
            {
                double dblSymbol;
                if (dblMutation == 0)
                {
                    dblSymbol = 1.0;
                }
                else
                {
                    dblSymbol = dblMutation/Math.Abs(dblMutation);
                }
                if (rng.NextDouble() >=
                    OptimisationConstants.DBL_FORCE_MUATION)
                {
                    dblMutation = dblSymbol*
                                  OptimisationConstants.DBL_MUATION_FACTOR;
                }
                else
                {
                    if (rng.NextDouble() >=
                        OptimisationConstants.DBL_FORCE_MUATION)
                    {
                        dblMutation = dblSymbol*
                                      MathConstants.DBL_ROUNDING_FACTOR;
                    }
                }
            }
            return dblMutation;
        }
    }
}
