using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SearchTargetCommand : Command
    {
        private HeroCard? card_;

        public SearchTargetCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            CardCollection<HeroCard> col = player_.PlayerDeck.SearchForTargets();

            if (col.GetCount() == 0)
            {
                Console.WriteLine("Deck did not contain any targets");
                return false;
            }

            HeroCard? card = MiscHelpers.GetCardFromIndex(col, verbose: true);
            if (card is null) return false;

            card_ = card;
            player_.MoveCardFromDeckToHand(card);

            return true;
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

            player_.Hand().Remove(card);
            player_.PlayerDeck.Add(card);
            player_.Shuffle();
        }
    }
}
