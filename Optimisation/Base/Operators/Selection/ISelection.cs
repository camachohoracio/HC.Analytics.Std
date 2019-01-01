#region

using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.Selection
{
    public interface ISelection
    {
        Individual DoSelection();
    }
}
