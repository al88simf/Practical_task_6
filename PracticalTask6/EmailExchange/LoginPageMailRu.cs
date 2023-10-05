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
    internal class LoginPageMailRu
    {
        const string TestUrl = "https://mail.ru";
        const int Timeout = 30;     // временная задержка

        IWebDriver driver;
        WebDriverWait wait;

        public LoginPageMailRu(IWebDriver driver)
        {
            this.driver = driver;
            wait = new(driver, TimeSpan.FromSeconds(Timeout));
            PageFactory.InitElements(driver, this);
        }
        
        [FindsBy(How = How.XPath, Using = "//button[. = 'Войти']")]
        IWebElement? enterButton;

        [FindsBy(How = How.CssSelector, Using = "input[placeholder = 'Имя "
                                                + "аккаунта']")]
        IWebElement? userNameField;

        [FindsBy(How = How.CssSelector, Using = "div[data-test-id = 'domain-"
                                                + "select']")]
        IWebElement? dropdownMenu;

        [FindsBy(How = How.XPath, Using = "//span[text() = '@inbox.ru']")]
        IWebElement? domainInboxRu;

        [FindsBy(How = How.CssSelector, Using = "label[data-test-id = "
                                                + "'saveauth']")]
        IWebElement? saveAuthButton;     // кнопка "запомнить"

        [FindsBy(How = How.CssSelector, Using = "button[data-test-id = 'next-"
                                                + "button']")]
        IWebElement? nextButton;         // кнопка "ввести пароль"

        [FindsBy(How = How.CssSelector, Using = "input[placeholder = 'Пароль']")]
        IWebElement? passwordField;

        [FindsBy(How = How.XPath, Using = "//span[text() = 'Войти']")]
        IWebElement? inboxButton;        // кнопка подтверждения и входа

        
        void WaitForPageCompletion()
        {
            wait.Until(p => ((IJavaScriptExecutor)p)
                            .ExecuteScript("return document.readyState")
                                .Equals("complete"));
        }

        void WaitForFrameCompletionAndSwitchToIt()
        {
            string popupFrame = "iframe[class = 'ag-popup__frame__layout__"
                                + "iframe']";

            var popupWin = wait
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(popupFrame)));

            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame(popupWin);
        }

        void SendUsername(string username)
        {
            userNameField?.SendKeys(username);
            dropdownMenu?.Click();
            domainInboxRu?.Click();
        }

        void SendPassword(string password)
        {
            passwordField?.SendKeys(password);
        }

        public void GoToPage() => driver.Navigate().GoToUrl(TestUrl);


        // Ожидает и проверяет загрузился ли логотип стартовой страницы, как
        // и сама страница.
        public bool GetLogo()
        {
            var logo = wait
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector("div[class = 'mailbox-icon "
                                                  + "ehh-cgec__1hjf1pg mailbox-"
                                                  + "icon__primary']")));
            var isDisplayed = logo.Displayed;

            return isDisplayed;
        }

        public void LogIn(string username, string password)
        {
            enterButton?.Click();
            WaitForFrameCompletionAndSwitchToIt();
            SendUsername(username);
            saveAuthButton?.Click();     // убирает "запомнить"
            nextButton?.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            SendPassword(password);
        }

        public InboxPageMailRu Submit()
        {
            inboxButton?.Click();       // нажимает "войти"
            WaitForPageCompletion();

            return new InboxPageMailRu(driver);
        }
    }
}
