using SOTMDecks.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class Game
    {
        public Game(Player player) 
        {
            Player = player;
            modifiers.Add(new Modifier("+1 damage dealt", ConsoleColor.Yellow));
            modifiers.Add(new Modifier("+1 damage taken", ConsoleColor.Magenta));
            modifiers.Add(new Modifier("-1 damage dealt", ConsoleColor.Blue));
            modifiers.Add(new Modifier("-1 damage taken", ConsoleColor.Green));
        }

        public Player Player { get; }
        private List<Modifier> modifiers = new List<Modifier>();
        private CardCollection KO = new CardCollection("Cards removed from the game");
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
                MiscHelpers.ColorPrint(ConsoleColor.Blue, "Innate Power: ");
                Console.WriteLine(Player.PlayerDeck.InnatePower);
            }
            PrintModifiers();
        }

        private void PrintModifiers()
        {
            MiscHelpers.ColorPrint(ConsoleColor.DarkYellow, "Modifiers: ", newLine: true);
            foreach (Modifier modifier in modifiers) 
            {
                if (modifier.Number > 0)
                {
                    modifier.Print();
                }
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
            string? commandStr = Console.ReadLine()?.ToLower();
            if (commandStr is null) return true;

            Command? command = null;

            switch (commandStr)
            {
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
                    command = new SearchCommand(Player);
                    break;
                case "move card":
                    command = new MoveCardCommand(Player);
                    break;
                case "santa":
                    command = new SantaCommand(Player);
                    break;
                case "santa play":
                    command = new SantaPlayCommand(Player);
                    break;
                case "hand":
                    Player.PrintLocation(Location.Hand);
                    break;
                case "play area":
                    Player.PrintLocation(Location.PlayArea);
                    break;
                case "discard pile":
                    Player.PrintLocation(Location.DiscardPile);
                    break;
                case "reveal":
                    RevealCards();
                    break;
                case "discard pile to deck":
                    Player.ShuffleDiscardIntoDeck();
                    break;
                case "set count":
                    SetCount();
                    Player.PrintLocation(Location.PlayArea);
                    break;
                case "damage":
                    DealDamage();
                    PrintSetup();
                    break;
                case "heal":
                    Heal();
                    PrintSetup();
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
                case "undo":
                    if (commands.Count > 0)
                        commands.Pop().Undo();
                    else
                        Console.WriteLine("No commands to undo");
                    break;
                case "q":
                    return false;
                case "":
                    PrintSetup();
                    break;
                default:
                    Console.WriteLine("Not a valid command");
                    break;
            }
            // TODO: Add ability to move card from discard pile to hand or deck
            if (command is not null)
            {
                if (command.Execute())
                {
                    commands.Push(command);
                }
            }
            return true;
        }

        public void RevealCards()
        {
            Console.WriteLine("How many?");
            string? intStr = Console.ReadLine();
            if (intStr is null)
            {
                Console.WriteLine("Need to provide a number");
                return;
            }

            int num = int.Parse(intStr);
            if (Player.PlayerDeck.GetCount() < num)
            {
                Player.ShuffleDiscardIntoDeck();
            }
            Player.RevealCards(num);
        }

        private void SetCount()
        {
            Card? card = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (card is null) return;

            int? count = MiscHelpers.GetIntFromPlayer("Set count to what?");
            if (count is null) return;

            card.Count = count.Value;
        }

        private void DealDamage()
        {

            int? damage = MiscHelpers.GetIntFromPlayer("How much?");
            if (damage is null) return;
            Player.DealDamage(damage.Value);
        }

        private void Heal()
        {
            int? heal = MiscHelpers.GetIntFromPlayer("How much?");
            if (heal is null)
            {
                Console.WriteLine("Must provide an integer");
                return;
            }

            Player.Heal(heal.Value);
        }

        private void AddMod()
        {
            for (int i = 0; i <= modifiers.Count; i++)
            {
                if (i < modifiers.Count) 
                {
                    Console.WriteLine($"\t{i}: {modifiers[i].Description}");
                }
                else
                {
                    Console.WriteLine($"\t{i}: Other");
                }
            }

            int? index = MiscHelpers.GetIntFromPlayer("");

            if (index is null) return;

            if (index > modifiers.Count)
            {
                Console.WriteLine("Selection out of range");
                return;
            }

            if (index < modifiers.Count)
            {
                modifiers[index.Value].Increment();
            }
            else // index == damageModifiers.Count (Other)
            {
                string? modStr = MiscHelpers.GetStringFromPlayer("Description");
                if (modStr is null)
                {
                    return;
                }
                Modifier modToAdd = new Modifier(modStr, ConsoleColor.Cyan);
                modToAdd.Increment();
                modifiers.Add(modToAdd);
            }
        }

        public void RemoveMod()
        {
            Console.WriteLine("Which one?");
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].Number > 0)
                    Console.WriteLine($"\t{i}: {modifiers[i]}");
            }

            int? modIndex = MiscHelpers.GetIntFromPlayer("");

            if (modIndex is null) return;
            if (modIndex >= modifiers.Count)
            {
                Console.WriteLine("Index out of range");
                return;
            }
            modifiers[modIndex.Value].Decrement();
        }

        private Modifier? ModifierPresent(string desc)
        {
            foreach (Modifier mod in modifiers)
            {
                if (mod.Description == desc)
                    return mod;
            }

            return null;
        }
    }
}
