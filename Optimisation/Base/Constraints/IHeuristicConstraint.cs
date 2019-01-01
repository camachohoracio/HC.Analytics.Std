#region

using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;

#endregion

namespace HC.Analytics.Optimisation.Base.Constraints
{
    public interface IHeuristicConstraint : IConstraint
    {
        /// <summary>
        ///   Variables are ranked by the constribution given
        ///   by evaluating the constraint.
        /// </summary>
        /// <returns></returns>
        List<VariableContribution> GetRankList();
    }
}
