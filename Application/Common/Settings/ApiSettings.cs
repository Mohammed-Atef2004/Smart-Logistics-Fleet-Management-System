namespace Application.Common.Settings;

public sealed class ApiSettings
{
    public const string SectionName = "ApiSettings";
    public string BaseUrl { get; init; } = default!;
}