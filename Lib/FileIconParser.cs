namespace BlazorFileShake.Lib;

public static class FileIconParser
{
    public static string GetFileIconStyle(string fileExtension)
    {
        Dictionary<string, string> iconStyles = new()
        {
            {"MP4,MKV,WMV,MOV,AVI,AVCHD,FLV,WEBM,MPEG,MPEG-2", "fa-solid fa-video"},
            {"JPG,JPEG,PNG,SVG,AVIF,GIF,WEBP", "fa-solid fa-image"},
            {"ZIP,RAR,TAR,GZ", "fa-solid fa-file-zipper"},
            {"TXT", "fa-solid fa-file-lines"},
            {"PDF", "fa-solid fa-file-pdf"},
            {"DOC, DOCX", "fa-solid fa-file-word"},
            {"XLS, XLSX", "fa-solid fa-file-excel"},
            {"PPT, PPTX", "fa-solid fa-file-powerpoint"}
        };
        foreach ((string extensions, string iconStyle) in iconStyles)
        {
            if (extensions.Contains(fileExtension))
            {
                return iconStyle;
            }
        }
        return "fa-solid fa-file";
    }
}