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

        public MoveCardCommand(Player player) : base(player)
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            Option<Location> srcOpt = MiscHelpers.GetLocationFromPlayer("Select the source location:");
            if (!srcOpt.HasValue) return false;
            src_ = srcOpt.ValueOr(() => throw new InvalidOperationException("No source location."));

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
            dest_ = destOpt.ValueOr(() => throw new InvalidOperationException("No destination location."));

            var card = card_.ValueOr(() => throw new InvalidOperationException("No card selected."));
            return player_.MoveCard(card, src_, dest_);
        }

        public override void Undo()
        {
            var card = card_.ValueOr(() => throw new InvalidOperationException("No card to undo."));
            player_.MoveCard(card, dest_, src_);
        }
    }
}
