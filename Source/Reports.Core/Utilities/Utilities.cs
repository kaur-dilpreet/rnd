using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;

using Newtonsoft.Json;

namespace Reports.Core.Utilities
{
    public class Utilities : IUtilities
    {
        private readonly ISettings Settings;
        private readonly ErrorHandling.IErrorHandler ErrorHandler;

        public Utilities(ISettings settings, ErrorHandling.IErrorHandler errorHandler)
        {
            this.Settings = settings;
            this.ErrorHandler = errorHandler;
        }

        public static string ConvertUrlsToLinks(string input)
        {
            string pattern = @"(http[s]?:\/\/[^\s^\)^\(]+)";
            string result = Regex.Replace(input, pattern, "<a href=\"$1\" target=\"_blank\">$1</a>");

            return result;
        }

        public static string ConvertUrlsToLinksChatGPI(string input)
        {
            string pattern = @"(http[s]?:\/\/[^\s^\)^\(]+)";
            
            string result = Regex.Replace(input, pattern, "<a href=\"$1\" target=\"_blank\" title=\"$1\">$1</a>");

            var matches = Regex.Matches(input, pattern);

            if(matches.Count > 0)
            {
                foreach(Match match in matches)
                {
                    if (match.Success)
                    {
                        if (match.Value.IndexOf("?") >= 0)
                        {
                            string shortUrl = match.Value;
                            shortUrl = shortUrl.Substring(0, shortUrl.IndexOf("?"));
                            result = result.Replace($">{match.Value}<", $">{shortUrl}<");
                        }
                    }
                }
            }
            

            return result;
        }

        public String GetQueryParameter(String url, String parameter)
        {
            Uri uri = new Uri(url);

            return this.GetQueryParameter(uri, parameter);
        }

        public String GetQueryParameter(Uri uri, String parameter)
        {
            String value = HttpUtility.ParseQueryString(uri.Query).Get(parameter);

            return value;
        }

        public void CheckFolder(String filePath)
        {
            if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(filePath)))
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(filePath));
        }

        public static BLL.CustomAuthentication.CustomPrincipal GetLoggedInUser()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                return (BLL.CustomAuthentication.CustomPrincipal)System.Web.HttpContext.Current.User;
            else
                return null;
        }

        public static Int32 GetLoggedInUserTimezoneOffset()
        {
            BLL.CustomAuthentication.CustomPrincipal user = GetLoggedInUser();

            if (user != null)
                return user.TimezoneOffset;
            else
                return 0;
        }

        public static String ReplaceLinks(string arg)
        //Replaces web and email addresses in text with hyperlinks

        {
            Regex urlregex = new Regex(@"(^|[\n ])(?<url>(www|ftp)\.[^ ,""\s<]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = urlregex.Replace(arg, " <a href=\"http://${url}\" target=\"_blank\">${url}</a>");
            Regex httpurlregex = new Regex(@"(^|[\n ])(?<url>(http://www\.|http://|https://)[^ ,""\s<]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = httpurlregex.Replace(arg, " <a href=\"${url}\" target=\"_blank\">${url}</a>");
            Regex emailregex = new Regex(@"(?<url>[a-zA-Z_0-9.-]+\@[a-zA-Z_0-9.-]+\.\w+\s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            arg = emailregex.Replace(arg, " <a href=\"mailto:${url}\">${url}</a> ");
            return arg;
        }

        public Boolean ValidateNTID(string ntid)
        {
            Match match = Regex.Match(ntid, Core.Domain.Enumerations.NTIdRegex);
            return match.Success;
        }

        public Boolean ValidateEmail(String email)
        {
            Match match = Regex.Match(email, Core.Domain.Enumerations.EmailAddressRegEx);
            return match.Success;
        }

        public String FixPhoneNumbers(String phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("(", String.Empty).Replace(")", String.Empty).Replace("-", String.Empty);

            if (phoneNumber.Length == 10)
                phoneNumber = String.Format("({0}){1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6, 4));

            return phoneNumber;
        }

        public static String GetFileMimeType(String fileName)
        {
            return MimeMapping.GetMimeMapping(fileName);
        }

        /// <summary>
        /// Replace email characters with *
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public String HideEmail(String email)
        {
            String emailDomain = email.ToLower().Substring(email.IndexOf("@"));
            String tempEmail = email.ToLower().Substring(0, email.IndexOf("@"));

            if (tempEmail.Length > 4)
                tempEmail = String.Format("{0}{1}{2}", tempEmail.Substring(0, 2), new string('*', tempEmail.Length - 4), tempEmail.Substring(tempEmail.Length - 2));
            else if (tempEmail.Length > 3)
                tempEmail = String.Format("{0}{1}{2}", tempEmail.Substring(0, 1), new string('*', tempEmail.Length - 2), tempEmail.Substring(tempEmail.Length - 1));

            tempEmail = String.Format("{0}{1}", tempEmail, emailDomain);

            return tempEmail;
        }

        /// <summary>
        /// Converts a hexadecimal String to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (Int32 i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// Takes the object as parameter that needs to serialize and Returns Serialized string
        /// </summary>	
        ///  <param name="serializeObject">Object to srialize</param>
        /// <returns>Serialized string</returns>
        public String Serialize(object serializeObject)
        {
            if (serializeObject != null)
            {
                //string serializeString = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(serializeObject);

                string serializeString = JsonConvert.SerializeObject(serializeObject);
                return serializeString;
            }

            return String.Empty;
        }

        /// <summary>
        /// Takes the object as parameter that needs to serialize and Returns Serialized string
        /// </summary>	
        ///  <param name="serializeObject">Object to srialize</param>
        /// <returns>Serialized string</returns>
        public String Serialize(object serializeObject, Boolean typeNameHandling)
        {
            if (serializeObject != null)
            {
                string serializeString = String.Empty;

                //if (typeNameHandling)
                //    serializeString = (new System.Web.Script.Serialization.JavaScriptSerializer(new System.Web.Script.Serialization.SimpleTypeResolver())).Serialize(serializeObject);
                //else
                //    serializeString = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(serializeObject);

                if (typeNameHandling)
                    serializeString = JsonConvert.SerializeObject(serializeObject, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
                else
                    serializeString = JsonConvert.SerializeObject(serializeObject);

                return serializeString;
            }

            return String.Empty;
        }

        public String SerializeWithType(object serializeObject)
        {
            if (serializeObject != null)
            {
                string serializeString = String.Empty;

                serializeString = (new System.Web.Script.Serialization.JavaScriptSerializer(new System.Web.Script.Serialization.SimpleTypeResolver())).Serialize(serializeObject);

                return serializeString;
            }

            return String.Empty;
        }

        /// <summary>
        /// Takes the object as parameter that needs to serialize and Returns Serialized string
        /// </summary>	
        ///  <param name="serializeObject">Object to srialize</param>
        /// <returns>Serialized string</returns>
        public static String SerializeStatic(object serializeObject)
        {
            if (serializeObject != null)
            {
                //string serializeString = (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(serializeObject);

                string serializeString = JsonConvert.SerializeObject(serializeObject);
                return serializeString;
            }

            return String.Empty;
        }

        /// <summary>
        /// Takes a json string and returns the object
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="serializedString">String to deserialize</param>
        /// <returns>Deserialized Object</returns>
        public static T DeserializeStatic<T>(String serializedString)
        {
            return JsonConvert.DeserializeObject<T>(serializedString);

            //return (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<T>(serializedString);
        }

        /// <summary>
        /// Takes a json string and returns the object
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="serializedString">String to deserialize</param>
        /// <returns>Deserialized Object</returns>
        public T Deserialize<T>(String serializedString)
        {
            serializedString = serializedString.Replace(Environment.NewLine, String.Empty).Replace("\t", String.Empty);

            return JsonConvert.DeserializeObject<T>(serializedString);

            //return (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<T>(serializedString);
        }

        /// <summary>
        /// Takes a json string and returns the object
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="serializedString">String to deserialize</param>
        /// <returns>Deserialized Object</returns>
        public T Deserialize<T>(String serializedString, Boolean typeNameHandling)
        {
            //if (typeNameHandling)
            //    return (new System.Web.Script.Serialization.JavaScriptSerializer(new System.Web.Script.Serialization.SimpleTypeResolver())).Deserialize<T>(serializedString);
            //else
            //    return (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<T>(serializedString);

            if (typeNameHandling)
                return JsonConvert.DeserializeObject<T>(serializedString, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            else
                return JsonConvert.DeserializeObject<T>(serializedString);
        }

        public T DeserializeWidthJavaScriptSerializer<T>(String serializedString, Boolean typeNameHandling)
        {
            if (typeNameHandling)
                return (new System.Web.Script.Serialization.JavaScriptSerializer(new System.Web.Script.Serialization.SimpleTypeResolver())).Deserialize<T>(serializedString);
            else
                return (new System.Web.Script.Serialization.JavaScriptSerializer()).Deserialize<T>(serializedString);
        }

        /// <summary>
        /// Converts String to Byte array
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <returns>Converted byte array</returns>
        public Byte[] ToByteArray(String text)
        {
            Byte[] array = new Byte[text.Length];

            for (Int32 i = 0; i < text.Length; i++)
                array[i] = Convert.ToByte(text[i]);

            return array;
        }

        public static Byte[] ToByteArrayStatic(String text)
        {
            Byte[] array = new Byte[text.Length];

            for (Int32 i = 0; i < text.Length; i++)
                array[i] = Convert.ToByte(text[i]);

            return array;
        }

        /// <summary>
        /// Converts byte array to string
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <returns>Converted string</returns>
        public String ByteToString(Byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();

            for (Int32 i = 0; i < byteArray.Count(); i++)
                sb.Append((Char)byteArray[i]);

            return sb.ToString();
        }

        public static String ByteToStringStatic(Byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();

            for (Int32 i = 0; i < byteArray.Count(); i++)
                sb.Append((Char)byteArray[i]);

            return sb.ToString();
        }

        /// <summary>
        /// Converts string to hex
        /// </summary>
        /// <param name="byteArray">String to convert</param>
        /// <returns>Converted string</returns>
        public String StringToHex(String text)
        {
            StringBuilder sb = new StringBuilder();

            for (Int32 i = 0; i < text.Length; i++)
            {
                String hex = ((Int16)text[i]).ToString("x");

                if (hex.Length == 1)
                    hex = String.Format("0{0}", hex);

                sb.Append(hex);
            }
            return sb.ToString();
        }

        public Int32 ParseInt32(String text)
        {
            if (String.IsNullOrEmpty(text))
                return 0;

            StringBuilder sb = new StringBuilder();

            text = text.Trim();

            Int32 tempInt = 0;

            for (Int32 i = 1; i <= text.Length; i++)
            {
                if (!Int32.TryParse(text.Substring(0, i), out tempInt))
                {
                    if (i == 1)
                        return 0;
                    else
                        return Int32.Parse(text.Substring(0, i - 1));
                }
            }

            if (Int32.TryParse(text, out tempInt))
                return tempInt;
            else
                return 0;
        }

        public static String TruncateString(String input, Int32 count, Boolean addEllipsis)
        {
            if (input.Length <= count)
                return input;
            else
            {
                if (addEllipsis)
                {
                    count -= 3;

                    if (count > 0)
                        return String.Format("{0}...", input.Substring(0, count));
                    else
                        return "...";
                }
                else
                    return input.Substring(0, count);
            }
        }

        public String RenderPartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("controller", controller.ToString());
                methodParam.Add("controller", viewName);
                methodParam.Add("model", this.Serialize(model));

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public String ToTitleCase(String input)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            return textInfo.ToTitleCase(input);
        }

        public String UrlPathEncode(String input)
        {
            string lower = System.Web.HttpUtility.UrlPathEncode(input);
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string upper = reg.Replace(lower, m => m.Value.ToUpperInvariant());

            return upper.Replace("+", "%2B").Replace("!", "%21").Replace(",", "%2C").Replace("/", "%2F").Replace(":", "%3A").Replace("=", "%3D");
        }

        public String UrlEncode(String input)
        {
            string lower = System.Web.HttpUtility.UrlEncode(input);
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string upper = reg.Replace(lower, m => m.Value.ToUpperInvariant());

            return upper;
        }

        public String EncryptDatabasePassword(String plainText)
        {
            byte[] cipherKey = this.ToByteArray(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(cipherKey);
            blowFish.IV = cipherKey.Reverse().Take(8).ToArray();

            String encryptedText = Convert.ToBase64String(this.ToByteArray(blowFish.Encrypt_ECB(plainText)));

            return encryptedText;
        }

        public String DecryptDatabasePassword(String encryptedText)
        {
            byte[] cipherKey = this.ToByteArray(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(cipherKey);
            blowFish.IV = cipherKey.Reverse().Take(8).ToArray();

            String plainText = blowFish.Decrypt_ECB(this.ByteToString(Convert.FromBase64String(encryptedText)));

            return plainText;
        }

        public CookieContainer GetCookies(String cookieString, String cookieDomain)
        {
            CookieContainer cookieContainer = new CookieContainer();

            List<String> cookies = cookieString.Split(';').ToList();

            foreach (String item in cookies)
            {
                if (item.Contains("="))
                {
                    Cookie cookie = new Cookie();
                    cookie.Name = item.Substring(0, item.IndexOf("=")).Trim();
                    cookie.Value = item.Substring(item.IndexOf("=") + 1).Trim();
                    cookie.Domain = cookieDomain;
                    cookieContainer.Add(cookie);
                }
            }

            return cookieContainer;
        }

        public static String GetScheme()
        {
            if (Core.Utilities.Settings.GetServerEnvironment() == Domain.Enumerations.Environment.Dev)
                return "http";
            else
                return "https";
        }

        public static String GetAuthority()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            {
                return System.Web.HttpContext.Current.Request.Url.Authority;
            }
            else
            {
                if (Core.Utilities.Settings.GetServerEnvironment() == Domain.Enumerations.Environment.Dev)
                    return "localhost:2261";
                else if (Core.Utilities.Settings.GetServerEnvironment() == Domain.Enumerations.Environment.QA)
                    return "localhost:2261";
                else if (Core.Utilities.Settings.GetServerEnvironment() == Domain.Enumerations.Environment.ITG)
                    return "c2w31492.itcs.hpecorp.net:3362";
                else
                    return "localhost:2261";
            }
        }

        public static String MapPath(String path)
        {
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            else
            {
                String serverPath = HttpRuntime.AppDomainAppPath;

                if (path.StartsWith("~"))
                    path = path.Substring(1);

                if (path.StartsWith("/"))
                    path = path.Substring(1);

                path = path.Replace("/", "\\");

                if (!serverPath.EndsWith("\\"))
                    serverPath = String.Format("{0}\\", serverPath);

                return String.Format("{0}{1}", serverPath, path);
            }
        }

        public String EncryptPassword(Guid uniqueId, String password)
        {
            if (String.IsNullOrEmpty(password))
                return String.Empty;

            String cypherKey = uniqueId.ToString();

            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(cypherKey);

            String encryptedPassword = String.Empty;

            if (!String.IsNullOrEmpty(password))
            {
                encryptedPassword = Convert.ToBase64String(this.ToByteArray(blowFish.Encrypt_ECB(password)));
            }

            return encryptedPassword;
        }

        public String DecryptPassword(Guid uniqueId, String encryptedPassword)
        {
            if (String.IsNullOrEmpty(encryptedPassword))
                return String.Empty;

            String cypherKey = uniqueId.ToString();

            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(cypherKey);

            String password = String.Empty;

            if (!String.IsNullOrEmpty(encryptedPassword))
            {
                try
                {
                    password = blowFish.Decrypt_ECB(this.ByteToString(Convert.FromBase64String(encryptedPassword)));
                }
                catch (Exception ex)
                {

                }
            }

            return password;
        }

        public String GetBaseUrl()
        {
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            {
                return String.Format("{0}://{1}/", System.Web.HttpContext.Current.Request.Url.Scheme, System.Web.HttpContext.Current.Request.Url.Authority);
            }
            else
            {
                return this.Settings.GetSettings(Domain.Enumerations.AppSettings.BaseUrl);
            }
        }

        public Boolean IsNumeric(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong;
        }

        public Boolean IsFloat(object value)
        {
            return value is float
                    || value is double
                    || value is decimal;
        }

        public static String MarkdownToHtml(String markdownText)
        {
            if (String.IsNullOrEmpty(markdownText))
                return String.Empty;

            return Markdig.Markdown.ToHtml(markdownText);
        }

        public static String WrapLEADIds(String text)
        {
            String pattern = @"LEAD-\d{10}";

            return Regex.Replace(text, pattern, match => $"<a href=\"/sdrai?leadid={match.Value}\" target=\"_blank\" class=\"leadId\">{match.Value}</a>");
        }
    }
}
