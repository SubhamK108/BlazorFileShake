namespace BlazorFileShake.Models;

public class FolderCreatingMetadata
{
    public string Name { get; set; } = "";
    public string MimeType { get; set; } = "";
}

public class FileCreatingMetadata : FolderCreatingMetadata
{
    public string[] Parents { get; set; } = [];
}

public class DriveMetadataMinimal
{
    public string Id { get; set; } = "";
    public string MimeType { get; set; } = "";
    public string Name { get; set; } = "";
    public string[] PermissionIds { get; set; } = [];
}

public class DriveMetadataStandard : DriveMetadataMinimal
{
    public string Size { get; set; } = "";
    public string FileExtension { get; set; } = "";
    public string FullFileExtension { get; set; } = "";
    public string WebContentLink { get; set; } = "";
    public string[] Parents { get; set; } = [];
}

public class DriveSearchResult
{
    public DriveMetadataMinimal[] Files { get; set; } = [];
}

public class PermissionCreatingMetadata
{
    public string Role { get; set; } = "reader";
    public string Type { get; set; } = "anyone";

}
