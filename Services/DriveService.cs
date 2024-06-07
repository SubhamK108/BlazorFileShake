namespace BlazorFileShake.Services;

public static class DriveService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private static readonly Random _random = new();

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
            DriveSearchResult driveSearchResult = JsonSerializer.Deserialize<DriveSearchResult>(responseBody, _jsonSerializerOptions) ??
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
        JsonContent folderMetadataJSONContent = JsonContent.Create(folderMetadata, new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions);

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppState.UserToken);
        try
        {
            const string REQUIRED_FIELDS = "id, name, mimeType, permissionIds";
            const string URL = $"https://www.googleapis.com/drive/v3/files?uploadType=media&fields={REQUIRED_FIELDS}";
            HttpResponseMessage response = await httpClient.PostAsync(URL, folderMetadataJSONContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            DriveMetadataMinimal createdFolder = JsonSerializer.Deserialize<DriveMetadataMinimal>(responseBody, _jsonSerializerOptions) ??
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
        JsonContent permissionMetadataJSONContent = JsonContent.Create(permissionMetadata, new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions);

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

    public static async Task<string> UploadFile(byte[] fileByteArray, string fileName, string fileType, string folderId)
    {
        FileCreatingMetadata fileMetadata = new()
        {
            Name = fileName,
            MimeType = fileType,
            Parents = [folderId]
        };
        JsonContent fileMetadataJSONContent = JsonContent.Create(fileMetadata, new MediaTypeHeaderValue("application/json"), _jsonSerializerOptions);
        ByteArrayContent fileByteArrayContent = new(fileByteArray);
        fileByteArrayContent.Headers.ContentType = new(fileType);

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AppState.UserToken);
        MultipartFormDataContent content = new()
        {
            { fileMetadataJSONContent, "metadata" },
            { fileByteArrayContent, "file" }
        };
        try
        {
            const string REQUIRED_FIELDS = "id, name, mimeType, size, fileExtension, fullFileExtension, webContentLink, parents, permissionIds";
            const string URL = $"https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart&fields={REQUIRED_FIELDS}";
            HttpResponseMessage response = await httpClient.PostAsync(URL, content);
            DriveMetadataStandard uploadResponseFile = await response.Content.ReadFromJsonAsync<DriveMetadataStandard>(_jsonSerializerOptions) ??
                throw new Exception("There was a problem in uploading the file!");
            string fileId = uploadResponseFile.Id;
            string fileName2 = HttpUtility.UrlEncode(uploadResponseFile.Name);
            string fileSize = HttpUtility.UrlEncode(Utils.FormatBytes(Convert.ToInt64(uploadResponseFile.Size)));
            string fileExtension = uploadResponseFile.FileExtension.Equals("") switch
            {
                true => fileName2[(fileName2.LastIndexOf('.') + 1)..].ToUpper(),
                false => uploadResponseFile.FileExtension.ToUpper()
            };
            string downloadKey = $"{fileId}&{fileName2}&{fileSize}&{fileExtension}";
            int key = _random.Next(1, 26);
            string encodedDownloadKey = Utils.CaesarCipher(downloadKey, key);
            string downloadLink = $"https://bfs.subhamk.com/r/{encodedDownloadKey}/{key}";
            return downloadLink;
        }
        catch
        {
            return "Error";
        }
    }

    public static async Task<bool> VerifySharedFile(string fileId)
    {
        try
        {
            string fileViewLink = $"https://drive.google.com/file/d/{fileId}/view?usp=drivesdk";
            using HttpClient httpClient = new();
            HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, fileViewLink));
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}