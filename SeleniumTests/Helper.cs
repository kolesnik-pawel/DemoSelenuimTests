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

        /// <summary>
        ///  Get For page information about Name of Plant and Left Time to Growup
        /// Plant name return only if its sown, else return empty string
        /// Left Time 
        /// </summary>
        /// <param name="plant"></param>
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

        /// <summary>
        /// Convert string to TimeSpan
        /// Count hours to days if possible
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private TimeSpan ConvertStringToTimeSpan(string time)
        {
            int hours = int.Parse(time.Remove(2));
            string minutesAndSeconds = time.Remove(0, 2);
            int days = 0;

            if (hours >= 24)
            {
                days = hours / 24;
                hours = hours % 24;
            }

            return TimeSpan.Parse($"{days}:{hours}{minutesAndSeconds}");
        }

        /// <summary>
        /// Span can return left time to gather or string 'Ready'
        /// Method check it and fill right field 
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="ReadOffTime"></param>
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

        /// <summary>
        /// Check then a field contain a plant
        /// Setup variable Ready To Sowing If field is empty
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool SetupReadyToSowing(IWebElement element)
        {
            try
            {
                if (element.FindElements(By.TagName("div"))[0].Displayed)
                {
                    if (element.FindElements(By.TagName("div"))[0].GetAttribute("class").Contains("plantImage") &&
                        element.FindElements(By.TagName("div"))[0].GetAttribute("style").Contains("0.gif"))
                    {
                        return true;
                    }
                }
            }
            catch (StaleElementReferenceException e)
            {
                Console.WriteLine(e);
                throw;
            }

            return false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="driver"></param>
        public Helper(IWebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Red all information about field and plant
        /// Save data in to List of Grid model 
        /// </summary>
        /// <returns></returns>
        public  List<Grid> GetGridElements()
        {
            List<Grid> result = new List<Grid>();
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
                   Wait(20);

                   readyToSowing = SetupReadyToSowing(element);

                   if (element.Displayed)
                   {
                       result.Add(new Grid()
                       {
                           Cell = element,
                           Raw = int.Parse(element.GetAttribute("class").Split()[1].Replace("row", "")),
                           Col = int.Parse(element.GetAttribute("class").Split()[2].Replace("col", "")),
                           Id = $"gardenTile{elementCount}",
                           Water = watter.GetAttribute("src").Contains("gegossen.gif"),
                           ReadyToDrop = readyToSowing

                       });

                       ReadLeftTimeForPlantAndPlantName(result.Last());

                       log.LogWrite(result.Last().Log());
                   }
                }
                catch (Exception e)
                {
                    //throw e;
                    continueValue = false;
                }
            } while (continueValue);

            return result;
        }

        /// <summary>
        /// Check if plant are watering 
        /// </summary>
        /// <param name="plants"></param>
        public void UpdateWateringActive(List<Grid> plants)
        {
            log.LogWrite("Start UpdateWateringActive");
            foreach (var plant in plants)
            {
                Wait(30);
                try
                {
                    if (driver.FindElement(By.Id($"{plant.Id}_water")).Displayed)
                    {
                        driver.FindElement(By.Id($"{plant.Id}_water")).GetAttribute("src")
                            .Contains("gegossen.gif");
                    }
                }
                catch (Exception e)
                {
                   log.LogWriteError($"Watering Error; plant log : {plant.Log()} ",e);
                    continue;
                }
            }
            log.LogWrite("End UpdateWateringActive");
        }

        /// <summary>
        /// Check if field are ready to Sowing 
        /// </summary>
        /// <param name="plants"></param>
        public void UpdateSowingActive(List<Grid> plants)
        {
            log.LogWrite("Start UpdateSowingActive");
            foreach (var plant in plants)
            {
                Wait(25);
                log.LogWrite(plant.Id);
                plant.ReadyToDrop = SetupReadyToSowing(plant.Cell);
            }
            log.LogWrite("End UpdateSowingActive");
        }

        /// <summary>
        /// Update left time to Gather
        /// </summary>
        /// <param name="plant"></param>
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
                
                Wait(20);
                ReadLeftTimeForPlantAndPlantName(cell);
            }
        }

        /// <summary>
        /// Get Tolls and save it to List of ToolsMenu model
        /// </summary>
        /// <returns></returns>
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
               log.LogWrite(result.Last().Log());
            }

            return result;
        }

        /// <summary>
        /// Gets Seed from Regal
        /// Regal are change to next if its possible
        /// Seed are save to List of Seed model
        /// </summary>
        /// <returns></returns>
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
                log.LogWrite(results.Last().Log());
            }

            return results;
        }

        /// <summary>
        /// Simulate click at element
        /// </summary>
        /// <param name="element"></param>
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
                log.LogWriteError($"Click At Element: {element} ", e);
                Console.WriteLine(e);
                throw;
            }
        }

        public void ClickAtElement(IWebElement element, string description)
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
                log.LogWriteError($"Click At Element: {element}, Description: {description} ", e);
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Overloading method ClickAtElement
        /// </summary>
        /// <param name="elements"></param>
        public void ClickAtElement(List<IWebElement> elements)
        {
            Actions actions = new Actions(driver);
            foreach (var element in elements)
            {
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
        }

        /// <summary>
        /// Wait method
        /// </summary>
        /// <param name="miliseconds"></param>
        public void Wait(int miliseconds)
        {
            DateTime start = DateTime.Now;
           
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(miliseconds));
            Thread.Sleep(miliseconds);
            DateTime end = DateTime.Now;
            //log.LogWrite($"Wait time {end - start } ");
        }

        /// <summary>
        /// Method to Gather up plants
        /// Update List of Grid model
        /// </summary>
        /// <param name="plants"></param>
        /// <param name="tools"></param>
        public void GatherUpPlant(List<Grid> plants, List<ToolsMenu> tools)
        {
            ClickAtElement(tools.Find(x => x.Name == BaseKey.GatherTool).Toll);
            //ClickAtElement(tools.Where(x => x.Name == BaseKey.GatherTool));

            foreach (var plant in plants) //.Where(x => x.RedyToGather == true))
            {
                if (plant.RedyToGather)
                {
                    ClickAtElement(plant.Cell);
                    Wait(20);
                    log.LogWrite($"Gather Up Plant : {plant.Id}, {plant.PlantName}");
                    plant.RedyToGather = false;
                    plant.ReadyToDrop = true;
                    plant.PlantName = string.Empty;
                    log.LogWrite($"Gather Up Plant : {plant.Log()}");
                }
            }
        }

        /// <summary>
        /// Method to Drop Seeds at field
        /// Update List of Grid model
        /// </summary>
        /// <param name="seeds"></param>
        /// <param name="plants"></param>
        /// <param name="tools"></param>
        /// <param name="dropSeedName"></param>
        public void DropSeed(List<Seed> seeds, List<Grid> plants, List<ToolsMenu> tools, string dropSeedName)
        {
            log.LogWrite("Start DropSeeds");
            GoToPage(seeds.Find(x => x.Name == dropSeedName).RegalNumber);
            /// Select tool for sowing 
            ClickAtElement(tools.Find(x => x.Name == BaseKey.SowingTool).Toll, "SowingTool");
            Wait(100);
            ///Select a seed to drop
            ClickAtElement(seeds.Find(x => x.Name == dropSeedName).SeedRegal, $"Select {dropSeedName}");
            Wait(100);
            /// Drop seed at empty plans
            foreach (var plant in plants) //.Where(x => x.ReadyToDrop == true))
            {
                if (plant.ReadyToDrop)
                {
                    ClickAtElement(plant.Cell, plant.Log());
                    Wait(100);
                    log.LogWrite($"Seeds plant : {plant.Id}, {plant.PlantName}");
                    plant.ReadyToDrop = false;
                    plant.RedyToGather = false;
                    plant.PlantName = dropSeedName;
                    plant.Water = false;
                    plant.TimeLeft = TimeSpan.Zero;
                    log.LogWrite($"Seeds plant : {plant.Log()} ");
                }
            }
            Wait(2000);
        }

        /// <summary>
        ///  Method to Watering plants
        ///  Update List of Grid model
        /// </summary>
        /// <param name="seeds"></param>
        /// <param name="plants"></param>
        /// <param name="tools"></param>
        public void Watering(List<Seed> seeds, List<Grid> plants, List<ToolsMenu> tools)
        {
            log.LogWrite("Start Watering");
            /// Select tool for watering 
            ClickAtElement(tools.Find(x => x.Name == BaseKey.WateringTool).Toll);
            Wait(100);
            foreach (var plant in plants)//.Where(x => x.ReadyToDrop == true))
            {
                if (plant.Water == false)
                {
                    ClickAtElement(plant.Cell);
                    Wait(150);
                    plant.Water = true;
                    log.LogWrite($"Watering {plant.Id}, {plant.Water}");
                }
            }
        }

        /// <summary>
        /// Method reset Regal of seed to default view 
        /// </summary>
        public void ResetsRegal()
        {
            IWebElement arrowLeft = driver.FindElement(By.XPath("//*[@id='lager_arrow_left']"));
            Wait(30);
            while (arrowLeft.GetAttribute("src").Contains("links_disabled.2.gif") == false)
            {
                ClickAtElement(driver.FindElement(By.XPath("//*[@id='lager_arrow_left']")));
            }
        }

        /// <summary>
        /// Method go to indicated Regal of seeds
        /// </summary>
        /// <param name="pageNumber"></param>
        public void GoToPage(int pageNumber)
        {
            ResetsRegal();

            for (int i = 1; i < pageNumber; i++)
            {
                ClickAtElement(driver.FindElement(By.XPath("//*[@id='lager_arrow_right']")));
            }
        }


        /// <summary>
        /// Closing welcome pop ups 
        /// </summary>
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
