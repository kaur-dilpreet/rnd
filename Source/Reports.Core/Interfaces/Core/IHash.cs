using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Encryption
{
    public interface IHash
    {
        String GetHash(String value, String salt, String key);
        String GetHash(String value, String Stringkey);
    }
}
