using System;

namespace SOTMDecks.View
{
    /// <summary>
    /// Renders a <see cref="CardCollection{T}"/>: the numbered selection list and the
    /// filtered "pretty" listing. Card-level rendering is delegated to <see cref="CardRenderer"/>.
    /// </summary>
    internal static class CardCollectionRenderer
    {
        /// <summary>Numbered list (index, name, type); verbose appends each card's inline text.</summary>
        public static void RenderList<T>(IOutput o, CardCollection<T> col, bool verbose = false) where T : Card
        {
            var cards = col.GetCards();
            for (int i = 0; i < cards.Count; i++)
            {
                o.Write($"\t{i}: ");
                o.Write(cards[i].Name, ConsoleColor.Green);
                o.Write($" ({cards[i].TypeAsString()})");
                if (verbose)
                {
                    CardRenderer.RenderText(o, cards[i]);
                }
                o.WriteLine();
            }
        }

        /// <summary>Header + each matching card rendered in full (or brief), separated by "----".</summary>
        public static void Render<T>(IOutput o, CardCollection<T> col, CardFilter filter = CardFilter.NONE, bool brief = false) where T : Card
        {
            var cards = col.GetCards();
            o.WriteLine($"{col.Description} ({cards.Count}):\n");

            bool first = true;
            foreach (T card in cards)
            {
                bool print = filter switch
                {
                    CardFilter.START => card.StartOfTurn != "" || card.HasCustomMechanicAtTime(MiscHelpers.Timing.START),
                    CardFilter.POWER => card is HeroCard hc && hc.Power != "",
                    CardFilter.END => card.EndOfTurn != "" || card.HasCustomMechanicAtTime(MiscHelpers.Timing.END),
                    CardFilter.TARGET => card.MaxHP > 0,
                    _ => true,
                };
                if (!print) continue;

                if (!first && !brief) o.WriteLine("----");
                CardRenderer.Render(o, card, brief);
                first = false;
            }
        }
    }
}
