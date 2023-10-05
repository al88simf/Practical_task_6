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
    internal class LoginPageYandex
    {
        const int Timeout = 30;

        IWebDriver driver;
        WebDriverWait wait;
        IJavaScriptExecutor js;

        public LoginPageYandex(IWebDriver driver)
        {
            this.driver = driver;
            wait = new(driver, TimeSpan.FromSeconds(Timeout));
            js = (IJavaScriptExecutor)driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.CssSelector, Using = "button[data-t = 'button:"
                                                + "default']")]
        [CacheLookup]
        IWebElement? mailLoginButton;        // кнопка входа по почтовому адресу
        
        [FindsBy(How = How.Name, Using = "login")]
        [CacheLookup]
        IWebElement? loginContainer;

        [FindsBy(How = How.Name, Using = "passwd")]
        [CacheLookup]
        IWebElement? passwordContainer;

        [FindsBy(How = How.Id, Using = "passp:sign-in")]
        [CacheLookup]
        IWebElement? nextButton;

        [FindsBy(How = How.Id, Using = "passp:sign-in")]
        [CacheLookup]
        IWebElement submitButton;   // одна и та же кнопка "войти"

        void WaitForPageCompletion()
        {
            wait.Until(p => js.ExecuteScript("return document.readyState"))
                .Equals("complete");
        }

        // Яндекс почта открывается не на странице входа непосредственно, поэтому
        // ищет кнопку входа и нажимает на нее.
        void EnterLoginPage()
        {
            var loginButton = wait
                .Until(ExpectedConditions
                    .ElementExists(By.
                        CssSelector("a[id = 'header-login-button']")));
            loginButton?.Click();
            WaitForPageCompletion();
        }

        public bool GetLogo()
        {
            EnterLoginPage();
            var logo = wait
                .Until(ExpectedConditions
                    .ElementExists(By.
                        CssSelector("svg[class = 'IdIcon IdLogo-icon']")));
            var isDisplayed = logo.Displayed;

            return isDisplayed;
        }

        public void LogIn(string username, string password)
        {
            mailLoginButton?.Click();   // вход по адресу почты
            loginContainer?.SendKeys(username);
            nextButton?.Click();        // нажимает для перехода на ввод пароля

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            passwordContainer?.SendKeys(password);
        }

        public InboxPageYandex Submit()
        {
            submitButton.Click();
            WaitForPageCompletion();

            return new InboxPageYandex(driver);
        }
    }
}
