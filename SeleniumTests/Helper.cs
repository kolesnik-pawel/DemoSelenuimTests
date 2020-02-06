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
        //public static string Login = "pablo0510";

        //public static string Password = "kosmos";

        private  IWebDriver driver;

        private LogWriter log  = new LogWriter("__________________________________________________________________\n\r \t\t\t\t\t\t\t\t Helper Start");

        /// <summary>
        ///  Get For page information about Name of Plant and Left Time to Growup
        /// Plant name return only if its sown, else return empty string
        /// Left Time 
        /// </summary>
        /// <param name="plant"></param>
        private void GatherUpReadyLeftTimeForPlantAndPlantName(Grid plant)
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

                    if (options.Count < 4)
                    {
                        if (options[1].Text.Contains(BaseKey.Decorations))
                        {
                            plant.Decoration = true;
                            SetupGridReadyToGatherOrTimeToLeft(plant, options[2].Text.Replace(BaseKey.DecorationReadyInTime, ""));
                            plant.RedyToGather = false;
                        }
                    }
                    else
                    {
                        plant.Decoration = false;
                        SetupGridReadyToGatherOrTimeToLeft(plant, options[3].Text.Replace(BaseKey.ReadyInTime, ""));
                    }
                }
            }
            else
            {
                plant.RedyToGather = false;
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
                plant.TimeLeft = TimeSpan.Zero;
            }
            else
            {
                plant.RedyToGather = false;
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
        private bool SetupReadyToSowing(string elementId)
        {
            IWebElement element = driver.FindElement(By.Id(elementId));
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

                       GatherUpReadyLeftTimeForPlantAndPlantName(result.Last());

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
                plant.ReadyToDrop = SetupReadyToSowing(plant.Id);
            }
            log.LogWrite("End UpdateSowingActive");
        }

        public void UpdateGatherUpActive(List<Grid> plants)
        {
            log.LogWrite("Start UpdateSowingActive");

            foreach (var plant in plants)
            {
                Wait(25);
                GatherUpReadyLeftTimeForPlantAndPlantName(plant);
            }
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
                GatherUpReadyLeftTimeForPlantAndPlantName(cell);
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

        /// <summary>
        ///  Simulate click at element
        ///  Log with description
        /// </summary>
        /// <param name="element"></param>
        /// <param name="description"></param>
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
            new WebDriverWait(driver, TimeSpan.FromMilliseconds(miliseconds));
            Thread.Sleep(miliseconds);
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
            if (plants.Select(x => x.TimeLeft < TimeSpan.FromSeconds(30)).ToList().Count > 0)
            {
                Wait(30);
            }

            foreach (var plant in plants.Where(x => x.RedyToGather == true))
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
                    ClickAtElement(driver.FindElement(By.Id(plant.Id)), plant.Log());
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
                    ClickAtElement(driver.FindElement(By.Id(plant.Id)));
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
        public void CloseNewsFrames()
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

        /// <summary>
        /// open Map pop up
        /// </summary>
        public void OpenMap()
        {
           ClickAtElement( driver.FindElement(By.Id(BaseKey.Bike)));
           Wait(200);
        }

        /// <summary>
        /// Get List of Garden
        /// </summary>
        /// <returns>List<Garden></returns>
        public List<Garden> GetGardens()
        {
            List<Garden> result = new List<Garden>();

            driver.SwitchTo().Frame(driver.FindElement(By.Name("multiframe")));

            ReadOnlyCollection<IWebElement> gardens = driver.FindElements(By.ClassName("link"));

            foreach (var garden in gardens)
            {
                
                if (garden.GetAttribute("onclick") != null && garden.GetAttribute("onclick").Contains(BaseKey.GardenSelectMethodOnClick))
                {
                    result.Add(new Garden()
                    {
                        Id = int.Parse(garden.GetAttribute("onclick").Replace(BaseKey.GardenSelectMethodOnClick, "").Remove(1)),
                        Name = garden.GetAttribute("onclick"),
                        IsActive = garden.GetAttribute("onmouseover") == null ? true : false,
                        WebElement = garden
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Switch gardens 
        /// </summary>
        /// <param name="gardens"></param>
        /// <param name="number"></param>
        public void GoToGarden(List<Garden> gardens, int number)
        {
            ClickAtElement(gardens.First(x => x.Id == number).WebElement);
            Wait(500);
            log.LogWrite(driver.CurrentWindowHandle);
            driver.SwitchTo().DefaultContent();
            log.LogWrite(driver.CurrentWindowHandle);
        }

        /// <summary>
        /// Gets Information about garden number and Server number 
        /// </summary>
        /// <returns></returns>
        public ServerGardenInfo GetGardenInfo()
        {
            ServerGardenInfo serverGardenInfo = new ServerGardenInfo();
            serverGardenInfo.ServerName =
                int.Parse(driver.FindElement(By.Id("menuServerInfo")).Text.Replace("Serwer: ", "").Remove(1));
            serverGardenInfo.GardenNumber =
                int.Parse(driver.FindElement(By.XPath("//*[@id='menuServerInfo']/span")).Text);

           return serverGardenInfo;
        }

        /// <summary>
        /// Method use a Helper Harvest to gather up plant
        /// </summary>
        /// <param name="plants"></param>
        public void GetAllActiveHarvest(List<Grid> plants)
        {
            log.LogWrite("Start method : 'GetAllActiveHarvest' ");

            ClickAtElement(driver.FindElement(By.ClassName("harvest")));
            Wait(3000);
            try
            {
                if (driver.FindElement(By.XPath("//*[@id='baseDialogText']/div")).Text.Contains(BaseKey.NothingToGatherUp))
                {
                    ClickAtElement(driver.FindElement(By.XPath("//*[@id='baseDialogButton']")));
                }
            }
            catch (Exception e)
            {
                ClickAtElement(driver.FindElement(By.XPath("//*[@id='ernte_log']/img")));
            }
            //if (driver.FindElement(By.XPath("//*[@id='baseDialogText']/div")).Text.Contains(BaseKey.NothingToGatherUp))
            //{
            //    ClickAtElement(driver.FindElement(By.XPath("//*[@id='baseDialogButton']")));
            //}
            //else
            //{
            //    ClickAtElement(driver.FindElement(By.XPath("//*[@id='ernte_log']/img")));
            //}
            
            UpdateSowingActive(plants);
            UpdateGatherUpActive(plants);

            log.LogWrite("End method : 'GetAllActiveHarvest' ");
        }

        /// <summary>
        /// Count plants ready to gather
        /// </summary>
        /// <param name="plants"></param>
        /// <returns></returns>
        public int CountReadyToGather(List<Grid> plants)
        {
            log.LogWrite("Start method: 'CountReadyToGather' ");
            int count = plants.Count(x => x.RedyToGather);
            log.LogWrite($"Method 'CountReadyToGather' return : {count}");
            return count;
        }

        /// <summary>
        /// Count plants ready to Drop 
        /// </summary>
        /// <param name="plants"></param>
        /// <returns></returns>
        public int CountReadyToDrop(List<Grid> plants)
        {
            log.LogWrite("Start method: 'CountReadyToDrop' ");
            int count = plants.Count(x => x.ReadyToDrop);
            log.LogWrite($"Method 'CountReadyToDrop' return : {count}");
            return count;
        }
    }
}
