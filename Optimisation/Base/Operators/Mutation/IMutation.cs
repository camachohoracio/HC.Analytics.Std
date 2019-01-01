#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Mutation
{
    public interface IMutation
    {
        Individual DoMutation(
            Individual individual);
    }
}
