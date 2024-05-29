namespace BlazorFileShake.Services;

public static class AppState
{
    public static string UserToken { get; set; } = "";
    public static bool IsSignedIn { get; set; } = false;
    public static bool IsUploadComplete { get; set; } = false;
    public static bool IsDriveUploadInitiated { get; set; } = false;
    public static bool IsDriveUploadComplete { get; set; } = false;
}