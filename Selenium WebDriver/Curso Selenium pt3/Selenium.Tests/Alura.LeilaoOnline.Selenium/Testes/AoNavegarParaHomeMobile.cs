using Alura.LeilaoOnline.Selenium.PageObjects;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;

namespace Alura.LeilaoOnline.Selenium.Testes;

public class AoNavegarParaHomeMobile : IDisposable
{
    private ChromeDriver driver;

    public AoNavegarParaHomeMobile() { }

    [Fact]
    public void DadaLargura400DeveMostrarMenuMobile()
    {
        //arrange
        var deviceSettings = new ChromiumMobileEmulationDeviceSettings();
        deviceSettings.Width = 400;
        deviceSettings.Height = 800;
        deviceSettings.UserAgent = "Customizada";
        var options = new ChromeOptions();
        options.EnableMobileEmulation(deviceSettings);
        driver = new ChromeDriver(options);
        var homePO = new HomeNaoLogadaPO(driver);

        //act
        homePO.Visitar();

        //assert
        Assert.True(homePO.Menu.MenuMobileVisivel);
    }

    [Fact]
    public void DadaLargura992DeveMostrarMenuMobile2()
    {
        //arrange
        var deviceSettings = new ChromiumMobileEmulationDeviceSettings();
        deviceSettings.Width = 992;
        deviceSettings.Height = 800;
        deviceSettings.UserAgent = "Customizada";
        var options = new ChromeOptions();
        options.EnableMobileEmulation(deviceSettings);
        driver = new ChromeDriver(options);
        var homePO = new HomeNaoLogadaPO(driver);

        //act
        homePO.Visitar();

        //assert
        Assert.True(homePO.Menu.MenuMobileVisivel);
    }

    [Fact]
    public void DadaLargura993DeveMostrarMenuMobile2()
    {
        //arrange
        var deviceSettings = new ChromiumMobileEmulationDeviceSettings();
        deviceSettings.Width = 993;
        deviceSettings.Height = 800;
        deviceSettings.UserAgent = "Customizada";
        var options = new ChromeOptions();
        options.EnableMobileEmulation(deviceSettings);
        driver = new ChromeDriver(options);
        var homePO = new HomeNaoLogadaPO(driver);

        //act
        homePO.Visitar();

        //assert
        Assert.False(homePO.Menu.MenuMobileVisivel);
    }

    public void Dispose()
    {
        driver.Quit();
    }
}
