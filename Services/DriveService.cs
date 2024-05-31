namespace BlazorFileShake.Services;

public static class DriveService
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static async Task<string> CheckForUploadFolder()
    {
        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppState.UserToken);
        try
        {
            const string SEARCH_KEY = "trashed = false and mimeType = 'application/vnd.google-apps.folder' and visibility = 'anyoneWithLink' and name = 'FileShake'";
            const string REQUIRED_FIELDS = "files(id, name, mimeType, permissionIds)";
            const string URL = $"https://www.googleapis.com/drive/v3/files?q={SEARCH_KEY}&fields={REQUIRED_FIELDS}";
            HttpResponseMessage response = await httpClient.GetAsync(URL);
            string responseBody = await response.Content.ReadAsStringAsync();
            DriveSearchResult driveSearchResult = JsonSerializer.Deserialize<DriveSearchResult>(responseBody, JsonSerializerOptions) ??
                throw new Exception("There was a problem in searching for the folder!");
            return driveSearchResult.Files.Length == 0 ? "" : driveSearchResult.Files[0].Id;
        }
        catch
        {
            return "Error";
        }
    }

    public static async Task<string> CreateUploadFolder()
    {
        FolderCreatingMetadata folderMetadata = new()
        {
            Name = "FileShake",
            MimeType = "application/vnd.google-apps.folder"
        };
        JsonContent folderMetadataJSONContent = JsonContent.Create(folderMetadata, new MediaTypeHeaderValue("application/json"), JsonSerializerOptions);

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppState.UserToken);
        try
        {
            const string REQUIRED_FIELDS = "id, name, mimeType, permissionIds";
            const string URL = $"https://www.googleapis.com/drive/v3/files?uploadType=media&fields={REQUIRED_FIELDS}";
            HttpResponseMessage response = await httpClient.PostAsync(URL, folderMetadataJSONContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            DriveMetadataMinimal createdFolder = JsonSerializer.Deserialize<DriveMetadataMinimal>(responseBody, JsonSerializerOptions) ??
                throw new Exception("There was a problem in creating the folder!");
            bool isPermissionCreated = await CreateSharedPermission(createdFolder.Id);
            return isPermissionCreated ? createdFolder.Id : throw new Exception("There was a problem in creating the shared folder!");
        }
        catch
        {
            return "Error";
        }
    }

    public static async Task<bool> CreateSharedPermission(string folderId)
    {
        PermissionCreatingMetadata permissionMetadata = new();
        JsonContent permissionMetadataJSONContent = JsonContent.Create(permissionMetadata, new MediaTypeHeaderValue("application/json"), JsonSerializerOptions);

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppState.UserToken);
        try
        {
            string URL = $"https://www.googleapis.com/drive/v3/files/{folderId}/permissions";
            HttpResponseMessage response = await httpClient.PostAsync(URL, permissionMetadataJSONContent);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public static string ProcessUploadResult(string uploadResponse)
    {
        try
        {
            DriveMetadataStandard uploadedFile = JsonSerializer.Deserialize<DriveMetadataStandard>(uploadResponse, JsonSerializerOptions) ??
                throw new Exception("There was a problem in uploading the file!");
            string downloadKey = $"{uploadedFile.Id}&{uploadedFile.Size}&{uploadedFile.Name}&{uploadedFile.FileExtension}";
            string encodedDownloadKey = HttpUtility.UrlEncode(Utils.CaesarCipher(downloadKey, 8));
            string downloadLink = $"https://bfs.subhamk.com/{encodedDownloadKey}";
            return downloadLink;
        }
        catch
        {
            return "Error";
        }
    }
}