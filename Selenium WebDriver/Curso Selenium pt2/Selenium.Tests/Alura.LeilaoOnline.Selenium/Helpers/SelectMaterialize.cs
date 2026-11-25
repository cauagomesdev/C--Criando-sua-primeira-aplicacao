using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Alura.LeilaoOnline.Selenium.Helpers;

public class SelectMaterialize
{
    private IWebDriver driver;
    private IWebElement selectWrapper;
    private IEnumerable<IWebElement> opcoes;

    public SelectMaterialize(IWebDriver driver, By locatorSelectWrapper)
    {
        this.driver = driver;
        selectWrapper = driver.FindElement(locatorSelectWrapper);
        opcoes = selectWrapper.FindElements(By.CssSelector("li>span"));
    }

    public IEnumerable<IWebElement> Options => opcoes;

    private void OpenWrapper()
    {
        selectWrapper.Click();
        // a lista abre com animação: espera as opções ficarem visíveis
        // (enquanto estão invisíveis, o Selenium lê o texto delas como vazio)
        new WebDriverWait(driver, TimeSpan.FromSeconds(5))
            .Until(_ => opcoes.All(o => o.Displayed));
    }

    private void LoseFocus()
    {
        selectWrapper.FindElement(By.TagName("li")).SendKeys(Keys.Tab);
        // espera a lista fechar, para ela não ficar por cima do próximo campo
        new WebDriverWait(driver, TimeSpan.FromSeconds(5))
            .Until(_ => opcoes.All(o => !o.Displayed));
    }

    public void DeselectAll()
    {
        OpenWrapper();
        opcoes
            .ToList()
            .ForEach(o =>
            {
                o.Click();
            });
        LoseFocus();
    }

    public void SelectByText(string option)
    {
        OpenWrapper();
        opcoes
            .Where(o => o.Text.Contains(option))
            .ToList()
            .ForEach(o =>
            {
                o.Click();
            });
        LoseFocus();
    }
}
