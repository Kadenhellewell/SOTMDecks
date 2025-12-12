using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SearchTargetCommand : Command
    {
        private Option<HeroCard> card_;
        private HeroCard Card_ => card_.ValueOrThrow();

        public SearchTargetCommand(Player player) : base(player)
        {
            card_ = Option.None<HeroCard>();
        }

        public override bool Execute()
        {
            CardCollection col = player_.PlayerDeck.SearchForTargets();

            if (col.GetCount() == 0)
            {
                Console.WriteLine("Deck did not contain any targets");
                return false;
            }

            card_ = MiscHelpers.GetCardFromIndex(col, verbose: true);
            if (!card_.HasValue) return false;

            player_.MoveCardFromDeckToHand(Card_);

            return true;
        }

        public override void Undo()
        {
            player_.Hand().Remove(Card_);
            player_.PlayerDeck.Add(Card_);
            player_.Shuffle();
        }
    }
}
