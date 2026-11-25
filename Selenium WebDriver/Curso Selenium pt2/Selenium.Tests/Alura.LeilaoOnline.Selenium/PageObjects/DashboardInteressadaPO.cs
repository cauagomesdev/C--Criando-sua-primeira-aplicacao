using Alura.LeilaoOnline.Selenium.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class DashboardInteressadaPO
{
    private IWebDriver driver;
    private By byLogoutLink;
    private By byMeuPerfilLink;
    private By bySelectCategorias;
    private By byInputTermo;
    private By byInputAndamento;
    private By byBotaoPesquisar;

    public DashboardInteressadaPO(IWebDriver driver)
    {
        this.driver = driver;
        byLogoutLink = By.Id("logout");
        byMeuPerfilLink = By.Id("meu-perfil");
        bySelectCategorias = By.ClassName("select-wrapper");
        byInputTermo = By.Name("termo");
        byInputAndamento = By.ClassName("switch");
        byBotaoPesquisar = By.CssSelector("form>button.btn");
    }

    public void PesquisarLeiloes(List<string> categorias, string termo, bool emAndamento)
    {
        var select = new SelectMaterialize(driver, bySelectCategorias);

        select.DeselectAll();

        categorias.ForEach(categoria =>
        {
            select.SelectByText(categoria);
        });
        driver.FindElement(byInputTermo).SendKeys(termo);
        if (emAndamento)
        {
            driver.FindElement(byInputAndamento).Click();
        }
        // espera a página de resultado carregar antes de o teste olhar a tela
        TestHelper.ClicarEEsperarTrocaDePagina(driver, driver.FindElement(byBotaoPesquisar));
    }

    public void EfetuarLogout()
    {
        var linkLogout = driver.FindElement(byLogoutLink);
        var linkMeuPerfil = driver.FindElement(byMeuPerfilLink);

        IAction acaoLogout = new Actions(driver)
            //mover para o elemento pai
            .MoveToElement(linkMeuPerfil)
            //mover para o link de logout
            .MoveToElement(linkLogout)
            //clicar no link de logout
            .Click()
            .Build();

        acaoLogout.Perform();
    }
}
