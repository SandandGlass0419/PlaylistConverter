namespace PlaylistConverter;

public static class PlaylistConverterErrorThrower
{
    public static Action<string, int>? DefaultErrorHandler { get; set; }
    
    public static void ThrowError(string message, int errorCode, bool throwError = true)
    {
        if (!throwError) return;
        
        DefaultErrorHandler?.Invoke(message, errorCode);
    }
    
    public static void ThrowError(string message, bool throwError = true)
    { ThrowError(message, 1, throwError); }

    public static void ThrowError(string message, int errorCode, Action<string, int>? CustomHandler, bool throwError = true)
    {
        if (!throwError) return;
        
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

public static class ValidExtensions
{
    public enum Enum
    {
        M3U8 = 0,
        SMPL = 1,
    }

    public static readonly List<string> ExtensionList = ["m3u8", "smpl"];

    public static string getString(Enum extension)
    {
        return ExtensionList[(int) extension];
    }
}

public interface IPlaylistConverterUtil
{
    public static char[] FolderSeparator = ['/', '\\'];    // prioritizes backslash

    public abstract static bool IsValidExtension(string arg, ValidExtensions.Enum expectedExtension);
    public abstract static string DefaultExportPath(string path, ValidExtensions.Enum extension);
    public abstract static bool IsWildCard(string arg);
    
}