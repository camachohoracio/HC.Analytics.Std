#region

using System.IO;
using HC.Analytics.MachineLearning.TextMining.PreProcessing;
using HC.Core.Helpers;

#endregion

namespace HC.Analytics.MachineLearning.NaiveBayes.Impl1
{
    /**
     * A Document stored as a file.
     *
     * @author Ray Mooney
     */

    public abstract class AFileDocument : ADocument
    {
        /**
         * The name of the file
         */
        public string m_fileName;
        /**
         * The I/O reader for accessing the file
         */
        protected StreamReader m_reader;

        /**
         * Creates a FileDocument and initializes its name and reader.
         */

        public AFileDocument(string fileName, bool stem) :
            base(stem,
            null,
            null,
            ',')
        {
            m_fileName = fileName;
            try
            {
                m_reader = new StreamReader(fileName);
            }
            catch (IOException e)
            {
                PrintToScreen.WriteLine("\nCould not open FileDocument: " + fileName);
            }
        }
    }
}
