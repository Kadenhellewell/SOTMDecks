using SOTMDecks;

class Program
{
    static void Main(string[] args)
    {
        string basePath = "C:\\Users\\kdex9\\OneDrive\\Documents\\Personal_Projects\\SOTM\\SOTMDecks";

        string filePath = "";

        string? filename = null;
        bool fileExists = false;
        while (filename is null || !fileExists)
        {
            Console.WriteLine("File name (don't include extension)?");
            filename = Console.ReadLine();
            filePath = $"{basePath}/{filename}.json";
            fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                fileExists = false;
                Console.WriteLine("File doesn't exist");
            }
        }
        
        Deck myDeck = new Deck(filePath);

        Game game = new Game(new Player(myDeck));
        game.Start();
    }
}
