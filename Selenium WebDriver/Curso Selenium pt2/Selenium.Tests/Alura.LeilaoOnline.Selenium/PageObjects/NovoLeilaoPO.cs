using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Alura.LeilaoOnline.Selenium.PageObjects;

public class NovoLeilaoPO
{
    private IWebDriver driver;
    private By byInputTitulo;
    private By byInputDescricao;
    private By byInputCategoria;
    private By byInputValorInicial;
    private By byInputImagem;
    private By byInputInicioPregao;
    private By byInputTerminoPregao;
    private By byBotaoSubmit;

    public NovoLeilaoPO(IWebDriver driver)
    {
        this.driver = driver;
        byInputTitulo = By.Id("Titulo");
        byInputDescricao = By.Id("Descricao");
        byInputCategoria = By.Id("Categoria");
        byInputValorInicial = By.Id("ValorInicial");
        byInputImagem = By.Id("ArquivoImagem");
        byInputInicioPregao = By.Id("InicioPregao");
        byInputTerminoPregao = By.Id("TerminoPregao");
        byBotaoSubmit = By.CssSelector("button[type=submit]");
    }

    public IEnumerable<string> Categorias
    {
        get
        {
            var elementoCategoria = new SelectElement(driver.FindElement(byInputCategoria));
            // elementoCategoria.FindElement(By.TagName("option")).Click();
            return elementoCategoria.Options.Where(o => o.Enabled).Select(o => o.Text);
        }
    }

    public void Visitar()
    {
        driver.Navigate().GoToUrl("http://localhost:5000/Leiloes/Novo");
    }

    public void PreencheFormulario(
        string titulo,
        string descricao,
        string categoria,
        double valor,
        string imagem,
        DateTime inicio,
        DateTime termino
    )
    {
        driver.FindElement(byInputTitulo).SendKeys(titulo);
        driver.FindElement(byInputDescricao).SendKeys(descricao);
        SelecionaCategoria(categoria);
        driver.FindElement(byInputValorInicial).SendKeys(valor.ToString());
        driver.FindElement(byInputImagem).SendKeys(imagem);
        driver.FindElement(byInputInicioPregao).SendKeys(inicio.ToString("dd/MM/yyyy"));
        driver.FindElement(byInputTerminoPregao).SendKeys(termino.ToString("dd/MM/yyyy"));
    }

    // O Materialize esconde o <select> original e mostra no lugar uma caixa clicável
    // (input.select-dropdown) com uma lista <ul> das opções. Por isso não dá para usar
    // SendKeys no <select>: é preciso clicar na caixa e depois na opção, como uma pessoa faria.
    private void SelecionaCategoria(string categoria)
    {
        var selectOriginal = driver.FindElement(byInputCategoria);
        var wrapper = selectOriginal.FindElement(By.XPath("..")); // div.select-wrapper do Materialize

        wrapper.FindElement(By.CssSelector("input.select-dropdown")).Click();

        // espera a lista abrir (tem animação) e clica na opção com o texto pedido
        var opcao = new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(_ =>
            wrapper
                .FindElements(By.CssSelector("ul.dropdown-content li"))
                .FirstOrDefault(li => li.Displayed && li.Text.Trim() == categoria)
        );
        opcao.Click();
    }

    internal void SubmeteFormulario()
    {
        driver.FindElement(byBotaoSubmit).Click();
    }
}
