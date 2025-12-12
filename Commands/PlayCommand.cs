using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks.Commands
{
    internal class PlayCommand : Command
    {
        private Option<HeroCard> card_;
        private HeroCard Card_ => card_.ValueOrThrow();

        public PlayCommand(Player player) : base(player)
        {
            card_ = Option.None<HeroCard>();
        }

        public override bool Execute()
        {
            card_ = MiscHelpers.GetCardFromIndex(player_.Hand());
            if (!card_.HasValue) return false;

            return player_.PlayCard(Card_);
        }

        public override void Undo()
        {
            if (Card_.IsOneshot())
            {
                player_.MoveCard(Card_, Location.DiscardPile, Location.Hand);
            }
            else
            {
                player_.MoveCard(Card_, Location.PlayArea, Location.Hand);
            }
        }
    }
}
