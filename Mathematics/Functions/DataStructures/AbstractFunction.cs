#region

using System;
using HC.Core.Exceptions;

#endregion

namespace HC.Analytics.Mathematics.Functions.DataStructures
{
    [Serializable]
    public abstract class AFunction
    {
        #region Members

        private bool m_blnHide;
        private bool m_blnShowBars;
        private bool m_blnShowBottom;
        private bool m_blnShowLinePoints;
        private bool m_blnShowPoints;
        private bool m_blnShowSolidLine;

        /// <summary>
        /// Default x and y values
        /// </summary>
        private double m_dblDefaultXMax;
        private double m_dblDefaultXMin;
        private double m_dblDefaultYMax;
        private double m_dblDefaultYMin;
        private int m_intLineWidth;
        private string m_strColour;
        public string FunctionName { get; set; }
        private string m_strId;

        #endregion

        #region Constructors

        public AFunction()
        {
            //
            // set default resolution
            //
            Resolution = MathConstants.INT_FUNCTION_POINTS;
            //
            // By default, set line type as solid
            //
            m_blnShowSolidLine = true;
            //
            // By default, set line width as one
            //
            m_intLineWidth = 1;
        }

        #endregion

        #region AbstractProperties

        public string XLabel { get; set; }

        public string YLabel { get; set; }

        #endregion

        #region Properties

        public double XMin { get; set; }
        public double XMax { get; set; }
        public double YMin { get; set; }
        public double YMax { get; set; }
        public int Resolution { get; set; }

        public string Id
        {
            get
            {
                if(string.IsNullOrEmpty(m_strId))
                {
                    m_strId = Guid.NewGuid().ToString();
                }
                return m_strId;
            }
        }


        #endregion

        #region AbstractMethods

        public abstract void SetFunctionLimits();

        #endregion

        public void InitialiseFunctionLimits()
        {
            //
            // check if function limits are not set yet
            //
            if ((XMin == XMax) || (YMin == YMax))
            {
                SetFunctionLimits();
            }
            if ((m_dblDefaultXMin == m_dblDefaultXMax) || (m_dblDefaultYMin == m_dblDefaultYMax))
            {
                //
                // set the new function limits as default values
                //
                SetDefaultFunctionLimits();
            }
        }

        private void SetDefaultFunctionLimits()
        {
            m_dblDefaultXMin = XMin;
            m_dblDefaultXMax = XMax;
            m_dblDefaultYMin = YMin;
            m_dblDefaultYMax = YMax;
        }

        public void SetShowLinePoints(
            bool blnShowLinesPoints)
        {
            m_blnShowLinePoints = blnShowLinesPoints;

            if (blnShowLinesPoints)
            {
                m_blnShowPoints = false;
                m_blnShowSolidLine = false;
                m_blnShowBars = false;
            }
        }

        public void SetColour(
            string strColour)
        {
            m_strColour = strColour;
        }

        public string GetColour()
        {
            return m_strColour;
        }

        public void SetShowBottom(
            bool blnShowBottom)
        {
            m_blnShowBottom = blnShowBottom;
        }

        public void SetShowSolidLine(
            bool blnShowSolidLine)
        {
            m_blnShowSolidLine = blnShowSolidLine;

            if (blnShowSolidLine)
            {
                m_blnShowPoints = false;
                m_blnShowLinePoints = false;
                m_blnShowBars = false;
            }
        }

        public void SetHide(bool blnHide)
        {
            m_blnHide = blnHide;
        }

        public void SetShowPoints(
            bool blnShowPoints)
        {
            m_blnShowPoints = blnShowPoints;

            if (blnShowPoints)
            {
                m_blnShowSolidLine = false;
                m_blnShowLinePoints = false;
                m_blnShowBars = false;
            }
        }

        public void SetShowBars(
            bool blnShowBars)
        {
            if (blnShowBars)
            {
                m_blnShowPoints = false;
                m_blnShowSolidLine = false;
                m_blnShowLinePoints = false;
            }

            m_blnShowBars = blnShowBars;
        }

        public bool GetShowSolidLine()
        {
            return m_blnShowSolidLine;
        }

        public bool GetShowPoints()
        {
            return m_blnShowPoints;
        }

        public bool GetShowLinePoints()
        {
            return m_blnShowLinePoints;
        }

        public bool GetHide()
        {
            return m_blnHide;
        }

        public bool GetShowBottom()
        {
            return m_blnShowBottom;
        }

        public bool GetShowBars()
        {
            return m_blnShowBars;
        }

        public int GetLineWidth()
        {
            return m_intLineWidth;
        }

        public void SetLineWidth(int intLineWidth)
        {
            m_intLineWidth = intLineWidth;
        }

        public void SetPlotterFunctionName(string strFunctionName)
        {
            FunctionName = strFunctionName;
        }

        public string GetChartFunctionName()
        {
            if (FunctionName == null || FunctionName.Equals(string.Empty))
            {
                throw new HCException("Error. Function not defined.");
            }
            return FunctionName.
                Replace("|", "").
                Replace(@"\", "");
        }

        public double GetDefaultXMin()
        {
            return m_dblDefaultXMin;
        }

        public double GetDefaultXMax()
        {
            return m_dblDefaultXMax;
        }

        public double GetDefaultYMin()
        {
            return m_dblDefaultYMin;
        }

        public double GetDefaultYMax()
        {
            return m_dblDefaultYMax;
        }

        public void SetDefaultYMin(
            double dblDefaultYMin)
        {
            m_dblDefaultYMin = dblDefaultYMin;
        }

        public void SetDefaultYMax(double dblDefaultYMax)
        {
            m_dblDefaultYMax = dblDefaultYMax;
        }

        public void SetDefaultXMin(
            double dblDefaultXMin)
        {
            m_dblDefaultXMin = dblDefaultXMin;
        }

        public void SetDefaultXMax(double dblDefaultXMax)
        {
            m_dblDefaultXMax = dblDefaultXMax;
        }
    }
}
