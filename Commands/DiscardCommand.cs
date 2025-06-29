using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks.Commands
{
    internal class DiscardCommand : Command
    {
        private int num_;
        private bool fromDeck_;
        private List<Card> cards_;

        public DiscardCommand(bool fromDeck, Player player) : base(player)
        {
            fromDeck_ = fromDeck;
            num_ = 0;
            cards_ = new List<Card>();
        }

        public override bool Execute()
        {
            if (fromDeck_)
            {
                Option<int> num = MiscHelpers.GetIntFromPlayer("How many?");
                if (!num.HasValue) return false;

                num_ = num.ValueOr(0);
                Option<List<Card>> cards = player_.PlayerDeck.GetTopCards(num_);
                if (!cards.HasValue || cards.ValueOr(new List<Card>()).Count == 0) return false;

                cards_ = cards.ValueOr(new List<Card>()).ToList();
                return player_.DiscardFromDeck(num_);
            }
            else
            {
                Option<List<Card>> cards = MiscHelpers.GetCardsFromInput(player_.Hand());
                if (!cards.HasValue) return false;

                num_ = cards.ValueOr(new List<Card>()).Count;
                cards_ = cards.ValueOr(new List<Card>()).ToList();
                bool result = true;
                foreach (Card card in cards_)
                {
                    bool thisResult = player_.Discard(card);
                    if (!thisResult) Console.WriteLine($"Failure discarding {card.Name}");
                    result &= thisResult;
                }
                return result;
            }
        }

        public override void Undo()
        {
            Location dest = fromDeck_ ? Location.TopOfDeck : Location.Hand;
            foreach (Card card in cards_)
            {
                if (!player_.MoveCard(card, Location.DiscardPile, dest))
                {
                    Console.WriteLine($"Failed to move {card.Name}");
                }
            }
        }
    }
}
