using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class PlayCommand : Command
    {
        private HeroCard? card_;

        public PlayCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            HeroCard? card = MiscHelpers.GetCardFromIndex(player_.Hand());
            if (card is null) return false;

            card_ = card;
            return player_.PlayCard(card);
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

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
