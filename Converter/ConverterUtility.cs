namespace PlaylistConverter;

public static class SmplConverter
{
    
}

public static class ConverterErrorThrower
{
    public static Action<string, int>? DefaultErrorHandler { get; set; }
    
    public static void ThrowError(string message, int errorCode)
    {
        DefaultErrorHandler?.Invoke(message, errorCode);
    }

    public static void ThrowError(string message, int errorCode, Action<string, int>? customHandler)
    {
        customHandler?.Invoke(message, errorCode);
    }
}