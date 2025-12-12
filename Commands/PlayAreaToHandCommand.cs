using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class PlayAreaToHandCommand : Command
    {
        private Option<HeroCard> card_;
        private HeroCard Card_ => card_.ValueOrThrow();

        public PlayAreaToHandCommand(Player player) : base(player)
        {
            card_ = Option.None<HeroCard>();
        }

        public override bool Execute()
        {
            card_ = MiscHelpers.GetCardFromIndex(player_.GetLocation(Location.PlayArea));
            if (!card_.HasValue) return false;

            return player_.MoveCard(Card_, Location.PlayArea, Location.Hand);
        }

        public override void Undo()
        {
            player_.MoveCard(Card_, Location.Hand, Location.PlayArea);
        }
    }
}