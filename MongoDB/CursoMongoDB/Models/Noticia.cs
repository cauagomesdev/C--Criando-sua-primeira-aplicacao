using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace CursoMongoDB.Models;

public class Noticia
{
    // public string Titulo { get; set; }

    [BsonId]
    [JsonIgnore]
    public ObjectId Id { get; set; }
    private string _titulo;
    public string Titulo
    {
        get => _titulo;
        set
        {
            _titulo = value;
            Url = GerarUrl(value);
        }
    }
    public string Url { get; set; }
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

    private static string GerarUrl(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            return string.Empty;

        // Remove acentos
        string url = titulo.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in url)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        url = sb.ToString().Normalize(NormalizationForm.FormC);

        // Substitui espaços por underline, deixa minúsculo e remove caracteres especiais
        url = url.Replace(' ', '_').ToLowerInvariant();
        url = Regex.Replace(url, @"[^a-z0-9_]", "");

        return url;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public BsonDocument ToBson()
    {
        return this.ToBsonDocument();
    }
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
