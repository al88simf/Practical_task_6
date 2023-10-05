using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace MailLogIn
{
    public class MailRuLogInTests
    {
        const string TestUrl = "https://mail.ru/";
        IWebDriver driver;

        
        // �������� ����� �������������� � ������������ �����.
        public string GetInputWarning()
        {
            string requiredCred = "small[data-test-id = 'required']";

            var element = driver.FindElement(By.CssSelector(requiredCred));
            var message = element.Text;
            return message;
        }

        [SetUp]
        public void InitBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [Test, Order(1)]
        public void LogInEmptyTest()
        {
            string popupFrame = "iframe[class = 'ag-popup__frame__layout__iframe']";
            string userNameField = "input[placeholder = '��� ��������']";
            string nextButtonCss = "button[data-test-id = 'next-button']";
            string login = "";
            string expectedFailInputMessage = "���� ���� �������� ������ ����"
                                              + " ���������";
            driver.Url = TestUrl;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Manage()             // ������� �������� ��������
                  .Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            var enterButton = driver    // �������� "�����"
                .FindElement(By.XPath("//button[. = '�����']"));
            enterButton.Click();

            var popupWin = wait         // ������� �������� ������������ ����
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(popupFrame)));

            // ����������� ���������� �� ����������� ����.
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame(popupWin);

            var userName = driver.FindElement(By.CssSelector(userNameField));
            userName.SendKeys(login);       // ���� ������� ��������
            var nextButton = driver.FindElement(By.CssSelector(nextButtonCss));
            nextButton.Click();             // �������� "������ ������"

            var warnMessage = GetInputWarning();
            StringAssert.Contains(expectedFailInputMessage, warnMessage);
        }

        [Test, Order(2)]
        [TestCase("Hello", "rhjk;hd")]
        [TestCase("callme88", "purushaman")]
        public void LogInDiffrentCredentialsTest(string loginValue,
                                                 string passwordValue)
        {
            string enter = "//button[. = '�����']";
            string popupFrame = "iframe[class = 'ag-popup__frame__layout__iframe']";
            string userNameField = "input[placeholder = '��� ��������']";
            string passwordField = "input[placeholder = '������']";
            string nextButtonCss = "button[data-test-id = 'next-button']";
            string domainSelect = "div[data-test-id = 'domain-select']";
            string inboxRuText = "//span[text() = '@inbox.ru']";
            string labelSave = "label[data-test-id = 'saveauth']";
            string inbox = "//span[text() = '�����']";
            string expectedInboxTitle = "����� Mail.ru";
            string expectedInvalidCredTitle = "����� � ����������� ����� Mail.ru � �������� �������� ������";

            driver.Url = TestUrl;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Manage()             // ������� �������� ��������
                  .Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            var enterButton = driver    // �������� "�����"
                .FindElement(By.XPath(enter));
            enterButton.Click();

            var popupWin = wait         // ������� �������� ������������ ����
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(popupFrame)));

            // ����������� ���������� �� ����������� ����.
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame(popupWin);

            var userName = driver.FindElement(By.CssSelector(userNameField));
            userName.SendKeys(loginValue);   // ���� ����������

            var dropDownMenu = driver.FindElement(By.CssSelector(domainSelect));
            dropDownMenu.Click();
            var domainInboxRu = driver.FindElement(By.XPath(inboxRuText));
            domainInboxRu.Click();           // ����� ������

            var saveAuth = driver.FindElement(By.CssSelector(labelSave));
            saveAuth.Click();               // ������� "���������"

            var nextButton = driver.FindElement(By.CssSelector(nextButtonCss));
            nextButton.Click();             // �������� "������ ������"

            var password = wait                 // ������� ���� ����� ������
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(passwordField)));
            password.SendKeys(passwordValue);   // ������ ������

            var inboxButton = driver.FindElement(By.XPath(inbox));
            inboxButton.Click();                // ������ � �����

            // �������� �������� �������� �����.
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var actualInboxTitle = driver.Title;
            if (string.Equals(expectedInboxTitle, actualInboxTitle))
                StringAssert.Contains(expectedInboxTitle, actualInboxTitle);
            else
            {
                try
                {
                    // ���� ������������ ������� ������ - ���������� �� 
                    // ��������� �������� ��������.
                    StringAssert.Contains(expectedInvalidCredTitle,
                                          actualInboxTitle);
                }
                catch (Exception e)
                {
                    // � ������ �������� �������� - ������� ������.
                    Assert.Fail(e.Message);
                }
            }
        }

        [TearDown]
        public void CloseBrowser() => driver.Quit();
    }
}