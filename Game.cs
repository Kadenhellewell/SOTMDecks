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
        }

        public Player Player { get; }
        
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

                if (Player.PlayerDeck.InnatePower2 != "")
                {
                    MiscHelpers.ColorPrint(ConsoleColor.Blue, "Innate Power: ");
                    Console.WriteLine(Player.PlayerDeck.InnatePower2);
                }
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

            bool brief = false;
            if (commandStr.EndsWith(" b"))
            {
                brief = true;
                commandStr = commandStr.Trim('b');
                commandStr = commandStr.Trim();

            }

            Command? command = null;
            
            switch (commandStr)
            {
                // TODO: add search special types command
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
                    command = new SearchCommand(Player);
                    break;
                case "search special type":
                    command = new SearchSpecialCommand(Player);
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
                    Player.PrintLocation(Location.Hand, brief: brief);
                    break;
                case "hand powers":
                    Player.PrintLocation(Location.Hand, CardCollection.Filter.POWER);
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
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.POWER, brief);
                    break;
                case "start":
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.START, brief);
                    break;
                case "end":
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.END, brief);
                    break;
                case "targets":
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief);
                    break;
                case "reveal":
                    RevealCards();
                    break;
                case "discard pile to deck":
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
                    DealDamage();
                    PrintSetup();
                    break;
                case "damage card":
                    DamageCard();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
                    break;
                case "damage cards":
                    DamageCards();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
                    break;
                case "damage all":
                    DamageAll();
                    PrintSetup();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
                    break;
                case "heal":
                    Heal();
                    PrintSetup();
                    break;
                case "heal card":
                    HealCard();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
                    break;
                case "heal cards":
                    HealCards();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
                    break;
                case "heal all":
                    HealAll();
                    PrintSetup();
                    Player.PrintLocation(Location.PlayArea, CardCollection.Filter.TARGET, brief: true);
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
                case "exit":
                    return false;
                case "":
                    PrintSetup();
                    break;
                default:
                    Console.WriteLine("Not a valid command");
                    break;
            }

            if (command is not null)
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
            return true;
        }

        public void RevealCards()
        {
            int? num = MiscHelpers.GetIntFromPlayer("How many?");
            if (num is null) return;

            if (Player.PlayerDeck.GetCount() < num)
            {
                Player.ShuffleDiscardIntoDeck();
            }
            Player.RevealCards(num.Value);
        }

        private void SetCount()
        {
            Card? card = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (card is null) return;

            int? count = MiscHelpers.GetIntFromPlayer("Set count to what?");
            if (count is null) return;

            card.Count = count.Value;
        }

        private void DamageCard()
        {
            Card? card = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (card is null) return;

            int? damage = MiscHelpers.GetIntFromPlayer("How much?");
            if (damage is null) return;

            if (card.MaxHP == 0)
            {
                Console.WriteLine("You must choose a target");
                return;
            }

            card.HP -= damage.Value;
            if (card.HP <= 0)
            {
                Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                card.HP = 0;
            }
        }

        private void DamageCards()
        {
            List<Card>? cards = MiscHelpers.GetCardsFromInput(Player.PlayArea());
            if (cards is null) return;

            int? damage = MiscHelpers.GetIntFromPlayer("How much?");
            if (damage is null) return;

            foreach (Card card in cards)
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (HP will still be removed from targets)");
                    continue;
                }

                card.HP -= damage.Value;
            }
        }

        private void DamageAll()
        {
            int? damage = MiscHelpers.GetIntFromPlayer("How much?");
            if (damage is null) return;

            Player.DealDamage(damage.Value);

            foreach (Card card in Player.GetLocation(Location.PlayArea).GetCards())
            {
                if (card.MaxHP == 0)
                {
                    continue;
                }

                card.HP -= damage.Value;
                if (card.HP <= 0)
                {
                    Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                    card.HP = 0;
                }
            }
        }

        private void HealCard()
        {
            Card? card = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (card is null) return;

            int? health = MiscHelpers.GetIntFromPlayer("How much?");
            if (health is null) return;

            card.HP += health.Value;
        }

        private void HealCards()
        {
            List<Card>? cards = MiscHelpers.GetCardsFromInput(Player.PlayArea());
            if (cards is null) return;

            int? health = MiscHelpers.GetIntFromPlayer("How much?");
            if (health is null) return;

            foreach (Card card in cards)
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (HP will still be added to targets)");
                    continue;
                }

                card.HP += health.Value;
            }
        }

        private void HealAll()
        {
            int? damage = MiscHelpers.GetIntFromPlayer("How much?");
            if (damage is null) return;

            Player.Heal(damage.Value);

            foreach (Card card in Player.GetLocation(Location.PlayArea).GetCards())
            {
                if (card.MaxHP == 0)
                {
                    continue;
                }

                card.HP += damage.Value;
            }
        }

        private void DealDamage()
        {
            // TODO: implement multi-select
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
            string? modStr = MiscHelpers.GetStringFromPlayer("Description");
            if (modStr is null)
            {
                Console.WriteLine("Didn't get that; try again");
                return;
            }

            Modifier modToAdd = new Modifier(modStr, ConsoleColor.Cyan);
            Player.Modifiers.Add(modToAdd);
        }

        public void RemoveMod()
        {
            Console.WriteLine("Which one?");
            for (int i = 0; i < Player.Modifiers.Count; i++)
            {
                Console.WriteLine($"\t{i}: {Player.Modifiers[i]}");
            }

            int? modIndex = MiscHelpers.GetIntFromPlayer("");

            if (modIndex is null) return;
            if (modIndex >= Player.Modifiers.Count)
            {
                Console.WriteLine("Index out of range");
                return;
            }
            Player.RemoveMod(modIndex.Value);
        }

        private Modifier? ModifierPresent(string desc)
        {
            foreach (Modifier mod in Player.Modifiers)
            {
                if (mod.Description == desc)
                    return mod;
            }

            return null;
        }
    }
}
