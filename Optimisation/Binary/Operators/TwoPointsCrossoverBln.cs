#region

using System;
using HC.Analytics.Optimisation.Base.Operators.Crossover;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Binary.Operators
{
    [Serializable]
    public class TwoPointsCrossoverBln : AbstractTwoPointsCrossover
    {
        #region Constructor

        public TwoPointsCrossoverBln(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        #region Protected methods

        protected override double GetChromosomeValue(
            Individual individual,
            int intIndex)
        {
            return individual.GetChromosomeValueBln(intIndex) ? 1 : 0;
        }

        protected override void AddChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveChromosomeValue(
            Individual individual,
            int intIndex,
            double dblWeight)
        {
            throw new NotImplementedException();
        }

        protected override double[] GetChromosomeCopy(
            Individual individual)
        {
            var blnChromosomeArr = individual.GetChromosomeCopyBln();
            var dblChromosomeArr = new double[blnChromosomeArr.Length];


            for (var i = 0; i < blnChromosomeArr.Length; i++)
            {
                dblChromosomeArr[i] = blnChromosomeArr[i] ? 1 : 0;
            }

            return dblChromosomeArr;
        }

        protected override double GetMaxChromosomeValue(int intIndex)
        {
            return 1.0;
        }

        protected override Individual CreateIndividual(double[] dblChromosomeArr)
        {
            var blnChromosomeArr = new bool[dblChromosomeArr.Length];
            for (var i = 0; i < dblChromosomeArr.Length; i++)
            {
                blnChromosomeArr[i] = Math.Round(dblChromosomeArr[i], 0) > 0;
            }

            return new Individual(blnChromosomeArr,
                                  m_heuristicProblem);
        }

        #endregion
    }
}
