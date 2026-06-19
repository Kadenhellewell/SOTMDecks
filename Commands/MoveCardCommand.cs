using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class MoveCardCommand : Command
    {
        private Location src_;
        private Location dest_;
        private HeroCard? card_;

        public MoveCardCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            if (MiscHelpers.GetLocationFromPlayer("Select the source location:") is not Location src) return false;
            src_ = src;

            if (src_ == Location.TopOfDeck)
            {
                Console.WriteLine("Use the command 'draw' instead.");
                return false;
            }

            if (src_ == Location.BottomOfDeck)
            {
                Console.WriteLine("Use the command 'draw bottom' instead.");
                return false;
            }

            HeroCard? card = MiscHelpers.GetCardFromIndex(player_.GetLocation(src_));
            if (card is null) return false;

            if (MiscHelpers.GetLocationFromPlayer("Select the destination location:") is not Location dest) return false;
            dest_ = dest;

            card_ = card;
            return player_.MoveCard(card, src_, dest_);
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

            player_.MoveCard(card, dest_, src_);
        }
    }
}
