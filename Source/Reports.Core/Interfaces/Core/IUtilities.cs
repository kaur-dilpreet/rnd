using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;

namespace Reports.Core.Utilities
{
    public interface IUtilities
    {
        String GetQueryParameter(String url, String parameter);
        String GetQueryParameter(Uri uri, String parameter);
        void CheckFolder(String filePath);
        Boolean ValidateEmail(String email);
        Boolean ValidateNTID(String ntid);
        String FixPhoneNumbers(String phoneNumber);
        String HideEmail(String email);
        byte[] HexToByte(string hexString);
        String Serialize(object serializeObject);
        String Serialize(object serializeObject, Boolean typeNameHandling);
        String SerializeWithType(object serializeObject);
        T Deserialize<T>(String serializedString);
        T DeserializeWidthJavaScriptSerializer<T>(String serializedString, Boolean typeNameHandling);
        Byte[] ToByteArray(String text);
        String ByteToString(Byte[] byteArray);
        String StringToHex(String text);
        Int32 ParseInt32(String text);
        //String TruncateString(String input, Int32 count, Boolean addEllipsis);
        String RenderPartialViewToString(Controller controller, string viewName, object model);
        String ToTitleCase(String input);
        String UrlPathEncode(String input);
        String UrlEncode(String input);
        String EncryptDatabasePassword(String plainText);
        String DecryptDatabasePassword(String encryptedText);
        CookieContainer GetCookies(String cookieString, String cookieDomain);
        String EncryptPassword(Guid uniqueId, String password);
        String DecryptPassword(Guid uniqueId, String encryptedPassword);
        String GetBaseUrl();
        Boolean IsNumeric(object value);
        Boolean IsFloat(object value);
    }
}
