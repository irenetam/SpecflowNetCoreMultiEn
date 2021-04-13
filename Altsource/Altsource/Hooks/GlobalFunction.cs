using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using TechTalk.SpecFlow;

namespace Altsource.Hooks
{
    public class GlobalFunction
    {
        public static string oldWindow;
        public static string customerfirstname;
        public static string customerssn;
        public static string customernumber;
        public static string servicenumber;
        public static string homephone;
        public static string env;
        public static bool headless;
        public static string retailactivationcode;
        public static string zipcode;
        public static string sim;
        public Table featureTable;
        public static string host = GetValueFromAppSettings().SMTPHost; // GetValueFromConfig("SMTPHost");
        public static string port = GetValueFromAppSettings().SMTPPort; // GetValueFromConfig("SMTPPort");
        public static string fromemail = GetValueFromAppSettings().ReportEmail;// GetValueFromConfig("ReportEmail");
        public static string emailpassword = GetValueFromAppSettings().ReportEmailPassword; //GetValueFromConfig("ReportEmailPassword");
        public static string toemail = GetValueFromAppSettings().RecipientsEmail; //GetValueFromConfig("RecipientsEmail");


        /// <summary>Determines whether element present.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///   <c>true</c> if [is element present] [the specified driver]; otherwise, <c>false</c>.</returns>
        public bool isElementPresent(IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        /// <summary>
        /// Captures the screen shot.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <returns>Image Base64</returns>
        /// <exception cref="ArgumentException">Fail to capture screenshot!</exception>
        public string CaptureScreenShot(IWebDriver driver)
        {
            try
            {
                ITakesScreenshot takesScreenshot = (ITakesScreenshot)driver;
                Screenshot screenshot = takesScreenshot.GetScreenshot();
                return screenshot.AsBase64EncodedString;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Fail to capture screenshot!", ex);
            }
        }

        /// <summary>Changes the value of an element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="value">The value.</param>
        public void ChangeValueOfAnElement(IWebDriver driver, By by, string value)
        {
            try
            {
                driver.FindElement(by).Click();
                ClearElementByJavascript(driver, by);
                driver.FindElement(by).SendKeys(value);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Cannot find element");
            }
        }

        /// <summary>Waits for element disappears.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void WaitForElementDisappears(IWebDriver driver, By by)
        {
            if (isElementPresent(driver, by))
            {
                WaitForElementDisappears(driver, by);
            }
            else
            {
                Console.WriteLine("Element disappears!");
            }

        }

        /// <summary>Waits for element appears.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void WaitForElementAppears(IWebDriver driver, By by)
        {
            if (!isElementPresent(driver, by))
            {
                WaitForElementAppears(driver, by);
            }
            else
            {
                Console.WriteLine("Element appears!");
            }
        }

        /// <summary>Clicks a dynamic element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///  true or false
        /// </returns>
        public bool ClickADynamicElement(IWebDriver driver, By by)
        {
            WaitForElementAppears(driver, by);

            try
            {
                if (isElementPresent(driver, by))
                {
                    driver.FindElement(by).Click();
                    return true;
                }
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>Verifies the text of element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   true or false
        /// </returns>
        public bool VerifyTextOfElement(IWebDriver driver, By by, string expected)
        {
            try
            {
                var elementText = GetTextOfElement(driver, by);
                Console.WriteLine(elementText + " - " + expected);
                Assert.AreEqual(true, elementText.Contains(expected));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>Deletes the old files.</summary>
        public static void DeleteOldFiles()
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Reports\");

            foreach (FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>Scrolls the until element visible.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void ScrollUntilElementVisible(IWebDriver driver, By by)
        {
            IWebElement element = driver.FindElement(by);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("arguments[0].scrollIntoView();", element);
        }

        /// <summary>Moves to an element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void MoveToElement(IWebDriver driver, By by)
        {
            IWebElement element = driver.FindElement(by);
            Actions act = new Actions(driver);
            Thread.Sleep(500);
            act.MoveToElement(element).Build().Perform();
        }

        /// <summary>Clicks an element by javascript.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void ClickByJavascript(IWebDriver driver, By by)
        {
            IWebElement element = driver.FindElement(by);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WaitForElement(driver, by, 15);

            js.ExecuteScript("arguments[0].click();", element);
        }

        /// <summary>Authentications this instance.</summary>
        public void Authentication()
        {
            Process proc = new Process();

            string appPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\"));
            proc.StartInfo.FileName = appPath + "Drivers\\Auto\\AutoIt\\authentication.exe";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
        }

        /// <summary>Verifies the element display.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///   true or false
        /// </returns>
        public bool VerifyElementDisplay(IWebDriver driver, By by)
        {
            var att = driver.FindElement(by).GetAttribute("style");

            if (att == "display: none;")
                return false;
            else
                return true;
        }

        /// <summary>Gets the text of element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///   text of specific element
        /// </returns>
        public string GetTextOfElement(IWebDriver driver, By by)
        {
            WaitForElement(driver, by, 60);

            var elementText = driver.FindElement(by).Text;
            if (elementText == "")
            {
                elementText = driver.FindElement(by).GetProperty("value");

                if (elementText == "")
                {
                    elementText = driver.FindElement(by).GetAttribute("textContent");
                }
            }
            return elementText;
        }

        /// <summary>Clears the element by javascript.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void ClearElementByJavascript(IWebDriver driver, By by)
        {
            IWebElement element = driver.FindElement(by);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("arguments[0].value = ''; ", element);
        }

        /// <summary>Scrolls the page.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="position">The position.</param>
        public void ScrollPage(IWebDriver driver, string position)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            switch (position)
            {
                case "top":
                    js.ExecuteScript("window.scrollTo(0,0)");
                    break;
                case "middle":
                    js.ExecuteScript("window.scrollTo(0,document.body.scrollHeight/2)");
                    break;
                case "bottom":
                    js.ExecuteScript("window.scrollTo(0,document.body.scrollHeight)");
                    break;
            }
        }

        /// <summary>Converts to us time.</summary>
        /// <param name="current">The current.</param>
        /// <returns>
        ///   date time with expected format
        /// </returns>
        public string ConvertToUSTime(string current)
        {
            DateTime today = DateTime.Now.Date;
            string stoday = today.ToString("MMM dd yyyy") + " " + current;
            DateTime converted = Convert.ToDateTime(stoday);

            TimeZoneInfo ptz = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime newdate = TimeZoneInfo.ConvertTime(converted, ptz);

            return newdate.ToString("MMM dd yyyy h:mm tt");
        }

        /// <summary>Waits for element.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   element after specific time out
        /// </returns>
        public IWebElement WaitForElement(IWebDriver driver, By by, int timeout)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(drv => drv.FindElement(by));
        }

        /// <summary>Navigates to cci application.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="app">The application.</param>
        public void NavigateToCCIApp(IWebDriver driver, string app)
        {
            if (((app == "Intranet" || app == "Fulfillment" || app == "Mine") && GetEnvironmentFromURL(GetValueFromConfig(app)) == "QA") || (app == "Kibana" && GetEnvironmentFromURL(GetValueFromConfig(app)) == "PROD"))
            {
                GetWebDriverWithCookie(driver, app);
            }
            else
            {
                Console.WriteLine("Application does not need authentication!");
            }

            if (app == "CCGoInternal")
            {
                driver.Navigate().GoToUrl(GetValueFromConfig(app) + GetValueFromConfig("AccountNumberHasCCGo"));
            }
            else
            {
                driver.Navigate().GoToUrl(GetValueFromConfig(app));
            }
        }

        /// <summary>Generates the phone number.</summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   random string for phone number format
        /// </returns>
        public string GeneratePhoneNumber(string input)
        {
            var rand = new Random();
            var mid = rand.Next(200, 999);
            var lastfour = rand.Next(1000, 9999);
            return input.Substring(0, 4) + mid.ToString() + lastfour.ToString();
        }

        /// <summary>Randoms the string.</summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///   return a random string with specific size
        /// </returns>
        public string RandomString(int size)
        {
            var rand = new Random();
            var builder = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                double flt = rand.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                char letter = Convert.ToChar(shift + 65);
                builder.Append(letter);
            }

            return builder.ToString();
        }

        /// <summary>Waits for disappears.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   true or false
        /// </returns>
        public bool WaitForDisappears(IWebDriver driver, By by, int timeout)
        {
            int size;

            for (int i = 0; i < timeout * 5; i++)
            {
                size = driver.FindElements(by).Count;
                Thread.Sleep(200);
                if (size == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Waits for page loading.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout.</param>
        public void WaitForPageLoading(IWebDriver driver, int timeout)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <summary>Waits for public site load.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   true or false
        /// </returns>
        public bool WaitForPublicSiteLoad(IWebDriver driver, By by, int timeout)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout)).Until(drv => drv.FindElement(by).GetAttribute("style") == "display: none;");
        }

        /// <summary>Determines whether the specified driver is selected.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///   <c>true</c> if the specified driver is selected; otherwise, <c>false</c>.</returns>
        public bool isSelected(IWebDriver driver, By by)
        {
            if (driver.FindElement(by).GetAttribute("class").ToString().Contains("selected"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Selects an option.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        public void SelectAnOption(IWebDriver driver, By by)
        {
            if (!isSelected(driver, by))
            {
                ClickByJavascript(driver, by);
            }
            else
            {
                Console.WriteLine("Element selected!");
            }
        }

        /// <summary>Waits for element display.</summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   true or false
        /// </returns>
        public bool WaitForElementDisplay(IWebDriver driver, By by, int timeout)
        {
            WaitForElement(driver, by, 10);
            var display = driver.FindElement(by).GetAttribute("style");

            for (int i = 0; i < timeout * 5; i++)
            {
                if (!display.Contains("display: none;"))
                {
                    return true;
                }
                Thread.Sleep(200);
                display = driver.FindElement(by).GetAttribute("style");
            }
            return false;
        }

        /// <summary>Saves the customer information.</summary>
        /// <param name="firstname">The firstname.</param>
        /// <param name="ssn">The SSN.</param>
        public void SaveCustomerInformation(string firstname, string ssn, string cnumber, string lineservice, string home)
        {
            customerfirstname = firstname;
            customerssn = ssn;
            customernumber = cnumber;
            servicenumber = lineservice;
            homephone = home;
            Console.WriteLine(customerfirstname + customerssn + customernumber + servicenumber + homephone);
        }

        /// <summary>Gets the customer information.</summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   customer information based on type
        /// </returns>
        /// <exception cref="InvalidOperationException">No customer information</exception>
        public string GetCustomerInformation(string type)
        {
            switch (type)
            {
                case "firstname":
                    return customerfirstname;
                case "lastssn":
                    return customerssn;
                case "customernumber":
                    return customernumber;
                case "servicenumber":
                    return servicenumber;
                case "homephone":
                    return homephone;
                default:
                    throw new InvalidOperationException("No customer information");
            }
        }

        /// <summary>
        ///     Switch to new frame in application
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The By.</param>
        public void SwitchToNewFrame(IWebDriver driver, By by)
        {
            driver.SwitchTo().Frame(driver.FindElement(by));
        }

        /// <summary>
        /// Gets the value from configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     value of the key from config
        /// </returns>
        public static string GetValueFromConfig(string key)
        {
            return (ConfigurationManager.AppSettings[key] != "") ? ConfigurationManager.AppSettings[key] : "";
        }

        /// <summary>
        /// Gets the web driver with cookie.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <returns>
        ///     webdriver with cookies
        /// </returns>
        public void GetWebDriverWithCookie(IWebDriver driver, string app)
        {
            var username = ReplaceSpecialChar(GetValueFromConfig("Username"));
            var password = ReplaceSpecialChar(GetValueFromConfig("Password"));
            var appurl = CutURL(GetValueFromConfig(app));
            var urlprefix = CutURLPrefix(GetValueFromConfig(app));
            driver.Navigate().GoToUrl(urlprefix + username + ":" + password + "@" + appurl);
        }

        /// <summary>
        /// Cuts the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        ///     url without prefix
        /// </returns>
        public string CutURL(string url)
        {
            return (url.Contains("https")) ? url.Substring(8) : url.Substring(7);
        }

        /// <summary>
        /// Cuts the URL prefix.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        ///     return prefix of an url
        /// </returns>
        public string CutURLPrefix(string url)
        {
            return (url.Contains("https")) ? "https://" : "http://";
        }

        /// <summary>
        ///     Navigate to Kibana page with filter
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="logValue">Filter value.</param>
        public void NavigateToKibanaPage(IWebDriver driver, string logValue)
        {
            var kibanaLink = GetValueFromConfig("Kibana") + "discover?_g=(refreshInterval:(pause:!t,value:0),time:(from:now-3m,mode:relative,to:now))&_a=(columns:!(Level,Message),filters:!(('$state':(store:appState),meta:(alias:!n,disabled:!f,index:c7324cd0-6a93-11e9-b72e-a9fd0e7c3041,key:MetaData.TopicId,negate:!f,params:(query:" + logValue + ",type:phrase),type:phrase,value:" + logValue + "),query:(match:(MetaData.TopicId:(query:" + logValue + ",type:phrase))))),index:c7324cd0-6a93-11e9-b72e-a9fd0e7c3041,interval:auto,query:(language:lucene,query:''),sort:!(Timestamp,desc))";
            driver.Navigate().GoToUrl(kibanaLink);
        }

        /// <summary>
        ///     Get current environment
        /// </summary>
        /// <param name="url">The application url.</param>
        /// <returns>
        ///     Environment value
        /// </returns>
        public string GetEnvironemnt(string url)
        {
            return (url.Contains("release")) ? "Qa" : "Release";
        }

        /// <summary>
        /// Gets the customer number.
        /// </summary>
        /// <param name="scenario">The scenario.</param>
        /// <returns>
        ///     customer number for specific test scenario
        /// </returns>
        public string GetCustomerNumber(string scenario)
        {
            switch (scenario)
            {
                case "CM - Print Invoice PDF":
                    return GetValueFromConfig("AccountNumberHasInvoices");
                case "API - Grandpad user information will be returned via grandpad api":
                    return GetValueFromConfig("AccountNumberHasGrandPad");
                case "CCG - CCGo flow on My Account":
                    return GetValueFromConfig("AccountNumberHasCCGo");
                case "CCG - CCGo Customer Support external flow ":
                    return GetValueFromConfig("AccountNumberHasCCGo");
                case "CM - Warranty Service of Apple Care plus":
                    return GetValueFromConfig("AccountNumberAppleCarePlus");
                case "CM - Warranty Service of Square Trade":
                    return GetValueFromConfig("AccountNumberSquareTrade");
                //case "CM - Intranet Carrier TMobile":
                //    return GetValueFromConfig("AccountNumberCarrier");
                //case "CM - Intranet Carrier ATT":
                //    return GetValueFromConfig("AccountNumberCarrier");
                default:
                    return GetValueFromConfig("AccountNumber");
            }
        }

        /// <summary>
        ///     Check Radio button is selected
        /// </summary>
        /// <param name="driver"> The driver.</param>
        /// <param name="by">The by.</param>
        /// <returns>
        ///     True or False
        /// </returns>
        public bool IsRadioButtonChecked(IWebDriver driver, By by)
        {
            var result = false;
            var elements = driver.FindElement(by).FindElements(By.TagName("input"));
            foreach (IWebElement ele in elements)
            {
                if (ele.GetAttribute("type").Contains("radio"))
                {
                    if (ele.GetAttribute("aria-checked") == "true")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Replace . or : sign to url format
        /// </summary>
        /// <param name="stringReplace"> String need to replace</param>
        /// <returns>
        ///     New string with url format
        /// </returns>
        public string ReplaceSpecialChar(string stringReplace)
        {
            return HttpUtility.UrlEncode(stringReplace);
        }

        /// <summary>
        /// Gets the environment from URL.
        /// </summary>
        /// <param name="appurl">The appurl.</param>
        /// <returns>
        ///     environment based on url
        /// </returns>
        public string GetEnvironmentFromURL(string appurl)
        {
            return (appurl.Contains("release") || appurl.Contains("altsrc.net")) ? "QA" : "PROD";
        }

        /// <summary>
        /// Waits the and then click.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="by">The by.</param>
        /// <param name="timeout">The timeout.</param>
        public void WaitAndThenClick(IWebDriver driver, By by, int timeout)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.Until(drv => drv.FindElements(by).Count != 0);
            wait.Until(ExpectedConditions.ElementToBeClickable(by));
            driver.FindElement(by).Click();
        }

        /// <summary>
        /// Checks the file downloaded.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        ///     true or false if the downloaded file exists or not
        /// </returns>
        public bool CheckFileDownloaded(string filename)
        {
            bool exist = false;
            string Path = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
            string[] filePaths = Directory.GetFiles(Path);
            foreach (string p in filePaths)
            {
                if (p.Contains(filename))
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        /// <summary>
        /// Expands the shadow root.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="element">The element.</param>
        /// <param name="script">The script.</param>
        /// <returns>
        ///     element in shadowRoot
        /// </returns>
        public IWebElement ExpandShadowRoot(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            IWebElement ele = (IWebElement)js.ExecuteScript("return arguments[0].shadowRoot", element);

            return ele;
        }

        /// <summary>
        /// Sends the report email.
        /// </summary>
        /// <param name="attachmentpath">The attachmentpath.</param>
        /// <exception cref="ArgumentException">Fail to send report via email!</exception>
        public static void SendReportEmail(string attachmentpath)
        {
            try
            {
                SmtpClient sc = new SmtpClient(host, Convert.ToInt32(port))
                {
                    Credentials = new NetworkCredential(fromemail, emailpassword),
                    EnableSsl = false
                };
                MailMessage msg = new MailMessage()
                {
                    From = new MailAddress(fromemail),
                    Subject = "AltSource Portal Automation Test - Report",
                    IsBodyHtml = true
                };
                msg.To.Add(toemail);
                msg.Attachments.Add(new Attachment(attachmentpath));
                sc.Send(msg);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Fail to send report via email!", ex);
            }
        }

        /// <summary>
        /// Sends the fail test email.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">Fail to send report via email!</exception>
        public static void SendFailTestEmail(string subject, string message)
        {
            try
            {
                SmtpClient sc = new SmtpClient(host, Convert.ToInt32(port))
                {
                    Credentials = new NetworkCredential(fromemail, emailpassword),
                    EnableSsl = false
                };
                MailMessage msg = new MailMessage()
                {
                    From = new MailAddress(fromemail),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                msg.To.Add(toemail);
                sc.Send(msg);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Fail to send report via email!", ex);
            }
        }

        ///// <summary>
        ///// Load CSV
        ///// </summary>
        ///// <returns>convert csv data to table</returns>
        //public DataTable LoadCSV()
        //{
        //    var csvTable = new DataTable();
        //    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        //    string dir = "c:\\Users\\" + userName + @"\Downloads";
        //    Console.WriteLine(dir);
        //    string[] csvlist = Directory.GetFiles(dir, "*.csv");
        //    Console.WriteLine(csvlist[0]);


        //    using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(csvlist[0])), true))
        //    {
        //        csvTable.Load(csvReader);
        //    }

        //    File.Delete(csvlist[0]);

        //    return csvTable;
        //}

        ///// <summary>
        ///// ValidateCSV
        ///// </summary>
        ///// <param name="expected"></param>
        //public void ValidateCSV(Table expected)
        //{
        //    var csvTable = LoadCSV();
        //    var recordset = expected.CreateSet<SalaryDetail>();
        //    int i = 0;

        //    foreach (SalaryDetail record in recordset)
        //    {
        //        Assert.AreEqual(record.Email, csvTable.Rows[i][2].ToString());
        //        Assert.AreEqual(record.Type, csvTable.Rows[i][4].ToString());
        //        Assert.AreEqual(record.EmployeeSI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][16].ToString()));
        //        Assert.AreEqual(record.EmployeeHI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][17].ToString()));
        //        Assert.AreEqual(record.EmployeeUI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][18].ToString()));
        //        Assert.AreEqual(record.EmployerSI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][20].ToString()));
        //        Assert.AreEqual(record.EmployerHI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][21].ToString()));
        //        Assert.AreEqual(record.EmployerUI.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][22].ToString()));
        //        Assert.AreEqual(record.EmployerUF.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][23].ToString()));
        //        Assert.AreEqual(record.AssesableIncome.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][27].ToString()));
        //        Assert.AreEqual(record.PIT.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][28].ToString()));
        //        Assert.AreEqual(record.NetIncome.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][29].ToString()));
        //        Assert.AreEqual(record.TotalSalary.Replace(",", ""), RemoveDecimalOutOfString(csvTable.Rows[i][30].ToString()));

        //        i++;
        //    }
        //}

        /// <summary>
        /// GetOutOfRangeDate
        /// </summary>
        /// <returns></returns>
        public DateTime GetOutOfRangeDate()
        {
            return DateTime.Now.AddDays(31);
        }

        /// <summary>
        /// RemoveDecimalOutOfString
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string after removing decimal</returns>
        public string RemoveDecimalOutOfString(string input)
        {
            var pos = input.IndexOf('.');
            return (input.Contains(".")) ? input.Substring(0, pos) : input;
        }

        private static IConfiguration config;
        private static IConfiguration GetConfig()
        {
            var appSettings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
            var environmentName = appSettings["Environment"].ToString();
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .Build();
            return config;
        }
        public static AppSetting GetValueFromAppSettings()
        {
            var appSetting = new AppSetting();
            if (config == null)
            {
                config = GetConfig();
            }
            config.GetSection("AppSetting").Bind(appSetting);
            return appSetting;
        }
    }
}
