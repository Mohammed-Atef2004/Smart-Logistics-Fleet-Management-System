namespace Domain.Claims.ValueObjects;

public record ClaimDocument(
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSizeBytes,
    DateTime UploadedAt)
{
    private const long MaxSizeBytes = 10 * 1024 * 1024; // 10MB

    public static ClaimDocument Create(string fileName, string fileUrl, string contentType, long fileSizeBytes)
        => new(fileName, fileUrl, contentType, fileSizeBytes, DateTime.UtcNow);

    public bool ExceedsMaxSize() => FileSizeBytes > MaxSizeBytes;

    public override string ToString() => $"{FileName} ({FileSizeBytes / 1024}KB)";
}
