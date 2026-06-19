using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SantaCommand : Command
    {
        private HeroCard? card_;

        public SantaCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            HeroCard? card = player_.PlayerDeck.Draw();
            if (card is null) return false;

            card_ = card;
            player_.AddCardToSantasBag(card);
            return true;
        }

        public override void Undo()
        {
            if (card_ is not { } card) return;

            if (!player_.RemoveCardFromSantasBag(card))
            {
                Console.WriteLine("Unable to remove card from Santa's bag");
                return;
            }

            player_.PlayerDeck.Insert(0, card);
        }
    }
}
