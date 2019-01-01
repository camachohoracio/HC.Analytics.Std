#region

using HC.Analytics.Mathematics.LinearAlgebra;

#endregion

namespace HC.Analytics.Analysis
{
    public class VectorExtended : Vector
    {
        #region Constructors

        public VectorExtended(double[] dblArr)
            : base(dblArr)
        {
        }

        public VectorExtended(int intDimensions)
            : base(intDimensions)
        {
        }

        #endregion

        public void Convolve(
            VectorExtended vectorExtended)
        {
            FourierTransform fourierTransform1 =
                new FourierTransform(
                    GetArr());
            FourierTransform fourierTransform2 =
                new FourierTransform(
                    vectorExtended.GetArr());
            FourierTransform fourierTransform3 =
                FourierTransform.Convolve(
                    fourierTransform1,
                    fourierTransform2);
            SetArr(
                fourierTransform3.GetAlternateInputData());
        }
    }
}
