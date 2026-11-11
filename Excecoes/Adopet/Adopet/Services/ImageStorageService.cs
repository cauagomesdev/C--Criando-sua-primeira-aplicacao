namespace Adopet.Services;

public class ImageStorageService
{
    private readonly string _uploadFolder;

    public ImageStorageService(IWebHostEnvironment webHostEnvironment)
    {
        _uploadFolder = Path.Combine(webHostEnvironment.ContentRootPath, "storage");
    }

    public string Upload(IFormFile imagem)
    {
        string novoNome = GerarNovoNome(imagem.FileName);

        string caminhoCompletoDoArquivo = Path.Combine(_uploadFolder, novoNome);

        using (var stream = new FileStream(caminhoCompletoDoArquivo, FileMode.Create))
        {
            imagem.CopyTo(stream);
        }

        return novoNome;
    }

    private string GerarNovoNome(string nomeOriginal)
    {
        string[] nomeSplit = nomeOriginal.Split('.');
        string extensao = "." + nomeSplit[1];

        return Guid.NewGuid() + extensao;
    }
}
