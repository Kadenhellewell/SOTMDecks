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

        public DestroyCommand(Player player) : base(player)
        {
            card_ = Option.None<Card>();
        }

        public override bool Execute()
        {
            card_ = MiscHelpers.GetCardFromIndex(player_.PlayArea());
            if (!card_.HasValue) return false;

            var card = card_.ValueOr(() => throw new InvalidOperationException("No card selected"));
            return player_.DestroyCard(card);
        }

        public override void Undo()
        {
            var card = card_.ValueOr(() => throw new InvalidOperationException("No card to undo"));
            player_.MoveCard(card, Location.DiscardPile, Location.PlayArea);
        }
    }
}
