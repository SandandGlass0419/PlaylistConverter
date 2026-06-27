using System.Xml.Linq;

namespace PlaylistConverter;

public interface IPlaylistFactory
{
    public List<Component> Components { get; set; }

    public void SetComponents(XElement configXml);
    
    public Playlist Create(string[] playlistFile);
    public string[] Export(Playlist playlist);
}