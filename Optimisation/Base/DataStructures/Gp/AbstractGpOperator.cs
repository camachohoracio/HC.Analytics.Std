#region

using System;
using System.Xml.Serialization;
using HC.Core.Reflection;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class AbstractGpOperator
    {
        private AbstractGpOperator m_instance;
        public Type TypeInnr { get; set; }

        public AbstractGpOperator()
        {
            TypeInnr = GetType();
        }

        #region Properties

        [XmlElement("NumbParameters", typeof (int))]
        public int NumbParameters { get; set; }

        #endregion

        #region Abstract Methods

        public virtual object Compute(object[] parameters)
        {
            CheckInstance();
            return m_instance.Compute(parameters);
        }

        private void CheckInstance()
        {
            if (m_instance == null)
            {
                //
                // we are in deserialize mode
                //
                m_instance = (AbstractGpOperator) ReflectorCache.GetReflector(TypeInnr).CreateInstance();
            }
        }

        public virtual string ComputeToString(string[] parameters)
        {
            CheckInstance();
            return m_instance.ComputeToString(parameters);
        }

        #endregion
    }
}
