namespace CursoTeste1;

public class CalculadoraTests
{
    [Test]
    public void Soma_DeveRetornarSomaCorreta()
    {
        // Prepara os valores para o teste (Arrange)
        double a = 5;
        double b = 3;

        // Executa o método que será testado (Act)
        double resultado = Calculadora.Soma(a, b);

        // Verifica se o resultado está correto (Assert)
        // No NUnit 4, Assert.AreEqual(8, resultado) virou:
        Assert.That(resultado, Is.EqualTo(8));
    }
}
