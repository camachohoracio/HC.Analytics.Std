using System;
using System.Collections.Generic;
using System.Linq;

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    public class FunctionFromData3D : AFunction3D
    {
        #region Properties

        public List<FunctionRow3D> FunctionData { get; set; }

        #endregion

        #region Constructors

        public FunctionFromData3D(
            string strFunctionName,
            List<FunctionRow3D> functionRow3Ds)
        {
            FunctionName = strFunctionName;
            FunctionData = functionRow3Ds;
            SetFunctionLimits();
        }

        #endregion

        public override void SetFunctionLimits()
        {
            if (FunctionData.Count == 0)
            {
                return;
            }

            var xResult = from n in FunctionData select n.X;
            var YResult = from n in FunctionData select n.Y;
            var ZResult = from n in FunctionData select n.Z;

            XMin = 1.5 * xResult.Min();
            XMax = 1.5 * xResult.Max();
            YMin = 1.5 * YResult.Min();
            YMax = 1.5 * YResult.Max();
            ZMin = 1.5 * ZResult.Min();
            ZMax = 1.5 * ZResult.Max();
        }

        public override double EvaluateFunction(double dblX, double dblY)
        {
            //
            // this should not be called
            //
            throw new NotImplementedException();
        }

        public override List<FunctionRow3D> LoadDataArr()
        {
            return FunctionData;
        }

        public void Add(FunctionRow3D functionRow2D)
        {
            FunctionData.Add(functionRow2D);
        }
    }
}

