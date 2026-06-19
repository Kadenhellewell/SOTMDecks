using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class DestroyCommand : Command
    {
        private List<(HeroCard card, int oldHP)>? snapshots_;

        public DestroyCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            List<HeroCard>? cards = MiscHelpers.GetCardsFromInput(player_.PlayArea());
            if (cards is null) return false;

            // Capture HP before destroying: OnDestroyed resets each card to full HP,
            // so undo must restore the pre-destroy values rather than leave them maxed.
            snapshots_ = cards.Select(c => (c, c.HP)).ToList();
            return player_.DestroyCards(cards);
        }

        public override void Undo()
        {
            if (snapshots_ is null) return;

            foreach (var (card, oldHP) in snapshots_)
            {
                player_.MoveCard(card, Location.DiscardPile, Location.PlayArea);
                card.HP = oldHP;
            }
        }
    }
}
