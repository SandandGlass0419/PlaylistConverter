using PlaylistConverter;

namespace ConsoleConverter;

class Program
{
    static void Main(string[] args)
    {
        ComponentRegistry.Register();

        LinePlaylistFactory factory = new("");
    }
}