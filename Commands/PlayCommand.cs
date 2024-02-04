using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class PlayCommand : Command
    {
        private Card? card_;

        public PlayCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            Card? card = MiscHelpers.GetCardFromIndex(player_.Hand());
            if (card is null) return false;

            card_ = card;

            if (card.Type.Contains("Oneshot"))
            {
                return player_.MoveCard(card, Location.Hand, Location.DiscardPile);
            }
            else
            {
                return player_.MoveCard(card, Location.Hand, Location.PlayArea);
            }
        }

        public override void Undo()
        {
            if (card_.Type.Contains("Oneshot"))
            {
                player_.MoveCard(card_, Location.DiscardPile, Location.Hand);
            }
            else
            {
                player_.MoveCard(card_, Location.PlayArea, Location.Hand);
            }
        }
    }
}
