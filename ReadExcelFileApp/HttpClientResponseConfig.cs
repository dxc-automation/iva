using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace ReadExcelFileApp {

    public class HttpClientResponseConfig {

        private static int   response_code { get; set; }
        public static string protocol_version { get; set; }
        public static string response_status_description { get; set; }
        public static string response_server { get; set; }
        public static string response_status_msg { get; set; }
        public static string response_error_msg { get; set; }
        public static string response_exception { get; set; }
        public static string pretty_json { get; set; }
        public static void GetResponseDetails(IRestResponse response)
        {
            try
            {
                //***   Get response details
                string protocol_version = getProtocolVersion(response);
                string status_description = response.StatusDescription;
                string response_server = response.Server;
                string response_status_msg = response.ResponseStatus.ToString();
                string response_error_msg = response.ErrorMessage;
                string response_content = response.Content;
                int response_code = (int)response.StatusCode;


            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //***   Used for get the response body
        public static string GetResponseBody(IRestResponse response)
        {
            string response_body = JValue.Parse(response.Content).ToString(Formatting.Indented);
            return response_body;
        }

        //***   Used for get the response code
        public static int getResponseCode(IRestResponse response)
        {
            return response_code = (int)response.StatusCode;
        }

        //***   Used for formatting json
        public static string PrettyJson(string response)
        {
            try
            {
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var jsonElement = JsonConvert.DeserializeObject<JsonElement>(response);
                pretty_json = System.Text.Json.JsonSerializer.Serialize(jsonElement, options);
                Console.WriteLine(pretty_json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
             //   test.Fail("<pre>" + e.StackTrace + "</pre>");
            }
            return pretty_json;
        }

        //***   Returns the version of the protocol
        public static string getProtocolVersion(IRestResponse response)
        {
            return protocol_version = response.ProtocolVersion.ToString();
        }

        //***   Returns response status description
        public static string getResponseStatusDescription(IRestResponse response)
        {
            return response.StatusDescription;
        }

        //***   Returns the name of the response server
        public static string getResponseServer(IRestResponse response)
        {
            return response_server = response.Server;
        }

        //***   Returns response status message
        public static string getResponseStatusMsg(IRestResponse response)
        {
            return response_status_msg = response.ResponseStatus.ToString();
        }

        //***   Returns error message
        public static string getResponseErrorMsg(IRestResponse response)
        {
            return response.ErrorMessage.ToString();
        }

        //***   Return exception
        public static Exception getResponseException(IRestResponse response, Exception exception)
        {
            response.ResponseStatus = ResponseStatus.Error;
            response.ErrorMessage = exception.Message;
            response.ErrorException = exception;
            Console.WriteLine(exception);
            return exception;
        }
    }
}
