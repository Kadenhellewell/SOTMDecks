using SOTMDecks;

class Program
{
    enum Mode { Hero, Environment }

    static void Main(string[] args)
    {
        Mode mode = ChooseMode(args);
        string subDir = mode == Mode.Hero ? "character_files" : "environment_files";
        string defaultBasePath = Path.Combine(AppContext.BaseDirectory, subDir);

        string? filename = args.Length > 1 ? args[1] : null;
        string basePath = args.Length > 2 ? args[2] : defaultBasePath;

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

        if (mode == Mode.Hero)
        {
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
        else
        {
            EnvironmentDeck envDeck = new EnvironmentDeck(filePath);
            if (envDeck.GetCount() == 0)
            {
                Console.WriteLine($"{envDeck.Name} has no cards");
                return;
            }

            EnvironmentGame game = new EnvironmentGame(envDeck);
            Console.Clear();
            game.Start();
        }
    }

    static Mode ChooseMode(string[] args)
    {
        if (args.Length > 0)
        {
            string a = args[0].ToLower();
            if (a == "hero" || a == "h") return Mode.Hero;
            if (a == "environment" || a == "env" || a == "e") return Mode.Environment;
            Console.WriteLine($"Unknown mode '{args[0]}' - falling through to prompt");
        }

        while (true)
        {
            Console.WriteLine("Mode? (1) Hero  (2) Environment");
            string? input = Console.ReadLine()?.Trim().ToLower();
            if (input == "q") Environment.Exit(0);
            switch (input)
            {
                case "1":
                case "h":
                case "hero":
                    return Mode.Hero;
                case "2":
                case "e":
                case "env":
                case "environment":
                    return Mode.Environment;
                default:
                    Console.WriteLine("Pick 1 or 2 (or q to quit)");
                    break;
            }
        }
    }
}
