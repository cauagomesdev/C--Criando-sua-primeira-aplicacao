using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class LoginPO
{
    IWebDriver driver;
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

    public void Visitar()
    {
        driver.Navigate().GoToUrl("http://localhost:5000/Autenticacao/Login");
    }

    public void PreencherFormulario(string login, string senha)
    {
        driver.FindElement(byInputLogin).SendKeys(login);
        driver.FindElement(byInputSenha).SendKeys(senha);
    }

    public void SubmeterFormulario()
    {
        driver.FindElement(byBotaoLogin).Submit();
    }
}
