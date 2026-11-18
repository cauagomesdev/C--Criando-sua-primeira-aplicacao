using FluentAssertions;

namespace CalculadoraApp.Testes;

public class CalculadoraTest
{
    [Fact]
    public void TestarSoma_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double a = 5;
        double b = 3;
        double resultadoEsperado = 8;
        // Act
        double resultado = calculadora.Somar(a, b);
        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void TestarSubtracao_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double a = 10;
        double b = 4;
        double resultadoEsperado = 6;
        // Act
        double resultado = calculadora.Subtrair(a, b);
        // Assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void TestarMultiplicacao_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double a = 6;
        double b = 7;
        double resultadoEsperado = 42;
        // Act
        double resultado = calculadora.Multiplicar(a, b);
        // Assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void TestarDivisao_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double a = 20;
        double b = 4;
        double resultadoEsperado = 5;
        // Act
        double resultado = calculadora.Dividir(a, b);
        // Assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void TestarDivisao_PorZero_DeveLancarExcecao()
    {
        // Arrange
        var calculadora = new Calculadora();
        double a = 10;
        double b = 0;
        // Act + Assert
        Action acao = () => calculadora.Dividir(a, b);
        acao.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void TestarPotencia_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double baseNum = 2;
        double expoente = 3;
        double resultadoEsperado = 8;
        // Act
        double resultado = calculadora.Potencia(baseNum, expoente);
        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void TestarRaizQuadrada_DeveRetornarValorCorreto()
    {
        // Arrange
        var calculadora = new Calculadora();
        double numero = 16;
        double resultadoEsperado = 4;
        // Act
        double resultado = calculadora.RaizQuadrada(numero);
        // Assert
        resultado.Should().Be(resultadoEsperado);
    }
}
