using System.Xml.Linq;

namespace PlaylistConverter;

public class LinePlaylistFactory : IPlaylistFactory
{
    public string typeName = "";
    public const string format = "line";
    public List<Component> Components { get; set; } = new();
    
    public LinePlaylistFactory(string filePath)
    {
        XElement configFile = XElement.Load(filePath);
        if (!ValidateFile(configFile)) return;
        
        SetTypeName(configFile);
        SetComponents(configFile);
    }
    
    public bool ValidateFile(XElement configXml)
    {
        if (configXml.Name != ComponentRegistry.Playlist.NameKey) return false; // doesn't set anything => can't read/export anything
        
        var playlistType = configXml.Attribute(PlaylistComponent.TypeAttribute);
        var playlistFormat = configXml.Attribute(PlaylistComponent.FormatAttribute);
        
        if (playlistType == null || playlistFormat == null) return false;
        if (playlistFormat.Value != format) return false;

        return true;
    }

    public void SetTypeName(XElement configXml)
    {
        var playlistType = configXml.Attribute(PlaylistComponent.TypeAttribute);
        if (playlistType == null) return;

        typeName = playlistType.Value;
    }
    
    public void SetComponents(XElement configXml)
    {
        var pairs = from item in configXml.Descendants() select (item.Name.ToString(), item.Value.Trim());

        foreach (var pair in pairs)
        {
            var component = ComponentRegistry.Get(pair.Item1);

            if (component != null)
            {
                component.Impl = pair.Item2;
                Components.Add(component);
            }
        }
    }

    public Playlist Create(string[] playlistFile)
    {
        throw new NotImplementedException();
    }

    public string[] Export(Playlist playlist)
    {
        throw new NotImplementedException();
    }
}