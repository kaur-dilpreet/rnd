using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Data
{
    public interface IFileIO
    {
        void WriteToFile(String relativeFilename, String data, Boolean overwrite);
        String ReadFromFile(String relativeFilename);
    }
}
