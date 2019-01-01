#region

using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.LowerBound;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Optimisation.Continuous.Operators
{
    /// <summary>
    ///   Lower bound class.
    ///   Calculates a solution which serves as a base point to a given solver
    /// </summary>
    public class LowerBoundDbl : AbstractLowerBound
    {
        public LowerBoundDbl(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #region Public

        /// <summary>
        ///   Standard lower bound method. Loads a list of variables 
        ///   ranked by reward/risk ratio.
        ///   Adds one by one of the ranked variables 
        ///   to the solution until the solution is out of bounds.
        /// </summary>
        /// <param name = "objectiveFunction">
        ///   Objective function
        /// </param>
        /// <param name = "m_heuristicProblem.Constraints">
        ///   Constraints
        /// </param>
        /// <param name = "cc">
        ///   Cluster
        /// </param>
        /// <param name = "repository">
        ///   Repository
        /// </param>
        /// <param name = "baseDistribution">
        ///   Base distribution
        /// </param>
        /// <param name = "repairIndividual">
        ///   Repair operator
        /// </param>
        /// <param name = "localSearch">
        ///   Local search operator
        /// </param>
        /// <returns>
        ///   Lower bound individual
        /// </returns>
        public override Individual GetLowerBound()
        {
            var listVariables = GetLowerBoundList();
            listVariables.Sort();
            var dblChromosomeArray =
                new double[
                    m_heuristicProblem.VariableCount];
            var individual =
                new Individual(
                    dblChromosomeArray,
                    m_heuristicProblem);
            // add variables
            foreach (VariableContribution variable in listVariables)
            {
                // do binary search to add each variable
                var dblLowValue = 0.0;
                var dblHighValue = 1.0;
                // initialize binary search

                individual.SetChromosomeValueDbl(variable.Index, dblHighValue);

                if (!m_heuristicProblem.CheckConstraints(individual))
                {
                    // go to mid value
                    var dblMidValue = (dblHighValue - dblLowValue)/2.0;
                    individual.SetChromosomeValueDbl(variable.Index, dblMidValue);

                    var intBinarySeachIterations = 200;
                    // iterate binary search algorithm
                    for (var i = 0; i < intBinarySeachIterations; i++)
                    {
                        if (!m_heuristicProblem.CheckConstraints(individual))
                        {
                            // move back in the search
                            dblHighValue = dblMidValue;
                        }
                        else
                        {
                            // move forward in the search
                            dblLowValue = dblMidValue;
                        }
                        dblMidValue = (dblHighValue - dblLowValue)/2.0;
                        var dblWeight =
                            dblMidValue -
                            individual.GetChromosomeValueDbl(variable.Index);
                        if (dblWeight > 0)
                        {
                        }
                        else
                        {
                        }

                        individual.AddChromosomeValueDbl(
                            variable.Index,
                            dblWeight);
                    }
                    // check that individual satisfy constraint, otherwise, 
                    // remove variable
                    if (!m_heuristicProblem.CheckConstraints(individual))
                    {
                        individual.SetChromosomeValueDbl(variable.Index, 0);
                    }
                }
            }

            // create a new individual based on the lower bound
            var dblChromosomeArray2 =
                individual.GetChromosomeCopyDbl();
            var individual2 =
                new Individual(
                    dblChromosomeArray2,
                    m_heuristicProblem);

            individual2.Evaluate(true,
                                 true,
                                 true,
                                 m_heuristicProblem);
            var m_dblLowerBound = individual2.Fitness;
            PrintToScreen.WriteLine("Lower bound = " + m_dblLowerBound);

            return
                new Individual(
                    dblChromosomeArray,
                    m_heuristicProblem);
        }

        #endregion
    }
}
