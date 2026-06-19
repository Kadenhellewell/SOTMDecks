using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SearchSpecialCommand : Command
    {
        HeroCard? card_;

        public SearchSpecialCommand(Player player) : base(player) { }

        public override bool Execute()
        {
            HeroCard? card = player_.PlayerDeck.GetSpecialType();
            if (card is null)
            {
                return false;
            }

            card_ = card;
            Console.WriteLine($"Found special type card: {card_.Name}");
            player_.MoveCardFromDeckToHand(card_);
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
