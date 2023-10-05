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
    internal class InboxPageYandex
    {
        const int Timeout = 30;

        IWebDriver? driver;
        WebDriverWait? wait;
        IJavaScriptExecutor? js;

        public InboxPageYandex(IWebDriver driver)
        {
            this.driver = driver;
            wait = new(driver, TimeSpan.FromSeconds(Timeout));
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.CssSelector, Using = "div[aria-label = 'Спам, "
                                                + "папка']")]
        [CacheLookup]
        IWebElement? spamFolder;

        [FindsBy(How = How.CssSelector, Using = "button[aria-label = 'Проверить, "
                                                + "есть ли новые письма']")]
        [CacheLookup]
        IWebElement? refreshButton;

        [FindsBy(How = How.CssSelector, Using = "span[title = 'Отметить как"
                                                + " прочитанное']")]
        [CacheLookup]
        IWebElement? readToggle;

        [FindsBy(How = How.CssSelector, Using = "span[title = "
                                                + "'callme88@inbox.ru']")]
        [CacheLookup]
        IWebElement? senderAddress;

        [FindsBy(How = How.CssSelector, Using = "span[title = 'Test Email "
                                                + "exchange']")]
        [CacheLookup]
        IWebElement emailTile;

        [FindsBy(How = How.XPath, Using = "//div[@class = 'MessageBody_body_pmf3j "
                                          + "react-message-wrapper__body']/div")]
        [CacheLookup]
        IWebElement emailText;

        string? GetTitle()
        {
            int waitTimeout = 1000;
            
            // Проверяет наличие конкретного письма, если нет, нажимает
            // "обновить".
            while (true)
            {
                try
                {
                    var email = driver?
                        .FindElement(By
                            .XPath("//span[text() = 'Test Email exchange']"));
                    var title = email?.Text;

                    return title;
                }
                catch (NoSuchElementException ex)
                {
                    // Пока письмо не пришло генерирует сообщения об ошибке.
                    refreshButton?.Click();
                    Console.WriteLine(ex.Message);
                    driver.Manage()
                          .Timeouts()
                          .ImplicitWait = TimeSpan.FromMilliseconds(waitTimeout);
                }
            }
        }

        public bool GetLogo()
        {
            var logo = wait?
                .Until(ExpectedConditions
                    .ElementExists(By.ClassName("PSHeaderService-Icon")));
            var isDisplayed = logo.Displayed;

            return isDisplayed;
        }

        public bool GetReceived(string subject)
        {
            // Если текст темы соответствует отправленному - письмо принято.
            spamFolder?.Click();
            var fetchedSubject = GetTitle();
            if (string.Equals(subject, fetchedSubject))
                return true;
            else throw new AssertionException("Test failed");
        }

        // Проверяет наличие значка "не прочитано"
        public bool GetUndread()
        {
            var isUnread = readToggle.Displayed;

            return isUnread;
        }

        // Получает значение атрибута title, чтобы получить логин отправителя.
        public string? GetSenderAddress()
        {
            var address = senderAddress?.GetAttribute("title");

            return address;
        }

        public string? ReadEmail()
        {
            emailTile.Click();      // открывает письмо
            var text = emailText.Text;

            return text;
        }
    }
}
