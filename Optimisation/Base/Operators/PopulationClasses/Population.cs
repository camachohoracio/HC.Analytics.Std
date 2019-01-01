#region

using System;
using System.Collections.Generic;
using System.Linq;
using HC.Analytics.Optimisation.Base.Clustering;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.PopulationClasses
{
    /// <summary>
    ///   Pupulation class.
    ///   Hold solutions explored by the solver. 
    ///   Each element in the population serves to the generation of new solutions for 
    ///   reproduction and repair purposes.
    /// </summary>
    [Serializable]
    public class Population : IDisposable
    {
        #region Members

        /// <summary>
        ///   Evaluates the similarity between two chromosomes.
        /// </summary>
        private readonly ChromosomeSimilarityMetric m_chromosomeSimilarityMetric;
        private readonly HeuristicProblem m_heuristicProblem;
        private readonly object m_newIndividualLockobject = new object();
        private Individual[] m_largePopulationArr;
        private int m_largePopulationSize;
        private List<Individual> m_newIndividualsList;
        private Individual[] m_populationArr;

        #endregion

        #region Properties

        public int LargePopulationSize
        {
            get
            {
                if (m_largePopulationArr == null)
                {
                    return 0;
                }
                return m_largePopulationArr.Length;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///   Default constructor.
        ///   Save solutions explored by the solver and cluster them. 
        ///   Each clustered element serves to the generation of new solutions for 
        ///   reproduction and repair purposes.
        /// </summary>
        public Population(
            HeuristicProblem heuristicProblem)
        {
            m_heuristicProblem = heuristicProblem;

            // initilaize similarity metrics
            m_chromosomeSimilarityMetric = new ChromosomeSimilarityMetric();

            InitializePopulation();
        }

        private void InitializePopulation()
        {
            if (m_heuristicProblem.PopulationSize > 0)
            {
                m_populationArr = new Individual[
                    m_heuristicProblem.PopulationSize];

                m_largePopulationSize = Math.Max(
                    2*m_heuristicProblem.PopulationSize,
                    150);


                lock (m_newIndividualLockobject)
                {
                    m_newIndividualsList = new List<Individual>(
                        2*m_largePopulationSize);
                }
                m_largePopulationArr = new Individual[m_largePopulationSize];
            }
        }

        #endregion

        #region Public

        /// <summary>
        ///   Replace the best clustered solution if the given solution is 
        ///   very similar to the best clustered solution. Otherwise,
        ///   cluster the solution.
        ///   This method avoids clustering too many similar vectors which will
        ///   cause an excesively quick convergence
        /// </summary>
        public void ClusterBestSolution(
            Individual individual,
            HeuristicProblem heuristicProblem)
        {
            if (m_largePopulationArr[0] == null)
            {
                lock (m_newIndividualLockobject)
                {
                    m_newIndividualsList.Add(individual);
                }
            }
            else
            {
                // proceed to cluster instance only if the fitness is 
                // greather than the best clustered solution
                if (individual.Fitness > m_largePopulationArr[0].Fitness)
                {
                    var individual1 = individual;
                    if (individual1.IndividualList != null &&
                        individual1.IndividualList.Count > 0)
                    {
                        individual1 = individual1.GetIndividual(
                            heuristicProblem.ProblemName);
                    }

                    var individual2 = m_largePopulationArr[0];
                    if (individual2.IndividualList != null &&
                        individual2.IndividualList.Count > 0)
                    {
                        individual2 = individual2.GetIndividual(
                            heuristicProblem.ProblemName);
                    }


                    // get the similarity between the given chromosome and
                    // the best clustered item
                    var dblSimilarityScore =
                        m_chromosomeSimilarityMetric.GetStringMetric(
                            individual2,
                            individual1);
                    //
                    // in order to avoid having too many similar individuals in the population
                    // then if the similarity between individuals is greather than 
                    // a certain threshold then replace the best solution by the new one.
                    // Otherwise, cluster the individual
                    //
                    if (dblSimilarityScore >= 0.95)
                    {
                        m_largePopulationArr[0] = individual;
                    }
                    else
                    {
                        lock (m_newIndividualLockobject)
                        {
                            m_newIndividualsList.Add(individual);
                        }
                    }
                }
            }
        }

        private static Individual GetNestedIndividual(
            Individual originalIndividual,
            Individual selectedIndividual,
            HeuristicProblem heuristicProblem)
        {
            if (selectedIndividual.IndividualList != null &&
                selectedIndividual.IndividualList.Count > 0)
            {
                var nestedIndividual =
                    selectedIndividual.GetIndividual(
                        heuristicProblem.ProblemName);

                if (nestedIndividual != null)
                {
                    return nestedIndividual;
                }
                //
                // check for nested individuals recursively
                //
                foreach (Individual currentInd in selectedIndividual.IndividualList)
                {
                    var retrievedIndividual = GetNestedIndividual(
                        originalIndividual,
                        currentInd,
                        heuristicProblem);
                    if (retrievedIndividual != null)
                    {
                        return retrievedIndividual;
                    }
                }
            }
            // check if individual is from its parent
            if (heuristicProblem.ProblemName.Equals(originalIndividual.ProblemName))
            {
                return originalIndividual;
            }

            return null;
        }

        public void AddIndividualToPopulation(
            Individual individual)
        {
            if (!individual.IsEvaluated)
            {
                throw new HCException("Error. Individual not evaluated.");
            }

            individual.SetReadOnly();
            lock (m_newIndividualLockobject)
            {
                m_newIndividualsList.Add(individual);
            }
        }

        public void SetPopulationSize()
        {
            if (m_populationArr == null ||
                m_heuristicProblem.PopulationSize !=
                m_populationArr.Length)
            {
                InitializePopulation();
            }
        }


        public Individual[] GetPopulationArr()
        {
            return (Individual[]) m_populationArr.Clone();
        }

        public void LoadPopulation()
        {
            Individual[] largePopulationArr = m_largePopulationArr;
            //
            // create a local copy of the new individuals
            //
            List<Individual> newIndividualList;
            lock (m_newIndividualLockobject)
            {
                newIndividualList = new List<Individual>(
                    (from n in m_newIndividualsList where n != null select n).ToList());
            }

            //
            // merge the new individuals with the large population
            //
            foreach (Individual individual in largePopulationArr)
            {
                if (individual != null)
                {
                    newIndividualList.Add(individual);
                }
            }
            newIndividualList.Sort();

            //
            // load population arrays
            //
            for (var i = 0;
                 i < Math.Min(newIndividualList.Count,
                              largePopulationArr.Length);
                 i++)
            {
                largePopulationArr[i] = newIndividualList[i];

                if (i < m_heuristicProblem.PopulationSize)
                {
                    m_populationArr[i] = newIndividualList[i];
                }
            }

            lock (m_newIndividualLockobject)
            {
                m_newIndividualsList = new List<Individual>(
                    largePopulationArr.Length*2);
            }
        }

        public Individual[] GetPopulationAndNewCandidates()
        {
            List<Individual> newIndividualList;
            lock (m_newIndividualLockobject)
            {
                newIndividualList = new List<Individual>(
                    m_newIndividualsList.Count*2);
                newIndividualList.AddRange(
                    m_newIndividualsList);
            }

            var intPopulation =
                m_heuristicProblem.PopulationSize;
            var intCandidateListLength = newIndividualList.Count;
            var pop = new Individual[
                m_populationArr.Length +
                intCandidateListLength];

            Array.Copy(
                m_populationArr,
                0,
                pop,
                0,
                intPopulation);

            if (intCandidateListLength > 0)
            {
                Array.Copy(
                    newIndividualList.ToArray(),
                    0,
                    pop,
                    intPopulation,
                    intCandidateListLength);
            }
            return pop;
        }

        public Individual GetIndividualFromPopulation(
            HeuristicProblem heuristicProblem,
            int intIndex)
        {
            if (m_populationArr == null)
            {
                throw new HCException("Error. Population is null.");
            }

            var selectedIndividual = m_populationArr[intIndex];
            if (selectedIndividual == null)
            {
                return null;
            }

            //
            // select individual from  given heuristic problem
            // this will allow to select individuals from the population
            //
            selectedIndividual = GetNestedIndividual(
                selectedIndividual,
                selectedIndividual,
                heuristicProblem);

            return selectedIndividual;
        }

        public Individual GetIndividualFromLargePopulation(
            HeuristicProblem heuristicProblem,
            int intIndex)
        {
            var selectedIndividual = m_largePopulationArr[intIndex];

            //
            // select individual from a given heuristic problem
            // this will allow to select nested individuals from the population
            //
            //
            if (selectedIndividual != null &&
                heuristicProblem != null)
            {
                selectedIndividual = GetNestedIndividual(
                    selectedIndividual,
                    selectedIndividual,
                    heuristicProblem);

                if (selectedIndividual == null)
                {
                    throw new HCException("Invalid nested individual");
                }
            }
            return selectedIndividual;
        }

        public void LoadPopulationMultiObjective(
            Individual[] pop,
            int[] intRanksArr)
        {
            //
            // assign ranks to temporary population array
            //
            for (var i = 0; i < pop.Length; i++)
            {
                pop[i] = pop[i].Clone(m_heuristicProblem);
                pop[i].SetFitnessValue(-intRanksArr[i]);
            }
            Array.Sort(pop);

            //
            // load individuals to population
            //
            for (var i = 0; i < m_heuristicProblem.PopulationSize; i++)
            {
                m_populationArr[i] = pop[i];
            }
            m_largePopulationArr =
                m_populationArr;
            lock (m_newIndividualLockobject)
            {
                m_newIndividualsList = new List<Individual>();
            }
        }

        #endregion

        #region Population Stats

        private double GetFitnessMean()
        {
            double dblSum = 0;
            foreach (Individual individual in m_populationArr)
            {
                if (individual == null)
                {
                    continue;
                }
                dblSum += individual.Fitness;
            }
            return dblSum/m_populationArr.Length;
        }

        public double GetFinessVarCoeff()
        {
            var dblMean = GetFitnessMean();
            var dblStdDev = GetFinessStdDev(dblMean);
            return GetFinessVarCoeff(
                dblMean,
                dblStdDev);
        }

        public double GetFinessVarCoeff(
            double dblMean,
            double dblStdDev)
        {
            return dblStdDev/dblMean;
        }

        public double GetFinessStdDev()
        {
            var dblMean = GetFitnessMean();
            var dblStdDev = GetFinessStdDev(dblMean);
            return dblStdDev;
        }

        public double GetFinessStdDev(double dblMean)
        {
            var dblStdDev = Math.Sqrt(GetFinessVariance(dblMean));
            return dblStdDev;
        }

        public double GetFinessVariance()
        {
            var dblMean = GetFitnessMean();
            return GetFinessVariance(dblMean);
        }

        public double GetFinessVariance(double dblMean)
        {
            double dblSumSq = 0;
            foreach (Individual individual in m_populationArr)
            {
                if (individual == null)
                {
                    continue;
                }
                dblSumSq +=
                    Math.Pow((individual.Fitness - dblMean), 2);
            }
            var dblVariance = dblSumSq/(m_populationArr.Length - 1.0);

            return dblVariance;
        }

        public string ToStringPopulationStats()
        {
            var dblMean = GetFitnessMean();
            var dblStdDev = GetFinessStdDev(dblMean);
            var dblVarCoeff = GetFinessVarCoeff(
                dblMean,
                dblStdDev);
            return
                "Population Stats:" +
                "\nMean = " + dblMean +
                "\nStd. Dev. = " + GetFinessStdDev(dblMean) +
                "\nCoeff. of Variation = " + dblVarCoeff;
        }

        #endregion

        ~Population()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_largePopulationArr != null)
            {
                for (int i = 0; i < m_largePopulationArr.Length; i++)
                {
                    var currInd = m_largePopulationArr[i];
                    if(currInd != null)
                    {
                        currInd.Dispose();
                    }
                }
                m_largePopulationArr = null;
            }

            if (m_newIndividualsList != null)
            {
                for (int i = 0; i < m_newIndividualsList.Count; i++)
                {
                    var currInd = m_newIndividualsList[i];
                    if (currInd != null)
                    {
                        currInd.Dispose();
                    }
                }
                m_newIndividualsList.Clear();
                //m_newIndividualsList = null; // avoid because workers may get too late here
            }

            if (m_populationArr != null)
            {
                for (int i = 0; i < m_populationArr.Length; i++)
                {
                    var currInd = m_populationArr[i];
                    if (currInd != null)
                    {
                        currInd.Dispose();
                    }
                }
                m_populationArr = null;
            }
        }
    }
}
