using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Alura.LeilaoOnline.Selenium.Helpers;

public static class TestHelper
{
    public static string PastaDoExecutavel =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;

    /// <summary>
    /// Clica no elemento e espera (até o tempo limite) a página atual ser trocada
    /// pela resposta do servidor. Se o navegador barrar o envio do formulário
    /// (validação no próprio navegador), a página não troca e o método só segue em frente.
    /// </summary>
    public static void ClicarEEsperarTrocaDePagina(
        IWebDriver driver,
        IWebElement elemento,
        int segundosMaximos = 3)
    {
        elemento.Click();
        try
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(segundosMaximos))
                .Until(_ => SaiuDaPagina(elemento));
        }
        catch (WebDriverTimeoutException)
        {
            // A página não trocou (ex.: formulário inválido barrado no navegador). Tudo bem.
        }
    }

    private static bool SaiuDaPagina(IWebElement elemento)
    {
        try
        {
            _ = elemento.Enabled; // qualquer acesso falha se a página já foi trocada
            return false;
        }
        catch (StaleElementReferenceException)
        {
            return true;
        }
        catch (WebDriverException ex) when (ex.Message.Contains("does not belong to the document"))
        {
            // o Chrome às vezes avisa a troca de página com esse erro em vez do "stale"
            return true;
        }
    }
}
