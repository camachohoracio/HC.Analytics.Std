#region

using System;
using System.Text;
using HC.Analytics.Colt.doubleAlgo;

#endregion

namespace HC.Analytics.Colt
{
    ////package ref;

    ////import IAxis;
    ////import IHistogram1D;
    ////import IHistogram2D;
    ////import IHistogram3D;

    /**
     * Histogram conversions, for example to string and XML format; 
     * This class requires the Colt distribution, whereas the rest of the //package is entirelly stand-alone.
     */

    [Serializable]
    public class Converter
    {
        /**
         * Creates a new histogram converter.
         */
        /** 
         * Returns all edges of the given axis.
         */

        public double[] edges(IAxis axis)
        {
            int b = axis.bins();
            double[] bounds = new double[b + 1];
            for (int i = 0; i < b; i++)
            {
                bounds[i] = axis.binLowerEdge(i);
            }
            bounds[b] = axis.upperEdge();
            return bounds;
        }

        private string form(Former formatter, double value)
        {
            return formatter.form(value);
        }

        /** 
         * Returns an array[h.xAxis().bins()]; ignoring extra bins.
         */

        public double[] toArrayErrors(IHistogram1D h)
        {
            int xBins = h.xAxis().bins();
            double[] array = new double[xBins];
            for (int j = xBins; --j >= 0;)
            {
                array[j] = h.binError(j);
            }
            return array;
        }

        /** 
         * Returns an array[h.xAxis().bins(),h.yAxis().bins()]; ignoring extra bins.
         */

        public double[,] toArrayErrors(IHistogram2D h)
        {
            int xBins = h.xAxis().bins();
            int yBins = h.yAxis().bins();
            double[,] array = new double[xBins,yBins];
            for (int i = yBins; --i >= 0;)
            {
                for (int j = xBins; --j >= 0;)
                {
                    array[j, i] = h.binError(j, i);
                }
            }
            return array;
        }

        /** 
         * Returns an array[h.xAxis().bins()]; ignoring extra bins.
         */

        public double[] toArrayHeights(IHistogram1D h)
        {
            int xBins = h.xAxis().bins();
            double[] array = new double[xBins];
            for (int j = xBins; --j >= 0;)
            {
                array[j] = h.binHeight(j);
            }
            return array;
        }

        /** 
         * Returns an array[h.xAxis().bins(),h.yAxis().bins()]; ignoring extra bins.
         */

        public double[,] toArrayHeights(IHistogram2D h)
        {
            int xBins = h.xAxis().bins();
            int yBins = h.yAxis().bins();
            double[,] array = new double[xBins,yBins];
            for (int i = yBins; --i >= 0;)
            {
                for (int j = xBins; --j >= 0;)
                {
                    array[j, i] = h.binHeight(j, i);
                }
            }
            return array;
        }

        /** 
         * Returns an array[h.xAxis().bins(),h.yAxis().bins(),h.zAxis().bins()]; ignoring extra bins.
         */

        public double[,,] toArrayHeights(IHistogram3D h)
        {
            int xBins = h.xAxis().bins();
            int yBins = h.yAxis().bins();
            int zBins = h.zAxis().bins();
            double[,,] array = new double[xBins,yBins,zBins];
            for (int j = xBins; --j >= 0;)
            {
                for (int i = yBins; --i >= 0;)
                {
                    for (int k = zBins; --k >= 0;)
                    {
                        array[j, i, k] = h.binHeight(j, i, k);
                    }
                }
            }
            return array;
        }

        /**
         * Returns a string representation of the specified array.  The string
         * representation consists of a list of the arrays's elements, enclosed in square brackets
         * (<tt>"[]"</tt>).  Adjacent elements are separated by the characters
         * <tt>", "</tt> (comma and space).
         * @return a string representation of the specified array.
         */

        public static string ToString(double[] array)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("[");
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append(array[i]);
                if (i < maxIndex)
                {
                    buf.Append(", ");
                }
            }
            buf.Append("]");
            return buf.ToString();
        }

        /** 
         * Returns a string representation of the given argument.
         */

        public string ToString(IAxis axis)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("Range: [" + axis.lowerEdge() + "," + axis.upperEdge() + ")");
            buf.Append(", Bins: " + axis.bins());
            buf.Append(", Bin edges: " + ToString(edges(axis)) + Environment.NewLine);
            return buf.ToString();
        }

        /** 
         * Returns a string representation of the given argument.
         */

        public string ToString(IHistogram1D h)
        {
            string columnAxisName = null; //"X";
            string rowAxisName = null;
            BinFunction1D[] aggr = null; //{BinFunctions1D.sum};
            string format = "%G";
            //string format = "%1.2G";

            Former f = new FormerFactory().create(format);
            string sep = ",";
            int[] minMaxBins = h.minMaxBins();
            string title = h.title() + ":" + sep +
                           "   Entries=" + form(f, h.entries()) + ", ExtraEntries=" + form(f, h.extraEntries()) + sep +
                           "   Mean=" + form(f, h.mean()) + ", Rms=" + form(f, h.rms()) + sep +
                           "   MinBinHeight=" + form(f, h.binHeight(minMaxBins[0])) + ", MaxBinHeight=" +
                           form(f, h.binHeight(minMaxBins[1])) + sep +
                           "   Axis: " +
                           "Bins=" + form(f, h.xAxis().bins()) +
                           ", Min=" + form(f, h.xAxis().lowerEdge()) +
                           ", Max=" + form(f, h.xAxis().upperEdge());

            string[] xEdges = new string[h.xAxis().bins()];
            for (int i = 0; i < h.xAxis().bins(); i++)
            {
                xEdges[i] = form(f, h.xAxis().binLowerEdge(i));
            }

            string[] yEdges = null;

            DoubleMatrix2D heights = new DenseDoubleMatrix2D(1, h.xAxis().bins());
            heights.viewRow(0).assign(toArrayHeights(h));
            //DoubleMatrix2D errors = new DenseDoubleMatrix2D(1,h.xAxis().bins());
            //errors.viewRow(0).assign(toArrayErrors(h));

            return title + sep +
                   "Heights:" + sep +
                   new FormatterDoubleAlgo().toTitleString(
                       heights, yEdges, xEdges, rowAxisName, columnAxisName, null, aggr);
            /*
            + sep +
            "Errors:" + sep +
            new doublealgo.Formatter().toTitleString(
                errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
            */
        }

        /** 
         * Returns a string representation of the given argument.
         */

        public string ToString(IHistogram2D h)
        {
            string columnAxisName = "X";
            string rowAxisName = "Y";
            BinFunction1D[] aggr = {BinFunctions1D.sum};
            string format = "%G";
            //string format = "%1.2G";

            Former f = new FormerFactory().create(format);
            string sep = "";
            int[] minMaxBins = h.minMaxBins();
            string title = h.title() + ":" + sep +
                           "   Entries=" + form(f, h.entries()) + ", ExtraEntries=" + form(f, h.extraEntries()) + sep +
                           "   MeanX=" + form(f, h.meanX()) + ", RmsX=" + form(f, h.rmsX()) + sep +
                           "   MeanY=" + form(f, h.meanY()) + ", RmsY=" + form(f, h.rmsX()) + sep +
                           "   MinBinHeight=" + form(f, h.binHeight(minMaxBins[0], minMaxBins[1])) + ", MaxBinHeight=" +
                           form(f, h.binHeight(minMaxBins[2], minMaxBins[3])) + sep +
                           "   xAxis: " +
                           "Bins=" + form(f, h.xAxis().bins()) +
                           ", Min=" + form(f, h.xAxis().lowerEdge()) +
                           ", Max=" + form(f, h.xAxis().upperEdge()) + sep +
                           "   yAxis: " +
                           "Bins=" + form(f, h.yAxis().bins()) +
                           ", Min=" + form(f, h.yAxis().lowerEdge()) +
                           ", Max=" + form(f, h.yAxis().upperEdge());

            string[] xEdges = new string[h.xAxis().bins()];
            for (int i = 0; i < h.xAxis().bins(); i++)
            {
                xEdges[i] = form(f, h.xAxis().binLowerEdge(i));
            }

            string[] yEdges = new string[h.yAxis().bins()];
            for (int i = 0; i < h.yAxis().bins(); i++)
            {
                yEdges[i] = form(f, h.yAxis().binLowerEdge(i));
            }
            new ObjectArrayList(yEdges).reverse(); // keep coord. system

            DoubleMatrix2D heights = new DenseDoubleMatrix2D(toArrayHeights(h));
            heights = heights.viewDice().viewRowFlip(); // keep the histo coord. system
            //heights = heights.viewPart(1,1,heights.Rows()-2,heights.Columns()-2); // ignore under&overflows

            //DoubleMatrix2D errors = new DenseDoubleMatrix2D(toArrayErrors(h));
            //errors = errors.viewDice().viewRowFlip(); // keep the histo coord system
            ////errors = errors.viewPart(1,1,errors.Rows()-2,errors.Columns()-2); // ignore under&overflows

            return title + sep +
                   "Heights:" + sep +
                   new FormatterDoubleAlgo().toTitleString(
                       heights, yEdges, xEdges, rowAxisName, columnAxisName, null, aggr);
            /*
            + sep +
            "Errors:" + sep +
            new doublealgo.Formatter().toTitleString(
                errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
            */
        }

        /** 
         * Returns a string representation of the given argument.
         */

        public string ToString(IHistogram3D h)
        {
            string columnAxisName = "X";
            string rowAxisName = "Y";
            string sliceAxisName = "Z";
            BinFunction1D[] aggr = {BinFunctions1D.sum};
            string format = "%G";
            //string format = "%1.2G";

            Former f = new FormerFactory().create(format);
            string sep = ",";
            int[] minMaxBins = h.minMaxBins();
            string title = h.title() + ":" + sep +
                           "   Entries=" + form(f, h.entries()) + ", ExtraEntries=" + form(f, h.extraEntries()) + sep +
                           "   MeanX=" + form(f, h.meanX()) + ", RmsX=" + form(f, h.rmsX()) + sep +
                           "   MeanY=" + form(f, h.meanY()) + ", RmsY=" + form(f, h.rmsX()) + sep +
                           "   MeanZ=" + form(f, h.meanZ()) + ", RmsZ=" + form(f, h.rmsZ()) + sep +
                           "   MinBinHeight=" + form(f, h.binHeight(minMaxBins[0], minMaxBins[1], minMaxBins[2])) +
                           ", MaxBinHeight=" + form(f, h.binHeight(minMaxBins[3], minMaxBins[4], minMaxBins[5])) + sep +
                           "   xAxis: " +
                           "Bins=" + form(f, h.xAxis().bins()) +
                           ", Min=" + form(f, h.xAxis().lowerEdge()) +
                           ", Max=" + form(f, h.xAxis().upperEdge()) + sep +
                           "   yAxis: " +
                           "Bins=" + form(f, h.yAxis().bins()) +
                           ", Min=" + form(f, h.yAxis().lowerEdge()) +
                           ", Max=" + form(f, h.yAxis().upperEdge()) + sep +
                           "   zAxis: " +
                           "Bins=" + form(f, h.zAxis().bins()) +
                           ", Min=" + form(f, h.zAxis().lowerEdge()) +
                           ", Max=" + form(f, h.zAxis().upperEdge());

            string[] xEdges = new string[h.xAxis().bins()];
            for (int i = 0; i < h.xAxis().bins(); i++)
            {
                xEdges[i] = form(f, h.xAxis().binLowerEdge(i));
            }

            string[] yEdges = new string[h.yAxis().bins()];
            for (int i = 0; i < h.yAxis().bins(); i++)
            {
                yEdges[i] = form(f, h.yAxis().binLowerEdge(i));
            }
            new ObjectArrayList(yEdges).reverse(); // keep coord. system

            string[] zEdges = new string[h.zAxis().bins()];
            for (int i = 0; i < h.zAxis().bins(); i++)
            {
                zEdges[i] = form(f, h.zAxis().binLowerEdge(i));
            }
            new ObjectArrayList(zEdges).reverse(); // keep coord. system

            DoubleMatrix3D heights = new DenseDoubleMatrix3D(toArrayHeights(h));
            heights = heights.viewDice(2, 1, 0).viewSliceFlip().viewRowFlip(); // keep the histo coord. system
            //heights = heights.viewPart(1,1,heights.Rows()-2,heights.Columns()-2); // ignore under&overflows

            //DoubleMatrix2D errors = new DenseDoubleMatrix2D(toArrayErrors(h));
            //errors = errors.viewDice().viewRowFlip(); // keep the histo coord system
            ////errors = errors.viewPart(1,1,errors.Rows()-2,errors.Columns()-2); // ignore under&overflows

            return title + sep +
                   "Heights:" + sep +
                   new FormatterDoubleAlgo().toTitleString(
                       heights, zEdges, yEdges, xEdges, sliceAxisName, rowAxisName, columnAxisName, "", aggr);
            /*
            + sep +
            "Errors:" + sep +
            new doublealgo.Formatter().toTitleString(
                errors,yEdges,xEdges,rowAxisName,columnAxisName,null,aggr);
            */
        }

        /** 
         * Returns a XML representation of the given argument.
         */

        public string toXML(IHistogram1D h)
        {
            StringBuilder buf = new StringBuilder();
            string sep = ",";
            buf.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
            buf.Append(sep);
            buf.Append("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
            buf.Append(sep);
            buf.Append("<plotML>");
            buf.Append(sep);
            buf.Append("<plot>");
            buf.Append(sep);
            buf.Append("<dataArea>");
            buf.Append(sep);
            buf.Append("<data1d>");
            buf.Append(sep);
            buf.Append("<bins1d title=\"" + h.title() + "\">");
            buf.Append(sep);
            for (int i = 0; i < h.xAxis().bins(); i++)
            {
                buf.Append(h.binEntries(i) + "," + h.binError(i));
                buf.Append(sep);
            }
            buf.Append("</bins1d>");
            buf.Append(sep);
            buf.Append("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
            buf.Append(" Min=\"" + h.xAxis().lowerEdge() + "\"");
            buf.Append(" Max=\"" + h.xAxis().upperEdge() + "\"");
            buf.Append(" numberOfBins=\"" + h.xAxis().bins() + "\"");
            buf.Append("/>");
            buf.Append(sep);
            buf.Append("<statistics>");
            buf.Append(sep);
            buf.Append("<statistic name=\"Entries\" value=\"" + h.entries() + "\"/>");
            buf.Append(sep);
            buf.Append("<statistic name=\"Underflow\" value=\"" + h.binEntries(Constants.UNDERFLOW) + "\"/>");
            buf.Append(sep);
            buf.Append("<statistic name=\"Overflow\" value=\"" + h.binEntries(Constants.OVERFLOW) + "\"/>");
            buf.Append(sep);
            if (!Double.IsNaN(h.mean()))
            {
                buf.Append("<statistic name=\"Mean\" value=\"" + h.mean() + "\"/>");
                buf.Append(sep);
            }
            if (!Double.IsNaN(h.rms()))
            {
                buf.Append("<statistic name=\"RMS\" value=\"" + h.rms() + "\"/>");
                buf.Append(sep);
            }
            buf.Append("</statistics>");
            buf.Append(sep);
            buf.Append("</data1d>");
            buf.Append(sep);
            buf.Append("</dataArea>");
            buf.Append(sep);
            buf.Append("</plot>");
            buf.Append(sep);
            buf.Append("</plotML>");
            buf.Append(sep);
            return buf.ToString();
        }

        /** 
         * Returns a XML representation of the given argument.
         */

        public string toXML(IHistogram2D h)
        {
            StringBuilder out_ = new StringBuilder();
            string sep = ",";
            out_.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
            out_.Append(sep);
            out_.Append("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
            out_.Append(sep);
            out_.Append("<plotML>");
            out_.Append(sep);
            out_.Append("<plot>");
            out_.Append(sep);
            out_.Append("<dataArea>");
            out_.Append(sep);
            out_.Append("<data2d type=\"xxx\">");
            out_.Append(sep);
            out_.Append("<bins2d title=\"" + h.title() + "\" xSize=\"" + h.xAxis().bins() + "\" ySize=\"" +
                        h.yAxis().bins() + "\">");
            out_.Append(sep);
            for (int i = 0; i < h.xAxis().bins(); i++)
            {
                for (int j = 0; j < h.yAxis().bins(); j++)
                {
                    out_.Append(h.binEntries(i, j) + "," + h.binError(i, j));
                    out_.Append(sep);
                }
            }
            out_.Append("</bins2d>");
            out_.Append(sep);
            out_.Append("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
            out_.Append(" Min=\"" + h.xAxis().lowerEdge() + "\"");
            out_.Append(" Max=\"" + h.xAxis().upperEdge() + "\"");
            out_.Append(" numberOfBins=\"" + h.xAxis().bins() + "\"");
            out_.Append("/>");
            out_.Append(sep);
            out_.Append("<binnedDataAxisAttributes type=\"double\" axis=\"y0\"");
            out_.Append(" Min=\"" + h.yAxis().lowerEdge() + "\"");
            out_.Append(" Max=\"" + h.yAxis().upperEdge() + "\"");
            out_.Append(" numberOfBins=\"" + h.yAxis().bins() + "\"");
            out_.Append("/>");
            out_.Append(sep);
            //out_.Append("<statistics>"); out_.Append(sep);
            //out_.Append("<statistic name=\"Entries\" value=\""+h.entries()+"\"/>"); out_.Append(sep);
            //out_.Append("<statistic name=\"MeanX\" value=\""+h.meanX()+"\"/>"); out_.Append(sep);
            //out_.Append("<statistic name=\"RmsX\" value=\""+h.rmsX()+"\"/>"); out_.Append(sep);
            //out_.Append("<statistic name=\"MeanY\" value=\""+h.meanY()+"\"/>"); out_.Append(sep);
            //out_.Append("<statistic name=\"RmsY\" value=\""+h.rmsY()+"\"/>"); out_.Append(sep);
            //out_.Append("</statistics>"); out_.Append(sep);
            out_.Append("</data2d>");
            out_.Append(sep);
            out_.Append("</dataArea>");
            out_.Append(sep);
            out_.Append("</plot>");
            out_.Append(sep);
            out_.Append("</plotML>");
            out_.Append(sep);
            return out_.ToString();
        }
    }
}
