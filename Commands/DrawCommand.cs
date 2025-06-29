using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DrawCommand : Command
    {
        private Option<Card> card_;
        private Card Card_ => card_.ValueOrThrow();
        private bool bottom_;
        public DrawCommand(Player player, bool fromBottom) : base(player)
        {
            bottom_ = fromBottom;
        }

        public override bool Execute()
        {
            card_ = player_.Draw(verbose: true, fromBottom: bottom_);
            if (!card_.HasValue) return false;

            return player_.Hand().GetCards().Contains(Card_);
        }

        public override void Undo()
        {
            player_.UndoDraw(Card_, bottom_);
            player_.PlayerDeck.Shuffle();
        }
    }
}
