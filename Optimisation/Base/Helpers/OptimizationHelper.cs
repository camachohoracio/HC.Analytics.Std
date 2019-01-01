#region

using System;
using System.IO;
using System.Text;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Probability.Distributions.LossDistributions;
using HC.Analytics.Probability.Distributions.Portfolio;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Base.Helpers
{
    /// <summary>
    ///   Optimisation helper class.
    ///   Note: This class it not threadsafe.
    /// </summary>
    public static class OptimizationHelper
    {
        #region Public

        //public static bool CompareTwoIndividuals(
        //    IIndividual individual1,
        //    IIndividual individual2)
        //{
        //    if (CompareTwoChromosomes(
        //        individual1,
        //        individual2))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //public static bool CompareTwoChromosomes(
        //    IIndividual individual1,
        //    IIndividual individual2)
        //{
        //    for (int i = 0; i < individual1.HeuristicOptimizationProblem_.VariableCount; i++)
        //    {
        //        if (individual1.GetChromosomeValue(i) != 
        //            individual2.GetChromosomeValue(i))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        /// <summary>
        ///   compare two individuals. Check if the two chromosomes are equal
        /// </summary>
        /// <param name = "individual1">
        ///   IIndividual 1
        /// </param>
        /// <param name = "individual2">
        ///   IIndividual 2
        /// </param>
        /// <returns></returns>
        /// <summary>
        ///   Compare two chrosomes and return true if the 
        ///   two chromosomes are equal
        /// </summary>
        /// <param name = "dblChromosome1">
        ///   Chromosome 1
        /// </param>
        /// <param name = "dblChromosome2">
        ///   Chromosome 2
        /// </param>
        /// <returns></returns>
        /// <summary>
        ///   Get result row. Method called when optimizer finish working
        /// </summary>
        /// <param name = "dblChromosomeArray">
        ///   Chromosome
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
        /// <param name = "clusterRow">
        ///   IIndividual
        /// </param>
        /// <param name = "reproduction">
        ///   Reproduction
        /// </param>
        /// <param name = "blnGetFinalResults">
        ///   Get final results
        /// </param>
        /// <param name = "repository">
        ///   Repository
        /// </param>
        /// <param name = "baseDistribution">
        ///   Base distribution
        /// </param>
        /// <param name = "dblReturnPeriod">
        ///   Return period
        /// </param>
        /// <param name = "epType">
        ///   EP type
        /// </param>
        /// <param name = "dblBaseRisk">
        ///   Base risk
        /// </param>
        /// <param name = "repairIndividual">
        ///   Repair solution
        /// </param>
        /// <param name = "localSearch">
        ///   Local search
        /// </param>
        /// <returns></returns>
        public static ResultRow GetResultRow(
            double[] dblChromosomeArray,
            bool blnGetFinalResults,
            Individual clusterRow)
        {
            return null;
        }

        /// <summary>
        ///   Get a random number distributed normally
        /// </summary>
        /// <returns>
        ///   Normal random number
        /// </returns>
        public static double Noise(
            RngWrapper rng)
        {
            double fac, rsq, v1, v2;

            do
            {
                v1 = 2.0*rng.NextDouble() - 1.0;
                v2 = 2.0*rng.NextDouble() - 1.0;
                rsq = v1*v1 + v2*v2;
            } while ((rsq >= 1.0) || (rsq == 0.0));

            fac = Math.Sqrt(-2.0*Math.Log(rsq)/rsq);
            return v2*fac;
        }

        /// <summary>
        ///   Save all the marginal risks in a text file
        /// </summary>
        /// <param name = "enumEpType">
        ///   EP type
        /// </param>
        /// <param name = "dblReturnPeriod">
        ///   Return period
        /// </param>
        /// <param name = "repository">
        ///   Repository
        /// </param>
        /// <param name = "intCapacity">
        ///   Repository capacity
        /// </param>
        /// <param name = "baseDistribution">
        ///   Base distribution
        /// </param>
        public static void SaveMarginalRisks(
            EnumEpType enumEpType,
            double dblReturnPeriod,
            IRepository repository,
            int intCapacity,
            IDistribution baseDistribution)
        {
            PrintToScreen.WriteLine("Saving marginal risks. Please wait...");
            var strMarginalRiskFileName = "marginalRisks.csv";
            var sw = new StreamWriter(strMarginalRiskFileName);
            sw.WriteLine(ToStringMarginalRisks(
                enumEpType,
                dblReturnPeriod,
                repository,
                intCapacity,
                baseDistribution));
            sw.Close();
            PrintToScreen.WriteLine("Saved file: " + strMarginalRiskFileName);
        }

        /// <summary>
        ///   Return a string description of the marginal risks
        /// </summary>
        /// <param name = "enumEpType">
        ///   EP type
        /// </param>
        /// <param name = "dblReturnPeriod">
        ///   Return period
        /// </param>
        /// <param name = "repository">
        ///   Repository
        /// </param>
        /// <param name = "intCapacity">
        ///   Repository capacity
        /// </param>
        /// <param name = "baseElt">
        ///   Base ETL
        /// </param>
        /// <returns>
        ///   String description of marginal risks
        /// </returns>
        public static string ToStringMarginalRisks(
            EnumEpType enumEpType,
            double dblReturnPeriod,
            IRepository repository,
            int intCapacity,
            IDistribution baseElt)
        {
            var sb = new StringBuilder();
            var riskList = repository.GetMarginalRiskList(
                dblReturnPeriod,
                enumEpType,
                intCapacity);

            double dblTotalRisk;
            double dblBaseRisk = 0;
            double dblMarginalRisk;
            double dblStandaloneRisk;
            double dblAal;
            double dblStdDeviation;
            int intIndex;

            sb.Append(Environment.NewLine + "IIndividual marginal Risks:" +
                      Environment.NewLine);
            sb.Append("solver_id,rdm_Id,Description,MarginalRisk,TotalRisk,BaseRisk,StandaloneRisk,AAL,Premium,stdDev" +
                      Environment.NewLine);

            var lossDistributionAll =
                repository.GetDistributionTable();
            if (baseElt != null)
            {
                dblBaseRisk = baseElt.GetPnl(dblReturnPeriod, enumEpType);
                lossDistributionAll.MergeDistribution(baseElt, 1.0);
            }

            dblTotalRisk = lossDistributionAll.GetPnl(dblReturnPeriod, enumEpType);

            for (var i = 0; i < repository.GetSize(); i++)
            {
                intIndex = riskList[i].Index;
                dblAal = repository.GetDistribution(intIndex).GetAal();
                dblStdDeviation = repository.GetDistribution(intIndex).GetStdDev();
                dblStandaloneRisk = riskList[i].ContributorialRisk;
                dblMarginalRisk = riskList[i].MarginalRisk;
                sb.Append(intIndex + "," +
                          "" + "," +
                          "" + "," +
                          dblMarginalRisk + "," +
                          dblTotalRisk + "," +
                          dblBaseRisk + "," +
                          dblStandaloneRisk + "," +
                          dblAal + "," +
                          0.0 + "," +
                          //repository.GetDistribution(i).TotalPremium + "," +
                          dblStdDeviation + Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        ///   Get number of iterations performed by the GA solver
        /// </summary>
        /// <param name = "intCapacity">
        ///   Repository capacity
        /// </param>
        /// <returns>
        ///   Number of iterations
        /// </returns>
        public static int GetHeuristicSolverIterations(int intVariables)
        {
            var iterationsPerHundredPolicies = 10000;
            if (intVariables <= 10)
            {
                return 200;
            }
            var intIterations = (intVariables*iterationsPerHundredPolicies)/100;
            return intIterations > 20000 ? 20000 : intIterations;
        }

        /// <summary>
        ///   Get the number of iterations to be performed by a solver
        /// </summary>
        /// <param name = "intCapacity">
        ///   Repository capacity
        /// </param>
        /// <param name = "zeroOneCount">
        ///   Number of zeros/ones in an initial solution set.
        ///   Provide an insight of the combinatorial level of the current problem
        /// </param>
        /// <returns>
        ///   Number of iterations
        /// </returns>
        public static int GetSolverIterations(int intCapacity, int zeroOneCount)
        {
            // return the number of iterations according to how difficult the problem is.
            // if the problem contains, on average, the half amount of ones and zeros
            // then run the full amount of iteratios
            var intIterations = GetHeuristicSolverIterations(intCapacity);
            return Math.Max((zeroOneCount*intIterations)/(intCapacity/2), 200);
        }

        public static int GetMsIterations(int intProblemSize, int intCapacity)
        {
            if (intCapacity >= intProblemSize)
            {
                return 1;
            }
            var intPercentage = 100 - (intCapacity*100)/intProblemSize;
            intPercentage = intPercentage*5/40;
            return intPercentage > 3 ? 3 : intPercentage;
        }


        /// <summary>
        ///   Get a string reperesentation of a given chromosome
        /// </summary>
        /// <param name = "dblChromosomeArray">
        ///   Chromosome array
        /// </param>
        /// <returns>
        ///   String representation of a given chromosome
        /// </returns>
        public static string GetChromosomeString(
            double[] dblChromosomeArray)
        {
            var sb = new StringBuilder();
            if (dblChromosomeArray != null)
            {
                sb.Append("|| ");
                for (var i = 0; i < dblChromosomeArray.Length; i++)
                {
                    sb.Append(dblChromosomeArray[i] + " ");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///   Get a chromosome array from a give string
        /// </summary>
        /// <param name = "strCandidate">
        ///   String description
        /// </param>
        /// <returns></returns>
        public static double[] GetChromosomeArray(string strCandidate)
        {
            var chromosome = new double[strCandidate.Length];

            for (var i = 0; i < strCandidate.Length; i++)
            {
                if (strCandidate[i].Equals('1'))
                {
                    chromosome[i] = 1;
                }
            }
            return chromosome;
        }

        #endregion

        #region Private

        /// <summary>
        ///   Check if two vectors are identical
        /// </summary>
        /// <param name = "intVector1">
        ///   Vector 1
        /// </param>
        /// <param name = "intVector2">
        ///   Vector 2
        /// </param>
        /// <returns></returns>
        private static bool CompareTwoVectors(double[] intVector1, double[] intVector2)
        {
            for (var i = 0; i < intVector1.Length; i++)
            {
                if (intVector1[i] != intVector2[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
