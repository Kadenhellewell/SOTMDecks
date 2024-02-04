using System;
using System.Collections.Generic;
using System.Linq;
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
        SantasBag
    }

    internal class Player
    {

        public Player(Deck deck) 
        {
            PlayerDeck = deck;
            Name = deck.Name;
            HP = deck.StartingHP;

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

        public bool UndoPreviousDiscard(int num, bool fromDeck = false)
        {
            bool success = true;
            for (int i = 0; i < num; ++i)
            {
                if (fromDeck)
                {
                    Card? card = DiscardPile().GetLastCard();
                    if (card is null) // Discard pile is empty
                    {
                        return false;
                    }

                    PlayerDeck.Add(card);
                }
                else
                {
                    Card? card = DiscardPile().GetLastCard();
                    if (card is null) // Discard pile is empty
                    {
                        return false;
                    }

                    CardGroups[Location.Hand].Add(card);
                }
            }

            if (fromDeck) Shuffle();
            return success;
        }

        public bool MoveCard(Card card, Location src, Location dest)
        {
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
            CardGroups[dest].Add(card);
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
                Console.WriteLine($"Drew {newCard.Name}");
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

        public void PrintLocation(Location loc)
        {
            CardGroups[loc].PrettyPrint();
            if (loc == Location.PlayArea && CardGroups[Location.SantasBag].GetCount() > 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.DarkCyan, "Santa's bag: ", newLine: false);
                Console.WriteLine(CardGroups[Location.SantasBag].GetCount());
            }
        }
    }
}
