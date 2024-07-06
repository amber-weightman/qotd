using System.Diagnostics.CodeAnalysis;

namespace Qotd.Infrastructure.Exceptions;

[ExcludeFromCodeCoverage]
public class AIException: Exception
{
    public AIException(string message) : base(message)
    {

    }
}
