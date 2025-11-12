namespace PlaylistConverter;

public interface IPlaylist
{
    public string? Name { get; } 
    public SortedDictionary<int, string> PlaylistContents { get; }

    public abstract static object Create<T>(T playlistFile) where T : IPlaylist;

    public void Deserialize(string fileText);
    public string Serialize();

    public void Export(string path);
}