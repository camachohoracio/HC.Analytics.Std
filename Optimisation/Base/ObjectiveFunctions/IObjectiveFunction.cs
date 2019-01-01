using System;

namespace HC.Analytics.Optimisation.Base.ObjectiveFunctions
{
    public interface IObjectiveFunction : IDisposable
    {
        #region Properties

        ObjectiveFunctionType ObjectiveFunctionType { get; }

        #endregion

        /// <summary>
        ///   Count the number of variables
        /// </summary>
        /// <returns></returns>
        int VariableCount { get; }

        double Evaluate();

        /// <summary>
        ///   Get the description of a given variable
        /// </summary>
        /// <param name = "intIndex">
        ///   Variable index
        /// </param>
        /// <returns>
        ///   Variable description
        /// </returns>
        string GetVariableDescription(int intIndex);
    }
}
