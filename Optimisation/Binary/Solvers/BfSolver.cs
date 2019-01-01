#region

using System;
using System.Collections.Generic;
using System.IO;
using HC.Analytics.Optimisation.Base.Constraints;
using HC.Analytics.Optimisation.Base.Helpers;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Analytics.Optimisation.Base.Solvers;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Binary.Solvers
{
    /// <summary>
    ///   Brute-force solver.
    ///   Evaluate all possible combination of solutions.
    /// </summary>
    public class BfSolver : AbstractHeuristicSolver
    {
        #region Members

        /// <summary>
        ///   Number of solutions to be explored
        /// </summary>
        private readonly int m_intN;

        /// <summary>
        ///   Solutions to be explored
        /// </summary>
        private readonly List<string> m_listCandidateSolutions;

        #endregion

        #region Private

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "repository">
        ///   Distribution repository
        /// </param>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <param name = "constraints">
        ///   Constraints
        /// </param>
        /// <param name = "cc">
        ///   Cluster
        /// </param>
        public BfSolver(HeuristicProblem heuristicProblem) :
            base(heuristicProblem)
        {
            m_strSolverName = "Brute Force";
            // progress bar percentages
            // this should sum up to 100
            InitialCompletionPercentage = 5;
            PercentageCompletionValue = 45;
            m_intFinalResultsPercentage = 50;

            m_listCandidateSolutions = new List<string>();
            m_intN = -1;
            PrintToScreen.WriteLine("Adding solutions to candidate list");
            GetCandidateList("", 0);
            PrintToScreen.WriteLine("Finish adding solutions to candidate list");

            // set an unlimited limit
            foreach (IConstraint constraint in 
                heuristicProblem.Constraints.ListConstraints)
            {
                constraint.Boundary = double.MaxValue;
            }
        }

        #endregion

        #region Public

        /// <summary>
        ///   Solve
        /// </summary>
        public override void Solve()
        {
            var sw = new StreamWriter("bruteForceSolution.csv");
            var swReport = new StreamWriter("bruteForceReport.csv");
            var startTime = DateTime.Now;

            double[] currentChromosome;
            Individual currentIndividual;
            var strCurrentChromosome = "";
            var strBestChromosome = "";
            double
                dblCurrentReturn,
                dblCurrentRisk,
                dblBestReturn = -1,
                dblBestRisk = -1;
            int intCandidateCounter = 1,
                intTotalCandidates = m_listCandidateSolutions.Count;

            // iterate each possible solution
            foreach (string strSolution in m_listCandidateSolutions)
            {
                currentChromosome = OptimizationHelper.GetChromosomeArray(strSolution);
                currentIndividual =
                    new Individual(
                        currentChromosome,
                        HeuristicProblem);


                // evaluate individual
                currentIndividual.Evaluate(false,
                                           true,
                                           true,
                                           HeuristicProblem);

                dblCurrentReturn = currentIndividual.Fitness;

                dblCurrentRisk = 0;
                if (dblCurrentReturn > dblBestReturn)
                {
                    BestFitness = dblCurrentReturn;
                    dblBestRisk = dblCurrentRisk;
                    strBestChromosome = strCurrentChromosome;
                    m_bestIndividual = currentIndividual;
                }

                // schema: chromosome | return | risk
                dblCurrentReturn = HeuristicProblem.ObjectiveFunction.Evaluate(
                    currentIndividual,
                    HeuristicProblem);

                sw.WriteLine(strCurrentChromosome + "," +
                             dblCurrentReturn + "," +
                             dblCurrentRisk);


                var intPercentage = (100*intCandidateCounter)/intTotalCandidates;
                intPercentage = InitialCompletionPercentage + ((intPercentage*PercentageCompletionValue)/100);

                var strMessage = "Adding candidate " + intCandidateCounter + " of " +
                                 intTotalCandidates + ". Completed = " + intPercentage;

                PrintToScreen.WriteLine(strMessage);

                UpdateProgressBar(intPercentage, strMessage);

                intCandidateCounter++;
            }
            sw.Close();

            var totalTime = ((DateTime.Now - startTime)).TotalSeconds;
            swReport.WriteLine("Total time (secs): " + totalTime);
            swReport.WriteLine("Solutions explored: " + m_listCandidateSolutions.Count);
            swReport.WriteLine("Best solution: " + strBestChromosome);
            swReport.WriteLine("Risk: " + dblBestRisk);
            swReport.WriteLine("Return: " +
                               HeuristicProblem.ObjectiveFunction.Evaluate(
                                   m_bestIndividual,
                                   HeuristicProblem));
            swReport.Close();
        }

        #endregion

        #region Private

        /// <summary>
        ///   Get a list of canditaes to be explored by the solver
        /// </summary>
        /// <param name = "output">
        ///   Output string
        /// </param>
        /// <param name = "level">
        ///   IIndividual level
        /// </param>
        private void GetCandidateList(string output, int level)
        {
            level++;
            if (level == m_intN + 1)
            {
                m_listCandidateSolutions.Add(output);
                return;
            }
            var output1 = output + "0";
            GetCandidateList(output1, level);

            var output2 = output + "1";
            GetCandidateList(output2, level);
        }

        #endregion
    }
}
