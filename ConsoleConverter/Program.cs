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

        if (args.Length != 0 && ArgCommands.TryGetValue(args[0], out var Command))
        {
            Command.Invoke(args);
        }
        else
        {
            PlaylistConverterErrorThrower.ThrowError($"Can not recognize argument {args[0]}. Try using 'help'.");
        }
    }

    public static readonly Dictionary<string, Action<string[]>> ArgCommands = new()
    {
        { Commands.TOM3U8, Commands.ConvertToM3u8 },
        { Commands.TOSMPL, Commands.ConvertToSmpl },
        { Commands.HELP, Commands.Help }
    };

    public static class Commands
    {
        public const string TOM3U8 = "tom3u8";
        public const string TOSMPL = "tosmpl";
        public const string HELP = "help";
        
        public static void ConvertToM3u8(string[] args)
        {
            ValidateArgLength(args, 3);
            
            PlaylistConverterUtil.ValidatePath(args[1], ValidExtensions.Enum.SMPL);
            string inpPath = args[1];
            
            SmplFile originalSmpl = new(inpPath);
            M3u8File convertedM3u8 = (M3u8File) M3u8File.Create(originalSmpl);

            string outPath = args[2];
            if (!PlaylistConverterUtil.IsValidExtension(outPath, ValidExtensions.Enum.M3U8))
            {
                outPath = PlaylistConverterUtil.DefaultExportPath(args[1], ValidExtensions.Enum.M3U8);
            }
            
            convertedM3u8.Export(outPath);
        }

        public static void ConvertToSmpl(string[] args)
        {
            ValidateArgLength(args, 3);
            
            PlaylistConverterUtil.ValidatePath(args[1], ValidExtensions.Enum.M3U8);
            string inpPath = args[1];

            M3u8File originalM3u8 = new(inpPath);
            SmplFile convertedSmpl = (SmplFile) SmplFile.Create(originalM3u8);

            string outPath = args[2];
            if (!PlaylistConverterUtil.IsValidExtension(outPath, ValidExtensions.Enum.SMPL))
            {
                outPath = PlaylistConverterUtil.DefaultExportPath(inpPath, ValidExtensions.Enum.SMPL);
            }
            
            convertedSmpl.Export(outPath);
        }

        public static void Help(string[] args)
        {
            PlaylistConverterErrorThrower.ThrowError
            (
                "PlaylistConverter converts .smpl playlist files into .m3u8 playlist files. Use following commands to convert files.\n" +
                "Commands:\n" +
                "\n" +
                $"{TOSMPL} InputM3u8FilePath OutputSmplFilePath\n" +
                "   Converts the file in path InputM3u8FilePath, and generates a new file at path OutputSmplFilePath.\n" +
                "   If OutputSmplFilePath exists, it will be overwrited.\n" +
                "\n" +
                $"{TOM3U8} InputSmplFilePath OutputM3u8FilePath\n" +
                "   Converts the file in path InputSmplFilePath, and generates a new file at path OutputSmplFilePath.\n" +
                "   If OutputSmplFilePath exists, it will be overwrited.\n" +
                "\n" +
                $"{HELP}\n" +
                "   Displays this message."
            );
        }
    }

    public static void ValidateArgLength(string[] args, int expected)
    {
        if (args.Length != expected)
        {
            PlaylistConverterErrorThrower.ThrowError($"Invalid argmuents used! (check length, expected {expected} arguments)");
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
        Directory.CreateDirectory(getDirectory(path));
        File.WriteAllText(path, contents, Encoding.UTF8);
    }
    
    private static string getDirectory(string path)
    {
        return path.Remove(path.Length - path.Split('\\').Last().Length);
    }
}

public class PlaylistConverterUtil : IPlaylistConverterUtil
{
    public static bool ValidatePath(string arg, ValidExtensions.Enum expectedExtension, bool throwError = true)
    {
        if (!File.Exists(arg))
        {
            PlaylistConverterErrorThrower.ThrowError("This file doesn't exist. Try a different one!", throwError);
            return false;
        }

        if (!IsValidExtension(arg, expectedExtension))
        {
            PlaylistConverterErrorThrower.ThrowError("This file isn't using the right extension. Try ones with " + String.Join(" or ", ValidExtensions.ExtensionList), throwError);
            return false;
        }

        return true;
    }

    public static bool IsValidExtension(string arg, ValidExtensions.Enum expectedExtension)
    {
        string extension = arg.Split('.').Last();
        return extension == ValidExtensions.getString(expectedExtension);
    }

    public static string DefaultExportPath(string path, PlaylistConverter.ValidExtensions.Enum extension)
    {
        return path + '.' + ValidExtensions.getString(extension);
    }

    public static bool IsWildCard(string arg)
    {
        return arg.Split('\\').Last().StartsWith("*.");
    }
}