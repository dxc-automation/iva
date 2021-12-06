using RestSharp;
using System;
using System.IO;            //Needs to be added
using System.Net;           

namespace ReadExcelFileApp
{

    public class HttpClientRequestConfig
    {
        public static string request_method { get; set; }
        public static string request_resource { get; set; }
        public static int    request_timeout { get; set; }


        public static string getReqxuestMethod(IRestRequest request)
        {
            return request_method = request.Method.ToString();
        }
    }
}