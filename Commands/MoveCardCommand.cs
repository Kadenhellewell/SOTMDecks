using Optional;
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
        private Option<Card> card_;
        private Card Card_ => card_.ValueOrThrow();

        public MoveCardCommand(Player player) : base(player)
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            Option<Location> srcOpt = MiscHelpers.GetLocationFromPlayer("Select the source location:");
            if (!srcOpt.HasValue) return false;
            src_ = srcOpt.ValueOrThrow();

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

            card_ = MiscHelpers.GetCardFromIndex(player_.GetLocation(src_));
            if (!card_.HasValue) return false;

            Option<Location> destOpt = MiscHelpers.GetLocationFromPlayer("Select the destination location:");
            if (!destOpt.HasValue) return false;
            dest_ = destOpt.ValueOrThrow();

            return player_.MoveCard(Card_, src_, dest_);
        }

        public override void Undo()
        {
            player_.MoveCard(Card_, dest_, src_);
        }
    }
}
