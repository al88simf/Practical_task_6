using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailExchange
{
    internal class InboxPageMailRu
    {
        const int Timeout = 30;

        IWebDriver driver;
        WebDriverWait wait;
        IJavaScriptExecutor js;

        public InboxPageMailRu(IWebDriver driver)
        {
            this.driver = driver;
            wait = new(driver, TimeSpan.FromSeconds(30));
            js = (IJavaScriptExecutor)driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.XPath, Using = "//span[text() = 'Написать письмо']")]
        [CacheLookup]
        IWebElement? composeButton;

        [FindsBy(How = How.Name, Using = "Subject")]
        [CacheLookup]
        IWebElement? subjectContainer;

        [FindsBy(How = How.CssSelector, Using = "div[role = 'textbox']")]
        [CacheLookup]
        IWebElement? textbox;

        [FindsBy(How = How.XPath, Using = "//span[text() = 'Отправить']")]
        [CacheLookup]
        IWebElement? send;

        void WaitForPageCompletion()
        {
            wait.Until(p => js.ExecuteScript("return document.readyState"))
                .Equals("complete");
        }

        void FillInAddress(string address)
        {
            string to = "input[class = 'container--H9L5q size_s_compressed"
                        + "--2c-eV']";

            var toContainer = wait
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(to)));
            toContainer?.SendKeys(address);
        }

        void FillInSubject(string subject) => subjectContainer?.SendKeys(subject);


        void FillInText(string text) => textbox?.SendKeys(text);


        public bool GetLogo()
        {
            var logo = wait
                .Until(ExpectedConditions
                    .ElementExists(By
                        .CssSelector("img[class = 'ph-logo__img svelte-"
                                     + "vnijy7']")));
            var isDisplayed = logo.Displayed;

            WaitForPageCompletion();

            return isDisplayed;
        }

        public void WriteEmail(string address, string subject, string text)
        {
            composeButton?.Click();     // нажимает "написать письмо"
            FillInAddress(address);
            FillInSubject(subject);
            FillInText(text);
        }

        public LoginPageYandex Submit()
        {
            string testUrl = "https://mail.yandex.ru";
            
            send?.Click();

            js?.ExecuteScript("window.open(arguments[0])", testUrl);
            WaitForPageCompletion();

            // Проверяет на количество окон и, что второе окно открылось.
            var windows = driver.WindowHandles.Count;
            var newWindowHandle = driver.WindowHandles[1];
            if ((windows == 2) && (!string.IsNullOrEmpty(newWindowHandle)))
            {
                driver.SwitchTo().Window(driver.WindowHandles[1]);

                return new LoginPageYandex(driver);
            }
            else throw new AssertionException("Test failed");
        }
    }
}
