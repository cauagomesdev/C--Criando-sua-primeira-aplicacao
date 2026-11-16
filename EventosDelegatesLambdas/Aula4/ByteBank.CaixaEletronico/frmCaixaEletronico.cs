namespace ByteBank.CaixaEletronico
{
    public partial class frmCaixaEletronico : Form
    {
        private readonly CaixaEletronico caixaEletronico;

        public frmCaixaEletronico()
        {
            InitializeComponent();
            caixaEletronico = new CaixaEletronico();
            caixaEletronico.OnDeposito += CaixaEletronico_OnDeposito;
            caixaEletronico.OnSaque += CaixaEletronico_OnSaque;
            caixaEletronico.OnSaldoInsuficiente += CaixaEletronico_OnSaldoInsuficiente;

            ImprimirLogo();

            btnSacar.Click += BtnSacar_Click;

            btnDepositar.Click += BtnDepositar_Click;

            btnSaldo.Click += BtnSaldo_Click;

            btnExtrato.Click += BtnExtrato_Click;
        }

        private void ImprimirLogo()
        {
            txtConsole.Text =
            "▒█▀▀█ █░░█ ▀▀█▀▀ █▀▀ ▒█▀▀█ █▀▀█ █▀▀▄ █░█\r\n" +
            "▒█▀▀▄ █▄▄█ ░░█░░ █▀▀ ▒█▀▀▄ █▄▄█ █░░█ █▀▄\r\n" +
            "▒█▄▄█ ▄▄▄█ ░░▀░░ ▀▀▀ ▒█▄▄█ ▀░░▀ ▀░░▀ ▀░▀\r\n";
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.ScrollToCaret();
        }

        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CaixaEletronico_OnSaldoInsuficiente(object sender, TransacaoEventArgs e)
        {
            WriteToConsole("Saldo insuficiente!");
        }

        private void CaixaEletronico_OnSaque(object sender, TransacaoEventArgs e)
        {
            string mensagem = $"Saque de {e.ValorTransacao:C} realizado com sucesso!";
            WriteToConsole(mensagem);
            txtValor.Text = string.Empty;
        }

        private void CaixaEletronico_OnDeposito(object sender, TransacaoEventArgs e)
        {
            string mensagem = $"Depósito de {e.ValorTransacao:C} realizado com sucesso!";
            WriteToConsole(mensagem);
            txtValor.Text = string.Empty;
        }

        private void BtnNumero_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            txtValor.Text += btn.Name.Last();
        }

        private void BtnSacar_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValor.Text, out decimal valorSaque))
            {
                caixaEletronico.Sacar(valorSaque);
            }
            else
            {
                WriteToConsole("Valor inválido!");
                txtValor.Text = string.Empty;
            }
        }

        private void BtnDepositar_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtValor.Text, out decimal valorDeposito))
            {
                caixaEletronico.Depositar(valorDeposito);
            }
            else
            {
                WriteToConsole("Valor inválido!");
                txtValor.Text = string.Empty;
            }
        }

        private void BtnSaldo_Click(object sender, EventArgs e)
        {
            decimal saldo = caixaEletronico.Saldo();
            WriteToConsole($"Saldo atual: {saldo:C}");
        }

        private void BtnExtrato_Click(object? sender, EventArgs e)
        {
            string extrato = caixaEletronico.Extrato();
            WriteToConsole(extrato);
        }

        private void btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Location = new Point(btn.Location.X + 4, btn.Location.Y + 4);
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Location = new Point(btn.Location.X - 4, btn.Location.Y - 4);
        }

        private void btnOperacao_Click(object sender, EventArgs e)
        {
            txtValor.Focus();
        }

        private void WriteToConsole(string mensagem)
        {
            txtConsole.Text += mensagem;
            txtConsole.Text += Environment.NewLine;
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.ScrollToCaret();
        }
    }
}