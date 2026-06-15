namespace PlaylistConverter;

public static class PlaylistComponents
{
    private static Dictionary<string, PlaylistComponent> Components = new();
    
    public static readonly PlaylistComponent Header = new("header", []);
    public static readonly PlaylistComponent PlaylistTitle = new("playlist_title", [PlaylistComponentParameters.PlaylistTitle]);
    public static readonly PlaylistComponent SongPath = new("song_path", [PlaylistComponentParameters.SongPath]);

    public static void Register(params PlaylistComponent[] components)
    {
        foreach (var component in components)
        {
            Components.Add(component.Name, component);
        }
    }

    public static void RegisterDefaults()
    {
        Register(Header, PlaylistTitle, SongPath);
    }
}

public static class PlaylistComponentParameters
{
    public static readonly char Syntax = '%';

    public static readonly string PlaylistTitle = AddSyntax("playlist_title");
    public static readonly string SongTitle = AddSyntax("song_title");
    public static readonly string SongArtist = AddSyntax("song_artist");
    public static readonly string SongPath = AddSyntax("song_path");

    public static string AddSyntax(string parameter) => Syntax + parameter + Syntax;
}

public record struct PlaylistComponent(string Name, string[] Parameters, string Implementation = "")
{
    
}
