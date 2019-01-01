#region

using System;
using HC.Core.Reflection;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class AbstractNodeEvaluator
    {
        private AbstractNodeEvaluator m_instance;
        public Type TypeInnr { get; set; }

        public AbstractNodeEvaluator()
        {
            TypeInnr = GetType();
        }

        public virtual object EvaluateVarNode(
            AbstractGpVariable gpVariable,
            AbstractGpNode gpOperatorNode)
        {
            CheckInstance();
            return m_instance.EvaluateVarNode(gpVariable, gpOperatorNode);
        }

        private void CheckInstance()
        {
            if (m_instance == null)
            {
                //
                // we are in deserialize mode
                //
                m_instance = (AbstractNodeEvaluator)ReflectorCache.GetReflector(TypeInnr).CreateInstance();
            }
        }
    }
}
