using System;
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
    }

    //TearDown
    public void Dispose()
    {
        Driver.Quit();
        // Driver.Dispose();
    }
}
