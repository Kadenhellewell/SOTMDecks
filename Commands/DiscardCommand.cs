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
        private List<HeroCard> cards_;

        public DiscardCommand(bool fromDeck, Player player) : base(player)
        {
            fromDeck_ = fromDeck;
            num_ = 0;
            cards_ = new List<HeroCard>();
        }

        public override bool Execute()
        {
            if (fromDeck_)
            {
                if (MiscHelpers.GetIntFromPlayer("How many?") is not int num) return false;

                num_ = num;
                List<HeroCard>? cards = player_.PlayerDeck.GetTopCards(num_);
                if (cards is null || cards.Count == 0) return false;

                cards_ = cards.ToList();
                return player_.DiscardFromDeck(num_);
            }
            else
            {
                List<HeroCard>? cards = MiscHelpers.GetCardsFromInput(player_.Hand());
                if (cards is null) return false;

                num_ = cards.Count;
                cards_ = cards.ToList();
                bool result = true;
                foreach (HeroCard card in cards_)
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
            foreach (HeroCard card in cards_)
            {
                if (!player_.MoveCard(card, Location.DiscardPile, dest))
                {
                    Console.WriteLine($"Failed to move {card.Name}");
                }
            }
        }
    }
}
