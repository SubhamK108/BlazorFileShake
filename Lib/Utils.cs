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

    public static string CaesarCipher(string text, int key)
    {
        char[] buffer = text.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char letter = buffer[i];
            if (char.IsLetter(letter))
            {
                char offset = char.IsUpper(letter) ? 'A' : 'a';
                letter = (char)(((letter + key - offset) % 26) + offset);
                buffer[i] = letter;
            }
        }
        return new string(buffer);
    }

    public static string DecryptCaesarCipher(string cipherText, int key)
    {
        return CaesarCipher(cipherText, 26 - key);
    }
}