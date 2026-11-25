using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoNavegarParaHome
{
    private IWebDriver driver;

    //Setup
    public AoNavegarParaHome(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoChromeAbertoDeveMostrarLeiloesNoTitulo()
    {
        //arrange

        //act

        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        //assert

        Assert.Contains("Leilões", driver.Title);
    }

    [Fact]
    public void DadoChromeAbertoDeveMostrarProximosLeiloesNaPagina()
    {
        //arrange

        //act

        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        //assert

        Assert.Contains("Próximos Leilões", driver.PageSource);
    }

    [Fact]
    public void DadoChromeAbertoFormRegistroNaoDeveMostrarMensagemDeErro()
    {
        //arrange

        //act

        var registroPO = new RegistroPO(driver);
        registroPO.Visitar();

        //assert

        foreach (var mensagem in registroPO.MensagensDeErro)
        {
            Assert.True(string.IsNullOrEmpty(mensagem));
        }
    }
}
