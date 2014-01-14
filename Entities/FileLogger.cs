using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FileLogger : IMercurioLogger
    {
        private string filePath;
        private LogMessageLevelEnum level = LogMessageLevelEnum.Normal;

        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }

        public void SetLevel(LogMessageLevelEnum level)
        {
            this.level = level;
        }

        public void Log(LogMessageLevelEnum level, string message)
        {
            if (level.GetHashCode() <= this.level.GetHashCode())
            {
                FileStream stream = File.Open(filePath, FileMode.Append);
                StreamWriter writer = new StreamWriter(stream);
                string logMessage = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()
                    + " " + level.ToString() + " " + message;
                writer.WriteLine(logMessage);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
