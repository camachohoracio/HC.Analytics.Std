#region

using System;
using System.IO;
using HC.Analytics.Probability.Random;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.Colt
{
    //package ref;

    //import IHistogram1D;
    //import IHistogram2D;

    //import java.io.FileWriter;
    //import java.io.IOException;
    //import java.io.PrintWriter;
    //import RngWrapper;

    /**
     * A very(!) basic test of the reference implementations of AIDA histograms.
     */

    [Serializable]
    public class Test
    {
        public static void main(string[] argv)
        {
            RngWrapper r = new RngWrapper();
            IHistogram1D h1 = new Histogram1D("AIDA 1D Histogram", 40, -3, 3);
            for (int i = 0; i < 10000; i++)
            {
                h1.fill(r.NextDouble());
            }

            IHistogram2D h2 = new Histogram2D("AIDA 2D Histogram", 40, -3, 3, 40, -3, 3);
            for (int i = 0; i < 10000; i++)
            {
                h2.fill(r.NextDouble(), r.NextDouble());
            }

            // Write the results as a PlotML files!
            writeAsXML(h1, "aida1.xml");
            writeAsXML(h2, "aida2.xml");

            // Try some projections

            writeAsXML(h2.projectionX(), "projectionX.xml");
            writeAsXML(h2.projectionY(), "projectionY.xml");
        }

        public static void writeAsXML(IHistogram1D h, string filename)
        {
            try
            {
                using (StreamWriter out_ = new StreamWriter(filename))
                {
                    out_.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                    out_.WriteLine("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
                    out_.WriteLine("<plotML>");
                    out_.WriteLine("<plot>");
                    out_.WriteLine("<dataArea>");
                    out_.WriteLine("<data1d>");
                    out_.WriteLine("<bins1d title=\"" + h.title() + "\">");
                    for (int i = 0; i < h.xAxis().bins(); i++)
                    {
                        out_.WriteLine(h.binEntries(i) + "," + h.binError(i));
                    }
                    out_.WriteLine("</bins1d>");
                    out_.Write("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
                    out_.Write(" Min=\"" + h.xAxis().lowerEdge() + "\"");
                    out_.Write(" Max=\"" + h.xAxis().upperEdge() + "\"");
                    out_.Write(" numberOfBins=\"" + h.xAxis().bins() + "\"");
                    out_.WriteLine("/>");
                    out_.WriteLine("<statistics>");
                    out_.WriteLine("<statistic name=\"Entries\" value=\"" + h.entries() + "\"/>");
                    out_.WriteLine("<statistic name=\"Underflow\" value=\"" + h.binEntries(Constants.UNDERFLOW) + "\"/>");
                    out_.WriteLine("<statistic name=\"Overflow\" value=\"" + h.binEntries(Constants.OVERFLOW) + "\"/>");
                    if (!Double.IsNaN(h.mean()))
                    {
                        out_.WriteLine("<statistic name=\"Mean\" value=\"" + h.mean() + "\"/>");
                    }
                    if (!Double.IsNaN(h.rms()))
                    {
                        out_.WriteLine("<statistic name=\"RMS\" value=\"" + h.rms() + "\"/>");
                    }
                    out_.WriteLine("</statistics>");
                    out_.WriteLine("</data1d>");
                    out_.WriteLine("</dataArea>");
                    out_.WriteLine("</plot>");
                    out_.WriteLine("</plotML>");
                }
            }
            catch (IOException x)
            {
                PrintToScreen.WriteLine(x.StackTrace);
            }
        }

        public static void writeAsXML(IHistogram2D h, string filename)
        {
            try
            {
                using (StreamWriter out_ = new StreamWriter(filename))
                {
                    out_.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                    out_.WriteLine("<!DOCTYPE plotML SYSTEM \"plotML.dtd\">");
                    out_.WriteLine("<plotML>");
                    out_.WriteLine("<plot>");
                    out_.WriteLine("<dataArea>");
                    out_.WriteLine("<data2d type=\"xxx\">");
                    out_.WriteLine("<bins2d title=\"" + h.title() + "\" xSize=\"" + h.xAxis().bins() + "\" ySize=\"" +
                                   h.yAxis().bins() + "\">");
                    for (int i = 0; i < h.xAxis().bins(); i++)
                    {
                        for (int j = 0; j < h.yAxis().bins(); j++)
                        {
                            out_.WriteLine(h.binEntries(i, j) + "," + h.binError(i, j));
                        }
                    }
                    out_.WriteLine("</bins2d>");
                    out_.Write("<binnedDataAxisAttributes type=\"double\" axis=\"x0\"");
                    out_.Write(" Min=\"" + h.xAxis().lowerEdge() + "\"");
                    out_.Write(" Max=\"" + h.xAxis().upperEdge() + "\"");
                    out_.Write(" numberOfBins=\"" + h.xAxis().bins() + "\"");
                    out_.WriteLine("/>");
                    out_.Write("<binnedDataAxisAttributes type=\"double\" axis=\"y0\"");
                    out_.Write(" Min=\"" + h.yAxis().lowerEdge() + "\"");
                    out_.Write(" Max=\"" + h.yAxis().upperEdge() + "\"");
                    out_.Write(" numberOfBins=\"" + h.yAxis().bins() + "\"");
                    out_.WriteLine("/>");
                    //out_.WriteLine("<statistics>");
                    //out_.WriteLine("<statistic name=\"Entries\" value=\""+h.entries()+"\"/>");
                    //out_.WriteLine("<statistic name=\"MeanX\" value=\""+h.meanX()+"\"/>");
                    //out_.WriteLine("<statistic name=\"RmsX\" value=\""+h.rmsX()+"\"/>");
                    //out_.WriteLine("<statistic name=\"MeanY\" value=\""+h.meanY()+"\"/>");
                    //out_.WriteLine("<statistic name=\"RmsY\" value=\""+h.rmsY()+"\"/>");
                    //out_.WriteLine("</statistics>");
                    out_.WriteLine("</data2d>");
                    out_.WriteLine("</dataArea>");
                    out_.WriteLine("</plot>");
                    out_.WriteLine("</plotML>");
                }
            }
            catch (IOException x)
            {
                PrintToScreen.WriteLine(
                    x.StackTrace);
            }
        }
    }
}
