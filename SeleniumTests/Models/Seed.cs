using OpenQA.Selenium;

namespace SeleniumTests.Models
{
    /// <summary>
    /// Seed model
    /// </summary>
    public class Seed
    {
        /// <summary>
        /// Web Element Seed at Regal
        /// </summary>
        public IWebElement SeedRegal;

        /// <summary>
        /// Seed name
        /// </summary>
        public string Name;

        /// <summary>
        /// HTML Id attribute 
        /// </summary>
        public string Id;

        /// <summary>
        /// Number of Regal
        /// </summary>
        public int RegalNumber;
    }
}
