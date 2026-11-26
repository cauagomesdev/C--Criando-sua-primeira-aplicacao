using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class MenuLogadoPO
{
    private IWebDriver driver;
    private By byLogoutLink;
    private By byMeuPerfilLink;

    public MenuLogadoPO(IWebDriver driver)
    {
        this.driver = driver;
        byLogoutLink = By.Id("logout");
        byMeuPerfilLink = By.Id("meu-perfil");
        // byLogoutLink = By.Xpath("//table/tbody/tr[last()]");
    }

    public void EfetuarLogout()
    {
        var linkMeuPerfil = driver.FindElement(byMeuPerfilLink);
        var linkLogout = driver.FindElement(byLogoutLink);

        IAction acaoLogout = new Actions(driver)
            //mover para o elemento meu-perfil
            .MoveToElement(linkMeuPerfil)
            //mover para o link de logout
            .MoveToElement(linkLogout)
            //clicar no link de logout
            .Click()
            .Build();

        acaoLogout.Perform();
    }
}
