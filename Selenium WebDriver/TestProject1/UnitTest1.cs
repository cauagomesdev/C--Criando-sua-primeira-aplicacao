using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestProject1;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        //arrange - dado que o navegador está aberto

        IWebDriver driver = new ChromeDriver();

        //act - quando navegamos para a URL https://www.caelum.com.br

        driver.Navigate().GoToUrl("https://www.caelum.com.br");

        //assert - então esperamos que a página apresentada seja da Caelum

        Assert.Contains("Caelum", driver.Title);
    }
}
