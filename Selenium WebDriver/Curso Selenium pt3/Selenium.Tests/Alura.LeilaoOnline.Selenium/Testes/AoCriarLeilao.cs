using Alura.LeilaoOnline.Selenium.Helpers;
using OpenQA.Selenium;
using Alura.LeilaoOnline.Selenium.PageObjects;
using Alura.LeilaoOnline.Selenium.Fixtures;

namespace Alura.LeilaoOnline.Selenium.Testes;

[Collection("Chrome Driver")]
public class AoCriarLeilao
{
    private IWebDriver driver;

    public AoCriarLeilao(TestFixture fixture)
    {
        driver = fixture.Driver;
    }

    [Fact]
    public void DadoLoginAdminEInfoValidasDeveCadastrarLeilao()
    {
        //arrange
        var loginPO = new LoginPO(driver);
        loginPO.Visitar();
        loginPO.PreencheFormulario("admin@example.org", "123");
        loginPO.SubmeteFormulario();

        var novoLeilaoPO = new NovoLeilaoPO(driver);
        novoLeilaoPO.Visitar();
        novoLeilaoPO.PreencheFormulario(
            "Leilão de Coleção 1",
            "Nullam aliquet condimentum elit vitae volutpat. Vivamus ut nisi venenatis, facilisis odio eget, lobortis tortor. Cras mattis sit amet dolor id bibendum. Nulla turpis justo, porttitor eget leo vel, dictum tempor diam. Sed dui arcu, feugiat nec placerat ac, suscipit a mi. Etiam eget risus et tellus placerat tincidunt at ut lorem.",
            "Item de Colecionador",
            4000,
            // imagem copiada para a pasta do build (Alura.LeilaoOnline.Selenium/Imagens)
            Path.Combine(TestHelper.PastaDoExecutavel, "Imagens", "colecao1.jpg"),
            DateTime.Now.AddDays(20),
            DateTime.Now.AddDays(40)
        );

        //act
        novoLeilaoPO.SubmeteFormulario();

        //assert
        Assert.Contains("Leilões cadastrados no sistema", driver.PageSource);
    }
}
