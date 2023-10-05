using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EmailExchange
{
    public class MailRuToYandexExchangeTests
    {
        IWebDriver driver;
        
        [SetUp]
        public void InitBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test]
        public void SendEmailToYandexAndAnswerTest()
        {
            string mailRuUsername = "callme88";
            string mailRuPassword = "purushaman";
            string yandexUsername = "al1988simf@yandex.ru";
            string yandexPassword = "65pracritiiswoman735";
            string address = "al1988simf@yandex.ru";
            string subject = "Test Email exchange";
            string text = "Sending Email from Mail.ru inbox 'callme88@inbox.ru'"
                          + " to Yandex.ru 'al1988simf@yandex.ru'.";

            var loginMailRu = new LoginPageMailRu(driver);

            // ������� �� ������� �������� Mail.ru
            loginMailRu.GoToPage();

            var isLoginPageMailRuLogoDisplayed = loginMailRu.GetLogo();
            Assert.IsTrue(isLoginPageMailRuLogoDisplayed);

            // ������� � �������� ����.
            loginMailRu.LogIn(mailRuUsername, mailRuPassword);
            loginMailRu.Submit();

            var inboxMailRu = new InboxPageMailRu(driver);

            var isInboxPageMailRuLogoDisplayed = inboxMailRu.GetLogo();
            Assert.IsTrue(isInboxPageMailRuLogoDisplayed);

            // ����� ������.
            inboxMailRu.WriteEmail(address, subject, text);
            inboxMailRu.Submit();

            var loginYandex = new LoginPageYandex(driver);

            var isLoginPageYandexLogoDisplayed = loginYandex.GetLogo();
            Assert.IsTrue(isLoginPageYandexLogoDisplayed);

            // ������� � �������� ���� Yandex.
            loginYandex.LogIn(yandexUsername, yandexPassword);
            loginYandex.Submit();

            var inboxYandex = new InboxPageYandex(driver);

            var isInboxPageYandexLogoDisplayed = inboxYandex.GetLogo();
            Assert.IsTrue(isInboxPageYandexLogoDisplayed);

            // ��������� �������� �� ������, ��� ���� � ����������.
            var isEmailReceived = inboxYandex.GetReceived(subject);
            Assert.IsTrue(isEmailReceived);
            var isEmailUnread = inboxYandex.GetUndread();
            Assert.IsTrue(isEmailUnread);
            var actualSender = inboxYandex.GetSenderAddress();
            StringAssert.Contains(mailRuUsername, actualSender);

            // ��������� ������ � ��������� ��� ����������.
            var actualText = inboxYandex.ReadEmail();
            StringAssert.Contains(text, actualText);

        }

        [TearDown]
        public void ClearBrowser() => driver.Quit();
    }
}