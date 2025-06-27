using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                int? num = MiscHelpers.GetIntFromPlayer("How many?");
                if (num is null) return false;

                num_ = num.Value;
                List<Card>? cards = player_.PlayerDeck.GetTopCards(num_);
                if (cards is null || cards.Count == 0) return false;

                cards_ = cards.ToList();
                return player_.DiscardFromDeck(num_);
            }
            else
            {
                List<Card>? cards = MiscHelpers.GetCardsFromInput(player_.Hand());
                if (cards is null) return false;

                num_ = cards.Count;
                cards_ = cards.ToList();
                bool result = true;
                foreach (Card card in cards)
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
