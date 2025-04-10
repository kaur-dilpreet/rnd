using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace Reports.Data
{
    public interface IAPIHelper
    {
        String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data);
        String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data, String contentType);
        String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data, String jsonData, String contentType);
        Int32 DownloadFile(String remoteFilename, String localFilename, CookieContainer cookieContainer, NameValueCollection headerElements, String contentType, String referer, String userAgent);
    }
}
