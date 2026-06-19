using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class CollectTypeCommand : Command
    {
        private CardCollection<HeroCard>? cards_;

        public CollectTypeCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            string? type = MiscHelpers.GetStringFromPlayer("What type?");
            if (type is null) return false;

            if (MiscHelpers.GetIntFromPlayer("How many?") is not int num) return false;

            CardCollection<HeroCard> cards = player_.PlayerDeck.RevealByType(type, num);

            if (cards.GetCount() == 0)
            {
                Console.WriteLine($"Deck did not contain any cards of type '{type}'");
                return false;
            }

            cards_ = cards;
            player_.MoveCardsFromDeckToHand(cards);

            foreach (HeroCard card in cards.GetCards())
            {
                Console.WriteLine($"Collected {card.Name}");
            }

            return true;
        }

        public override void Undo()
        {
            if (cards_ is not { } cards) return;

            foreach (HeroCard card in cards.GetCards())
            {
                player_.Hand().Remove(card);
                player_.PlayerDeck.Add(card);
            }
            player_.Shuffle();
        }
    }
}
