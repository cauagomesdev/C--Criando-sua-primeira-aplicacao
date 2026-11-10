using System;

namespace ByteBank.Common;

public class BoletosPorCedente
{
    [NomeColuna("Nome")]
    public string CedenteNome { get; set; }

    [NomeColuna("CPF/CNPJ")]
    public string CedenteCpfCnpj { get; set; }

    [NomeColuna("Agência")]
    public string CedenteAgencia { get; set; }

    [NomeColuna("Conta")]
    public string CedenteConta { get; set; }

    [NomeColuna("Total")]
    public decimal Valor { get; set; }

    [NomeColuna("Qtd")]
    public int Quantidade { get; set; }
}
