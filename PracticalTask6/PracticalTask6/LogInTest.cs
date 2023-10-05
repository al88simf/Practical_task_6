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

        
        // Получает текст предупреждения о неправильном вводе.
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
            string userNameField = "input[placeholder = 'Имя аккаунта']";
            string nextButtonCss = "button[data-test-id = 'next-button']";
            string login = "";
            string expectedFailInputMessage = "Поле «Имя аккаунта» должно быть"
                                              + " заполнено";
            driver.Url = TestUrl;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Manage()             // ожидает загрузки страницы
                  .Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            var enterButton = driver    // нажимает "Войти"
                .FindElement(By.XPath("//button[. = 'Войти']"));
            enterButton.Click();

            var popupWin = wait         // ожидает загрузки всплывающего окна
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(popupFrame)));

            // Переключает выполнение на всплывающее окно.
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame(popupWin);

            var userName = driver.FindElement(By.CssSelector(userNameField));
            userName.SendKeys(login);       // ввод пустого значения
            var nextButton = driver.FindElement(By.CssSelector(nextButtonCss));
            nextButton.Click();             // нажимает "Ввести пароль"

            var warnMessage = GetInputWarning();
            StringAssert.Contains(expectedFailInputMessage, warnMessage);
        }

        [Test, Order(2)]
        [TestCase("Hello", "rhjk;hd")]
        [TestCase("callme88", "purushaman")]
        public void LogInDiffrentCredentialsTest(string loginValue,
                                                 string passwordValue)
        {
            string enter = "//button[. = 'Войти']";
            string popupFrame = "iframe[class = 'ag-popup__frame__layout__iframe']";
            string userNameField = "input[placeholder = 'Имя аккаунта']";
            string passwordField = "input[placeholder = 'Пароль']";
            string nextButtonCss = "button[data-test-id = 'next-button']";
            string domainSelect = "div[data-test-id = 'domain-select']";
            string inboxRuText = "//span[text() = '@inbox.ru']";
            string labelSave = "label[data-test-id = 'saveauth']";
            string inbox = "//span[text() = 'Войти']";
            string expectedInboxTitle = "Почта Mail.ru";
            string expectedInvalidCredTitle = "Войти в электронную почту Mail.ru — надежный почтовый клиент";

            driver.Url = TestUrl;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Manage()             // ожидает загрузки страницы
                  .Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            var enterButton = driver    // нажимает "Войти"
                .FindElement(By.XPath(enter));
            enterButton.Click();

            var popupWin = wait         // ожидает загрузки всплывающего окна
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(popupFrame)));

            // Переключает выполнение на всплывающее окно.
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame(popupWin);

            var userName = driver.FindElement(By.CssSelector(userNameField));
            userName.SendKeys(loginValue);   // ввод псевдонима

            var dropDownMenu = driver.FindElement(By.CssSelector(domainSelect));
            dropDownMenu.Click();
            var domainInboxRu = driver.FindElement(By.XPath(inboxRuText));
            domainInboxRu.Click();           // выбор домена

            var saveAuth = driver.FindElement(By.CssSelector(labelSave));
            saveAuth.Click();               // убирает "запомнить"

            var nextButton = driver.FindElement(By.CssSelector(nextButtonCss));
            nextButton.Click();             // нажимает "Ввести пароль"

            var password = wait                 // ожидает поле ввода пароля
                .Until(ExpectedConditions
                    .ElementExists(By.CssSelector(passwordField)));
            password.SendKeys(passwordValue);   // вводит пароль

            var inboxButton = driver.FindElement(By.XPath(inbox));
            inboxButton.Click();                // входит в почту

            // Ожидание загрузки страницы почты.
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var actualInboxTitle = driver.Title;
            if (string.Equals(expectedInboxTitle, actualInboxTitle))
                StringAssert.Contains(expectedInboxTitle, actualInboxTitle);
            else
            {
                try
                {
                    // Если неправильные входные данные - сравнивает со 
                    // страницей проверки личности.
                    StringAssert.Contains(expectedInvalidCredTitle,
                                          actualInboxTitle);
                }
                catch (Exception e)
                {
                    // В случае неверной страницы - выводит ошибку.
                    Assert.Fail(e.Message);
                }
            }
        }

        [TearDown]
        public void CloseBrowser() => driver.Quit();
    }
}