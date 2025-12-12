using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DiscardHandCommand : Command
    {
        private CardCollection<HeroCard> cards_;
        public DiscardHandCommand(Player player) : base(player)
        {
            cards_ = new CardCollection<HeroCard>("discard hand");
        }

        public override bool Execute()
        {
            cards_.AddCollection(player_.Hand());
            return player_.DiscardHand();
        }

        public override void Undo()
        {
            player_.DiscardPile().RemoveAll(cards_);
            player_.Hand().AddCollection(cards_);
        }
    }
}
