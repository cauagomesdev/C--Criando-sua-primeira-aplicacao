using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class RegistroPO
{
    private IWebDriver driver;
    private By byFormRegistro;
    private By byInputNome;
    private By byInputEmail;
    private By byInputPassword;
    private By byInputConfirmPassword;
    private By byBotaoRegistro;
    private By bySpanErroNome;
    private By bySpanErroEmail;

    public string NomeMensagemErro => driver.FindElement(bySpanErroNome).Text;

    public string EmailMensagemErro => driver.FindElement(bySpanErroEmail).Text;

    /// <summary>Textos de todas as mensagens de erro (spans) do formulário de registro.</summary>
    public IEnumerable<string> MensagensDeErro =>
        driver.FindElement(byFormRegistro)
            .FindElements(By.TagName("span"))
            .Select(span => span.Text);

    public RegistroPO(IWebDriver driver)
    {
        this.driver = driver;
        byFormRegistro = By.TagName("form");
        byInputNome = By.Id("Nome");
        byInputEmail = By.Id("Email");
        byInputPassword = By.Id("Password");
        byInputConfirmPassword = By.Id("ConfirmPassword");
        byBotaoRegistro = By.Id("btnRegistro");
        bySpanErroNome = By.CssSelector("span.msg-erro[data-valmsg-for=Nome]");
        bySpanErroEmail = By.CssSelector("span.msg-erro[data-valmsg-for=Email]");
    }

    public void Visitar()
    {
        driver.Navigate().GoToUrl("http://localhost:5000");
    }

    public void SubmeteFormulario()
    {
        driver.FindElement(byBotaoRegistro).Click();
    }

    public void PreencherFormulario(
        string nome,
        string email,
        string password,
        string confirmPassword
    )
    {
        driver.FindElement(byInputNome).SendKeys(nome);
        driver.FindElement(byInputEmail).SendKeys(email);
        driver.FindElement(byInputPassword).SendKeys(password);
        driver.FindElement(byInputConfirmPassword).SendKeys(confirmPassword);
    }
}
