#region

using System;
using HC.Analytics.Optimisation.Base.Problem;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.IndividualClasses.IndividualFactories
{
    [Serializable]
    public class IndividualFactory : AbstractIndividualFactory
    {
        #region Constructors

        public IndividualFactory(
            HeuristicProblem heuristicProblem) :
                base(heuristicProblem)
        {
        }

        #endregion

        public override Individual BuildRandomIndividual()
        {
            return new Individual(
                m_heuristicProblem);
        }
    }
}
