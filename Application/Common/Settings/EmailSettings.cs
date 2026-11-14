namespace Application.Common.Settings;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
}