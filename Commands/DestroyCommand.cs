using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DestroyCommand : Command
    {
        private Card? card_;

        public DestroyCommand(Player player) : base(player) { }
        public override bool Execute()
        {
            Card? card = MiscHelpers.GetCardFromIndex(player_.PlayArea());
            if (card is null) return false;

            card_ = card;
            return player_.DestroyCard(card_);
        }

        public override void Undo()
        {
            player_.MoveCard(card_, Location.DiscardPile, Location.PlayArea);
        }
    }
}
