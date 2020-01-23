using OpenQA.Selenium;

namespace SeleniumTests.Models
{
    /// <summary>
    /// Tools model
    /// </summary>
    class ToolsMenu
    {
        /// <summary>
        /// Web Element Tools 
        /// </summary>
        public IWebElement Toll { get; set; }

        /// <summary>
        /// Tool Name 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Log about Tools Menu
        /// </summary>
        /// <returns></returns>
        public string Log()
        {
            string log =
                $" Tool Name : {Name}";
            return log;
        }
    }
}
