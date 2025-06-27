using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SearchSpecialCommand : Command
    {
        Card? card_;

        public SearchSpecialCommand(Player player) : base(player) { }

        public override bool Execute()
        {
            Card? card = player_.PlayerDeck.GetSpecialType();
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
            player_.Hand().Remove(card_);
            player_.PlayerDeck.Add(card_);
            player_.Shuffle();
        }
    }
}
