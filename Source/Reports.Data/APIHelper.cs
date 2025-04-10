using System;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Reports.Data
{
    public class APIHelper : IAPIHelper
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Utilities.ISettings Settings;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;


        public APIHelper(Core.ErrorHandling.IErrorHandler errorHandler,
                         Core.Utilities.IUtilities utilities,
                            Core.Utilities.ISettings settings)
        {
            this.ErrorHandler = errorHandler;
            this.Utilities = utilities;
            this.Settings = settings;
        }

        public String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data)
        {
            return GetResponse(uri, requestMethod, headerElements, data, String.Empty);
        }

        public String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data, String contentType)
        {
            return GetResponse(uri, requestMethod, headerElements, data, String.Empty, contentType);
        }

        public Boolean PageExists(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //if (Core.Utilities.Settings.GetServerEnvironment() != Core.Domain.Enumerations.Environment.Dev)
                //    request.Proxy = new WebProxy("https://web-proxy.houston.hpecorp.net:8080");

                request.Method = "GET";
                
                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch (System.Net.WebException ex)
            {
                Stream exStream = ex.Response.GetResponseStream();

                StreamReader readStream = new StreamReader(exStream, Encoding.UTF8);

                String error = readStream.ReadToEnd();

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public String GetResponse(String uri, Core.Domain.Enumerations.RequestMethod requestMethod, NameValueCollection headerElements, NameValueCollection data, String jsonData, String contentType)
        {
            try
            {
                if (headerElements == null)
                    headerElements = new NameValueCollection();

                if (data == null)
                    data = new NameValueCollection();

                String query = String.Empty;

                if (data != null)
                {
                    for (Int32 i = 0; i < data.Count; i++)
                    {
                        String temp = String.Empty;

                        temp = String.Format("{0}={1}", data.Keys[i], HttpUtility.UrlEncode(data[i]));

                        if (String.IsNullOrEmpty(query))
                        {
                            query = temp;
                        }
                        else
                        {
                            query = String.Format("{0}&{1}", query, temp);
                        }
                    }

                    if (requestMethod == Core.Domain.Enumerations.RequestMethod.GET && !String.IsNullOrEmpty(query))
                        uri = String.Format("{0}{1}{2}", uri, uri.IndexOf("?") >=0 ? "&" : "?", query);
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                //if (Core.Utilities.Settings.GetServerEnvironment() != Core.Domain.Enumerations.Environment.Dev)
                //{
                //    request.Proxy = new WebProxy("http://web-proxy.houston.hpecorp.net:8080");
                //    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                //}

                if (headerElements != null)
                {
                    for (Int32 i = 0; i < headerElements.Count; i++)
                    {
                        if (headerElements.Keys[i].ToLower().Equals("host"))
                            request.Host = headerElements[i];
                        else
                            request.Headers.Add(headerElements.Keys[i], headerElements[i]);
                    }
                }

                request.Method = requestMethod.ToString();

                if (!String.IsNullOrEmpty(contentType))
                    request.ContentType = contentType;

                if (!string.IsNullOrEmpty(query) && (requestMethod == Core.Domain.Enumerations.RequestMethod.POST || requestMethod == Core.Domain.Enumerations.RequestMethod.PUT))
                {
                    byte[] postData = Encoding.ASCII.GetBytes(query);

                    request.ContentLength = postData.Length;
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(postData, 0, postData.Length);
                    }
                }

                if (!String.IsNullOrEmpty(jsonData))
                {
                    if (requestMethod == Core.Domain.Enumerations.RequestMethod.POST || requestMethod == Core.Domain.Enumerations.RequestMethod.PUT || requestMethod == Core.Domain.Enumerations.RequestMethod.OPTIONS)
                    {
                        byte[] postData = Encoding.ASCII.GetBytes(jsonData);

                        request.ContentLength = postData.Length;
                        using (var dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(postData, 0, postData.Length);
                        }
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();

                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                    String responseString = readStream.ReadToEnd();

                    return responseString;
                }
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response != null)
                {
                    Stream exStream = ex.Response.GetResponseStream();

                    StreamReader readStream = new StreamReader(exStream, Encoding.UTF8);

                    String error = readStream.ReadToEnd();

                    throw new Core.Domain.Exceptions.WebException(ex.Message, error);
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Int32 DownloadFile(String remoteFilename, String localFilename, CookieContainer cookieContainer, NameValueCollection headerElements, String contentType, String referer, String userAgent)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error
            try
            {
                // Create a request for the specified remote file name
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(remoteFilename);

                //if (Core.Utilities.Settings.GetServerEnvironment() != Core.Domain.Enumerations.Environment.Dev)
                //    request.Proxy = new WebProxy("https://web-proxy.houston.hpecorp.net:8080");

                if (request != null)
                {
                    if (cookieContainer != null)
                        request.CookieContainer = cookieContainer;

                    if (headerElements != null)
                    {
                        for (Int32 i = 0; i < headerElements.Count; i++)
                        {
                            if (headerElements.Keys[i].ToLower().Equals("host"))
                                request.Host = headerElements[i];
                            else
                                request.Headers.Add(headerElements.Keys[i], headerElements[i]);
                        }
                    }

                    if (!String.IsNullOrEmpty(contentType))
                        request.ContentType = contentType;

                    if (!String.IsNullOrEmpty(referer))
                        request.Referer = referer;

                    if (!String.IsNullOrEmpty(userAgent))
                        request.UserAgent = userAgent;

                    // Send the request to the server and retrieve the
                    // WebResponse object 
                    response = request.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file
                        localStream = File.Create(localFilename);

                        // Allocate a 1k buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned
                        do
                        {
                            // Read data (up to 1k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Close the response and streams objects here 
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }

            // Return total bytes processed to caller.
            return bytesProcessed;
        }
    }
}
