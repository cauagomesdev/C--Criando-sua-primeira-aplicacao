using OpenQA.Selenium;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class HomeNaoLogadaPO
{
    private IWebDriver driver;
    public MenuNaoLogadoPO Menu { get; }

    public HomeNaoLogadaPO(IWebDriver driver)
    {
        this.driver = driver;
        Menu = new MenuNaoLogadoPO(driver);
    }

    public void Visitar()
    {
        driver.Navigate().GoToUrl("http://localhost:5000");
    }
}
