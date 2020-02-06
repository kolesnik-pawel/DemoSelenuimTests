using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
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

        private void Logon()
        {
            driver.Url = "https://www.zieloneimperium.pl";
            driver.Manage().Window.Maximize();
            //IWebElement link = driver.FindElement(By.ClassName("top"));

            IWebElement login = driver.FindElement(By.Id("login_user"));
            IWebElement password = driver.FindElement(By.Id("login_pass"));

            if (login.Displayed && password.Displayed)
            {
                login.SendKeys(DecodePassword.User);
                //login.SendKeys(Helper.Login);
                password.SendKeys(DecodePassword.Password);
                password.Submit();
            }

            Thread.Sleep(200);
        }

        [Test]
        public void Test()
        {
            Logon();

            //IWebElement menu = driver.FindElement(By.Id("menuButtons"));

            Helper helper = new Helper(driver);

            helper.CloseNewsFrames();

            List<ToolsMenu> toolsMenus = helper.GetToolElements();

            List<Seed> Seeds = helper.GetSeedRegal();

            List<Grid> tmp = helper.GetGridElements();
            //helper.GetLeftTime(tmp);

            helper.GatherUpPlant(tmp, toolsMenus);
            helper.UpdateWateringActive(tmp);
            helper.UpdateSowingActive(tmp);

            helper.DropSeed(Seeds,tmp,toolsMenus,"Marchew");
            helper.UpdateWateringActive(tmp);
            helper.UpdateSowingActive(tmp);

            helper.Wait(3000);
           // helper.UpdateWateringActive(tmp);
            helper.Watering(Seeds, tmp, toolsMenus);
            //helper.UpdateWateringActive(tmp);
            //helper.UpdateSowingActive(tmp);

            //List<IWebElement> pola = new List<IWebElement>();

        }

        [Test]
        public void CheckSwitchingGardens()
        {
            Logon();

            Helper helper = new Helper(driver);

            helper.OpenMap();
            helper.GoToGarden(helper.GetGardens(), 2);
            helper.GetGardenInfo().GardenNumber.Should().Be(2);
            helper.OpenMap();
            helper.GoToGarden(helper.GetGardens(), 3);
            helper.GetGardenInfo().GardenNumber.Should().Be(3);

        }

        [Test]
        public void CheckThenAllOfPlantsAreGatherUpByHelperHarvest()
        {
          //  DecodePassword newDecodePassword = new DecodePassword();
            Logon();

            Helper helper = new Helper(driver);

            helper.CloseNewsFrames();
            // helper.openMap();
            // helper.GoToGarden(helper.GetGardens(),2);
            // helper.GetGardenInfo().GardenNumber.Should().Be(2);


            List<Grid> plants = helper.GetGridElements();
            int readyToDropBeforeGatherUp = helper.CountReadyToDrop(plants);
            int readyToGatherUpBeforeGatherUp = helper.CountReadyToGather(plants);

            helper.GetAllActiveHarvest(plants);
            int readyToDropAfterGatherUp = helper.CountReadyToDrop(plants);
            int readyToGatherUpAfterGatherUp = helper.CountReadyToGather(plants);

            if (readyToDropBeforeGatherUp != readyToDropAfterGatherUp &&
                readyToGatherUpBeforeGatherUp != readyToGatherUpAfterGatherUp)
            {
                readyToGatherUpAfterGatherUp.Should().Be(204 - (readyToDropAfterGatherUp + readyToDropBeforeGatherUp));
            }
            
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver.Close();
            
        }

    }
}
