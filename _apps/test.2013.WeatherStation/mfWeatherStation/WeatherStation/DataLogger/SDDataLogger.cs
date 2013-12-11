using System;
using System.IO;

namespace WeatherStation
{
    /// <summary>
    /// Class for logging weather information on file
    /// </summary>
    public class SDDataLogger : IDataLogger, IDisposable
    {
        #region Constants...

        // format for file name data logging (YYYYMMDD.txt)
        private const string FILE_NAME_FORMAT = "yyyyMMdd";
        // extension for data logging files
        private const string FILE_EXT = ".txt";

        #endregion

        #region Fields...

        // directory on SD for data logging
        private string dirPath;
        // file stream for data logging
        private FileStream fs;
        // text writer for data logging
        private TextWriter tw;

        // disposing state
        private bool disposed = false;

        // data logger opened or not
        private bool isOpen;

        // RTC for file name
        private Rtc rtc;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dirPath">Directory on SD for data logging</param>
        public SDDataLogger(string dirPath)
        {
            this.dirPath = dirPath + @"\";
        }

        #region IDataLogger interface ...

        public void Open()
        {
            // create root data directory if doesn't exist
            if (!Directory.Exists(this.dirPath))
                Directory.CreateDirectory(this.dirPath);

            this.rtc = Rtc.Instance;
            
            // create file path for data logging
            string filePath = this.dirPath + this.rtc.GetDateTime().Date.ToString(FILE_NAME_FORMAT) + FILE_EXT;
            
            // open the file in append mode or create it
            if (this.fs == null)
            {
                this.fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                this.tw = new StreamWriter(this.fs);
            }

            this.isOpen = true;
        }

        public void Close()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);

            this.isOpen = false;
        }

        public void Log(string text)
        {
            this.tw.WriteLine(text);
            this.tw.Flush();
        }

        public bool IsOpen
        {
            get { return this.isOpen; }
        }

        #endregion

        /// <summary>
        /// Destructor
        /// </summary>
        ~SDDataLogger()
        {
            this.Dispose(false);
        }

        

        #region IDisposable pattern ...

        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Disposing managed and unmanaged resources
        /// </summary>
        /// <param name="disposing">Dispose managed resources</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // dispose managed resources

                    if (this.tw != null)
                        this.tw.Dispose();
                    this.tw = null;

                    if (this.fs != null)
                        this.fs.Dispose();
                    this.fs = null;
                }

                // no unmanaged resources to dispose
            }

            this.disposed = true;
        }

        #endregion
    }
}
