using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Alura.LeilaoOnline.Selenium.Fixtures;

public class TestFixture : IDisposable
{
    public IWebDriver Driver { get; private set; }

    //Setup
    public TestFixture()
    {
        Driver = new ChromeDriver();

        // Janela grande: o menu do usuário (meu-perfil/logout) usa
        // "hide-on-med-and-down" e some em telas com até 992px de largura.
        Driver.Manage().Window.Maximize();
        // Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    //TearDown
    public void Dispose()
    {
        Driver.Quit();
    }
}
