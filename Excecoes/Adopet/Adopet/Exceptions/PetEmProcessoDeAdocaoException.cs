using System;

namespace Adopet.Exceptions;

public class PetEmProcessoDeAdocaoException : AdocaoException
{
    public PetEmProcessoDeAdocaoException(string? mensagem)
        : base(mensagem) { }
}
