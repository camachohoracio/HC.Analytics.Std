#region

using System;
using HC.Analytics.Optimisation.Base.Operators.IndividualClasses;

#endregion

namespace HC.Analytics.Optimisation.Base.Operators.LocalSearch
{

    #region Events

    /// <summary>
    ///   Update progress bar
    /// </summary>
    /// <param name = "intProgress">
    ///   Progress
    /// </param>
    /// <param name = "strMessage">
    ///   Messagte
    /// </param>
    public delegate void UpdateLsProgressEventHandler(
        int intProgress,
        string strMessage);

    /// <summary>
    ///   Fire if event if local search find an improvement
    /// </summary>
    public delegate void LsImprovementFound();

    #endregion

    /// <summary>
    ///   Local serach.
    ///   Find better solutions in the neighbourhood of a 
    ///   given individual
    /// </summary>
    public interface ILocalSearch : IDisposable
    {
        #region Events

        /// <summary>
        ///   Update progress bar
        /// </summary>
        event UpdateLsProgressEventHandler UpdateProgressEvent;

        /// <summary>
        ///   Call this event if an improvement is found
        /// </summary>
        event LsImprovementFound ImprovementFoundEvent;

        #endregion

        #region Properties

        /// <summary>
        ///   Likelihood of selecting current reproduction operator
        /// </summary>
        double LocalSearchProb { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Do local search
        /// </summary>
        /// <param name = "individual">
        ///   IIndividual
        /// </param>
        void DoLocalSearch(Individual individual);

        #endregion
    }
}
