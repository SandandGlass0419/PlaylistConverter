using System.Text.Json;

namespace PlaylistConverter;

public partial class SmplFile
{
    private string name = "My Playlist";
    public string? Name
    {
        get { return name; }
        protected set { name = value != null ? value : name; }
    }

    public SortedDictionary<int, string> Members { get; protected set; } = new();

    public SmplFile(string path)
    {
        Deserialize(PlaylistConverterFileReadWrite.Read(path));
    }
    
    public void Deserialize(string fileText)
    {
        JsonElement RootElement = JsonDocument.Parse(fileText).RootElement;

        Name = ParseName(RootElement);

        var ParsedMembers = ParseMembers(RootElement);
        foreach (var pair in ParsedMembers)
        {
            Members.Add(pair.Key, pair.Value);
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

public class M3u8File
{
    
}