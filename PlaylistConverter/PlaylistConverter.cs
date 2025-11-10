namespace PlaylistConverter;

public class ToSmplConverter
{
    
}

public class ToM3u8Converter
{
    
}

public static class PlaylistConverterErrorThrower
{
    public static Action<string, int>? DefaultErrorHandler { get; set; }
    
    public static void ThrowError(string message, int errorCode)
    {
        DefaultErrorHandler?.Invoke(message, errorCode);
    }
    
    public static void ThrowError(string message)
    { ThrowError(message, 1); }

    public static void ThrowError(string message, int errorCode, Action<string, int>? CustomHandler)
    {
        CustomHandler?.Invoke(message, errorCode);
    }
}

public static class PlaylistFileReadWrite
{
    public static Func<string, string>? DefaultReader { get; set; }
    public static Action<string, string>? DefaultWriter { get; set; }

    public static string Read(string path)
    {
        string? fileText =  DefaultReader?.Invoke(path);

        return fileText != null ? fileText : String.Empty;
    }

    public static string Read(string path, Func<string, string>? CustomReader)
    {
        string? fileText = CustomReader?.Invoke(path);
        
        return fileText != null ? fileText : String.Empty;
    }

    public static void Write(string path, string contents)
    {
        DefaultWriter?.Invoke(path, contents);
    }

    public static void Write(string path, string contents, Action<string, string>? CustomWrite)
    {
        CustomWrite?.Invoke(path, contents);
    }
}

public interface IPlaylistConverterUtil
{
    public static char[] FolderSeparator = ['/', '\\'];    // prioritizes backslash
    
    public abstract static Dictionary<string, string> ValidExtensions { get; }

    public abstract static bool IsValidExtension(string inpArg, string command);
    public abstract static bool IsWildCard(string inpArg);

    public abstract static string getFileAsString(string dir);
}