using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tests.Helpers
{
    public class HarStorage
    {
        public string HostUrl { get; }

        public HarStorage(string hostUrl)
        {
            HostUrl = hostUrl;
        }

        public bool UploadFile(string fileName, out string harReportUrl)
        {
            var fileContent = new ByteArrayContent(File.ReadAllBytes(fileName));
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        FileName = fileName,
                        Name = "file"
                    };
                    formData.Add(fileContent);

                    string url = string.Concat(HostUrl, "/results/upload");
                    var response = client.PostAsync(url, formData).Result;
                    Console.WriteLine("Response:");
                    Console.WriteLine(response.ToString());
                    if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.OK)
                    {
                        harReportUrl = string.Concat(HostUrl, "/results/details?label=", (new System.IO.FileInfo(fileName)).Name.Split('.')[0]);
                        return true;
                    }

                    harReportUrl = string.Empty;
                    return false;
                }
            }
        }
    }
}
