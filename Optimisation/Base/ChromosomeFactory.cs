#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.Base
{
    public static class ChromosomeFactory
    {
        public static void BuildRandomChromosome(
            out double[] dblChromosomeArr,
            out int[] intChromosomeArr,
            out bool[] blnChromosomeArr,
            HeuristicProblem heuristicProblem,
            RngWrapper rngWrapper)
        {
            dblChromosomeArr = null;
            intChromosomeArr = null;
            blnChromosomeArr = null;

            if (heuristicProblem.EnumOptimimisationPoblemType ==
                EnumOptimimisationPoblemType.BINARY)
            {
                blnChromosomeArr =
                    BuildRandomChromosomeBln(
                        heuristicProblem,
                        rngWrapper);
            }
            else if (heuristicProblem.EnumOptimimisationPoblemType ==
                     EnumOptimimisationPoblemType.CONTINUOUS)
            {
                dblChromosomeArr =
                    BuildRandomChromosomeDbl(
                        heuristicProblem,
                        rngWrapper);
            }
            else if (heuristicProblem.EnumOptimimisationPoblemType ==
                     EnumOptimimisationPoblemType.INTEGER)
            {
                intChromosomeArr =
                    BuildRandomChromosomeInt(
                        heuristicProblem,
                        rngWrapper);
            }
            else if (heuristicProblem.EnumOptimimisationPoblemType ==
                     EnumOptimimisationPoblemType.GENETIC_PROGRAMMING)
            {
                // do nothing
            }
            else
            {
                throw new HCException("Error. Problem type not supported.");
            }
        }

        public static double[] BuildRandomChromosomeDbl(
            HeuristicProblem heuristicProblem,
            RngWrapper rngWrapper)
        {
            try
            {
                var dblChromosomeArr = new double[
                    heuristicProblem.VariableCount];

                for (var i = 0; i < heuristicProblem.VariableCount; i++)
                {
                    double dblRng = Math.Round(rngWrapper.NextDouble(),
                                                     MathConstants.ROUND_DECIMALS);
                    dblChromosomeArr[i] = dblRng;
                }
                return dblChromosomeArr;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }

        public static bool[] BuildRandomChromosomeBln(
            HeuristicProblem heuristicProblem,
            RngWrapper rngWrapper)
        {
            var blnChromosomeArr = new bool[
                heuristicProblem.VariableCount];

            for (var i = 0; i < heuristicProblem.VariableCount; i++)
            {
                blnChromosomeArr[i] = (rngWrapper.NextInt(0, 1) == 1);
            }
            return blnChromosomeArr;
        }

        public static int[] BuildRandomChromosomeInt(
            HeuristicProblem heuristicProblem,
            RngWrapper rngWrapper)
        {
            var intChromosomeArr = new int[
                heuristicProblem.VariableCount];

            for (var i = 0; i < heuristicProblem.VariableCount; i++)
            {
                intChromosomeArr[i] = rngWrapper.NextInt(0,
                                                         (int) heuristicProblem.VariableRangesIntegerProbl[i]);
            }
            return intChromosomeArr;
        }
    }
}
