using System;

namespace Adopet.Exceptions;

public class AdocaoException : Exception
{
    public AdocaoException(string? message)
        : base(message) { }

    public AdocaoException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
