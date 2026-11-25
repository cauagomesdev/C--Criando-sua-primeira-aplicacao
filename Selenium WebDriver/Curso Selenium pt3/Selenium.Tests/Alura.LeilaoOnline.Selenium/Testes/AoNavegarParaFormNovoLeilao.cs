using OpenQA.Selenium;
using Alura.LeilaoOnline.Selenium.PageObjects;
using Alura.LeilaoOnline.Selenium.Fixtures;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoNavegarParaFormNovoLeilao
{
    private IWebDriver driver;

    public AoNavegarParaFormNovoLeilao(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoLoginAdmDeveMostrarTresCategorias()
    {
        //arrange
        //arrange
        var loginPO = new LoginPO(driver);
        loginPO.Visitar();
        loginPO.PreencheFormulario("admin@example.org", "123");
        loginPO.SubmeteFormulario();

        var novoLeilaoPO = new NovoLeilaoPO(driver);

        //act
        novoLeilaoPO.Visitar();

        //assert
        Assert.Equal(3, novoLeilaoPO.Categorias.Count());
    }
}
