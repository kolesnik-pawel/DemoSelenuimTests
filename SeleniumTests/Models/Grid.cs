using System;
using OpenQA.Selenium;

namespace SeleniumTests.Models
{
    public class Grid 
    {
        public IWebElement Cell { get; set; }

        public int Raw { get; set; }

        public int Col { get; set; }

        public string Id { get; set; }

        public string PlantName { get; set; }

        public bool RedyToGather { get; set; }

        public bool ReadyToDrop { get; set; }

        public TimeSpan TimeLeft { get; set; }

        public bool Water { get; set; }

        public string Log()
        {
            string log =
                $"Cell Id : {Id} Raw : {Raw.ToString()}, Column : {Col.ToString()}, Frut : {(PlantName == null ? "No Frut at plant " : PlantName)}," +
                $" Left Time To Growing : {TimeLeft}, Watering : {Water}, Ready To Gather : {ReadyToDrop}, Ready To Drop : {ReadyToDrop} ";
            return log;
        }

    }
}
