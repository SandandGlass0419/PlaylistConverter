using System.Text.Json;

namespace PlaylistConverter;

public partial class SmplFile : IPlaylist
{
    public string name = "Default Playlist Name";
    public string? Name
    {
        get { return name; }
        set { name = value != null ? value : name; }
    }

    public SortedDictionary<int, string> PlaylistContents { get; protected set; } = new();
    
    public SmplFile() {}
    
    public SmplFile(string path)
    {
        Deserialize(PlaylistFileReadWrite.Read(path));
    }

    public static object Create<T>(T playlistFile) where T : IPlaylist
    {
        return new SmplFile()
        {
            Name = playlistFile.Name,
            PlaylistContents = playlistFile.PlaylistContents
        };
    }


    public void Deserialize(string fileText)
    {
        JsonElement RootElement = JsonDocument.Parse(fileText).RootElement;

        Name = ParseName(RootElement);

        var ParsedMembers = ParseMembers(RootElement);
        foreach (var pair in ParsedMembers)
        {
            PlaylistContents.Add(pair.Key, pair.Value);
        }
    }

    protected string? ParseName(JsonElement RootElement)
    {
        if (RootElement.TryGetProperty("name", out var nameProperty))
        {
            return nameProperty.GetString();
        }

        return null;
    }

    protected KeyValuePair<int, string>[] ParseMembers(JsonElement RootElement)
    {
        if (!RootElement.TryGetProperty("members", out var MemberProperty))
        { PlaylistConverterErrorThrower.ThrowError("File has wrong format! (Doesn't have members property)"); }

        if (MemberProperty.ValueKind != JsonValueKind.Array)
        { PlaylistConverterErrorThrower.ThrowError("File has wrong format! (members property isn't array type"); }

        int playlistSize = MemberProperty.GetArrayLength();
        KeyValuePair<int, string>[] ParsedMembers = new KeyValuePair<int, string>[playlistSize];

        for (int i = 0; i < playlistSize; i++)
        {
            KeyValuePair<int, string>? parsedMemberInfo = ParseMemberInfo(MemberProperty[i]);
            if (parsedMemberInfo == null)
            { PlaylistConverterErrorThrower.ThrowError($"File has wrong format! ({i}th elemnent in members property failed to parse)"); }

            ParsedMembers[i] = (KeyValuePair<int, string>) parsedMemberInfo;
        }

        return ParsedMembers;
    }

    protected KeyValuePair<int, string>? ParseMemberInfo(JsonElement MemberElement)
    {
        if (!MemberElement.TryGetProperty("order", out var orderProperty)) return null;

        if (!MemberElement.TryGetProperty("info", out var infoProperty)) return null;

        return new KeyValuePair<int, string>(orderProperty.GetInt32(), infoProperty.GetString());
    }
}

public partial class M3u8File : IPlaylist
{
    public const string Header = "#EXTM3U";

    public string? Name { get; set; }

    public SortedDictionary<int, string> PlaylistContents { get; protected set; } = new();

    public M3u8File() {}
    
    public M3u8File(string path)
    {
        Deserialize(PlaylistFileReadWrite.Read(path));
    }

    public static object Create<T>(T playlistFile) where T : IPlaylist
    {
        return new M3u8File()
        {
            Name = playlistFile.Name,
            PlaylistContents = playlistFile.PlaylistContents
        };
    }

    public void Deserialize(string fileText)
    {
        string[] splitText = fileText.Split('\n');

        if (!HasHeaderDirective(splitText.First())) return;
        
        foreach (var entery in splitText)
        {
            if (ParsePramedDirectives(entery)) continue;

            var pair = ParsePaths(PlaylistContents.Count, entery);
            PlaylistContents.Add(pair.Key, pair.Value);
        }
    }

    public bool ParsePramedDirectives(string entery)
    {
        if (!entery.StartsWith("#")) return false;
        if (entery == Header) return true;

        string[] splitEntry = entery.Split(":");

        if (splitEntry.Length != 2) return false;
        
        switch (splitEntry.First())
        {
            case "#PLAYLIST":   // no ':' in directive, already removed in Split()
                Name = splitEntry.Last();
                return true;
        }

        return false;
    }

    public static bool HasHeaderDirective(string firstEntery)
    {
        return firstEntery == Header;
    }

    public KeyValuePair<int, string> ParsePaths(int order, string entery)
    {
        return new KeyValuePair<int, string>(order, entery);
    }
}