using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SOTMDecks
{
    internal class CardCollection<T> where T : Card
    {
        public enum Filter
        {
            NONE,
            START,
            POWER,
            TARGET,
            END
        }

        public CardCollection(string desc) 
        {
            cards_ = new List<T>();
            Description = desc;
        }

        public string Description { get; }

        protected List<T> cards_;

        public void SetCards(List<T> cards)
        {
            cards_ = cards;
        }

        public List<T> GetCards()
        {
            return cards_;
        }

        public T? GetLastCard()
        {
            if (cards_.Count == 0) return null;

            T card = cards_[cards_.Count - 1];
            cards_.Remove(card);
            return card;
        }

        public int GetCount()
        {
            return cards_.Count;
        }

        public void Add(T card)
        {
            cards_.Add(card);
        }

        public void Insert(int loc, T card)
        {
            cards_.Insert(loc, card);
        }

        public void AddCollection(CardCollection<T> newCards)
        {
            foreach (T card in newCards.GetCards())
            {
                Add(card);
            }
        }

        public CardCollection<T> SearchByType(string type)
        {
            CardCollection<T> collection = new CardCollection<T>($"{type} cards");

            foreach (T card in cards_)
            {
                if (card.IsType(type)) collection.Add(card);
            }

            return collection;
        }

        public CardCollection<T> RevealByType(string type, int num)
        {
            int numFound = 0;

            CardCollection<T> collection = new CardCollection<T>($"{type} cards");

            foreach (T card in cards_)
            {
                if (card.IsType(type)) 
                {
                    collection.Add(card);
                    ++numFound;
                }

                if (numFound >= num) break;
            }

            return collection;
        }

        public CardCollection<T> SearchForTargets()
        {
            CardCollection<T> collection = new CardCollection<T>("Targets");

            foreach (T card in cards_)
            {
                if (card.IsTarget) collection.Add(card);
            }

            return collection;
        }

        public T? GetSpecialType()
        {
            HashSet<string> ignoredTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Ongoing",
                "Limited",
                "Equipment",
                "Oneshot",
                "One-Shot"
            };

            foreach (T card in cards_)
            {
                if (card.Type.Any(t => !ignoredTypes.Contains(t)))
                {
                    return card;
                }
            }

            Console.WriteLine("No special cards found");

            return null;
        }

        public T? GetCardFromName(string name)
        {
            foreach (T card in cards_)
            {
                if (card.Name == name) return card;
            }

            Console.WriteLine($"Deck does not include \"{name}\"");
            return null;
        }

        public bool Contains(T card)
        {
            return cards_.Contains(card);
        }

        public bool Remove(T card)
        {
            return cards_.Remove(card);
        }

        public bool RemoveAll(CardCollection<T> collection)
        {
            if (collection.GetCount() == 0)
            {
                Console.WriteLine("Cannot remove list of cards - empty");
                return false;
            }

            cards_ = cards_.Except(collection.GetCards()).ToList();
            return true;
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
                Console.Write($" ({cards_[i].TypeAsString()})");
                if (verbose)
                {
                    cards_[i].PrintText();
                }
                Console.WriteLine();
            }
        }

        public void PrettyPrint(Filter filter = Filter.NONE, bool brief = false)
        {
            Console.WriteLine($"{Description} ({cards_.Count}):\n");
            bool first = true;
            foreach (T card in cards_)
            {
                bool print = true;
                switch (filter)
                {
                    case Filter.NONE:
                        print = true; break;
                    case Filter.START:
                        print = card.StartOfTurn != "" || card.HasCustomMechanicAtTime(MiscHelpers.Timing.START); break;
                    case Filter.POWER:
                        print = (card is HeroCard hc && hc.Power != "");
                        break;
                    case Filter.END:
                        print = card.EndOfTurn != "" || card.HasCustomMechanicAtTime(MiscHelpers.Timing.END); break;
                    case Filter.TARGET:
                        print = card.MaxHP > 0;
                        break;
                    default: 
                        print = true; break;
                }

                if (print)
                {

                    if (first)
                    {
                        card.PrettyPrint(brief);
                        first = false;
                    }
                    else
                    {
                        if (!brief) Console.WriteLine("----");
                        card.PrettyPrint(brief);
                    }
                }
            }
        }
    }
}
