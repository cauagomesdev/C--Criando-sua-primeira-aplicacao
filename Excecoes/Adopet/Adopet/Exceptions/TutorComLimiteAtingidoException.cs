using System;

namespace Adopet.Exceptions;

public class TutorComLimiteAtingidoException : AdocaoException
{
    public TutorComLimiteAtingidoException(string? mensagem)
        : base(mensagem) { }
}
