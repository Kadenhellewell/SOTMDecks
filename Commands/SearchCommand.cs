using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SearchCommand : Command
    {
        Card? card_;

        public SearchCommand(Player player) : base(player) { }

        public override bool Execute()
        {
            string? types = MiscHelpers.GetStringFromPlayer("What types (separate with spaces)?");
            if (types is null) return false;

            CardCollection col = new CardCollection($"Types: {types}");
            foreach (string type in types.Split(" "))
            {
                col.AddCollection(player_.PlayerDeck.SearchByType(type));
            }

            if (col.GetCount() == 0)
            {
                Console.WriteLine($"Deck did not contain any cards of type '{types}'");
                return false;
            }

            Card? card = MiscHelpers.GetCardFromIndex(col, verbose: true);
            if (card is null) return false;

            card_ = card;
            player_.MoveCardFromDeckToHand(card_);
            return true;
        }

        public override void Undo()
        {
            player_.Hand().Remove(card_);
            player_.PlayerDeck.Add(card_);
            player_.Shuffle();
        }
    }
}
