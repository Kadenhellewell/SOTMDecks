using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DrawCommand : Command
    {
        private Card? card_;
        private bool bottom_;
        public DrawCommand(Player player, bool fromBottom) : base(player)
        {
            bottom_ = fromBottom;
        }

        public override bool Execute()
        {
            Card? card = player_.Draw(verbose: true, fromBottom: bottom_);
            if (card == null) return false;
            card_ = card;
            return player_.Hand().GetCards().Contains(card_);
        }

        public override void Undo()
        {
            player_.UndoDraw(card_, bottom_);
        }
    }
}
