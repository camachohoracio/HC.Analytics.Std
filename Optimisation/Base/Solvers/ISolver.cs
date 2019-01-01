#region

using System;
using System.Collections.Generic;
using HC.Analytics.Optimisation.Base.DataStructures;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Optimisation.Base.Solvers
{

    #region Delegates"

    /// <summary>
    ///   Event fired once the solver is finished.
    /// </summary>
    /// <param name = "resultList">
    ///   List with results
    /// </param>
    public delegate void FinishSolverDel(List<ResultRow> resultList);

    /// <summary>
    ///   Event called if an exception is thrown
    /// </summary>
    /// <param name = "e">
    ///   HCException
    /// </param>
    public delegate void SolverExceptionOccurredDel(HCException e);

    /// <summary>
    ///   Progress bar event
    /// </summary>
    /// <param name = "intProgress">
    ///   Progress value
    /// </param>
    /// <param name = "strMessage">
    ///   Progress message
    /// </param>
    public delegate void UpdateSolverProgressDel(int intProgress, string strMessage);

    #endregion

    /// <summary>
    ///   Solver interface
    /// </summary>
    public interface ISolver : IDisposable
    {
        #region Events

        /// <summary>
        ///   Event fired once the solver is finished.
        /// </summary>
        event FinishSolverDel OnSolverFinished;

        /// <summary>
        ///   Event called if an exception is thrown
        /// </summary>
        event SolverExceptionOccurredDel ExceptionOccurred;

        /// <summary>
        ///   Progress bar event
        /// </summary>
        event UpdateSolverProgressDel UpdateProgress;

        #endregion

        #region Interface methods

        /// <summary>
        ///   Solve
        /// </summary>
        void Solve();

        /// <summary>
        ///   Get solver name
        /// </summary>
        /// <returns>
        ///   Solver name
        /// </returns>
        string GetSolverName();

        void SetSolverName(string strSolverName);

        #endregion
    }
}
