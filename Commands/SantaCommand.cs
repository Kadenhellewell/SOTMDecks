using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SantaCommand : Command
    {
        Card card_;
        public SantaCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            Card? card = player_.PlayerDeck.Draw();
            if (card is null) return false;

            card_ = card;

            player_.AddCardToSantasBag(card_);
            return true;
        }

        public override void Undo()
        {
            // Remove card from the bag
            if (!player_.RemoveCardFromSantasBag(card_))
            {
                Console.WriteLine("Unable to remove card from Santa's bag");
                return;
            }

            // Put the card back on top of the deck
            player_.PlayerDeck.Insert(0, card_);
        }
    }
}
