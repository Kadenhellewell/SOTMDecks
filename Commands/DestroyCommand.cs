using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DestroyCommand : Command
    {
        private List<HeroCard>? cards_;

        public DestroyCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            cards_ = MiscHelpers.GetCardsFromInput(player_.PlayArea());
            if (cards_ is null) return false;

            return player_.DestroyCards(cards_);
        }

        public override void Undo()
        {
            if (cards_ is not { } cards) return;

            foreach (HeroCard card in cards)
                player_.MoveCard(card, Location.DiscardPile, Location.PlayArea);
        }
    }
}
