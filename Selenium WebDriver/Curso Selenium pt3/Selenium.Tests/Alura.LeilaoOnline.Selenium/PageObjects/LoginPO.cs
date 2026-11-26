using Alura.LeilaoOnline.Selenium.Helpers;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class LoginPO
{
    private IWebDriver driver;
    private By byInputLogin;
    private By byInputSenha;
    private By byBotaoLogin;

    public LoginPO(IWebDriver driver)
    {
        this.driver = driver;
        byInputLogin = By.Id("Login");
        byInputSenha = By.Id("Password");
        byBotaoLogin = By.Id("btnLogin");
    }

    public LoginPO Visitar()
    {
        driver.Navigate().GoToUrl("http://localhost:5000/Autenticacao/Login");
        return this;
    }

    public LoginPO PreencheFormulario(string login, string senha)
    {
        return InformarEmail(login).InformarSenha(senha);
    }

    public LoginPO InformarEmail(string email)
    {
        driver.FindElement(byInputLogin).SendKeys(email);
        return this;
    }

    public LoginPO InformarSenha(string senha)
    {
        driver.FindElement(byInputSenha).SendKeys(senha);
        return this;
    }

    public LoginPO EfetuarLogin()
    {
        return SubmeteFormulario();
    }

    public LoginPO SubmeteFormulario()
    {
        TestHelper.ClicarEEsperarTrocaDePagina(driver, driver.FindElement(byBotaoLogin));
        return this;
    }

    public void EfetuarLoginComCredenciais(string login, string senha)
    {
        Visitar().PreencheFormulario(login, senha).SubmeteFormulario();
    }
}
