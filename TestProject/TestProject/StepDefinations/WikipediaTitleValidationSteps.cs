using NUnit.Framework;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace Altsource.StepDefinations
{
    [Binding]
    public class WikipediaTitleValidationSteps
    {
        private readonly IWebDriver driver;
        public WikipediaTitleValidationSteps(IWebDriver webDriver)
        {
            driver = webDriver;
        }
        [Given(@"I have navigated to the ""(.*)"" page on Wikipedia")]
        public void GivenIHaveNavigatedToThePageOnWikipedia(string subject)
        {
            driver.Url = $"https://en.wikipedia.org/wiki/{subject}";
        }
        
        [Then(@"the title of the page should be ""(.*)""")]
        public void ThenTheTitleOfThePageShouldBe(string title)
        {
            Assert.AreEqual(title, driver.Title);
        }
    }
}
