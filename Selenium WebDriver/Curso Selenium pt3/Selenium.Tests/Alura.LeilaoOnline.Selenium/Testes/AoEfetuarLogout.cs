using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoEfetuarLogout
{
    private IWebDriver driver;

    public AoEfetuarLogout(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoLoginValidoDeveIrParaHomeNaoLogada()
    {
        //arrange
        new LoginPO(driver).Visitar()
            .PreencheFormulario("fulano@example.org", "123")
            .SubmeteFormulario();

        var dashboardPO = new DashboardInteressadaPO(driver);

        //act - efetuar logout
        dashboardPO.Menu.EfetuarLogout();

        //assert
        Assert.Contains("Próximos Leilões", driver.PageSource);
    }
}
