using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class CollectTypeCommand : Command
    {
        private Option<CardCollection> cards_;
        public CollectTypeCommand(Player player) : base(player)
        {

        }

        public override bool Execute()
        {
            Option<string> typesOpt = MiscHelpers.GetStringFromPlayer("What type?");
            if (!typesOpt.HasValue) return false;

            Option<int> numOpt = MiscHelpers.GetIntFromPlayer("How many?");
            if (!numOpt.HasValue) return false;

            int num = numOpt.ValueOrThrow();
            string type = typesOpt.ValueOr("");

            cards_ = Option.Some(player_.PlayerDeck.RevealByType(type, num));

            if (cards_.ValueOrThrow().GetCount() == 0)
            {
                Console.WriteLine($"Deck did not contain any cards of type '{type}'");
                return false;
            }

            player_.MoveCardsFromDeckToHand(cards_.ValueOrThrow());

            foreach (Card card in cards_.ValueOrThrow().GetCards()) 
            {
                Console.WriteLine($"Collected {card.Name}");
            }

            return true;
        }

        public override void Undo()
        {
            foreach (var card in cards_)
            {
                player_.MoveAllCards(Location.Hand, Location.Deck);
            }
            player_.Shuffle();
        }
    }
}
