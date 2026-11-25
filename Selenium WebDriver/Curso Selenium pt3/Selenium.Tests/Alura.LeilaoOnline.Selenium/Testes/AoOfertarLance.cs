using Alura.LeilaoOnline.Selenium.Fixtures;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoOfertarLance
{
    private IWebDriver driver;

    public AoOfertarLance(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoLoginInteressadaDeveAtualizarLanceAtual()
    {
        //arrange
        var loginPO = new LoginPO(driver);
        loginPO.Visitar();
        loginPO.PreencheFormulario("fulano@example.org", "123");
        loginPO.SubmeteFormulario();

        var detalhePO = new DetalheLeilaoPO(driver);
        detalhePO.Visitar(1); // em andamento

        //act
        detalhePO.OfertarLance(300);

        //assert
        Assert.Equal(300, detalhePO.LanceAtual);
    }
}
