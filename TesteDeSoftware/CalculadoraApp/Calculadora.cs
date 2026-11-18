namespace CalculadoraApp;

public class Calculadora
{
    public double Somar(double a, double b)
    {
        return a + b;
    }

    public double Subtrair(double a, double b)
    {
        return a - b;
    }

    public double Multiplicar(double a, double b)
    {
        return a * b;
    }

    public double Dividir(double a, double b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Não é possível dividir por zero.");
        }

        return a / b;
    }

    public double Potencia(double baseNum, double expoente)
    {
        return Math.Pow(baseNum, expoente);
    }

    public double RaizQuadrada(double numero)
    {
        if (numero < 0)
        {
            throw new ArgumentException(
                "Não é possível calcular a raiz quadrada de um número negativo."
            );
        }

        return Math.Sqrt(numero);
    }
}
