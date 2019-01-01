#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Optimisation.Base.ObjectiveFunctions;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.MultiObjective
{
    [Serializable]
    public class MultiObjectiveRanking
    {
        #region Constants

        private const double M_DBL_KAPPA = 0.05; // recommended value
        private const double M_DBL_Z = 3.5; // reference point
        private const double OBJECTIVE_FACTOR = 0.9;
        private const double RANGE_FACTOR = 0.1;
        private const double VALID_RANK_PROPORTION = 0.3;
        private const int VALID_COUNT = 3;

        #endregion

        #region Members

        private readonly HeuristicProblem m_heuristicProblem;
        private readonly bool m_blnRepairRangeRank;
        private readonly List<IHeuristicObjectiveFunction> m_objectives;
        private readonly bool m_blnRepairEquialityRank;

        #endregion

        #region Constructor

        public MultiObjectiveRanking(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;
            m_objectives = ((HeuristicMultiObjectiveFunction)m_heuristicProblem.ObjectiveFunction).ObjectiveFunctions;

            var constrainedObjectives = from n in m_objectives
                                        where (n as ARangeConstrainedObjFunc) != null
                                        select n;

            m_blnRepairRangeRank = constrainedObjectives.Any();

            constrainedObjectives = from n in m_objectives
                                    where (n as AEqualityConstrainObjFunc) != null
                                    select n;

            m_blnRepairEquialityRank = constrainedObjectives.Any();
        }

        #endregion

        public void Rank()
        {
            var intObjectiveCount =
                m_heuristicProblem.ObjectiveCount;
            //
            // load temporary population list
            //
            Individual[] populationArr =
                m_heuristicProblem.Population.GetPopulationAndNewCandidates();

            var objs = new double[populationArr.Length][];
            for (int i = 0; i < populationArr.Length; i++)
            {
                objs[i] = populationArr[i].GetFitnessArrCopy();
            }
            int[] ranks = GetRanks(intObjectiveCount, objs);
            //
            // Step 4. Repair ranks based on constrained objectives
            //
            ranks = RepairRangeRanks(intObjectiveCount, populationArr, ranks);
            ranks = RepairEqualityRanks(intObjectiveCount, populationArr, ranks);

            //
            // Step 4. Assign rankings to individuals
            //
            m_heuristicProblem.Population.LoadPopulationMultiObjective(
                populationArr,
                ranks);
        }

        private int[] RepairEqualityRanks(
            int intObjectives,
            Individual[] populationArr,
            int[] ranks)
        {
            if (!m_blnRepairEquialityRank)
            {
                return ranks;
            }

            var rankObjs = new RankObj[populationArr.Length];
            bool blnValidRank = false;
            for (int i = 0; i < populationArr.Length; i++)
            {
                Individual individual = populationArr[i];
                bool blnIsValid = true;
                var objs = new double[intObjectives];
                for (int j = 0; j < intObjectives; j++)
                {
                    objs[j] = individual.GetFitnesValue(j);
                    var constrObj = m_objectives[j] as AEqualityConstrainObjFunc;
                    if (constrObj != null)
                    {
                        if (!constrObj.CheckConstraint(individual))
                        {
                            blnIsValid = false;
                            break;
                        }
                    }
                }
                rankObjs[i] = new RankObj
                {
                    Index = i,
                    IsValid = blnIsValid,
                    Rank = ranks[i],
                    Objs = objs,
                };
                if (blnIsValid)
                {
                    blnValidRank = true;
                }
            }
            if (blnValidRank)
            {
                //
                // rank valid
                //
                var validRanked = (from n in rankObjs
                                   where n.IsValid
                                   select n).ToList();
                validRanked.Sort((x, y) => x.Rank.CompareTo(y.Rank));
                int i = 0;
                for (; i < validRanked.Count; i++)
                {
                    RankObj rankObj = validRanked[i];
                    ranks[rankObj.Index] = i + 1;
                }

                //
                // rank non-valid
                //
                var nonValidRanked = (from n in rankObjs
                                      where !n.IsValid
                                      select n).ToList();
                nonValidRanked.Sort((x, y) => x.Rank.CompareTo(y.Rank));
                for (int j = 0; j < nonValidRanked.Count; j++)
                {
                    ranks[nonValidRanked[j].Index] = i + 1;
                    i++;
                }

                //
                // check proportion of ranks
                //
                CheckValidRankProportion(intObjectives, rankObjs, validRanked);
            }

            return ranks;
        }

        private void CheckValidRankProportion(
            int intObjectives,
            RankObj[] rankObjs,
            List<RankObj> validRanked)
        {
            double dblValidRankProportion = (validRanked.Count * 1.0) / rankObjs.Length;
            if (dblValidRankProportion > VALID_RANK_PROPORTION)
            {
                //
                // we can reduce the search space
                //
                var objFunction = (HeuristicMultiObjectiveFunction)m_heuristicProblem.ObjectiveFunction;
                for (int j = 0; j < intObjectives; j++)
                {
                    IHeuristicObjectiveFunction heuristicObjectiveFunction = objFunction.ObjectiveFunctions[j];
                    AEqualityConstrainObjFunc currEqObj;
                    if ((currEqObj = heuristicObjectiveFunction as AEqualityConstrainObjFunc) != null)
                    {
                        if (currEqObj.LowValue == currEqObj.HighValue)
                        {
                            continue;
                        }
                        //
                        // get min and max obj values
                        //
                        var allObjValues = (from n in rankObjs
                                         select n.Objs[j]).ToList();
                        double dblMaxObjectiveAll = allObjValues.Max();
                        double dblMinObjectiveAll = allObjValues.Min();
                        
                        List<double> objValues = (from n in validRanked
                                         select n.Objs[j]).ToList();
                        
                        double dblMaxObjective = objValues.Max();
                        double dblMinObjective = objValues.Min();
                        double dblMinObj = Math.Max(
                            currEqObj.LowValue,
                            dblMinObjective);
                        double dblMaxObj = Math.Min(
                            currEqObj.HighValue,
                            dblMaxObjective);
                        double dblHalfRange = (dblMaxObj - dblMinObj) / 2.0;
                        double dblObjRange = OBJECTIVE_FACTOR * dblHalfRange;
                        double dblLowValue = currEqObj.TargetValue - dblObjRange;
                        double dblHighValue = currEqObj.TargetValue + dblObjRange;

                        int intValidCount = (from n in objValues
                                             where n >= dblLowValue &&
                                                   n <= dblHighValue
                                             select n).Count();

                        bool blnUpdateValues = false;
                        if (intValidCount > VALID_COUNT)
                        {
                            //
                            // do not set new values if there are no valid
                            // solutions for the new area
                            //
                            currEqObj.LowValue = Math.Max(dblLowValue,
                                                          currEqObj.LowValue);
                            currEqObj.HighValue = Math.Min(dblHighValue,
                                                           currEqObj.HighValue);
                            blnUpdateValues = true;
                        }
                        else
                        {
                            //
                            // check if we are moving in the right direction
                            //
                            double dblObjectiveRangeFactor = (dblMaxObjective - dblMinObjective) * RANGE_FACTOR;
                            if (dblLowValue > currEqObj.TargetValue)
                            {
                                if (currEqObj.HighValue > dblMaxObjectiveAll)
                                {
                                    currEqObj.HighValue = dblMaxObjectiveAll;
                                }
                                currEqObj.HighValue = currEqObj.HighValue - dblObjectiveRangeFactor;
                                blnUpdateValues = true;
                            }
                            else if (dblHighValue < currEqObj.TargetValue)
                            {
                                if (currEqObj.LowValue < dblMinObjectiveAll)
                                {
                                    currEqObj.LowValue = dblMinObjectiveAll;
                                }
                                currEqObj.LowValue = currEqObj.LowValue - dblObjectiveRangeFactor;
                                blnUpdateValues = true;
                            }
                        }

                        if(blnUpdateValues)
                        {
                            m_heuristicProblem.Solver.OptiGuiHelper.PublishLog(
                                "EqualityConstraint [" +
                                currEqObj.ObjectiveName + "], low["+
                                currEqObj.LowValue + "], high["+
                                currEqObj.HighValue + "], target[" +
                                currEqObj.TargetValue + "]");
                        }

                        if (currEqObj.LowValue > currEqObj.HighValue ||
                            currEqObj.LowValue == currEqObj.HighValue)
                        {
                            throw new HCException("Invalid min/max values");
                        }
                    }
                }
            }
        }

        private int[] RepairRangeRanks(
            int intObjectives,
            Individual[] populationArr,
            int[] ranks)
        {
            if (!m_blnRepairRangeRank)
            {
                return ranks;
            }

            var rankObjs = new RankObj[populationArr.Length];
            bool blnValidRank = false;
            for (int i = 0; i < populationArr.Length; i++)
            {
                Individual individual = populationArr[i];
                bool blnIsValid = true;
                for (int j = 0; j < intObjectives; j++)
                {
                    var constrObj = m_objectives[j] as ARangeConstrainedObjFunc;
                    if (constrObj != null)
                    {
                        if (!constrObj.CheckConstraint(
                            individual,
                            m_heuristicProblem))
                        {
                            blnIsValid = false;
                            break;
                        }
                    }
                }
                rankObjs[i] = new RankObj
                {
                    Index = i,
                    IsValid = blnIsValid,
                    Rank = ranks[i],
                };
                if (blnIsValid)
                {
                    blnValidRank = true;
                }
            }
            if (blnValidRank)
            {
                //
                // rank valid
                //
                var ranked = (from n in rankObjs
                              where n.IsValid
                              select n).ToList();
                ranked.Sort((x, y) => x.Rank.CompareTo(y.Rank));
                int i = 0;
                for (; i < ranked.Count; i++)
                {
                    RankObj rankObj = ranked[i];
                    ranks[rankObj.Index] = i + 1;
                }

                //
                // rank non-valid
                //
                var nonRanked = (from n in rankObjs
                                 where !n.IsValid
                                 select n).ToList();
                nonRanked.Sort((x, y) => x.Rank.CompareTo(y.Rank));
                for (int j = 0; j < nonRanked.Count; j++)
                {
                    ranks[nonRanked[j].Index] = i + 1;
                    i++;
                }
            }
            return ranks;
        }

        public static int[] GetRanks(int nObj, double[][] pop)
        {
            var intPopSize = pop.Length;
            //
            // Step 1. Calculate epsilon values for each pair of individuals
            //
            double m_dblC;
            var indicatorArr = CalculateDominateIndicatorValues(pop, nObj, out m_dblC);
            //
            // Step 2. Add epsilon values for each individual
            //
            var epsilonArr = new double[intPopSize];
            for (var i = 0; i < intPopSize; i++)
            {
                for (var j = 0; j < intPopSize; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    //
                    // The exponential amplifies influence of dominating individuals
                    //
                    epsilonArr[i] -= Math.Exp(-indicatorArr[i, j] / M_DBL_KAPPA * m_dblC);
                }
            }
            //
            // Step 3. Calculate rankings
            //
            var ranks = new int[intPopSize];
            for (var i = 0; i < intPopSize; i++)
            {
                var worst = Min(epsilonArr);
                ranks[worst] = intPopSize - i;

                // assign very large value to worst so that 
                // is no longer considered
                epsilonArr[worst] = Double.MaxValue;

                // update values of individuals in list
                for (var j = 0; j < intPopSize; j++)
                {
                    // ignore worst
                    if (epsilonArr[j] == Double.MaxValue)
                    {
                        continue;
                    }

                    epsilonArr[j] += Math.Exp(-indicatorArr[j, worst] / M_DBL_KAPPA * m_dblC);
                }
            }
            return ranks;
        }

        /// <summary>
        ///   Returns the index of the minimum value in array
        /// </summary>
        /// <param name = "array"></param>
        private static int Min(double[] array)
        {
            var min = 0;
            for (var i = 1; i < array.Length; i++)
            {
                min = array[i] < array[min] ? i : min;
            }
            return min;
        }

        /// <summary>
        ///   calculates indicator values for each pair of individuals
        /// </summary>
        private static double[,] CalculateDominateIndicatorValues(
            double[][] objs,
            int nObj,
            out double dblC)
        {
            var normalisedObj = NormaliseObjectives(objs, nObj);
            dblC = 0.01; // to prevent a value of 0, if all individuals are the same
            var N = normalisedObj.Length;
            var E = new double[N, N];
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    E[i, j] = HVolume(normalisedObj[j], normalisedObj[i], nObj);
                    dblC = Math.Abs(E[i, j]) > dblC ? Math.Abs(E[i, j]) : dblC;
                }
            }
            return E;
        }

        /// <summary>
        ///   Calculates the size of the region that is dominated by b and not
        ///   dominated by a. Let c be the region dominated by a and b, i.e. c is such
        ///   that c[i] = Max{a[i], b[i]}. The area dominated by a and b is, H(a,b) =
        ///   HV(a) + HV(b) - HV(c). Therefore the area dominated by b, but not a is
        ///   HV(a,b)- HV(a) = HV(b) - HV(c).
        /// </summary>
        /// <param name = "a"></param>
        /// <param name = "b"></param>
        /// <param name = "nObj"></param>
        private static double HVolume(double[] a, double[] b, int nObj)
        {
            //Step 1. determine vector c
            var c = new double[nObj];
            for (var i = 0; i < nObj; i++)
            {
                c[i] = b[i] > a[i] ? b[i] : a[i];
            }

            //Step 2. calculate hypervolume of b and c
            double HVb = 1, HVc = 1;
            for (var i = 0; i < nObj; i++)
            {
                HVb *= (M_DBL_Z - b[i]);
                HVc *= (M_DBL_Z - c[i]);
            }

            //Step 3. calculate and return final value
            return HVb - HVc;
        }

        /// <summary>
        ///   Normalises the individual function values to the [0,1] range
        /// </summary>
        /// <param name = "objs"></param>
        /// <param name = "nObj"></param>
        private static double[][] NormaliseObjectives(
            double[][] objs, 
            int nObj)
        {
            var popLength = objs.Length;
            var max = GetNadir(objs, nObj);
            var min = GetUtopia(objs, nObj);
            var normalisedObj = new double[popLength][];
            //double[] F;

            for (var j = 0; j < popLength; j++)
            {
                //F = objs[j].FitnessArr;
                normalisedObj[j] = new double[nObj];
                for (var i = 0; i < nObj; i++)
                {
                    normalisedObj[j][i] = (objs[j][i] - min[i]) /
                                        (max[i] - min[i] + 1.0E-100);
                }
            }
            return normalisedObj;
        }

        /// <summary>
        ///   Returns nadir point, which is the point X such that
        ///   x_i is the maximum of all the x_i values in all solutions
        ///   in the population
        /// </summary>
        /// <param name = "objs"></param>
        /// <param name = "nObj"></param>
        private static double[] GetNadir(double[][] objs, int nObj)
        {
            var nadir = (double[])objs[0].Clone();
            for (var j = 1; j < objs.Length; j++)
            {
                for (var i = 0; i < nObj; i++)
                {
                    if (objs[j][i] > nadir[i])
                    {
                        nadir[i] = objs[j][i];
                    }
                }
            }
            return nadir;
        }

        /// <summary>
        ///   Returns nadir point, which is the point X such that
        ///   x_i is the minimum of all the x_i values in all solutions
        ///   in the population
        /// </summary>
        /// <param name = "objs"></param>
        /// <param name = "nObj"></param>
        private static double[] GetUtopia(double[][] objs, int nObj)
        {
            var utopia = (double[])objs[0].Clone();
            for (var j = 1; j < objs.Length; j++)
            {
                for (var i = 0; i < nObj; i++)
                {
                    if (objs[j][i] < utopia[i])
                    {
                        utopia[i] = objs[j][i];
                    }
                }
            }
            return utopia;
        }
    }
}
