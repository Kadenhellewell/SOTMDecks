using System;
using System.Collections.Generic;

namespace SOTMDecks
{
    internal class EnvironmentGame
    {
        public EnvironmentGame(EnvironmentDeck deck)
        {
            EnvDeck = deck;
            PlayArea = new CardCollection<EnvironmentCard>("Play Area");
            DiscardPile = new CardCollection<EnvironmentCard>("Discard Pile");
            KO = new CardCollection<EnvironmentCard>("Cards removed from the game");
        }

        public EnvironmentDeck EnvDeck { get; }
        private CardCollection<EnvironmentCard> PlayArea;
        private CardCollection<EnvironmentCard> DiscardPile;
        private CardCollection<EnvironmentCard> KO;

        public void Start()
        {
            EnvDeck.Shuffle();
            Console.WriteLine($"Environment: {EnvDeck.Name}");
            Console.WriteLine($"Cards in deck: {EnvDeck.GetCount()}");
            Console.WriteLine();
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

        private bool GetCommand()
        {
            Console.Write("> ");
            string? commandStr = Console.ReadLine()?.ToLower().Trim();
            if (commandStr is null) return true;

            commandStr = MiscHelpers.ExtractBrief(commandStr, out bool brief);

            switch (commandStr)
            {
                case "reveal":
                    Reveal();
                    break;
                case "play area":
                case "pa":
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.NONE, brief);
                    break;
                case "discard pile":
                case "dp":
                    DiscardPile.PrettyPrint(CardCollection<EnvironmentCard>.Filter.NONE, brief);
                    break;
                case "deck count":
                    Console.WriteLine($"Cards in deck: {EnvDeck.GetCount()}");
                    break;
                case "discard":
                    DiscardFromPlayArea();
                    break;
                case "destroy":
                    Destroy();
                    break;
                case "damage card":
                    DamageCard();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "damage cards":
                    DamageCards();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "damage all":
                    DamageAll();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "heal card":
                    HealCard();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "heal cards":
                    HealCards();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "heal all":
                    HealAll();
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief: true);
                    break;
                case "shuffle":
                    EnvDeck.Shuffle();
                    Console.WriteLine("Deck shuffled");
                    break;
                case "discard pile to deck":
                case "dp to deck":
                    ShuffleDiscardIntoDeck();
                    break;
                case "remove card":
                    RemoveCard();
                    break;
                case "targets":
                    PlayArea.PrettyPrint(CardCollection<EnvironmentCard>.Filter.TARGET, brief);
                    break;
                case "key words":
                    PrintKeyWords();
                    break;
                case "q":
                case "exit":
                    return false;
                case "":
                    PlayArea.PrettyPrint();
                    break;
                default:
                    Console.WriteLine("Not a valid command");
                    break;
            }
            return true;
        }

        private void Reveal()
        {
            if (EnvDeck.GetCount() == 0)
            {
                if (DiscardPile.GetCount() == 0)
                {
                    Console.WriteLine("Deck and discard pile are both empty");
                    return;
                }
                Console.WriteLine("Deck empty - shuffling discard pile back in");
                ShuffleDiscardIntoDeck();
            }

            EnvironmentCard? card = EnvDeck.Draw();
            if (card is null)
            {
                Console.WriteLine("Could not draw a card");
                return;
            }

            PlayArea.Add(card);
            Console.WriteLine("Revealed:");
            card.PrettyPrint();
        }

        private void DiscardFromPlayArea()
        {
            EnvironmentCard? card = MiscHelpers.GetCardFromIndex(PlayArea);
            if (card is null) return;

            PlayArea.Remove(card);
            DiscardPile.Add(card);
            Console.WriteLine($"Discarded {card.Name}");
        }

        private void Destroy()
        {
            EnvironmentCard? card = MiscHelpers.GetCardFromIndex(PlayArea);
            if (card is null) return;

            PlayArea.Remove(card);
            DiscardPile.Add(card);
            Console.WriteLine($"Destroyed {card.Name}");
            if (!string.IsNullOrEmpty(card.OnDestroy))
            {
                Console.WriteLine($"On destroy: {card.OnDestroy}");
            }
        }

        private void RemoveCard()
        {
            EnvironmentCard? card = MiscHelpers.GetCardFromIndex(PlayArea);
            if (card is null) return;

            PlayArea.Remove(card);
            KO.Add(card);
            Console.WriteLine($"Removed {card.Name} from the game");
        }

        private void DamageCard()
        {
            EnvironmentCard? card = MiscHelpers.GetCardFromIndex(PlayArea);
            if (card is null) return;

            if (MiscHelpers.GetIntFromPlayer("How much?") is not int dmg) return;

            if (card.MaxHP == 0)
            {
                Console.WriteLine("That card is not a target");
                return;
            }

            card.HP -= dmg;
            if (card.HP <= 0)
            {
                Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                card.HP = 0;
            }
        }

        private void DamageCards()
        {
            List<EnvironmentCard>? cards = MiscHelpers.GetCardsFromInput(PlayArea);
            if (cards is null) return;

            if (MiscHelpers.GetIntFromPlayer("How much?") is not int dmg) return;

            foreach (EnvironmentCard card in cards)
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (skipping)");
                    continue;
                }
                card.HP -= dmg;
                if (card.HP <= 0)
                {
                    Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                    card.HP = 0;
                }
            }
        }

        private void DamageAll()
        {
            if (MiscHelpers.GetIntFromPlayer("How much?") is not int dmg) return;

            foreach (EnvironmentCard card in PlayArea.GetCards())
            {
                if (card.MaxHP == 0) continue;

                card.HP -= dmg;
                if (card.HP <= 0)
                {
                    Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                    card.HP = 0;
                }
            }
        }

        private void HealCard()
        {
            EnvironmentCard? card = MiscHelpers.GetCardFromIndex(PlayArea);
            if (card is null) return;

            if (MiscHelpers.GetIntFromPlayer("How much?") is not int heal) return;

            if (card.MaxHP == 0)
            {
                Console.WriteLine("That card is not a target");
                return;
            }

            card.HP += heal;
            if (card.HP > card.MaxHP) card.HP = card.MaxHP;
        }

        private void HealCards()
        {
            List<EnvironmentCard>? cards = MiscHelpers.GetCardsFromInput(PlayArea);
            if (cards is null) return;

            if (MiscHelpers.GetIntFromPlayer("How much?") is not int heal) return;

            foreach (EnvironmentCard card in cards)
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (skipping)");
                    continue;
                }
                card.HP += heal;
                if (card.HP > card.MaxHP) card.HP = card.MaxHP;
            }
        }

        private void HealAll()
        {
            if (MiscHelpers.GetIntFromPlayer("How much?") is not int heal) return;

            foreach (EnvironmentCard card in PlayArea.GetCards())
            {
                if (card.MaxHP == 0) continue;
                card.HP += heal;
                if (card.HP > card.MaxHP) card.HP = card.MaxHP;
            }
        }

        private void ShuffleDiscardIntoDeck()
        {
            if (DiscardPile.GetCount() == 0)
            {
                Console.WriteLine("Discard pile empty");
                return;
            }
            EnvDeck.AddCollection(DiscardPile);
            DiscardPile.Clear();
            EnvDeck.Shuffle();
        }

        private void PrintKeyWords()
        {
            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Bury: ");
            Console.WriteLine("Put the indicated card on the bottom of the associated deck, unless that deck has zero cards in it, in which case the card goes to the top of the appropriate trash instead.");
            Console.WriteLine();

            MiscHelpers.ColorPrint(ConsoleColor.Magenta, "Discover: ");
            Console.WriteLine("Reveal cards from the top of the deck until you reveal the indicated card/card-type or reach the bottom of the deck. Shuffle other revealed cards back into the deck. Then play the indicated cards in the order revealed.");
            Console.WriteLine();
        }
    }
}
