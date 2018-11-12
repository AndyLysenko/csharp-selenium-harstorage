using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Tests
{
    [TestClass]
    public class SearchTests : BaseHarStorageTest
    {
        [TestMethod]
        public void GoogleTest()
        {
            driver.Navigate().GoToUrl("https://www.google.com/");
            driver.FindElement(By.Name("q")).SendKeys("cat");
            driver.FindElement(By.Name("q")).Submit();
            //Assert.Fail();
        }

        [TestMethod]
        public void YahooTest()
        {
            driver.Navigate().GoToUrl("https://search.yahoo.com/");
            driver.FindElement(By.Name("p")).SendKeys("cat");
            driver.FindElement(By.Name("p")).Submit();
        }

        [TestMethod]
        public void BingTest()
        {
            driver.Navigate().GoToUrl("https://www.bing.com/");
            driver.FindElement(By.Name("q")).SendKeys("cat");
            driver.FindElement(By.Name("q")).Submit();
        }
    }
}


