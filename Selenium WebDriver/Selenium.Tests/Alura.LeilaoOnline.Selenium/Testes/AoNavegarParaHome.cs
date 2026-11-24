using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

        var form = driver.FindElement(By.TagName("form"));
        var spans = form.FindElements(By.TagName("span"));
        foreach (var span in spans)
        {
            Assert.True(string.IsNullOrEmpty(span.Text));
        }
    }
}
