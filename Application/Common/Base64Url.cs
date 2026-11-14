namespace Application.Common;

/// <summary>
/// URL-safe Base64 encoding/decoding for tokens embedded in links.
/// Replaces Microsoft.AspNetCore.WebUtilities.WebEncoders (vulnerable version removed).
/// </summary>
public static class Base64Url
{
    public static string Encode(byte[] bytes)
    {
        var encoded = Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return encoded;
    }

    public static byte[] Decode(string encoded)
    {
        var base64 = encoded
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}