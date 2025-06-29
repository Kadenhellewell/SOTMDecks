using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DestroyCommand : Command
    {
        private Option<Card> card_;

        private Card Card_ => card_.ValueOrThrow();

        public DestroyCommand(Player player) : base(player)
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            card_ = MiscHelpers.GetCardFromIndex(player_.PlayArea());
            if (!card_.HasValue) return false;

            return player_.DestroyCard(Card_);
        }

        public override void Undo()
        {
            player_.MoveCard(Card_, Location.DiscardPile, Location.PlayArea);
        }
    }
}
