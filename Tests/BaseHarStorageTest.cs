using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tests.Helpers;

namespace Tests
{
    [TestClass]
    public class BaseHarStorageTest
    {
        private static int proxyPort = Properties.Settings.Default.ProxyPort;
        private static readonly int proxyManagementPort = Properties.Settings.Default.ProxyManagementPort;
        private static readonly string harStorageUrl = Properties.Settings.Default.HarStorageUrl;
        private static readonly string proxyRunFile = Properties.Settings.Default.proxyRunFile;

        private readonly string harFilesPath = Directory.GetCurrentDirectory() + @"\HarFiles\";
        private readonly HarStorage HarStorage =  new HarStorage(harStorageUrl);
        private readonly string proxyBaseUrlt = "http://localhost:" + proxyManagementPort.ToString() + "/proxy/" +
                                       proxyPort.ToString();
        private string testFileName;
        private static int processId;
        protected IWebDriver driver;

        private TestContext testContext;
        public TestContext TestContext
        {
            get => testContext;
            set => testContext = value;
        }

        [AssemblyInitialize]
        public static void RunProxy(TestContext testContext)
        {
            var proxySettings = PsHelper.Run(System.IO.File.ReadAllText(proxyRunFile));
            if (proxySettings?["process_id"] != null)
            {
                testContext.Properties.Add("ProxyProcessId", proxySettings["process_id"]);
                processId = proxySettings["process_id"];
            }
        }

        [AssemblyCleanup]
        public static void KillProxy()
        {
            try
            {
                var process = Process.GetProcessById(processId);
                process?.CloseMainWindow();
                Console.WriteLine($"Killed process {processId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown {ex}");
            }

            PostWebCleanup();
        }

        private static void PostWebCleanup()
        {
            try
            {
                var processes = Process.GetProcessesByName("chromedriver");
                processes?.ToList().ForEach(p => p.Kill());
                Console.WriteLine($"Killed webdriver process {processId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown {ex}");
            }
        }

        [TestInitialize]
        public void InitWeb()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArguments("--proxy-server=127.0.0.1:8081");
            driver = new ChromeDriver(options);
        }

        [TestInitialize]
        public void ResetProxy()
        {
            testFileName =testContext.TestName;

            string urlString = string.Concat(proxyBaseUrlt,
                "/har/?captureContent=true&captureHeaders=true&captureCookies=true&initialPageRef=",
                testFileName);
            HttpHelper.Put(urlString, null);
        }

        [TestCleanup]
        public void ReportHarFile()
        {
            string harContentUrl = string.Concat(proxyBaseUrlt, "/har");
            var harContent = HttpHelper.Get(harContentUrl);
            //var harContent = HttpHelper.Get("http://localhost:9595/proxy/8081/har");
            
            string fullFilePath = string.Concat(harFilesPath, testFileName, ".har");
            System.IO.FileInfo file = new System.IO.FileInfo(fullFilePath);
            file.Directory.Create();
            System.IO.File.WriteAllText(file.FullName, harContent);

            bool result = HarStorage.UploadFile(fullFilePath, out string reportLink);
            if (result)
            {
                System.Diagnostics.Process.Start(reportLink);
            }
            else
            {
                Console.WriteLine($"Failed to post {file.FullName} to {harContentUrl}");
            }
            Console.WriteLine($"Cleanup 1");

        }

        [TestCleanup]
        public void CleanupWeb()
        {
            driver?.Quit();
            Console.WriteLine($"Cleanup 2");
        }
    }
}

