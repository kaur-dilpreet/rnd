using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Reports.Data
{
    public class FileIO : IFileIO
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;

        public FileIO(Core.ErrorHandling.IErrorHandler errorHandler,
                      Core.Utilities.ISettings settings,
                      Core.Utilities.IUtilities utilities)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
        }

        public void WriteToFile(String relativeFilename, String data, Boolean overwrite)
        {
            String absoluteFilePath = Core.Utilities.Utilities.MapPath(relativeFilename);

            if (overwrite || !File.Exists(absoluteFilePath))
            {
                if (!Directory.Exists(Path.GetDirectoryName(absoluteFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(absoluteFilePath));
                }

                using (StreamWriter sw = new StreamWriter(absoluteFilePath, false, Encoding.UTF8))
                {
                    sw.Write(data);
                }
            }
        }

        public String ReadFromFile(String relativeFilename)
        {
            String absoluteFilePath = Core.Utilities.Utilities.MapPath(relativeFilename);

            String data = String.Empty;

            if (File.Exists(absoluteFilePath))
            {
                using (StreamReader sw = new StreamReader(absoluteFilePath, Encoding.UTF8))
                {
                    data = sw.ReadToEnd();
                }
            }

            return data;
        }
    }
}
