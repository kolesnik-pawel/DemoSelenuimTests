using System;
using System.IO;
using System.Reflection;

namespace SeleniumTests
{
    class LogWriter
    {
        private string m_exePath = string.Empty;

        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }

        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void LogWriteError(string logMessage, Exception e)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Error(logMessage, w, e);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine("{0:dd-MM-yyyy HH:mm:ss.fff} :\t {1}", DateTime.Now, logMessage);
                //txtWriter.Write("  \t:{0}", );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Error(string logMessage, TextWriter txtWriter, Exception exception )
        {
            try
            {
                txtWriter.WriteLine("------------------- ERROR ----------------------------");
                Log(logMessage, txtWriter);
                Log(exception.Message, txtWriter);
                txtWriter.WriteLine("------------------------------------------------------");
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }
    }
}
