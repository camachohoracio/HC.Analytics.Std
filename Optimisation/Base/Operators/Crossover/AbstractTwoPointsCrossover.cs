#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Crossover
{
    [Serializable]
    public abstract class AbstractTwoPointsCrossover : AbstractCrossover
    {
        #region Constructor

        public AbstractTwoPointsCrossover(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override Individual DoCrossover(
            RngWrapper rng,
            Individual[] individuals)
        {
            var parent1 = individuals[0];
            var parent2 = individuals[1];

            double[] c3 = null;
            if (m_heuristicProblem.VariableCount > 0)
            {
                var c1 = GetChromosomeCopy(parent1); //chromosome parent 1
                var c2 = GetChromosomeCopy(parent2); //chromosome parent 2
                c3 = new double[m_heuristicProblem.VariableCount]; //new chromosome  

                //Generate crossing points
                var p1 = (int) (m_heuristicProblem.VariableCount*
                                rng.NextDouble()); //crossing point 1
                var p2 = (int) (m_heuristicProblem.VariableCount*
                                rng.NextDouble()); //crossing point 2
                while (p1 == p2)
                {
                    p2 = (int) (m_heuristicProblem.VariableCount*rng.NextDouble());
                }
                if (p1 > p2)
                {
                    var temp = p1;
                    p1 = p2;
                    p2 = temp;
                }

                // Copy everything between point1 and point2 from parent1, the rest from parent2        
                for (var i = 0; i < m_heuristicProblem.VariableCount; i++)
                {
                    if (i < p1)
                    {
                        c3[i] = c2[i];
                    }
                    else if (i < p2)
                    {
                        c3[i] = c1[i];
                    }
                    else
                    {
                        c3[i] = c2[i];
                    }
                }
            }

            // Create and return new individual using the new chromosome
            var individual =
                CreateIndividual(c3);

            return individual;
        }

        #region AbstractMethods

        protected abstract double GetChromosomeValue(
            Individual individual,
            int intIndex);

        protected abstract void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight);

        protected abstract double[] GetChromosomeCopy(
            Individual individual);

        protected abstract double GetMaxChromosomeValue(int intIndex);

        protected abstract Individual CreateIndividual(double[] dblChromosomeArr);

        #endregion
    }
}
