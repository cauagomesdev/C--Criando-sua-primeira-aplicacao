# Alura Report Generator

O **Alura Report Generator** é uma biblioteca .NET criada para facilitar a geração e exportação de relatórios em formato CSV a partir de coleções de dados.

## 🚀 Funcionalidades

- Conversão automática de dados estruturados (`List<Dictionary<string, string>>`) para formato CSV.
- Extração dinâmica do cabeçalho do relatório com base nas chaves dos dicionários.
- Exportação direta do ficheiro no disco com retorno do caminho absoluto.

## 📦 Instalação

Para instalar o pacote no teu projeto executável via CLI do .NET, executa:

```bash
dotnet add package Alura_ReportGenerator