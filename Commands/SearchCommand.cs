using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks.Commands
{
    internal class SearchCommand : Command
    {
        Option<Card> card_;

        public SearchCommand(Player player) : base(player) 
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            Option<string> typesOpt = MiscHelpers.GetStringFromPlayer("What types (separate with spaces)?");
            if (!typesOpt.HasValue) return false;

            string types = typesOpt.ValueOr("");

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

            card_ = MiscHelpers.GetCardFromIndex(col, verbose: true);
            if (!card_.HasValue) return false;

            var card = card_.ValueOr(() => throw new InvalidOperationException("No card."));

            player_.MoveCardFromDeckToHand(card);
            
            return true;
        }

        public override void Undo()
        {
            var card = card_.ValueOr(() => throw new InvalidOperationException("No card."));
            player_.Hand().Remove(card);
            player_.PlayerDeck.Add(card);
            player_.Shuffle();
        }
    }
}
