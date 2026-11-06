namespace ScreenSound.Modelos;

internal class Avaliacao
{
    public Avaliacao(float nota)
    {
        if(nota <= 0) Nota = 0;
        else if(nota >= 10) Nota = 10;
        else Nota = nota;
    }

    public float Nota { get; }

    public static Avaliacao Parse(string texto)
    {
        float nota = float.Parse(texto);
        return new Avaliacao(nota);
    }
}
