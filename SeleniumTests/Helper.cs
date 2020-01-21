using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Models;

namespace SeleniumTests
{
    class Helper
    {
        public static string Login = "pablo0510";

        public static string Password = "kosmos";

        private  IWebDriver driver;

        private LogWriter log  = new LogWriter("Helper Start");

        private void ReadLeftTimeForPlantAndPlantName(Grid plant)
        {
            plant.PlantName = string.Empty;
            if (plant.ReadyToDrop == false)
            {
                ClickAtElement(plant.Cell);
                if (driver.FindElement(By.XPath("//*[@id='sprcontent']")).Displayed)
                {
                    ReadOnlyCollection<IWebElement> options =
                        driver.FindElements(By.XPath("//*[@id='sprcontent']/div/span"));
                    plant.PlantName = options[0].Text;

                    SetupGridReadyToGatherOrTimeToLeft(plant, options[3].Text.Replace(BaseKey.ReadyInTime, ""));
                }
            }
        }
        private TimeSpan ConvertStringToTimeSpan(string time)
        {
            int hours = int.Parse(time.Remove(2));
            string minutsAndSeconds = time.Remove(0, 2);
            int days = 0;

            if (hours >= 24)
            {
                days = hours / 24;
                hours = hours % 24;
            }

            return TimeSpan.Parse($"{days}:{hours}{minutsAndSeconds}");
        }

        private void SetupGridReadyToGatherOrTimeToLeft(Grid plant, string ReadOffTime)
        {
            if (ReadOffTime == BaseKey.Ready)
            {
                plant.RedyToGather = true;
            }
            else
            {
                plant.TimeLeft = ConvertStringToTimeSpan(ReadOffTime);
            }
        }

        private bool SetupReadyToSowing(IWebElement element)
        {
            if (element.FindElements(By.TagName("div"))[0].GetAttribute("class").Contains("plantImage") &&
                element.FindElements(By.TagName("div"))[0].GetAttribute("style").Contains("0.gif"))
            {
              return  true;
               
            }
            else
            {
                return false;
            }
        }

        public Helper(IWebDriver driver)
        {
            this.driver = driver;
        }

        public  List<Grid> GetGridElements()
        {
            List<Grid> result = new List<Grid>();
            Grid plant = new Grid();
            IWebElement element;
            IWebElement watter;
            bool readyToSowing = false;
            int elementCount = 0;
            Thread.Sleep(100);
            
            Wait(200);

            bool continueValue = true;

            do
            {
                elementCount++;
                try
                {
                   element = driver.FindElement(By.Id($"gardenTile{elementCount}"));
                   watter = driver.FindElement(By.Id($"gardenTile{elementCount}")).FindElement(By.ClassName("wasser"));
                   Wait(10);

                   readyToSowing = SetupReadyToSowing(element);

                   if (element.Displayed)
                   {
                       plant.Cell = element;
                       plant.Raw = int.Parse(element.GetAttribute("class").Split()[1].Replace("row", ""));
                       plant.Col = int.Parse(element.GetAttribute("class").Split()[2].Replace("col", ""));
                       plant.Id = $"gardenTile{elementCount}";
                       plant.Water = watter.GetAttribute("src").Contains("gegossen.gif");
                       plant.ReadyToDrop = readyToSowing;
                       ReadLeftTimeForPlantAndPlantName(plant);

                       log.LogWrite(plant.Log());
                   }

                   result.Add(plant);
                }
                catch (Exception e)
                {
                    //throw e;
                    continueValue = false;
                }
            } while (continueValue);

            return result;
        }

        public void UpdateWateringActive(List<Grid> plants)
        {
            foreach (var plant in plants)
            {
                Wait(30);
               plant.Water = plant.Cell.FindElement(By.ClassName("wasser")).GetAttribute("src").Contains("gegossen.gif");
            }
        }

        public void UpdateSowingActive(List<Grid> plants)
        {
            foreach (var plant in plants)
            {
                if (plant.Cell != null && (plant.Cell.FindElements(By.TagName("div"))[0].GetAttribute("class").Contains("plantImage") &&
                                           plant.Cell.FindElements(By.TagName("div"))[0].GetAttribute("style").Contains("0.gif")))
                {
                    plant.ReadyToDrop = true;
                }
                else
                {
                    plant.ReadyToDrop = false;
                }
            }
        }

        public void GetLeftTime(List<Grid> plant)
        {
            string time = String.Empty;
            foreach (var cell in plant)
            {
                if (cell.ReadyToDrop)
                {
                    continue;
                }

                ClickAtElement(cell.Cell);
                
                Wait(300);
                ReadLeftTimeForPlantAndPlantName(cell);

                //ClickAtElement(cell.Cell);
                //if (driver.FindElement(By.XPath("//*[@id='sprcontent']")).Displayed)
                //{
                //    ReadOnlyCollection<IWebElement> options = driver.FindElements(By.XPath("//*[@id='sprcontent']/div/span"));
                //    cell.Frut = options[0].Text;

                //    SetupGridReadyToGatherOrTimeToLeft(cell, options[3].Text.Replace(BaseKey.ReadyInTime, ""));
                //}
            }
        }


        public List<ToolsMenu> GetToolElements()
        {
            List<ToolsMenu> result = new List<ToolsMenu>();

            ReadOnlyCollection<IWebElement> elements;

            elements = driver.FindElements(By.XPath("//*[@id='menuButtons']/*"));

            foreach (var element in elements)
            {
               result.Add(new ToolsMenu()
               {
                   Name = element.GetAttribute("Id"),
                   Toll = element
               }); 
            }

            return result;
        }

        public List<Seed> GetSeedRegal()
        {
            List<Seed> results = new List<Seed>();
            ReadOnlyCollection<IWebElement> elements;
            IWebElement SeedDescription;

            int pageNumber = 1;

            elements = driver.FindElements(By.XPath("//*[@id='regal']/*"));

            foreach (var element in elements)
            {
                if (element.GetAttribute("style").Contains("display: none"))
                {
                    Wait(30);
                    ClickAtElement(driver.FindElement(By.XPath("//*[@id='lager_arrow_right']")));
                    pageNumber++;
                    Wait(30);
                }
                ClickAtElement(element);
                Wait(20);
                SeedDescription = driver.FindElement(By.XPath("//*[@id='lager_name']"));
                results.Add(new Seed
                {
                    SeedRegal = element,
                    Id = element.GetAttribute("Id"),
                    Name = SeedDescription.Text,
                    RegalNumber = pageNumber
                });
                ClickAtElement(SeedDescription);
            }

            return results;
        }

        public void ClickAtElement(IWebElement element)
        {
            Actions actions = new Actions(driver);

            try
            {
                if (element.Displayed)
                {
                    actions.MoveToElement(element).Click().Perform();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public void ClickAtElement(List<IWebElement> elements)
        {
            Actions actions = new Actions(driver);
            foreach (var element in elements)
            {
                if (element.Displayed)
                {
                    actions.MoveToElement(element).Click().Perform();
                }
                else
                {
                    //  throw new Exception("Element do not displayed");
                }
            }
        }

        public void Wait(int miliseconds)
        {
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(miliseconds));
        }

        public void GatherUpPlant(List<Grid> plants, List<ToolsMenu> tools)
        {
            ClickAtElement(tools.Find(x => x.Name == BaseKey.GatherTool).Toll);
            //ClickAtElement(tools.Where(x => x.Name == BaseKey.GatherTool));

            foreach (var plant in plants.Where(x => x.RedyToGather == true))
            {
                if (plant.RedyToGather)
                {
                    ClickAtElement(plant.Cell);
                    Wait(120);
                    plant.RedyToGather = false;
                    plant.ReadyToDrop = true;
                }
            }
        }

        public void DropSeed(List<Seed> seeds, List<Grid> plants, List<ToolsMenu> tools, string dropSeedName)
        {
            GoToPage(seeds.Find(x => x.Name == dropSeedName).RegalNumber);
            /// Select tool for sowing 
            ClickAtElement(tools.Find(x => x.Name == BaseKey.SowingTool).Toll);
            Wait(100);
            ///Select a seed to drop
            ClickAtElement(seeds.Find(x => x.Name == dropSeedName).SeedRegal);
            Wait(100);
            /// Drop seed at empty plans
            foreach (var plant in plants.Where(x => x.ReadyToDrop == true))
            {

                if (plant.ReadyToDrop)
                {
                    ClickAtElement(plant.Cell);
                    Wait(2000);
                    plant.ReadyToDrop = false;
                    plant.RedyToGather = false;
                }
            }
        }

        public void Wathering(List<Seed> seeds, List<Grid> plants, List<ToolsMenu> tools)
        {
            /// Select tool for watering 
            ClickAtElement(tools.Find(x => x.Name == BaseKey.WateringTool).Toll);
            Wait(100);
            foreach (var plant in plants.Where(x => x.ReadyToDrop == true))
            {
                ClickAtElement(plant.Cell);
                Wait(150);
                plant.Water = true;
            }
        }

        public void RestestRegal()
        {
            IWebElement arrowLeft = driver.FindElement(By.XPath("//*[@id='lager_arrow_left']"));
            Wait(30);
            while (arrowLeft.GetAttribute("src").Contains("links_disabled.2.gif") == false)
            {
                ClickAtElement(driver.FindElement(By.XPath("//*[@id='lager_arrow_left']")));
            }
        }

        public void GoToPage(int pageNumber)
        {
            RestestRegal();

            for (int i = 1; i < pageNumber; i++)
            {
                ClickAtElement(driver.FindElement(By.XPath("//*[@id='lager_arrow_right']")));
            }
        }

        public void closeNewsFrames()
        {
            if (driver.FindElement(By.XPath("//*[@id='newszwergLayer']")).GetAttribute("style")
                .Contains("display: block;"))
            {
                ClickAtElement(
                    driver.FindElement(By.XPath("//*[@id='newszwergLayer']/img")));
            }

            if (driver.FindElement(By.XPath("//*[@id='dailyloginbonus']")).GetAttribute("style")
                .Contains("display: block;"))
            {
                ClickAtElement(
                    driver.FindElement(By.XPath("//*[@id='dailyloginbonus']/div[2]/div[6]")));
            }
        }
    }
}
