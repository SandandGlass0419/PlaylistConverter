namespace PlaylistConverter;

public static class ComponentRegistry
{
    private static Dictionary<string, Component> Registry = new();

    public static readonly PlaylistComponent Playlist = new();
    public static readonly HeaderComponent Header = new();
    public static readonly TitleComponent Title = new();
    public static readonly SongComponent Song = new();

    public static void Register(params Component[] components)
    {
        foreach (var component in components)
        {
            Registry.Add(component.NameKey, component);
        }
    }

    public static void Register()
    {
        Register(Header, Title, Song);
    }

    public static Component? Get(string key)
    {
        if (!Registry.ContainsKey(key)) return null;

        return Registry[key].Clone();
    }
}

public static class ComponentParams
{
    public static readonly char Syntax = '%';

    public static readonly string PlaylistTitle = AddSyntax("playlist_title");
    public static readonly string SongTitle = AddSyntax("song_title");
    public static readonly string SongArtist = AddSyntax("song_artist");
    public static readonly string SongPath = AddSyntax("song_path");

    public static string AddSyntax(string parameter) => Syntax + parameter + Syntax;
}

public abstract class Component
{
    public abstract string NameKey { get; }
    public abstract string[] Params { get; }
    public abstract string Impl { get; set; }

    public abstract Component Clone();

    public string ReplaceParams(params string[] newValues)
    {
        string replacedImpl = Impl;

        for (int i = 0; i < Params.Length; i++)
        {
            replacedImpl = i < newValues.Length ? 
                replacedImpl.Replace(Params[i], newValues[i]) :
                replacedImpl.Replace(Params[i], "");
        }

        return replacedImpl;
    }
}

public sealed class PlaylistComponent : Component
{
    public override string NameKey { get; } = "playlist";
    public override string[] Params { get; } = [];
    public override string Impl { get; set; } = "";

    public const string TypeAttribute = "type"; // xml type attribute name
    public const string FormatAttribute = "format"; // xml format attribute name
    
    public override Component Clone() => new PlaylistComponent();
}

public sealed class SongComponent : Component
{
    public override string NameKey { get; } = "song";
    public override string[] Params { get; } = [ComponentParams.SongPath, ComponentParams.SongTitle, ComponentParams.SongArtist];
    public override string Impl { get; set; }

    public SongComponent(string impl = "")
    {
        Impl = impl;
    }

    public string ReplaceParams(string songPath, string songTitle = "", string songArtist = "") => base.ReplaceParams(songPath, songTitle, songArtist);
    
    public override Component Clone() => new SongComponent(Impl);
}

public sealed class HeaderComponent : Component
{
    public override string NameKey { get; } = "header";
    public override string[] Params { get; } = [];
    public override string Impl { get; set; }

    public HeaderComponent(string impl = "")
    {
        Impl = impl;
    }
    
    public override Component Clone() => new HeaderComponent(Impl);
}

public sealed class TitleComponent : Component
{
    public override string NameKey { get; } = "title";
    public override string[] Params { get; } = [ComponentParams.PlaylistTitle];
    public override string Impl { get; set; }

    public TitleComponent(string impl = "")
    {
        Impl = impl;
    }
    
    public override Component Clone() => new TitleComponent(Impl);
    
    public string ReplaceParams(string playlistTitle) => base.ReplaceParams(playlistTitle);
}