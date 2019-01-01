#region

using System;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Continuous.Operators.Mutation;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators.Crossover
{
    [Serializable]
    public class DeCrossoverDbl : AbstractCrossover
    {
        #region Members

        private const double CR = 0.5; // Crossover probability
        private const double DE_CR_RATE = 0.5;

        #endregion

        #region Constructors

        public DeCrossoverDbl(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
        }

        #endregion

        /// <summary>
        ///   Receives four solutions and returns a new one generated through
        ///   differential evoluation crossover.
        /// </summary>
        /// <param name = "individuals"></param>
        /// <returns></returns>
        public override Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals)
        {
            /* Representations of the parents */
            var p0 = individuals[0].GetChromosomeCopyDbl();
            var p1 = individuals[1].GetChromosomeCopyDbl();
            var p2 = individuals[2].GetChromosomeCopyDbl();
            var p3 = individuals[3].GetChromosomeCopyDbl();

            var nVar = p0.Length;

            var offspring = new double[nVar];
            /* random in [0, nVar - 1] */
            var iRand = rng.NextInt(nVar);

            for (var i = 0; i < nVar; i++)
            {
                /* differential crossover */
                if (rng.NextDouble() < CR || i == iRand)
                {
                    var dblMutation = p3[i] - p2[i];
                    dblMutation = DeMutationHelper.ValidateMutationFactor(
                        rng,
                        dblMutation);

                    offspring[i] = p1[i] + DE_CR_RATE*dblMutation;
                }
                else
                {
                    offspring[i] = p0[i];
                }

                /*
                 * Validate crossover
                 */
                if (offspring[i] > 1.0)
                {
                    offspring[i] = 1.0;
                }

                if (offspring[i] < 0.0)
                {
                    offspring[i] = 0.0;
                }
            }

            return new Individual(
                offspring,
                m_heuristicProblem);
        }
    }
}
