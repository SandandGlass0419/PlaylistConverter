using System.Text;
using PlaylistConverter;

namespace ConsoleConverter;

class Program
{
    static void Main(string[] args)
    {
        PlaylistConverterErrorThrower.DefaultErrorHandler = ConsoleErrorHandler;
        PlaylistFileReadWrite.DefaultReader = ConsoleReader;
        PlaylistFileReadWrite.DefaultWriter = ConsoleWriter;

        // M3u8File file = new M3u8File(@"C:\samples\Classics.m3u8");
        //
        // file.Export(@"C:\samples\test.m3u8");

        SmplFile file = new SmplFile(@"C:\samples\Season 2-2.smpl");
        
        file.Export(@"C:\samples\smpltest.smpl");
    }

    public static readonly Dictionary<string, Func<string, string, string>> ArgCommands = new()
    {
        { "tom3u8", Commands.convertToM3u },
        { "tosmpl", Commands.convertToSmpl }
    };

    protected static class Commands
    {
        public static string convertToM3u(string inpArg, string command)
        {
            string dir = PlaylistConverterUtil.ValidatePath(inpArg, command);
            string fileString = PlaylistConverterUtil.getFileAsString(dir);
            
            return String.Empty;
        }

        public static string convertToSmpl(string inpArg, string command)
        {
            return String.Empty;
        }
    }
    
    private static void ConsoleErrorHandler(string message, int errorCode)
    {
        Console.Error.WriteLine(message);
        Environment.Exit(errorCode);
    }

    private static string ConsoleReader(string path)
    {
        return File.ReadAllText(path, Encoding.UTF8);
    }

    private static void ConsoleWriter(string path, string contents)
    {
        File.WriteAllText(path, contents, Encoding.UTF8);
    }
}

public class PlaylistConverterUtil : IPlaylistConverterUtil
{
    public static string ValidatePath(string inpArg, string command)
    {
        if (!File.Exists(inpArg))
        { PlaylistConverterErrorThrower.ThrowError("This file doesn't exist. Try a different one!"); }
        
        if (!IsValidExtension(inpArg, command))
        { PlaylistConverterErrorThrower.ThrowError("This file isn't using the right extension. Try ones with " + String.Join(" or ", ValidExtensions.Values)); }

        return inpArg;
    }

    public static Dictionary<string, string> ValidExtensions { get; } = new()
    {
        { "tom3u8", "m3u8" },
        { "tosmpl", "smpl" }
    };

    public static bool IsValidExtension(string inpArg, string command)
    {
        string extension = inpArg.Split('.').Last();
        return extension == ValidExtensions[command];
    }

    public static bool IsWildCard(string inpArg)
    {
        return inpArg.Split('\\').Last().StartsWith("*.");
    }

    public static string getFileAsString(string dir)
    {
        return File.ReadAllText(dir);
    }
}