using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Models;

namespace SeleniumTests
{
    class Demo
    {
        private IWebDriver driver;

        [SetUp]
        public void StartBrowser()
        {
            driver =  new ChromeDriver("C:\\Users\\pkol\\.nuget\\packages\\selenium.webdriver.chromedriver\\2.42.0.1\\driver\\win32");
        }

        [Test]
        public void Test()
        {
            driver.Url = "https://www.zieloneimperium.pl";
            driver.Manage().Window.Maximize();
            //IWebElement link = driver.FindElement(By.ClassName("top"));
           
            IWebElement login = driver.FindElement(By.Id("login_user"));
            IWebElement password = driver.FindElement(By.Id("login_pass"));

            if (login.Displayed && password.Displayed)
            {
                login.SendKeys(Helper.Login);
                password.SendKeys(Helper.Password);
                password.Submit();  
            }

            Thread.Sleep(200);

            //IWebElement menu = driver.FindElement(By.Id("menuButtons"));

            Helper helper = new Helper(driver);

            helper.closeNewsFrames();

            List<ToolsMenu> toolsMenus = helper.GetToolElements();

            List<Seed> Seeds = helper.GetSeedRegal();

            List<Grid> tmp = helper.GetGridElements();
            helper.GetLeftTime(tmp);

            helper.GatherUpPlant(tmp, toolsMenus);
            //helper.UpdateWateringActive(tmp);
            //helper.UpdateSowingActive(tmp);

            helper.DropSeed(Seeds,tmp,toolsMenus,"Marchew");
            //helper.UpdateWateringActive(tmp);
            //helper.UpdateSowingActive(tmp);

            helper.Wathering(Seeds, tmp, toolsMenus);
            //helper.UpdateWateringActive(tmp);
            //helper.UpdateSowingActive(tmp);

            //List<IWebElement> pola = new List<IWebElement>();

        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
            
        }

    }
}
