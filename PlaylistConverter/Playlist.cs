namespace PlaylistConverter;

public record struct Song(string Title, string? Artist = null) {}

public class Playlist
{
    public string? Title { get; set; }
    public SortedList<int, Song> Contents { get; set; } = new();

    // maybe add more stuff...
    
    public Playlist(string? title = null)
    {
        Title = title;
    }
}