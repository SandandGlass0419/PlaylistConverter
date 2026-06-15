using System.Xml;

namespace PlaylistConverter;

public class LinePlaylistFactory : IPlaylistFactory
{
    public string TypeName { get; set; } = String.Empty;
    public List<PlaylistComponent> Components { get; set; } = new();
    
    public LinePlaylistFactory(Stream configXML)
    {
        InitComponents(configXML);
    }
    
    public void InitComponents(Stream configXML)
    {
        throw new NotImplementedException();
    }

    public Playlist Create(byte[] playlistFile)
    {
        throw new NotImplementedException();
    }

    public byte[] Export(Playlist playlist)
    {
        throw new NotImplementedException();
    }
}