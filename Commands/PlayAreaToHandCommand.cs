using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class PlayAreaToHandCommand : Command
    {
        private HeroCard? card_;

        public PlayAreaToHandCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            HeroCard? card = MiscHelpers.GetCardFromIndex(player_.GetLocation(Location.PlayArea));
            if (card is null) return false;

            card_ = card;
            return player_.MoveCard(card, Location.PlayArea, Location.Hand);
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

            player_.MoveCard(card, Location.Hand, Location.PlayArea);
        }
    }
}
