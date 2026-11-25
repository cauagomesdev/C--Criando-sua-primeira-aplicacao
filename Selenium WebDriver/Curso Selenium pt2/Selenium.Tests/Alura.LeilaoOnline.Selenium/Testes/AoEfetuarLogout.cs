using Alura.LeilaoOnline.Selenium.Fixtures;
using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.Testes;

public class AoEfetuarLogout
{
    private IWebDriver driver;

    public AoEfetuarLogout(TestFixture fixture)
    {
        this.driver = fixture.Driver;
    }
}
