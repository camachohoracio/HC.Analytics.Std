#region

using HC.Analytics.Optimisation.Base;
using HC.Analytics.Optimisation.Base.DataStructures.Gp;
using HC.Analytics.Optimisation.MixedSolvers.DummyObjectiveFunctions;
using HC.Analytics.Optimisation.ProblemFactories;

#endregion

namespace HC.Analytics.Optimisation.MixedSolvers.DummyProblemFactories
{
    public class HeuristicProblFactDummy : AHeuristicProblemFactory
    {
        #region Constructors

        public HeuristicProblFactDummy(
            EnumOptimimisationPoblemType enumOptimimisationPoblemType,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpBridge gpBridge) :
            this(enumOptimimisationPoblemType,
            gpOperatorsContainer,
            gpBridge,
            false)
        {
            
        }

        public HeuristicProblFactDummy(
            EnumOptimimisationPoblemType enumOptimimisationPoblemType,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpBridge gpBridge,
            bool blnDoPublish) :
                this(
                enumOptimimisationPoblemType,
                0,
                gpOperatorsContainer,
                gpBridge,
                blnDoPublish)
        {
        }

        public HeuristicProblFactDummy(
            EnumOptimimisationPoblemType enumOptimimisationPoblemType,
            int intVariableCount) :
                this(
                enumOptimimisationPoblemType,
                intVariableCount,
                null,
                null,
                false)
        {
        }

        public HeuristicProblFactDummy(
            EnumOptimimisationPoblemType enumOptimimisationPoblemType,
            int intVariableCount,
            GpOperatorsContainer gpOperatorsContainer,
            AbstractGpBridge gpBridge,
            bool blnDoPublish) :
                base(
                enumOptimimisationPoblemType,
                new ObjectiveFunctionDummy(intVariableCount),
                gpOperatorsContainer,
                gpBridge,
                blnDoPublish)
        {
        }

        #endregion
    }
}
