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
        private Option<List<HeroCard>> cards_;

        private List<HeroCard> Cards_ => cards_.ValueOrThrow();

        public DestroyCommand(Player player) : base(player)
        {
            cards_ = Option.None<List<HeroCard>>();
        }

        public override bool Execute()
        {
            cards_ = MiscHelpers.GetCardsFromInput(player_.PlayArea());
            if (!cards_.HasValue) return false;

            return player_.DestroyCards(Cards_);
        }

        public override void Undo()
        {
            foreach (HeroCard card in Cards_)
                player_.MoveCard(card, Location.DiscardPile, Location.PlayArea);
        }
    }
}
