#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Probability.Random;
using HC.Core.Exceptions;
using HC.Core.SearchUtils;

#endregion

namespace HC.Analytics.Optimisation.Integer.Operators
{
    [Serializable]
    public class GuidedConvergenceInt : AbstractGuidedConvergence
    {
        #region Members

        private double[][][] m_dblProbArr;

        #endregion

        #region Constructors

        /// <summary>
        ///   Constructor
        /// </summary>
        public GuidedConvergenceInt(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_dblProbArr = new double[m_intVariableCount][][];
        }

        #endregion

        public override void UpdateGcProbabilities(
            HeuristicProblem heuristicProblem)
        {
            try
            {
                // dictionary will cound the number of integer values per variable
                var variableCounterArr =
                    new Dictionary<int, int>[m_intVariableCount];

                var intPopulationSize = GetPopulationSize();

                for (var j = 0; j < m_intVariableCount; j++)
                {
                    variableCounterArr[j] =
                        new Dictionary<int, int>(intPopulationSize + 1);
                }


                for (var i = 0; i < intPopulationSize; i++)
                {
                    for (var j = 0; j < m_intVariableCount; j++)
                    {
                        var selectedIndividual = m_heuristicProblem.Population.
                            GetIndividualFromLargePopulation(
                                m_heuristicProblem,
                                i);

                        var intValue = selectedIndividual.GetChromosomeValueInt(j);

                        if (!variableCounterArr[j].ContainsKey(intValue))
                        {
                            variableCounterArr[j].Add(intValue, 1);
                        }
                        else
                        {
                            //
                            // increase counter for variable
                            //
                            int intCount = variableCounterArr[j][intValue];
							intCount++;
							variableCounterArr[j][intValue] = intCount;
                        }
                    }
                }

                lock (m_dblProbArr)
                {
                    try
                    {
                        var dblTmpProbArr = new double[m_intVariableCount][][];
                        m_gcProbabilityArray = new double[m_intVariableCount];
                        double dblSumKeyValues = 0;
                        for (var j = 0; j < m_intVariableCount; j++)
                        {
                            dblTmpProbArr[j] = new double[2][];
                            dblTmpProbArr[j][0] = new double[variableCounterArr[j].Count];
                            dblTmpProbArr[j][1] = new double[variableCounterArr[j].Count];

                            var intI = 0;
                            double dblAccumProb = 0;
                            foreach (KeyValuePair<int, int> kvp in variableCounterArr[j])
                            {
                                var intKeyValue = kvp.Key;
                                dblSumKeyValues += intKeyValue;
                                m_gcProbabilityArray[j] += intKeyValue;
                                dblTmpProbArr[j][0][intI] = intKeyValue;
                                dblTmpProbArr[j][1][intI] = dblAccumProb;
                                dblAccumProb += kvp.Value/(double) intPopulationSize;
                                intI++;
                            }

                            if (Math.Round(dblAccumProb, 0) != 1.0)
                            {
                                throw new HCException("Error. Total weight is not 1.0");
                            }
                        }
                        m_dblProbArr = dblTmpProbArr;

                        double dblTotalProb = 0;
                        for (var j = 0; j < m_intVariableCount; j++)
                        {
                            var dblProb = m_gcProbabilityArray[j]/dblSumKeyValues;
                            m_gcProbabilityArray[j] = dblProb;
                            dblTotalProb += dblProb;
                        }

                        if(double.IsNaN(dblTotalProb))
                        {
                            dblTotalProb = 0;
                        }

                        if (Math.Round(dblTotalProb, 0) != 1.0)
                        {
                            //Debugger.Break();
                            throw new HCException("Error. prob[" + dblTotalProb +
                                "] not = 1");
                        }
                    }
                    catch (Exception e)
                    {
                        //Debugger.Break();
                        throw;
                    }
                }
            }
            catch (Exception e2)
            {
                //Debugger.Break();
                throw;
            }
        }

        private int GetPopulationSize()
        {
            if (m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                m_heuristicProblem,
                m_heuristicProblem.Population.LargePopulationSize - 1) != null)
            {
                return m_heuristicProblem.Population.LargePopulationSize;
            }
            for (var i = 0; i < m_heuristicProblem.Population.LargePopulationSize; i++)
            {
                if (m_heuristicProblem.Population.GetIndividualFromLargePopulation(
                    m_heuristicProblem,
                    m_heuristicProblem.Population.LargePopulationSize - 1) == null)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        public override double DrawGuidedConvergenceValue(
            int intIndex,
            RngWrapper rng)
        {
            var dblRandom = rng.NextDouble();

            if (m_dblProbArr[intIndex][1] == null)
            {
                //Debugger.Break();
            }

            var intRandomIndex = SearchUtilsClass.DoBinarySearch(
                m_dblProbArr[intIndex][1],
                dblRandom);
            return m_dblProbArr[intIndex][0][intRandomIndex];
        }

        protected override void InitializeGcProbabilities()
        {
            if (CheckPopulation())
            {
                UpdateGcProbabilities(m_heuristicProblem);
            }
        }
    }
}
