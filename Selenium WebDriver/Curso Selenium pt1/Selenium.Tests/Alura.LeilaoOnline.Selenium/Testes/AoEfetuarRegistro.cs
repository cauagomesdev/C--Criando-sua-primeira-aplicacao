using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
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
        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        registroPO.PreencherFormulario(
            nome: "Cauã Gomes",
            email: "cauaaguiarteste@gmail.com",
            password: "123456",
            confirmPassword: "123456"
        );

        //act - efetuamos o registro
        registroPO.SubmeteFormulario();

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
        //dados de registro inválidos informados
        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        registroPO.PreencherFormulario(nome, email, password, confirmPassword);

        //act - efetuamos o registro
        registroPO.SubmeteFormulario();

        //assert - devemos ficar na página home
        Assert.Contains("section-registro", driver.PageSource);
    }

    [Fact]
    public void DadoNomeEmBrancoDeveMostrarMensagemDeErro()
    {
        //arrange - dado Chrome aberto, página inicial do sistema de leilões
        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        //act - efetuamos o registro sem preencher nada
        registroPO.SubmeteFormulario();

        //assert - deve mostrar a mensagem de erro do nome
        Assert.Equal("The Nome field is required.", registroPO.NomeMensagemErro);
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
        Assert.Equal("Please enter a valid email address.", registroPO.EmailMensagemErro);
    }
}
