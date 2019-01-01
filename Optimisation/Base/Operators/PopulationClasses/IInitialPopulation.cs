#region

using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.PopulationClasses
{

    #region Delegates

    /// <summary>
    ///   Event fired when an exception is thrown when calculating
    ///   the inicital population
    /// </summary>
    /// <param name = "e">
    ///   HCException
    /// </param>
    public delegate void ExceptionOccurredEventHandler(HCException e);

    /// <summary>
    ///   Progress bar events
    /// </summary>
    /// <param name = "intProgress">
    ///   Progress value
    /// </param>
    /// <param name = "strMessage">
    ///   Progress message
    /// </param>
    public delegate void UpdateInitialPopProgressEventHandler(int intProgress, string strMessage);

    #endregion

    public interface IInitialPopulation
    {
        bool DoLocalSearch { get; set; }
        event ExceptionOccurredEventHandler ExceptionOccurred;
        event UpdateInitialPopProgressEventHandler UpdateProgress;

        void GetInitialPopulation();
    }
}
