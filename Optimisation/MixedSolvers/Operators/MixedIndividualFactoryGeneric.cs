#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories;
using HC.Analytics.Optimisation.Base.Problem;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.Operators
{
    [Serializable]
    public class MixedIndividualFactoryGeneric : AbstractIndividualFactory
    {
        #region Members

        private readonly List<HeuristicProblem> m_heuristicProblemList;

        #endregion

        #region Constructors

        public MixedIndividualFactoryGeneric(
            HeuristicProblem heuristicProblem,
            List<HeuristicProblem> heuristicProblemList) :
                base(heuristicProblem)
        {
            m_heuristicProblemList = heuristicProblemList;
        }

        #endregion

        public override Individual BuildRandomIndividual()
        {
            try
            {
                var randomIndividual = new Individual(
                    null,
                    null,
                    null,
                    0,
                    m_heuristicProblem)
                                          {
                                              IndividualList = new List<Individual>()
                                          };
                var heuristicProblemList = m_heuristicProblemList;
                for (int i = 0; i < heuristicProblemList.Count; i++)
                {
                    HeuristicProblem heuristicProblem = heuristicProblemList[i];
                    var individual = heuristicProblem.IndividualFactory.
                        BuildRandomIndividual();
                    individual.ProblemName = heuristicProblem.ProblemName;

                    randomIndividual.IndividualList.Add(
                        individual);
                }
                return randomIndividual;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
            }
            return null;
        }
    }
}
