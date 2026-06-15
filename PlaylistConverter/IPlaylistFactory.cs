namespace PlaylistConverter;

public interface IPlaylistFactory
{
    public List<PlaylistComponent> Components { get; set; }

    public void InitComponents(Stream configXML);
    
    public Playlist Create(byte[] playlistFile);
    public byte[] Export(Playlist playlist);
}