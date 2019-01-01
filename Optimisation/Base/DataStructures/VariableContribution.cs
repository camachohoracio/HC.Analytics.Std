#region

using System;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures
{
    /// <summary>
    ///   Hold a variable and its contribution.
    ///   In some cases it is important to sort variables 
    ///   by their contribution to the objective function.
    /// 
    ///   Note: This class is not threadsafe.
    /// </summary>
    public class VariableContribution :
        IComparable<VariableContribution>,
        IEquatable<VariableContribution>
    {
        #region Properties

        /// <summary>
        ///   Contribution value
        /// </summary>
        public double Contribution { get; set; }

        /// <summary>
        ///   Variable index
        /// </summary>
        public int Index { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name = "intIndex">
        ///   Variable index
        /// </param>
        /// <param name = "dblContribution">
        ///   Contribution value
        /// </param>
        public VariableContribution(
            int intIndex,
            double dblContribution)
        {
            Index = intIndex;
            Contribution = dblContribution;
        }

        #endregion

        #region Public

        #region IComparable<VariableContribution> Members

        /// <summary>
        ///   Sort object by their contribution.
        /// </summary>
        /// <param name = "obj">
        ///   Object to compare with.
        /// </param>
        /// <returns>
        ///   Compare value.
        /// </returns>
        public int CompareTo(VariableContribution obj)
        {
            var Compare = obj;
            var difference = Contribution - Compare.Contribution;
            if (difference < 0)
            {
                return 1;
            }
            if (difference > 0)
            {
                return -1;
            }
            difference = Index - Compare.Index;
            if (difference < 0)
            {
                return 1;
            }
            if (difference > 0)
            {
                return -1;
            }
            return 0;
        }

        #endregion

        #region IEquatable<VariableContribution> Members

        /// <summary>
        ///   Check if a variable is equals by 
        ///   comparing their indexes.
        /// </summary>
        /// <param name = "obj">
        ///   Object to compare with
        /// </param>
        /// <returns>
        ///   True if the two objects are equal. Zero otherwise.
        /// </returns>
        public bool Equals(VariableContribution obj)
        {
            return obj.Index == Index;
        }

        #endregion

        #endregion
    }
}
