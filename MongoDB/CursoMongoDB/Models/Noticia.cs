using System;
using System.Collections.Generic;

public class Noticia
{
    public string Titulo { get; set; }
    public string Texto { get; set; }
    public DateTime DataPublicacao { get; set; }
    public List<string> Tags { get; set; }
    public List<Jornalista> Jornalistas { get; set; }
    public List<Comentario> Comentarios { get; set; }
    public List<Anexo> Anexos { get; set; }
    public int Visualizacoes { get; set; }
    public int TotalComentarios { get; set; }
    public int Gostei { get; set; }
    public int NaoGostei { get; set; }
    public double TempoMedioLeitura { get; set; }
}

public class Jornalista
{
    public string Nome { get; set; }
}

public class Comentario
{
    public string Texto { get; set; }
    public int Curtidas { get; set; }
    public string Usuario { get; set; }
    public DateTime Data { get; set; }
}

public class Anexo
{
    public string NomeArquivo { get; set; }
    public string Url { get; set; }
    public long Tamanho { get; set; }
    public string Tipo { get; set; }
    public int Cliques { get; set; }
}
