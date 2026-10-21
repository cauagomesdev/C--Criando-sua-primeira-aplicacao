using System.Collections;

var diasDaSemana = new DiasDaSemana();

var carrinho = new List<Produto>()
{
    new Produto() {Nome = "Leite", Preco = 7.89},
    new Produto() {Nome = "Manteiga", Preco = 3.45}
};

var pares = NumerosParesComYield(10000);
var contador = 0;
foreach (var par in pares)
{
    contador++;
    System.Console.WriteLine(par);
    if (contador > 200) break;
}

IEnumerable<int> NumerosParesSemYield(int limite)
{
    var lista = new List<int>();
    for (var i = 0; i < limite; i++)
    {
        System.Console.WriteLine($"Processando elemento {i}");
        lista.Add(i * 2);
    }
    return lista;
}

IEnumerable<int> NumerosParesComYield(int limite)
{
    var i = 0;
    while (true)
    {
        System.Console.WriteLine($"Processando elemento {i}");
        yield return i * 2;
        if (i > 100)
        {
            System.Console.WriteLine("Saindo do enumerator");
            yield break;
        }
        i++;
    }
}

// PercorrendoComForEach();
void PercorrendoComEnumerator()
{
    var enumerator = diasDaSemana.GetEnumerator();
    while (enumerator.MoveNext())
    {
        var dia = enumerator.Current;
        System.Console.WriteLine(dia);
    }
}


void PercorrendoDiasDaSemana()
{
    foreach (string dia in diasDaSemana)
    {
        System.Console.WriteLine(dia);
    }
}



void PercorrendoComFor()
{
    for (int i = 0; i < carrinho.Count; i++)
    {
        var produto = carrinho[i];
        System.Console.WriteLine($"Produto: {produto.Nome}");
    }
}


void PercorrendoComForEach()
{

    foreach (var produto in carrinho)
    {
        System.Console.WriteLine($"Produto: {produto.Nome}");
    }
}


class Produto
{
    public string Nome { get; set; }
    public double Preco { get; set; }
}

// class DiasDaSemanaEnumerator : IEnumerator<string>
// {
//     private int posicao = -1;
//     private string[] dias = { "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };
//     public string Current => dias[posicao];

//     object IEnumerator.Current => Current;

//     public void Dispose()
//     {
//     }

//     public bool MoveNext()
//     {
//         posicao++;
//         return posicao < dias.Length;
//     }

//     public void Reset()
//     {
//         posicao = -1;
//     }
// }

class DiasDaSemana : IEnumerable<string>
{


    public IEnumerator<string> GetEnumerator()
    {
        yield return "Domingo";
        yield return "Segunda";
        yield return "Terça";
        yield return "Quarta";
        yield return "Quinta";
        yield return "Sexta";
        yield return "Sábado";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}