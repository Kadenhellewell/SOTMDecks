using SOTMDecks;

class Program
{
    static void Main(string[] args)
    {
        string basePath = "C:\\Users\\kdex9\\OneDrive\\Documents\\Personal_Projects\\SOTM\\SOTMDecks\\character_files";

        string filePath = "";

        string? filename = null;
        bool fileExists = false;
        while (filename is null || !fileExists)
        {
            Console.WriteLine("File name (don't include extension)?");
            filename = Console.ReadLine();
            if (filename == "q") Environment.Exit(0);
            filePath = $"{basePath}/{filename}.json";
            fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                fileExists = false;
                Console.WriteLine("File doesn't exist");
            }
        }
        

        Deck myDeck = new Deck(filePath);
        if (myDeck.GetCount() != 40) 
        {
            Console.WriteLine($"{myDeck.Name} doesn't have 40 cards - it actually has {myDeck.GetCount()}");
            return;
        }

        Game game = new Game(new Player(myDeck));

        Console.Clear();
        game.Start();
    }
}
