#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LowerBound
{
    public interface ILowerBound
    {
        Individual GetLowerBound();
    }
}
