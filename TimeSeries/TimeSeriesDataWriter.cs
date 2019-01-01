#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HC.Core;
using HC.Core.Exceptions;
using HC.Core.Io;
using HC.Core.Logging;

#endregion

namespace HC.Analytics.TimeSeries
{
    public class TimeSeriesDataWriter : IDisposable
    {
        #region Members

        private readonly char m_chrDelimiter;
        private readonly string m_strFileName;
        private readonly Encoding uniEncoding = new ASCIIEncoding();
        private FileStream m_sw;

        #endregion

        #region Constructor

        public TimeSeriesDataWriter(
            string strFileName,
            char chrDelimiter)
        {
            m_strFileName = strFileName;
            m_chrDelimiter = chrDelimiter;
            CreateStreamW();
        }

        #endregion

        #region Public

        public void WriteTimeSeriesRow(
            TsRow2D tsRow2D,
            Dictionary<DateTime, int> dateList)
        {
            try
            {
                WriteTimeSeriesRow(dateList, tsRow2D);
                m_sw.Flush();
            }
            catch (HCException e)
            {
                Logger.Log(e);
            }
        }

        public void WriteTimeSeriesList(
            List<TsRow2D> dblArr,
            Dictionary<DateTime, int> dateList)
        {
            try
            {
                CreateStreamW();
                foreach (TsRow2D functionRow2D in dblArr)
                {
                    try
                    {
                        WriteTimeSeriesRow(dateList, functionRow2D);
                    }
                    catch (HCException e)
                    {
                        //Debugger.Break();
                        throw;
                    }
                }
                m_sw.Flush();
            }
            catch (HCException e)
            {
                //Debugger.Break();
                throw;
            }
        }

        #endregion

        #region Private

        private void WriteTimeSeriesRow(
            Dictionary<DateTime, int> dateList,
            TsRow2D functionRow2D)
        {
            var intIndex = dateList[functionRow2D.Time];

            var strDate =
                functionRow2D.Time.Month + "/" +
                functionRow2D.Time.Day + "/" +
                functionRow2D.Time.Year;

            var strLine =
                intIndex +
                m_chrDelimiter.ToString() +
                strDate +
                m_chrDelimiter +
                functionRow2D.Fx;
            WriteStingIntoBuffer(
                strLine + '\n',
                m_sw);
        }

        private void WriteStingIntoBuffer(
            string strLine,
            Stream memoryStream)
        {
            var bytes = uniEncoding.GetBytes(strLine);
            memoryStream.Write(bytes, 0, bytes.Length);
        }


        private void CreateStreamW()
        {
            if (m_sw != null)
            {
                m_sw.Close();
            }
            // dummy an empty file here
            if (!FileHelper.Exists(
                m_strFileName,
                false))
            {
                try
                {
                    using (var sw = new StreamWriter(m_strFileName))
                    {
                    }
                }
                catch
                {
                }
            }
            m_sw = new FileStream(
                m_strFileName,
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.ReadWrite);

            //m_sw = new StreamWriter(FileName);
        }

        #endregion

        #region Disposable methods

        ~TimeSeriesDataWriter()
        {
            Dispose();
        }

        public void Dispose()
        {
            EventHandlerHelper.RemoveAllEventHandlers(this);
            if (m_sw != null)
            {
                m_sw.Close();
            }
        }

        #endregion
    }
}
