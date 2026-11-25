using Alura.LeilaoOnline.Selenium.Fixtures;
using Alura.LeilaoOnline.Selenium.PageObjects;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoEfetuarLogin
{
    private IWebDriver driver;

    public AoEfetuarLogin(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoCredenciaisValidasDeveIrParaDashboard()
    {
        //arrange

        var loginPO = new LoginPO(driver);
        loginPO.Visitar();
        loginPO.PreencherFormulario("cauaaguiarteste@gmail.com", "123456");

        //act

        loginPO.SubmeterFormulario();

        //assert

        Assert.Contains("Dashboard", driver.Title);
    }
}
