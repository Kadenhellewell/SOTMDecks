using System;

namespace SOTMDecks.View
{
    /// <summary>
    /// Renders deck-level views: revealing the top cards, and a hero's innate power.
    /// </summary>
    internal static class DeckRenderer
    {
        public static void RenderReveal<T>(IOutput o, Deck<T> deck, int num) where T : Card
        {
            var cards = deck.GetCards();
            for (int i = 0; i < num; i++)
            {
                if (i >= cards.Count)
                {
                    o.WriteLine("No more cards in deck");
                    return;
                }
                CardRenderer.Render(o, cards[i]);
                o.WriteLine();
                o.WriteLine("------");
            }
        }

        public static void RenderInnatePower(IOutput o, HeroDeck deck)
        {
            o.Write("Innate Power: ", ConsoleColor.Blue);
            o.WriteLine(deck.InnatePower);

            if (deck.InnatePower2 != "")
            {
                o.Write("Innate Power: ", ConsoleColor.Blue);
                o.WriteLine(deck.InnatePower2);
            }
        }
    }
}
