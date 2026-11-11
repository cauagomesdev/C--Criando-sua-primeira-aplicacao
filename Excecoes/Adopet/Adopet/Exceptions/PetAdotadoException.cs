using System;

namespace Adopet.Exceptions;

public class PetAdotadoException : AdocaoException
{
    public PetAdotadoException(string? mensagem)
        : base(mensagem) { }

    public PetAdotadoException(string? mensagem, Exception? innerException)
        : base(mensagem, innerException) { }
}
