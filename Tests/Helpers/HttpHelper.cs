using System;
using System.IO;
using System.Net;

namespace Tests.Helpers
{
    public static class HttpHelper
    {
        public static void Put(string putUrl, object payload)
        {
            var request = (HttpWebRequest)WebRequest.Create(putUrl);
            request.Method = "PUT";
            if (payload != null)
            {
                //request.ContentType = "application/xml";
                //request.ContentLength = Size(payload);
                //Stream dataStream = request.GetRequestStream();
                //Serialize(dataStream, payload);
                //dataStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string returnString = response.StatusCode.ToString();
            Console.WriteLine($"Response Code [{response.StatusCode.ToString()}]");
        }

        public static string Get(string getUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(getUrl);
            request.Method = "Get";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string returnString = reader.ReadToEnd();
            return returnString;
        }
    }
}
