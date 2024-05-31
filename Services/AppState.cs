namespace BlazorFileShake.Services;

public static class AppState
{
    public static string UserToken { get; set; } = "";
    public static bool IsSignedIn { get; set; } = false;
    public static bool IsUploadComplete { get; set; } = false;
    public static bool IsDriveUploadInitiated { get; set; } = false;
    public static bool IsDriveUploadComplete { get; set; } = false;
    public static List<UploadedFile> UploadedFiles { get; set; } = [];
    public static string FinalZipFileName { get; set; } = "";
    public static string FinalDownloadLink { get; set; } = "";

    public static void RefreshAppState()
    {
        UserToken = "";
        IsSignedIn = false;
        IsUploadComplete = false;
        IsDriveUploadInitiated = false;
        IsDriveUploadComplete = false;
        UploadedFiles = [];
        FinalZipFileName = "";
        FinalDownloadLink = "";
    }
}