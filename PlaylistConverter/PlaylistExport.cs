using System.Text.Json;

namespace PlaylistConverter;

public partial class SmplFile
{
    public string Export()
    {
        return JsonSerializer.Serialize(new SerializeableSmplFile(this));
    }

    public void ExportAndWrite(string path)
    {
        PlaylistConverterFileReadWrite.Write(path, Export());
    }
}

public class SerializeableSmplFile
{
    public List<SerializeableSmplSong> members;
    public string name;
    public const int recentlyPlayedDate = 0;
    public const int sortBy = 4;
    public const int version = 1;

    public SerializeableSmplFile(SmplFile smplFile)
    {
        this.members = SerializeMembers(smplFile.Members);
        this.name = smplFile.Name != null ? smplFile.Name : "My Playlist";
    }

    public static List<SerializeableSmplSong> SerializeMembers(SortedDictionary<int, string> members)
    {
        List<SerializeableSmplSong> serializedMembers = [];
        
        foreach (var pair in members)
        {
            serializedMembers.Add(new SerializeableSmplSong(pair));
        }

        return serializedMembers;
    }
}

public class SerializeableSmplSong
{
    public string artist = "\u003cunknown\u003e";
    public string info;
    public int order;
    public string title;
    public const int type = 65537;

    public SerializeableSmplSong(string artist, KeyValuePair<int, string> memberElement)
    {
        this.artist = artist;
        this.info = memberElement.Value;
        this.order = memberElement.Key;
        this.title = ExtractTitle(this.info);
    }
    
    public SerializeableSmplSong(KeyValuePair<int, string> memberElement)
    {
        this.info = memberElement.Value;
        this.order = memberElement.Key;
        this.title = ExtractTitle(this.info);
    }

    public static string ExtractTitle(string info)
    {
        foreach (var separator in IPlaylistConverterUtil.FolderSeparator)
        {
            if (info.Contains(separator))
            {
                return info.Split(separator).Last().Split('.').First();
            }
        }

        return info;
    }
}