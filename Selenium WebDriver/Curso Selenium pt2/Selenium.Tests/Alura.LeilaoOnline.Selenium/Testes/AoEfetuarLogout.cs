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
        this.driver = fixture.Driver;
    }

    [Fact]
    public void DadoLoginValidoDeveIrParaHomeNaoLogada()
    {
        //arrange
        var loginPO = new LoginPO(driver);
        loginPO.Visitar();
        loginPO.PreencheFormulario("fulano@example.org", "123");
        loginPO.SubmeteFormulario();

        var dashboardPO = new DashboardInteressadaPO(driver);

        //act - efetuar logout
        dashboardPO.EfetuarLogout();

        //assert
        Assert.Contains("Próximos Leilões", driver.PageSource);
    }
}
