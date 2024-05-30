namespace BlazorFileShake.Lib;

public static class Utils
{
    public static string FormatBytes(long bytes)
    {
        if (bytes == 0)
        {
            return "0 B";
        }
        string[] sizes = ["B", "KB", "MB", "GB", "TB", "PB"];
        int np = (int)Math.Floor(Math.Log(bytes) / Math.Log(1024));
        double finalSize = bytes / Math.Pow(1024, np);
        return string.Format("{0:0.#} {1}", finalSize, sizes[np]);
    }
}