using System;

namespace Alura.LeilaoOnline.Selenium.Fixtures;

public class CollectionFixture
{
    [CollectionDefinition("Chrome Driver")]
    public class ChromeDriverCollection : ICollectionFixture<TestFixture> { }
}
