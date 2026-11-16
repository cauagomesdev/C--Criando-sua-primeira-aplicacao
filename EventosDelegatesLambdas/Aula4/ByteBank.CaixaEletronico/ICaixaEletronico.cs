namespace ByteBank.CaixaEletronico
{
    public interface ICaixaEletronico
    {
        decimal Saldo();
        void Depositar(decimal valor);
        void Sacar(decimal valor);
        string Extrato();

        event SaldoInsuficienteEventHandler OnSaldoInsuficiente;
    }
}
