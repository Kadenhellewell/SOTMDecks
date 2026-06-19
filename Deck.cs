using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SOTMDecks
{
    internal abstract class Deck<T> : CardCollection<T> where T : Card
    {
        protected Deck(string description) : base(description)
        {
        }

        protected abstract List<T> ParseDeck(JObject json);

        public void Shuffle()
        {
            cards_ = cards_.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        public T? Draw(bool fromBottom = false)
        {
            if (cards_.Count == 0)
            {
                Console.WriteLine("No cards to draw");
                return null;
            }

            T drawn;
            if (fromBottom)
            {
                drawn = cards_[cards_.Count - 1];
                cards_.RemoveAt(cards_.Count - 1);
            }
            else
            {
                drawn = cards_.First();
                cards_.RemoveAt(0);
            }
            return drawn;
        }

        public List<T>? GetTopCards(int n)
        {
            if (cards_.Count < n)
            {
                Console.WriteLine($"Fewer than {n} cards");
                return null;
            }

            return cards_.Take(n).ToList();
        }

        public void RevealCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (i >= cards_.Count)
                {
                    Console.WriteLine("No more cards in deck");
                    return;
                }
                cards_[i].PrettyPrint();
                Console.WriteLine();
                Console.WriteLine("------");
            }
        }
    }
}
