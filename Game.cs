using SOTMDecks.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks
{
    internal class Game
    {
        public Game(Player player) 
        {
            Player = player;
        }

        public Player Player { get; }
        
        private CardCollection<HeroCard> KO = new CardCollection<HeroCard>("Cards removed from the game");
        private Stack<Command> commands = new Stack<Command>();

        private void Init()
        {
            Player.Shuffle();
            Console.WriteLine($"Playing as {Player.Name}");
            //Start with 4 cards
            Player.Draw(verbose: false);
            Player.Draw(verbose: false);
            Player.Draw(verbose: false);
            Player.Draw(verbose: false);

            PrintSetup();
            Player.PrintLocation(Location.Hand);
        }

        private void PrintSetup()
        {
            MiscHelpers.ColorPrint(ConsoleColor.Red, "HP: ");
            Console.WriteLine(Player.GetHP());
            if (Player.GetHP() == 0) 
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, "Incapacitated abilities:", newLine: true);
                foreach (string ability in Player.PlayerDeck.IncapacitatedAbilities)
                {
                    Console.WriteLine($"\t{ability}");
                }
            }
            else
            {
                Player.PlayerDeck.PrintInnatePower();
            }

            if (Player.Modifiers.Count > 0) 
            {
                PrintModifiers();
            }
        }

        private void PrintModifiers()
        {
            MiscHelpers.ColorPrint(ConsoleColor.DarkYellow, "Modifiers: ", newLine: true);
            foreach (Modifier modifier in Player.Modifiers) 
            {
                modifier.Print();
            }
            Console.WriteLine();
        }

        public void Start()
        {
            Init();
            GameLoop();
        }

        private void GameLoop()
        {
            bool keepGoing = true;

            while (keepGoing)
            {
                keepGoing = GetCommand();

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Retrieve the command from the player and call the associated function(s)
        /// </summary>
        /// <returns> Whether the command terminates the game </returns>
        private bool GetCommand()
        {
            Console.Write("> ");
            string? commandStr = Console.ReadLine()?.ToLower().Trim();
            if (commandStr is null) return true;

            commandStr = MiscHelpers.ExtractBrief(commandStr, out bool brief);

            Command? command = null;
            
            switch (commandStr)
            {
                // TODO: Implement a "search for the first x of a type" command
                case "draw":
                    command = new DrawCommand(Player, fromBottom: false);
                    break;
                case "draw bottom":
                    command = new DrawCommand(Player, fromBottom: true);
                    break;
                case "play": 
                    command = new PlayCommand(Player);
                    break;
                case "discard":
                    command = new DiscardCommand(fromDeck: false, Player);
                    break;
                case "discard hand":
                    command = new DiscardHandCommand(Player);
                    break;
                case "discard from deck":
                    command = new DiscardCommand(fromDeck: true, Player);
                    break;
                case "destroy":
                    command = new DestroyCommand(Player);
                    break;
                case "remove card":
                    command = new RemoveCommand(KO, Player);
                    break;
                case "search types":
                    command = new SearchTypeCommand(Player);
                    break;
                case "search special type":
                    command = new SearchSpecialCommand(Player);
                    break;
                case "search target":
                    command = new SearchTargetCommand(Player);
                    break;
                case "collect type":
                    command = new CollectTypeCommand(Player);
                    break;
                case "move card": 
                    command = new MoveCardCommand(Player);
                    break;
                case "pa to hand":
                    command = new PlayAreaToHandCommand(Player);
                    break;
                case "santa":
                    command = new SantaCommand(Player);
                    break;
                case "santa play":
                    command = new SantaPlayCommand(Player);
                    break;
                case "hand":
                    Player.PrintLocation(Location.Hand, brief: brief);
                    break;
                case "hand powers":
                    Player.PrintLocation(Location.Hand, CardCollection<HeroCard>.Filter.POWER);
                    break;
                case "play area":
                case "pa":
                    Player.PrintLocation(Location.PlayArea, brief: brief);
                    break;
                case "discard pile":
                case "dp":
                    Player.PrintLocation(Location.DiscardPile, brief: brief);
                    break;
                case "powers":
                    Player.PrintPowers(brief);
                    break;
                case "start":
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.START, brief);
                    break;
                case "end":
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.END, brief);
                    break;
                case "targets":
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief);
                    break;
                case "reveal":
                    RevealCards();
                    break;
                case "discard pile to deck":
                case "dp to deck":
                    Player.ShuffleDiscardIntoDeck();
                    break;
                case "shuffle":
                    Player.Shuffle();
                    break;
                case "set count":
                    SetCount();
                    Player.PrintLocation(Location.PlayArea);
                    break;
                case "damage":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Player, isDamage: true));
                    PrintSetup();
                    break;
                case "damage card":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Card, isDamage: true));
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "damage cards":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Cards, isDamage: true));
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "damage all":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.All, isDamage: true));
                    PrintSetup();
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "heal":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Player, isDamage: false));
                    PrintSetup();
                    break;
                case "heal card":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Card, isDamage: false));
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "heal cards":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.Cards, isDamage: false));
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "heal all":
                    RunCommand(new HPCommand(Player, HPCommand.Scope.All, isDamage: false));
                    PrintSetup();
                    Player.PrintLocation(Location.PlayArea, CardCollection<HeroCard>.Filter.TARGET, brief: true);
                    break;
                case "add modifier":
                case "add mod":
                    AddMod();
                    PrintSetup();
                    break;
                case "remove modifier":
                case "remove mod":
                    RemoveMod();
                    PrintSetup();
                    break;
                case "key words":
                    PrintKeyWords();
                    break;
                case "undo":
                    if (commands.Count > 0)
                    {
                        try
                        {
                            commands.Pop().Undo();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception undoing command: {ex}");
                        }
                    }
                        
                    else
                        Console.WriteLine("No commands to undo");
                    break;
                case "q":
                case "exit":
                    return false;
                case "":
                    PrintSetup();
                    break;
                default:
                    Console.WriteLine("Not a valid command");
                    break;
            }

            if (command is not null) RunCommand(command);

            return true;
        }

        /// <summary>
        /// Executes a command and, if it succeeds, pushes it onto the undo stack.
        /// </summary>
        private void RunCommand(Command command)
        {
            try
            {
                if (command.Execute())
                {
                    commands.Push(command);
                }
                else
                {
                    Console.WriteLine("Command failed to execute");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception executing command: {ex}");
            }
        }

        void PrintKeyWords()
        {
            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Bury: ");
            Console.WriteLine("Put the indicated card on the bottom of the associated deck, unless that deck has zero cards in it, in which case the card goes to the top of the appropriate trash instead. Character cards cannot be buried.");
            Console.WriteLine();

            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Collect: ");
            Console.WriteLine("Search the corresponding deck for the indicated amount of the indicated card/card-type. Put the card(s) found into your hand. Shuffle the deck.");
            Console.WriteLine();

            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Discover: ");
            Console.WriteLine("Reveal cards from the top of the associated deck until you reveal the indicated card/card-type or reach the bottom of the deck. Shuffle the other revealed cards into the deck, if any. If there are not any other revealed cards, do not shuffle the deck. Then, play the indicated cards in the order revealed.");
            Console.WriteLine();

            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Salvage: ");
            Console.WriteLine("Search the corresponding trash for the indicated amount of the indicated card/card-type, maintaining the order of cards in the trash. Put the card(s) found into your hand.");
            Console.WriteLine();

            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Summon: ");
            Console.WriteLine("Search for the indicated card(s). You may search the associated trash and deck for the indicated amount of the indicated card/card-type. Play the card(s) found. If you searched a deck, shuffle that deck.");
            Console.WriteLine();
        }

        public void RevealCards()
        {
            Option<int> numOpt = MiscHelpers.GetIntFromPlayer("How many?");
            if (!numOpt.HasValue) return;

            int num = numOpt.ValueOr(0);

            if (num <= 0)
            {
                Console.WriteLine($"I can't reveal {num} cards...");
                return;
            }

            if (Player.PlayerDeck.GetCount() < num)
            {
                Player.ShuffleDiscardIntoDeck();
            }
            Player.RevealCards(num);
        }

        private void SetCount()
        {
            Option<HeroCard> cardOpt = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (!cardOpt.HasValue) return;

            Option<int> countOpt = MiscHelpers.GetIntFromPlayer("Set count to what?");
            if (!countOpt.HasValue) return;

            cardOpt.ValueOrThrow().Count = countOpt.ValueOr(0);
        }

        private void AddMod()
        {
            Option<string> modStr = MiscHelpers.GetStringFromPlayer("Description");
            if (!modStr.HasValue) return;

            Modifier modToAdd = new Modifier(modStr.ValueOr(""), ConsoleColor.Cyan);
            Player.Modifiers.Add(modToAdd);
        }

        public void RemoveMod()
        {
            Console.WriteLine("Which one?");
            for (int i = 0; i < Player.Modifiers.Count; i++)
            {
                Console.WriteLine($"\t{i}: {Player.Modifiers[i]}");
            }

            Option<int> modIndexOpt = MiscHelpers.GetIntFromPlayer("");

            if (!modIndexOpt.HasValue) return;

            int modIndex = modIndexOpt.ValueOr(-1);

            if (modIndex >= Player.Modifiers.Count || modIndex < 0)
            {
                Console.WriteLine("Index out of range");
                return;
            }
            Player.RemoveMod(modIndex);
        }
    }
}
