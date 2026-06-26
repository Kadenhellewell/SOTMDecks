using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SOTMDecks.View;

namespace SOTMDecks
{
    internal abstract class Deck<T> : CardCollection<T> where T : Card
    {
        protected Deck(string description) : base(description)
        {
        }

        protected abstract List<T> ParseDeck(JObject json);

        // Test hook: when SOTMDECKS_SEED is set, shuffles become deterministic so output
        // can be compared across refactors. Unset (the normal case) keeps the Guid shuffle.
        private static readonly Random? _seededRng =
            int.TryParse(Environment.GetEnvironmentVariable("SOTMDECKS_SEED"), out int seed)
                ? new Random(seed)
                : null;

        public void Shuffle()
        {
            cards_ = _seededRng is null
                ? cards_.OrderBy(_ => Guid.NewGuid()).ToList()
                : cards_.OrderBy(_ => _seededRng.Next()).ToList();
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
                Display.Card(cards_[i]);
                Console.WriteLine();
                Console.WriteLine("------");
            }
        }
    }
}
