namespace BlazorFileShake.Models;

public class UploadedFile
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public byte[] FileBytes { get; init; } = [];
    public string FileName { get; init; } = "";
    public string FileSize { get; init; } = "";
    public string FileExtension { get; init; } = "";
    public string FileType { get; init; } = "";
}