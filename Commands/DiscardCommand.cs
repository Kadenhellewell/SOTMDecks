using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DiscardCommand : Command
    {
        private int num_;
        private bool fromDeck_;

        public DiscardCommand(bool fromDeck, Player player) : base(player)
        {
            fromDeck_ = fromDeck;
            num_ = 0;
        }

        public override bool Execute()
        {
            if (fromDeck_)
            {
                int? num = MiscHelpers.GetIntFromPlayer("How many?");
                if (num is null) return false;

                num_ = num.Value;
                return player_.DiscardFromDeck(num_);
            }
            else
            {
                Card? card = MiscHelpers.GetCardFromIndex(player_.Hand());
                if (card is null) return false;

                num_ = 1;
                return player_.Discard(card);
            }
        }

        public override void Undo()
        {
            player_.UndoPreviousDiscard(num_, fromDeck_);
        }
    }
}
