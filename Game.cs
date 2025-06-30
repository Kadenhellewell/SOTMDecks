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
            {//TODO: add a search for a target command
                //TODO: add play area to hand command
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

            if (command is not null)
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
                catch (Exception ex )
                {
                    Console.WriteLine($"Excpetion executing command: {ex}");
                }
            }
            return true;
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
            Option<Card> cardOpt = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (!cardOpt.HasValue) return;

            Option<int> countOpt = MiscHelpers.GetIntFromPlayer("Set count to what?");
            if (!countOpt.HasValue) return;

            cardOpt.ValueOrThrow().Count = countOpt.ValueOr(0);
        }

        private void DamageCard()
        {
            Option<Card> cardOpt = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (!cardOpt.HasValue) return;

            Option<int> damageOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!damageOpt.HasValue) return;

            var card = cardOpt.ValueOrThrow();

            if (card.MaxHP == 0)
            {
                Console.WriteLine("You must choose a target");
                return;
            }

            card.HP -= damageOpt.ValueOr(0);
            if (card.HP <= 0)
            {
                Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                card.HP = 0;
            }
        }

        private void DamageCards()
        {
            Option<List<Card>> cards = MiscHelpers.GetCardsFromInput(Player.PlayArea());
            if (!cards.HasValue) return;

            Option<int> damageOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!damageOpt.HasValue) return;

            foreach (Card card in cards.ValueOr(new List<Card>()))
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (HP will still be removed from targets)");
                    continue;
                }

                card.HP -= damageOpt.ValueOr(0);
            }
        }

        private void DamageAll()
        {
            Option<int> damageOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!damageOpt.HasValue) return;

            int damage = damageOpt.ValueOr(0);

            Player.DealDamage(damage);

            foreach (Card card in Player.GetLocation(Location.PlayArea).GetCards())
            {
                if (card.MaxHP == 0)
                {
                    continue;
                }

                card.HP -= damage;
                if (card.HP <= 0)
                {
                    Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                    card.HP = 0;
                }
            }
        }

        private void HealCard()
        {
            Option<Card> cardOpt = MiscHelpers.GetCardFromIndex(Player.PlayArea());
            if (!cardOpt.HasValue) return;

            Option<int> healthOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!healthOpt.HasValue) return;

            cardOpt.ValueOrThrow().HP += healthOpt.ValueOr(0);
        }

        private void HealCards()
        {
            Option<List<Card>> cards = MiscHelpers.GetCardsFromInput(Player.PlayArea());
            if (!cards.HasValue) return;

            Option<int> healthOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!healthOpt.HasValue) return;

            foreach (Card card in cards.ValueOr(new List<Card>()))
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (HP will still be added to targets)");
                    continue;
                }

                card.HP += healthOpt.ValueOr(0);
            }
        }

        private void HealAll()
        {
            Option<int> healthOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!healthOpt.HasValue) return;

            int health = healthOpt.ValueOr(0);

            Player.Heal(health);

            foreach (Card card in Player.GetLocation(Location.PlayArea).GetCards())
            {
                if (card.MaxHP == 0)
                {
                    continue;
                }

                card.HP += health;
            }
        }

        private void DealDamage()
        {
            Option<int> damageOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!damageOpt.HasValue) return;

            Player.DealDamage(damageOpt.ValueOr(0));
        }

        private void Heal()
        {
            Option<int> healthOpt = MiscHelpers.GetIntFromPlayer("How much?");
            if (!healthOpt.HasValue) return;

            Player.Heal(healthOpt.ValueOr(0));
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

        private Option<Modifier> ModifierPresent(string desc)
        {
            foreach (Modifier mod in Player.Modifiers)
            {
                if (mod.Description == desc)
                    return Option.Some(mod);
            }

            return Option.None<Modifier>();
        }
    }
}
