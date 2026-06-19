using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DrawCommand : Command
    {
        private HeroCard? card_;
        private bool bottom_;

        public DrawCommand(Player player, bool fromBottom) : base(player)
        {
            bottom_ = fromBottom;
        }

        public override bool Execute()
        {
            card_ = player_.Draw(verbose: true, fromBottom: bottom_);
            if (card_ is null) return false;

            return player_.Hand().GetCards().Contains(card_);
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

            player_.UndoDraw(card, bottom_);
            player_.PlayerDeck.Shuffle();
        }
    }
}
