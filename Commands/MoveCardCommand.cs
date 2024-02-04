using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class MoveCardCommand : Command
    {
        Location src_;
        Location dest_;
        Card? card_;

        public MoveCardCommand(Player player) : base(player) 
        {
            
        }

        public override bool Execute()
        {
            Location? src = MiscHelpers.GetLocationFromPlayer("Select the source location:");
            if (src is null) return false;
            src_ = src.Value;

            Card? card = MiscHelpers.GetCardFromIndex(player_.GetLocation(src_));
            if (card is null) return false;
            card_ = card;

            Location? dest = MiscHelpers.GetLocationFromPlayer("Select the destination location:");
            if (dest is null) return false;
            dest_ = dest.Value;

            return player_.MoveCard(card_, src_, dest_);
        }

        public override void Undo()
        {
            player_.MoveCard(card_, dest_, src_);
        }
    }
}
