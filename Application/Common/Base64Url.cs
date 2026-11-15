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
        return TryDecode(encoded, out var decoded)
            ? decoded
            : throw new FormatException("The value is not a valid Base64Url string.");
    }

    /// <summary>
    /// Non-throwing variant for untrusted input: returns false instead of
    /// throwing FormatException on malformed tokens.
    /// </summary>
    public static bool TryDecode(string encoded, out byte[] decoded)
    {
        decoded = Array.Empty<byte>();
        if (string.IsNullOrEmpty(encoded))
            return false;

        var base64 = encoded
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        try
        {
            decoded = Convert.FromBase64String(base64);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}