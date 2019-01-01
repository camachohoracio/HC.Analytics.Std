using System;
using HC.Core.Io.Serialization.Interfaces;

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public class FunctionLabel : ASerializable
    {
        #region Properties

        public double Rotate { get; set; }
        public double X { get; set; }
        
        /// <summary>
        /// Note: Leave setter exposed since we need to serialize it!
        /// </summary>
        public double Y { get; set; }
        
        /// <summary>
        /// Note: Leave setter exposed since we need to serialize it!
        /// </summary>
        public string Label { get; set; }

        #endregion

        /// <summary>
        /// Used for serialization
        /// </summary>
        public FunctionLabel(){}

        public FunctionLabel(
            double dblX,
            double dblY,
            string strLabel)
        {
            X = dblX;
            Y = dblY;
            Label = strLabel;
        }

    }
}
