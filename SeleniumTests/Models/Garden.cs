using OpenQA.Selenium;

namespace SeleniumTests.Models
{
    public class Garden
    {
        /// <summary>
        /// WebElement 
        /// </summary>
        public IWebElement WebElement { get; set; }

        /// <summary>
        /// Html Tag Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Garden Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Set Garden Active
        /// </summary>
        public  bool IsActive { get; set; }
    }
}
