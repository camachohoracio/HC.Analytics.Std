#region

using System;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpVariable : AbstractGpVariable
    {
        #region Constructors

        public GpVariable()
        {
        }

        public GpVariable(string name) :
            base(name)
        {
        }

        #endregion

        public override AbstractGpVariable Clone()
        {
            return new GpVariable(VariableName);
        }
    }
}
