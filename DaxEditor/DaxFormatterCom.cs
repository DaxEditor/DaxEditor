using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DaxEditor
{
    /// <summary>
    /// Format DAX query with www.daxformatter.com
    /// </summary>
    public class DaxFormatterCom
    {
        public class JsonRequest
        {
            public string Dax { get; set; }
            public char ListSeparator { get; set; }
            public char DecimalSeparator { get; set; }
            public string CallerApp { get; set; }
            public string CallerVersion { get; set; }

            public JsonRequest(string dax, char listSeparator, char decimalSeparator)
            {
                Dax = dax;
                ListSeparator = listSeparator;
                DecimalSeparator = decimalSeparator;
                CallerApp = "DaxEditor";
                CallerVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            public JsonRequest(string dax, FormattingCulture culture) : this(
                dax,
                culture == FormattingCulture.EU ? ';' : ',',
                culture == FormattingCulture.EU ? ',' : '.'
                )
            { }

            public static JsonRequest Parse(string text)
            {
                return JsonConvert.DeserializeObject<JsonRequest>(text);
            }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class JsonError
        {
            public int line { get; set; }
            public int column { get; set; }
            public string message { get; set; }
        }

        public class JsonString
        {
            [JsonProperty(PropertyName = "string")]
            public string String { get; set; }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }
        }

        public class JsonResponse
        {
            public IList<IList<JsonString>> formatted { get; set; }
            public IList<JsonError> errors { get; set; }
        }

        public class Response
        {
            public string DAX { get; set; }
            public string Error { get; set; }
            public JsonResponse JsonResponse { get; set; }

            public Response(string text)
            {
                //JsonResponse = new JsonResponse();
                //JsonConvert.PopulateObject(text, JsonResponse);
                JsonResponse = JsonConvert.DeserializeObject<JsonResponse>(text);
                DAX = string.Empty;
                for (var i = 0; i < JsonResponse.formatted.Count; ++i)
                {
                    var item = JsonResponse.formatted[i];
                    var line = item != null && item.Count > 0 ?
                        string.Join(string.Empty, item.Select(str => str.String)) :
                        string.Empty;
                    DAX += line + Environment.NewLine;
                    System.Diagnostics.Debug.WriteLine($"Line {i}: {line}");
                }
                Error = string.Join(Environment.NewLine, JsonResponse.errors.Select(error =>
                    $"Exception from www.daxformatter.com:" + Environment.NewLine +
                    $"  Message: {error.message}" + Environment.NewLine +
                    $"  Line: {error.line}" + Environment.NewLine +
                    $"  Column: {error.column}"
                ));
            }
        }

        public enum FormattingCulture { US, EU };
        private static string daxFormatterUrl = "http://www.daxformatter.com/api/daxformatter/daxrichformatverbose";
        private static string redirectUrl = null;

        /// <summary>
        /// Formats a string by calling www.daxformatter.com.
        /// </summary>
        /// <param name="inputDax">input dax text</param>
        /// <exception cref="FormatException">Error message from www.daxformatter.com</exception>
        /// <returns>formatted text</returns>
        public static string Format(string inputDax, FormattingCulture culture)
        {
            if (redirectUrl == null)
            {
                // www.daxformatter.com redirects request to another site.  HttpWebRequest does redirect with GET.  It fails, since the web service works only with POST
                // The following 2 requests are doing manual POST re-direct
                var redirectRequest = WebRequest.Create(daxFormatterUrl) as HttpWebRequest;
                redirectRequest.AllowAutoRedirect = false;
                var redirectResponse = redirectRequest.GetResponse() as HttpWebResponse;
                redirectUrl = redirectResponse.Headers["Location"];
            }
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    new JsonRequest(inputDax, culture).ToString(),
                    Encoding.UTF8, "application/json"
                    );
                Debug.WriteLine($"Content: {content.ReadAsStringAsync().Result}");
                var responseMessage = client.PostAsync(redirectUrl, content).Result;
                var responseString = responseMessage.Content.ReadAsStringAsync().Result;
                var response = GetValidJsonFromResponse(responseString);

                Debug.WriteLine($"Response: {response}");
                var result = new Response(response);
                if (!string.IsNullOrWhiteSpace(result.Error))
                {
                    Debug.WriteLine($"Error: {result.Error}");
                    throw new FormatException(result.Error);
                }

                Debug.WriteLine($"Formatted DAX: {result.DAX}");
                return result.DAX;
            }
        }

        private static string GetValidJsonFromResponse(string response)
        {
            return response.Trim('"')
                .Replace("\\r\\n", "\r\n")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("\\r", "\r")
                .Replace("\\\\\"", "\\\"")
                .Replace("\\\"", "\"")
                .Replace("\\\'", "\'")
                .Replace("\\/", "/"); //?
        }

        private static string JsonEncode(string stringData)
        {
            var sb = new StringBuilder(stringData.Length);
            foreach (char ch in stringData.ToCharArray())
            {
                switch (ch)
                {
                    case '\t':
                        sb.Append(@"\t");
                        break;
                    case '\"':
                        sb.Append(@"\""");
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    case '/':
                        sb.Append(@"\/");
                        break;
                    case '\n':
                    case '\r':
                        // This is NOT how \n\r to be encoded in JSON, but in formatting case this is enough.
                        sb.Append(' ');
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            return sb.ToString();
        }

        private static string JsonDecode(string jsonEncodedString)
        {
            var sb = new StringBuilder(jsonEncodedString.Length);
            var chars = jsonEncodedString.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                switch (c)
                {
                    case '\\':
                        if (++i == chars.Length)
                            throw new InvalidOperationException("Invalid JSON input");
                        c = chars[i];
                        switch (c)
                        {
                            case '\"':
                                sb.Append('\"');
                                break;
                            case '/':
                                sb.Append('/');
                                break;
                            case '\\':
                                sb.Append('\\');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            default:
                                throw new InvalidOperationException("Invalid JSON input");
                        }
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }
    }
}
