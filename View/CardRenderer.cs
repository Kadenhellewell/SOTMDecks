using System;

namespace SOTMDecks.View
{
    /// <summary>
    /// Renders a <see cref="Card"/> to an <see cref="IOutput"/>. One renderer handles both
    /// HeroCard and EnvironmentCard — the Hero-only pieces (OnEntry, Power, ENTRY mechanics)
    /// are gated on the concrete type, which also collapses what used to be two near-identical
    /// PrettyPrint/PrintText overrides on the model.
    /// </summary>
    internal static class CardRenderer
    {
        /// <summary>Full card display (name, HP, type, all timed text/mechanics). Brief = name + HP only.</summary>
        public static void Render(IOutput o, Card card, bool brief = false)
        {
            o.Write(card.Name, ConsoleColor.Green);
            if (card.MaxHP > 0) RenderHP(o, card);
            else o.WriteLine("");

            if (brief) return;

            o.WriteLine($"\t{card.TypeAsString()}");

            HeroCard? hero = card as HeroCard;

            if (hero is not null)
            {
                if (hero.OnEntry != "") o.WriteLine($"\t{hero.OnEntry}");
                RenderMechanics(o, card, MiscHelpers.Timing.ENTRY, newline: true);
            }

            if (card.StartOfTurn != "") o.WriteLine($"\t{card.StartOfTurn}");
            RenderMechanics(o, card, MiscHelpers.Timing.START, newline: true);

            if (card.Text != "") o.WriteLine($"\t{card.Text}");

            if (hero is not null && hero.Power != "")
            {
                o.Write("\tPower: ", ConsoleColor.Blue);
                o.WriteLine($"{hero.Power}");
            }

            RenderMechanics(o, card, MiscHelpers.Timing.NONE, newline: true);

            if (card.EndOfTurn != "") o.WriteLine($"\t{card.EndOfTurn}");
            RenderMechanics(o, card, MiscHelpers.Timing.END, newline: true);

            if (card.OnDestroy != "") o.WriteLine($"\t{card.OnDestroy}");
            RenderMechanics(o, card, MiscHelpers.Timing.DESTROY, newline: true);

            o.WriteLine();
        }

        /// <summary>Inline (single-line) form of a card's text, used by the verbose list view.</summary>
        public static void RenderText(IOutput o, Card card)
        {
            HeroCard? hero = card as HeroCard;

            if (hero is not null)
            {
                if (hero.OnEntry != "") o.Write($" {hero.OnEntry} ");
                RenderMechanics(o, card, MiscHelpers.Timing.ENTRY);
            }

            if (card.StartOfTurn != "") o.Write($"{card.StartOfTurn} ");
            RenderMechanics(o, card, MiscHelpers.Timing.START);

            if (card.Text != "") o.Write($" {card.Text}");

            if (hero is not null && hero.Power != "")
            {
                o.Write(" Power: ", ConsoleColor.Blue);
                o.Write($"{hero.Power}");
            }

            RenderMechanics(o, card, MiscHelpers.Timing.NONE);

            if (card.EndOfTurn != "") o.Write($" {card.EndOfTurn}");
            RenderMechanics(o, card, MiscHelpers.Timing.END);

            if (card.OnDestroy != "") o.Write($" {card.OnDestroy}");
            RenderMechanics(o, card, MiscHelpers.Timing.DESTROY);

            o.WriteLine();
        }

        private static void RenderHP(IOutput o, Card card)
        {
            o.Write(" (");
            ConsoleColor hpColor = card.HP == card.MaxHP ? ConsoleColor.Green : ConsoleColor.Red;
            o.Write($"{card.HP}", hpColor);
            o.Write("/");
            o.Write($"{card.MaxHP}", ConsoleColor.Green);
            o.WriteLine(")");
        }

        private static void RenderMechanics(IOutput o, Card card, MiscHelpers.Timing timing, bool newline = false)
        {
            foreach (CustomMechanic mechanic in card.CustomMechanics)
            {
                if (mechanic.Timing != timing) continue;

                o.Write($"\t{mechanic.Name}: ", mechanic.Color);
                if (newline) o.WriteLine($"{mechanic.Text} ");
                else o.Write($"{mechanic.Text} ");
            }
        }
    }
}
