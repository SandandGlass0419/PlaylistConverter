using System.Text.Encodings.Web;
using System.Text.Json;

namespace PlaylistConverter;

public partial class SmplFile
{
    public string Serialize()
    {
        return JsonSerializer.Serialize(new SerializeableSmplFile(this), 
            new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
    }

    public void Export(string path)
    {
        PlaylistFileReadWrite.Write(path, Serialize());
    }
}

public class SerializeableSmplFile
{
    public List<SerializeableSmplSong> members { get; protected set; }
    public string name { get; protected set; }
    public int recentlyPlayedDate { get; } = 0;
    public int sortBy { get; } = 4;
    public int version { get; } = 1;

    public SerializeableSmplFile(SmplFile smplFile)
    {
        this.members = SerializeMembers(smplFile.PlaylistContents);
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
    public string artist { get; protected set; } = "\u003cunknown\u003e";
    public string info { get; protected set; }
    public int order { get; protected set; }
    public string title { get; protected set; }
    public int type { get; } = 65537;

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

public partial class M3u8File
{
    public string Serialize()
    {
        string fileText = String.Empty;
        
        AddDirectives(ref fileText);
        
        AddPaths(ref fileText);

        return fileText;
    }

    public void AddDirectives(ref string fileText)
    {
        fileText = fileText.Insert(0, Header + '\n');

        if (!String.IsNullOrEmpty(Name))
        {
            fileText += "#PLAYLIST:" + Name + '\n';
        }
    }

    public void AddPaths(ref string fileText)
    {
        foreach (var pair in PlaylistContents)
        {
            fileText += pair.Value + '\n';
        }
    }
    
    public void Export(string path)
    {
        PlaylistFileReadWrite.Write(path, Serialize());
    }
}