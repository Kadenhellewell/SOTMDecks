using SOTMDecks;

class Program
{
    static void Main(string[] args)
    {
        string? filename = args.Length > 0 ? args[0] : null;
        string basePath = args.Length > 1
            ? args[1]
            : Path.Combine(AppContext.BaseDirectory, "character_files");

        string filePath = "";
        bool fileExists = false;
        while (filename is null || !fileExists)
        {
            if (filename is null)
            {
                Console.WriteLine("File name (don't include extension)?");
                filename = Console.ReadLine();
            }
            if (filename == "q") Environment.Exit(0);
            filePath = Path.Combine(basePath, $"{filename}.json");
            fileExists = File.Exists(filePath);
            if (!fileExists)
            {
                Console.WriteLine($"File doesn't exist: {filePath}");
                filename = null;
            }
        }
        

        HeroDeck myDeck = new HeroDeck(filePath);
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
