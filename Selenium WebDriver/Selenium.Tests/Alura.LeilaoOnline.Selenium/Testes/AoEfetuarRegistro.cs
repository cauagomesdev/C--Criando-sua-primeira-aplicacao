using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
using Microsoft.VisualStudio.TestPlatform.Common.Telemetry;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoEfetuarRegistro
{
    private IWebDriver driver;

    public AoEfetuarRegistro(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoInfoValidasDeveIrParaPaginaDeAgradecimento()
    {
        //arrange - dado Chrome aberto, página inicial do sistema de leilões
        //dados de registro válidos informados

        driver.Navigate().GoToUrl("http://localhost:5000");

        //nome
        var inputNome = driver.FindElement(By.Id("Nome"));

        //email
        var inputEmail = driver.FindElement(By.Id("Email"));

        //password
        var inputPassword = driver.FindElement(By.Id("Password"));

        //confirmpassword
        var inputConfirmPassword = driver.FindElement(By.Id("ConfirmPassword"));

        //botão de registro
        var botaoRegistro = driver.FindElement(By.Id("btnRegistro"));

        inputNome.SendKeys("Cauã Gomes");
        inputEmail.SendKeys("cauaaguiarteste@gmail.com");
        inputPassword.SendKeys("123456");
        inputConfirmPassword.SendKeys("123456");
        //act - efetuamos o registro

        botaoRegistro.Click();

        //assert - devemos nos direcionar para uma página de agradecimento

        Assert.Contains("Obrigado", driver.PageSource);
    }

    [Theory]
    [InlineData("", "cauaaguiarti@gmail.com", "123456", "123456")]
    [InlineData("Cauã Gomes", "aaa", "123456", "123456")]
    [InlineData("Cauã Gomes", "cauaaguiarti@gmail.com", "12345", "123456")]
    [InlineData("Cauã Gomes", "cauaaguiarti@gmail.com", "123456", "12345")]
    public void DadoInfoInvalidasDeveContinuarNaHome(
        string nome,
        string email,
        string password,
        string confirmPassword
    )
    {
        //arrange - dado Chrome aberto, página inicial do sistema de leilões
        //dados de registro válidos informados

        driver.Navigate().GoToUrl("http://localhost:5000");

        //nome
        var inputNome = driver.FindElement(By.Id("Nome"));

        //email
        var inputEmail = driver.FindElement(By.Id("Email"));

        //password
        var inputPassword = driver.FindElement(By.Id("Password"));

        //confirmpassword
        var inputConfirmPassword = driver.FindElement(By.Id("ConfirmPassword"));

        //botão de registro
        var botaoRegistro = driver.FindElement(By.Id("btnRegistro"));

        inputNome.SendKeys(nome);
        inputEmail.SendKeys(email);
        inputPassword.SendKeys(password);
        inputConfirmPassword.SendKeys(confirmPassword);

        //act - efetuamos o registro

        botaoRegistro.Click();

        //assert - devemos ficar na página home

        Assert.Contains("section-registro", driver.PageSource);
    }

    [Fact]
    public void DadoNomeEmBrancoDeveMostrarMensagemDeErro()
    {
        //arrange - dado Chrome aberto, página inicial do sistema de leilões
        //dados de registro válidos informados

        driver.Navigate().GoToUrl("http://localhost:5000");

        //botão de registro
        var botaoRegistro = driver.FindElement(By.Id("btnRegistro"));

        //act - efetuamos o registro
        botaoRegistro.Click();

        //assert - devemos ficar na página home
        IWebElement elemento = driver.FindElement(
            By.CssSelector("span.msg-erro[data-valmsg-for=Nome]")
        );

        Assert.Equal("The Nome field is required.", elemento.Text);
    }

    [Fact]
    public void DadoEmailInvalidoDeveMostrarMensagemDeErro()
    {
        //arrange
        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        registroPO.PreencherFormulario(nome: "", email: "caua", password: "", confirmPassword: "");

        //act
        registroPO.SubmeteFormulario();

        //assert

        Assert.Equal("Please enter a valid email address.", registroPO.NomeMensagemErro);
    }
}
