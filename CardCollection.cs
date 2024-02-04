using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SOTMDecks
{
    internal class CardCollection
    {
        public CardCollection(string desc) 
        {
            cards_ = new List<Card>();
            Description = desc;
        }

        public string Description { get; }

        protected List<Card> cards_;

        public void SetCards(List<Card> cards)
        {
            cards_ = cards;
        }

        public List<Card> GetCards()
        {
            return cards_;
        }

        public Card? GetLastCard()
        {
            if (cards_.Count == 0) return null;

            Card card = cards_[cards_.Count - 1];
            cards_.Remove(card);
            return card;
        }

        public int GetCount()
        {
            return cards_.Count;
        }

        public void Add(Card card)
        {
            cards_.Add(card);
        }

        public void Insert(int loc, Card card)
        {
            cards_.Insert(loc, card);
        }

        public void AddCollection(CardCollection newCards)
        {
            foreach (Card card in newCards.GetCards())
            {
                Add(card);
            }
        }

        public CardCollection SearchByType(string type)
        {
            CardCollection collection = new CardCollection($"{type} cards");

            foreach (Card card in cards_)
            {
                if (card.Type.Contains(type)) collection.Add(card);
            }

            return collection;
        }

        public Card? GetCardFromName(string name)
        {
            foreach (Card card in cards_)
            {
                if (card.Name == name) return card;
            }

            Console.WriteLine($"Deck does not include \"{name}\"");
            return null;
        }

        public bool Contains(Card card)
        {
            return cards_.Contains(card);
        }

        public bool Remove(Card card)
        {
            return cards_.Remove(card);
        }

        public void Clear()
        {
            cards_.Clear();
        }

        public void ListPrint(bool verbose = false)
        {
            for (int i = 0; i < cards_.Count; i++)
            {
                Console.Write($"\t{i}: ");
                MiscHelpers.ColorPrint(ConsoleColor.Green, cards_[i].Name);
                if (verbose)
                {
                    Console.WriteLine($"; {cards_[i].Type}; {cards_[i].Text}");
                }
                Console.WriteLine();
            }
        }

        public void PrettyPrint()
        {
            Console.WriteLine($"{Description} ({cards_.Count}):\n");
            bool first = true;
            foreach (Card card in cards_)
            {
                if (first)
                {
                    card.PrettyPrint();
                    first = false;
                }
                else
                {
                    Console.WriteLine("----");
                    card.PrettyPrint();
                }
            }
        }
    }
}
