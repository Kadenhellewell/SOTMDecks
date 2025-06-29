using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks.Commands
{
    internal class PlayCommand : Command
    {
        private Option<Card> card_;

        public PlayCommand(Player player) : base(player)
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            card_ = MiscHelpers.GetCardFromIndex(player_.Hand());
            if (!card_.HasValue) return false;

            var card = card_.ValueOr(() => throw new InvalidOperationException("No card."));

            return player_.PlayCard(card);
        }

        public override void Undo()
        {
            var card = card_.ValueOr(() => throw new InvalidOperationException("No card."));
            if (card.IsOneshot())
            {
                player_.MoveCard(card, Location.DiscardPile, Location.Hand);
            }
            else
            {
                player_.MoveCard(card, Location.PlayArea, Location.Hand);
            }
        }
    }
}
