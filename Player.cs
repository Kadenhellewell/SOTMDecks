using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SOTMDecks
{
    enum Location
    {
        Hand,
        PlayArea,
        DiscardPile,
        TopOfDeck,
        BottomOfDeck,
        SantasBag
    }

    internal class Player
    {

        public Player(Deck deck) 
        {
            PlayerDeck = deck;
            Name = deck.Name;
            HP = deck.StartingHP;
            MaxHP = deck.StartingHP;

            CardGroups = new Dictionary<Location, CardCollection>
            {
                { Location.Hand, new CardCollection("Hand") },
                { Location.PlayArea, new CardCollection("Play Area") },
                { Location.DiscardPile, new CardCollection("Discard Pile") },
                { Location.SantasBag, new CardCollection("SantasBag") }
            };
        }

        public Deck PlayerDeck { get; }
        public string Name { get; }
        private int HP;
        private int MaxHP;
        public List<Modifier> Modifiers { get; } = new List<Modifier>();

        Dictionary<Location, CardCollection> CardGroups;

        public CardCollection Hand()
        {
            return CardGroups[Location.Hand];
        }

        public CardCollection PlayArea()
        {
            return CardGroups[Location.PlayArea];
        }

        public CardCollection DiscardPile()
        {
            return CardGroups[Location.DiscardPile];
        }

        public CardCollection SantasBag()
        {
            return CardGroups[Location.SantasBag];
        }

        public CardCollection GetLocation(Location loc)
        {
            return CardGroups[loc];
        }

        public int GetHP()
        {
            return HP;
        }

        public void AddMod(Modifier newMod)
        {
            Modifiers.Add(newMod);
        }

        public void AddMods(List<Modifier> newMods)
        {
            foreach (Modifier mod in newMods)
            {
                Modifiers.Add(mod);
            }
        }

        public void RemoveMod(Modifier mod) 
        {
            Modifiers.Remove(mod); 
        }

        public void RemoveMod(int index)
        {
            Modifiers.RemoveAt(index);
        }

        public void RemoveMods(List<Modifier> mods)
        {
            foreach (Modifier mod in mods)
            {
                Modifiers.Remove(mod);
            }
        }

        public void DealDamage(int damage)
        {
            if (damage <= 0)
            {
                Console.WriteLine("Damage value must be positive");
                return;
            }

            HP -= damage;
        }

        public void Heal(int healing) 
        {
            if (healing <= 0)
            {
                Console.WriteLine("Healing value must be positive");
                return;
            }

            HP += healing;
            if (HP > MaxHP) HP = MaxHP;
        }

        public bool Discard(Card card) 
        {
            if (CardGroups[Location.Hand].Contains(card))
            {
                CardGroups[Location.Hand].Remove(card);
                CardGroups[Location.DiscardPile].Add(card);
                return true;
            }

            Console.WriteLine($"{card.Name} not in hand");
            return false;
        }

        public bool DiscardHand()
        {
            if (CardGroups[Location.Hand].GetCount() == 0) 
            {
                Console.WriteLine("Hand is empty");
                return false;
            }

            return MoveAllCards(Location.Hand, Location.DiscardPile);
        }

        public bool DiscardFromDeck(int num)
        {
            if (PlayerDeck.GetCount() == 0)
            {
                ShuffleDiscardIntoDeck();
            }

            for (int i = 0; i < num; ++i)
            {
                Card? card = PlayerDeck.Draw();
                if (card is null) return false;

                CardGroups[Location.DiscardPile].Add(card);
                Console.Write("Discarded ");
                MiscHelpers.ColorPrint(ConsoleColor.Green, card.Name, newLine: true);
            }

            return true;
        }

        public bool MoveCard(Card card, Location src, Location dest)
        {
            if (src == dest)
            {
                Console.WriteLine("What are you talking about?");
                return false;
            }

            if (!CardGroups[src].Contains(card))
            {
                Console.WriteLine($"{card.Name} is not in {CardGroups[src].Description}");
                return false;
            }

            if (!CardGroups[src].Remove(card))
            {
                Console.WriteLine($"Error removing {card.Name} from {CardGroups[src].Description}");
                return false;
            }

            if (src == Location.PlayArea && card.Modifiers.Count > 0)
            {
                RemoveMods(card.Modifiers);
            }
            else if (dest == Location.PlayArea)
            {
                OnCardPlayed(card);
            }

            if (dest == Location.TopOfDeck)
            {
                PlayerDeck.Insert(0, card);
            }
            else if (dest == Location.BottomOfDeck)
            {
                PlayerDeck.Insert(PlayerDeck.GetCount() - 1, card);
            }
            else
            {
                CardGroups[dest].Add(card);
            }
            return true;
        }

        public bool PlayCard(Card card)
        {
            Location dest = card.IsOneshot() ? Location.DiscardPile : Location.PlayArea;
            if (card.IsOneshot()) { Console.WriteLine($"{card.Text}");  }

            return MoveCard(card, Location.Hand, dest);
        }

        public void OnCardPlayed(Card card)
        {
            Console.WriteLine($"{card.OnEntry}");

            if (card.Modifiers.Count > 0)
            {
                AddMods(card.Modifiers);
            }
        }

        public bool DestroyCard(Card card)
        {
            bool result = MoveCard(card, Location.PlayArea, Location.DiscardPile);

            OnCardDestroyed(card);

            return result;
        }

        public void OnCardDestroyed(Card card)
        {
            Console.WriteLine($"{card.OnDestroy}");

            if (card.Modifiers.Count > 0)
            {
                RemoveMods(card.Modifiers);
            }
        }

        public bool MoveAllCards(Location src, Location dest)
        {
            if (CardGroups[src].GetCount() == 0)
            {
                Console.WriteLine($"No cards in {src}");
                return false;
            }

            if (dest == Location.TopOfDeck || dest == Location.BottomOfDeck)
            {
                PlayerDeck.AddCollection(CardGroups[src]);
                CardGroups[src].Clear();
                PlayerDeck.Shuffle();
                return true;
            }

            CardGroups[dest].AddCollection(CardGroups[src]);
            CardGroups[src].Clear();
            return true;
        }

        public void AddCardToSantasBag(Card card)
        {
            CardGroups[Location.SantasBag].Add(card);
        }

        public bool RemoveCardFromSantasBag(Card card)
        {
            return CardGroups[Location.SantasBag].Remove(card);
        }

        public Card? Draw(bool verbose = true, bool fromBottom = false)
        {
            Card? newCard = PlayerDeck.Draw(fromBottom);
            if (newCard is null) return null;
            CardGroups[Location.Hand].Add(newCard);
            if (verbose)
                Console.WriteLine($"Drew {newCard.Name} [{newCard.Type}]");
            return newCard;
        }

        public void UndoDraw(Card card, bool fromBottom)
        {
            if (!CardGroups[Location.Hand].Remove(card))
            {
                Console.WriteLine($"{card.Name} not in hand.");
                return;
            }

            if (fromBottom)
            {
                PlayerDeck.Add(card);
            }
            else
            {
                PlayerDeck.GetCards().Insert(0, card);
            }
        }

        public void MoveCardFromDeckToHand(Card card)
        {
            if (!PlayerDeck.Remove(card))
            {
                Console.WriteLine($"{card.Name} not in deck.");
                return;
            }
            CardGroups[Location.Hand].Add(card);
        }

        public void RevealCards(int num)
        {
            PlayerDeck.RevealCards(num);
        }

        public void Shuffle()
        {
            PlayerDeck.Shuffle();
        }

        public void ShuffleDiscardIntoDeck()
        {
            if (CardGroups[Location.DiscardPile].GetCount() == 0)
            {
                Console.WriteLine("Discard pile empty");
                return;
            }

            PlayerDeck.AddCollection(CardGroups[Location.DiscardPile]);
            CardGroups[Location.DiscardPile].Clear();
            PlayerDeck.Shuffle();
        }

        public void PrintLocation(Location loc, CardCollection.Filter filter = CardCollection.Filter.NONE, bool brief = false)
        {
            CardGroups[loc].PrettyPrint(filter, brief);
            if (loc == Location.PlayArea && CardGroups[Location.SantasBag].GetCount() > 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.DarkCyan, "Santa's bag: ", newLine: false);
                Console.WriteLine(CardGroups[Location.SantasBag].GetCount());
            }
        }
    }
}
