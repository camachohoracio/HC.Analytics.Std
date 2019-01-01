#region

using System;
using HC.Analytics.Mathematics;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.GenericSolver
{
    public static class RepairChromosomeHelper
    {
        public static void FixIndividual(
            Individual individual,
            HeuristicProblem heuristicProblem,
            OptChromosomeFactory optChromosomeFactory)
        {
            double[] dblChromosomeArr = null;
            int[] intChromosomeArr = null;
            bool[] blnChromosomeArr = null;

            foreach (Individual individual1 in individual.IndividualList)
            {
                if (individual1.ContainsChromosomeBln())
                {
                    blnChromosomeArr = individual1.GetChromosomeCopyBln();
                }
                if (individual1.ContainsChromosomeDbl())
                {
                    dblChromosomeArr = individual1.GetChromosomeCopyDbl();
                }
                if (individual1.ContainsChromosomeInt())
                {
                    intChromosomeArr = individual1.GetChromosomeCopyInt();
                }
            }

            FixChromosomes(
                heuristicProblem,
                optChromosomeFactory,
                dblChromosomeArr,
                intChromosomeArr,
                blnChromosomeArr);


            foreach (Individual individual1 in individual.IndividualList)
            {
                if (individual1.ContainsChromosomeBln())
                {
                    if (blnChromosomeArr == null)
                    {
                        throw new HCException("Null chromosome");
                    }

                    for (int i = 0; i < individual1.GetChromosomeCopyBln().Length; i++)
                    {
                        individual1.SetChromosomeValueBln(i,
                                                          blnChromosomeArr[i]);
                    }
                }
                if (individual1.ContainsChromosomeDbl())
                {
                    if (dblChromosomeArr == null)
                    {
                        throw new HCException("Null chromosome");
                    }

                    for (int i = 0; i < individual1.GetChromosomeCopyDbl().Length; i++)
                    {
                        individual1.SetChromosomeValueDbl(i,
                                                          dblChromosomeArr[i]);
                    }
                }
                if (individual1.ContainsChromosomeInt())
                {
                    if (intChromosomeArr == null)
                    {
                        throw new HCException("Null chromosome");
                    }

                    for (int i = 0; i < individual1.GetChromosomeCopyInt().Length; i++)
                    {
                        individual1.SetChromosomeValueInt(i,
                                                          intChromosomeArr[i]);
                    }
                }

            }
        }

        private static void FixChromosomes(
            HeuristicProblem heuristicProblem,
            OptChromosomeFactory optChromosomeFactory,
            double[] dblChromosome,
            int[] intChromosome,
            bool[] blnChromosome)
        {
            if(heuristicProblem.Constraints != null &&
                heuristicProblem.Constraints.ListConstraints.Count > 0)
            {
                foreach (OptConstraint optConstraint in heuristicProblem.Constraints.ListConstraints)
                {
                    string strParamName1 = optConstraint.ParameterName1;
                    string strParamName2 = optConstraint.ParameterName2;
                    InequalityType inequalityType = optConstraint.Inequality;

                    if(!string.IsNullOrEmpty(strParamName1) &&
                        !string.IsNullOrEmpty(strParamName2) &&
                        ContainsChromosome(strParamName1,
                            optChromosomeFactory) &&
                        ContainsChromosome(strParamName2,
                            optChromosomeFactory))
                    {
                        double dblRawChromosomeValue1;
                        double dblChromosomeValue1 = GetChrmosomeValue(
                            strParamName1,
                            dblChromosome,
                            intChromosome,
                            blnChromosome,
                            out dblRawChromosomeValue1,
                            optChromosomeFactory);

                        double dblRawChromosomeValue2;
                        double dblChromosomeValue2 = GetChrmosomeValue(
                            strParamName2,
                            dblChromosome,
                            intChromosome,
                            blnChromosome,
                            out dblRawChromosomeValue2,
                            optChromosomeFactory);

                        bool blnIneq = MathHelper.CheckInequality(
                            inequalityType,
                            dblChromosomeValue1,
                            dblChromosomeValue2);

                        if(!blnIneq)
                        {
                            if(inequalityType == InequalityType.GREATER_THAN ||
                                inequalityType == InequalityType.LESS_THAN ||
                                inequalityType == InequalityType.LESS_OR_EQUAL ||
                                inequalityType == InequalityType.GREATER_OR_EQUAL)
                            {
                                //
                                // swap chromosomes
                                //
                                SetChromosomeValue(
                                    strParamName1,
                                    dblChromosome,
                                    intChromosome,
                                    blnChromosome,
                                    dblRawChromosomeValue2,
                                    optChromosomeFactory,
                                    heuristicProblem);
                                SetChromosomeValue(
                                    strParamName2,
                                    dblChromosome,
                                    intChromosome,
                                    blnChromosome,
                                    dblRawChromosomeValue1,
                                    optChromosomeFactory,
                                    heuristicProblem);
                            }
                            else if(inequalityType == InequalityType.EQUALS)
                            {
                                //
                                // set only one of the chromosomes
                                //
                                SetChromosomeValue(
                                    strParamName1,
                                    dblChromosome,
                                    intChromosome,
                                    blnChromosome,
                                    dblRawChromosomeValue2,
                                    optChromosomeFactory,
                                    heuristicProblem);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                }
            }
        }

        private static void SetChromosomeValue(
            string strParamName, 
            double[] dblChromosome, 
            int[] intChromosome, 
            bool[] blnChromosome, 
            double dblRawChromosomeValue, 
            OptChromosomeFactory optChromosomeFactory, 
            HeuristicProblem heuristicProblem)
        {
            if (optChromosomeFactory.OptParams.DblParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.DblParams.IndexOf(strParamName);
                dblChromosome[intIndex] = dblRawChromosomeValue;
            }
            else if (optChromosomeFactory.OptParams.IntParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.IntParams.IndexOf(strParamName);
                dblRawChromosomeValue = Math.Min(
                    heuristicProblem.VariableRangesIntegerProbl[intIndex],
                    dblRawChromosomeValue);
                intChromosome[intIndex] = (int)dblRawChromosomeValue;
            }
            if (optChromosomeFactory.OptParams.BlnParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.BlnParams.IndexOf(strParamName);
                blnChromosome[intIndex] = dblRawChromosomeValue == 1;
            }
            else
            {
                throw new HCException("Param not found: " + strParamName);
            }
        }

        private static double GetChrmosomeValue(
            string strParamName, 
            double[] dblChromosome, 
            int[] intChromosome, 
            bool[] blnChromosome, 
            out double dblRawChromosomeValue, 
            OptChromosomeFactory optChromosomeFactory)
        {
            double dblChromosomeValue;
            if(optChromosomeFactory.OptParams.DblParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.DblParams.IndexOf(strParamName);
                dblRawChromosomeValue = dblChromosome[intIndex];
                dblChromosomeValue = OptChromosomeFactory.ScaleParamDbl(
                    optChromosomeFactory.OptParams.ParamsNormalizerDbl,
                    strParamName,
                    dblRawChromosomeValue);
            }
            else if (optChromosomeFactory.OptParams.IntParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.IntParams.IndexOf(strParamName);
                dblRawChromosomeValue = intChromosome[intIndex];
                dblChromosomeValue = OptChromosomeFactory.ScaleParamInt(
                    optChromosomeFactory.OptParams.ParamsNormalizerInt,
                    strParamName,
                    (int)dblRawChromosomeValue);
            }
            else if (optChromosomeFactory.OptParams.BlnParams.Contains(strParamName))
            {
                int intIndex = optChromosomeFactory.OptParams.BlnParams.IndexOf(strParamName);
                dblRawChromosomeValue = blnChromosome[intIndex] ? 1 : 0;
                dblChromosomeValue = dblRawChromosomeValue;
            }
            else
            {
                throw new Exception("Param not found: " + strParamName);
            }
            return dblChromosomeValue;
        }

        private static bool ContainsChromosome(
            string strParamName, 
            OptChromosomeFactory optChromosomeFactory)
        {
            if(optChromosomeFactory.OptParams.DblParams.Contains(strParamName) ||
                optChromosomeFactory.OptParams.IntParams.Contains(strParamName) ||
                optChromosomeFactory.OptParams.BlnParams.Contains(strParamName))
            {
                return true;
            }
            return false;
        }
    }
}

