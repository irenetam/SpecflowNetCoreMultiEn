using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TechTalk.SpecFlow;

namespace Altsource.Hooks
{
    [Binding]
    public class Hooks: GlobalFunction
    {
        private readonly IObjectContainer _objectContainer;
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver _driver;
        private WebDriverWait _wait;
        private readonly IConfiguration _configuration;

        private static AventStack.ExtentReports.ExtentReports report;
        private static ExtentTest featureName;
        private static ExtentTest scenarioName;
        public static string htmlReportFolder;
        public static string reportPath;
        public static string attachmentPath;

        public Hooks(IObjectContainer objectContainer, FeatureContext featureContext, ScenarioContext scenarioContext, IConfiguration configuration)
        {
            _objectContainer = objectContainer;
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _configuration = configuration;
        }
        private static IConfiguration config;
        [BeforeTestRun]
        public static void InitializeReport()
        {
            if (config == null)
            {
                var appSettings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
                var environmentName = appSettings["Environment"].ToString();
                config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", true)
                    .Build();
                var myconfig = new MyConfig();
                config
                .GetSection("MyConfig")
                .Bind(myconfig);
            
            }

            if (GetValueFromConfig("reportPath") != "")
            {
                reportPath = GetValueFromConfig("reportPath");
            }
            else
            {
                reportPath = @"C:\Reports\";
            }
            htmlReportFolder = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToString("MM-dd-yyyy_hh-mm-ss");
            var htmlReporter = new ExtentV3HtmlReporter(reportPath + "\\Reports_" + htmlReportFolder + "\\FinalReport.html");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            attachmentPath = reportPath + "\\Reports_" + htmlReportFolder + "\\FinalReport.html";

            report = new AventStack.ExtentReports.ExtentReports();
            report.AttachReporter(htmlReporter);
        }

        [AfterTestRun]
        public static void PrintReport()
        {
            report.Flush();
            //SendReportEmail(attachmentPath);
        }

        [Before]
        public void CreateFeatureNode()
        {
            featureName = report.CreateTest<Feature>(_featureContext.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void Initialize()
        {
            scenarioName = featureName.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);
            var param = new Dictionary<string, object>();

            ChromeOptions option = new ChromeOptions();
            option.PageLoadStrategy = PageLoadStrategy.Normal;

            headless = (GetValueFromConfig("headless") != "") ? bool.Parse(GetValueFromConfig("headless")) : true;
            if (headless)
            {
                option.AddArguments("--headless");

                // Add option to download file with headless mode
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                string dir = "c:\\Users\\" + userName + "\\Downloads";

                option.AddUserProfilePreference("download.prompt_for_download", "false");
                option.AddUserProfilePreference("download.directory_upgrade", "true");
                option.AddUserProfilePreference("download.prompt_for_download", "false");
                option.AddUserProfilePreference("safebrowsing.enabled", "false");
                option.AddUserProfilePreference("safebrowsing.disable_download_protection", "true");
                option.AddArguments("--disable-web-security");
                option.AddUserProfilePreference("download.default_directory", dir);

                param.Add("behavior", "allow");
                param.Add("downloadPath", dir);

                ChromeDriver drv = new ChromeDriver(ChromeDriverService.CreateDefaultService(), option, TimeSpan.FromMinutes(3));
                drv.ExecuteChromeCommand("Page.setDownloadBehavior", param);
                _driver = drv;

            }
            else
            {
                _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), option, TimeSpan.FromMinutes(3));
            }


            _driver.Manage().Window.Size = new Size(1920, 1080);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

            _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
            _objectContainer.RegisterInstanceAs<WebDriverWait>(_wait);
            _objectContainer.RegisterInstanceAs<IConfiguration>(config);
        }

        [AfterScenario]
        public void CleanUp()
        {

            _driver.Close();
            _driver.Dispose();
            _driver.Quit();
        }

        [BeforeStep]
        public void WaitForPageLoad()
        {
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [AfterStep]
        public void CreateStepNode()
        {
            var steptype = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();

            if (_scenarioContext.TestError == null)
            {
                switch (steptype)
                {
                    case "Given":
                        scenarioName.CreateNode<Given>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text);
                        break;
                    case "When":
                        scenarioName.CreateNode<When>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text);
                        break;
                    case "Then":
                        scenarioName.CreateNode<Then>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text);
                        break;
                }
            }
            else
            {
                string base64string = CaptureScreenShot(_driver);

                switch (steptype)
                {
                    case "Given":
                        scenarioName.CreateNode<Given>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64string).Build());
                        break;
                    case "When":
                        scenarioName.CreateNode<When>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64string).Build());
                        break;
                    case "Then":
                        scenarioName.CreateNode<Then>(steptype + " " + ScenarioStepContext.Current.StepInfo.Text).Fail(_scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64string).Build());
                        break;
                }
                //SendFailTestEmail("FAILED - " + ScenarioContext.Current.ScenarioInfo.Title, ScenarioContext.Current.TestError.Message);
            }
        }
    }
}
