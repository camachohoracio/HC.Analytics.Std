#region

using System;
using System.Xml.Serialization;

#endregion

namespace HC.Analytics.Optimisation.Base.DataStructures.Gp
{
    [Serializable]
    public class GpSolution : IComparable<GpSolution>
    {
        #region Properties

        [XmlArray("XArr")]
        [XmlArrayItem("X", typeof (double))]
        public double[] XArr { get; set; }

        [XmlElement("YValue", typeof (double))]
        public double YValue { get; set; }

        #endregion

        #region Constructors

        public GpSolution()
        {
        }

        public GpSolution(double[] x)
        {
            XArr = x;
        }

        #endregion

        #region IComparable<GpSolution> Members

        public int CompareTo(GpSolution o)
        {
            if (YValue < o.YValue)
            {
                return 1;
            }
            if (YValue > o.YValue)
            {
                return -1;
            }
            return 0;
        }

        #endregion
    }
}
