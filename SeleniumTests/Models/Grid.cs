using System;
using OpenQA.Selenium;

namespace SeleniumTests.Models
{
    /// <summary>
    /// model field 
    /// </summary>
    public class Grid 
    {
        /// <summary>
        /// Web Element field
        /// </summary>
        public IWebElement Cell { get; set; }

        /// <summary>
        /// Raw position at grid
        /// </summary>
        public int Raw { get; set; }

        /// <summary>
        /// Column position at grid
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// HTML Id attribute 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Plant Name
        /// </summary>
        public string PlantName { get; set; }

        /// <summary>
        /// Bool information if plant is Read to Gather
        /// </summary>
        public bool RedyToGather { get; set; }

        /// <summary>
        /// Bool information if field is Ready to Sowing
        /// </summary>
        public bool ReadyToDrop { get; set; }

        /// <summary>
        /// Time Left to Gather
        /// </summary>
        public TimeSpan TimeLeft { get; set; }

        /// <summary>
        /// Bool information if plant are Watering
        /// </summary>
        public bool Water { get; set; }

        /// <summary>
        /// Set true if plant are decoration 
        /// </summary>
        public bool Decoration { get; set; }

        /// <summary>
        /// Method to logging 
        /// </summary>
        /// <returns></returns>
        public string Log()
        {
            string log;

            if (Decoration)
            {
                log = $"Cell Id : {Id}, Raw : {Raw.ToString()}, Column : {Col.ToString()}, Decoration : {(PlantName == null ? "No Decoration" : PlantName)}," +
                      $" Left Time to tear down : {TimeLeft}";
            }
            else
            {
                log =
                $"Cell Id : {Id}, Raw : {Raw.ToString()}, Column : {Col.ToString()}, Frut : {(PlantName == null ? "No Frut at plant " : PlantName)}," +
                $" Left Time To Growing : {TimeLeft}, Watering : {Water}, Ready To Gather : {RedyToGather}, Ready To Drop : {ReadyToDrop} ";
            }
            
            return log;
        }
    }
}
