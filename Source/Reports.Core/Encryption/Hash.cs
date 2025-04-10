using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Reports.Core.Encryption
{
    public class Hash : IHash
    {
        private readonly Core.Utilities.IUtilities Utilities;

        public Hash(Core.Utilities.IUtilities utilities)
        {
            this.Utilities = utilities;
        }

        public String GetHash(String value, String salt, String Hexkey)
        {
            HMACSHA1 hash = new HMACSHA1 { Key = this.Utilities.HexToByte(Hexkey) };
            String hashedString = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(String.Format("{0}{1}", value, salt))));

            return hashedString;
        }

        public String GetHash(String value, String Stringkey)
        {
            HMACSHA1 hash = new HMACSHA1 { Key = this.Utilities.ToByteArray(Stringkey) };

            byte[] computedHash = hash.ComputeHash(this.Utilities.ToByteArray(value));

            String computedHashString = BitConverter.ToString(computedHash);

            String hashedString = Convert.ToBase64String(computedHash);

            return hashedString;
        }

        
    }
}
