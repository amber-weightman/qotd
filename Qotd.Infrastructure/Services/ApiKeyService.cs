using System.Security.Cryptography;

namespace Qotd.Infrastructure.Services;

internal interface IApiKeyService
{
    string Generate();
}

internal sealed record ApiKeyService : IApiKeyService
{
    private const string _prefix = "QOTD-";
    private const int _numberOfSecureBytesToGenerate = 32;
    private const int _lengthOfKey = 32;

    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

        string base64String = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_");

        var keyLength = _lengthOfKey - _prefix.Length;

        return _prefix + base64String[..keyLength];
    }
}
