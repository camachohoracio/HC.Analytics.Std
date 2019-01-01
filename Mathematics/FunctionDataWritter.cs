#region

using System.Collections.Generic;
using System.IO;
using HC.Analytics.Mathematics.Functions.DataStructures;

#endregion

namespace HC.Analytics.Mathematics
{
    public class FunctionDataWritter
    {


        public static void WriteFunctionList(
            string strFileName,
            char chrDelimiter,
            List<FuncRow2D> dblArr)
        {
            using (StreamWriter sw = new StreamWriter(strFileName))
            {
                foreach (FuncRow2D functionRow2D in dblArr)
                {
                    string strLine = functionRow2D.X + chrDelimiter.ToString() +
                                     functionRow2D.Fx;
                    sw.WriteLine(strLine);
                }
            }
        }

        public static void WriteFunctionList(
            string strFileName,
            char chrDelimiter,
            List<FunctionRow3D> dblArr)
        {
            using (StreamWriter sw = new StreamWriter(strFileName))
            {
                foreach (FunctionRow3D functionRow3D in dblArr)
                {
                    string strLine = functionRow3D.X + chrDelimiter.ToString() +
                                     functionRow3D.Y + chrDelimiter +
                                     functionRow3D.Z;
                    sw.WriteLine(strLine);
                }
            }
        }
    }
}
