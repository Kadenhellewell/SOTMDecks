using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SantaCommand : Command
    {
        private Option<HeroCard> card_;
        private HeroCard Card_ => card_.ValueOrThrow();

        public SantaCommand(Player player) : base(player)
        {
            card_ = Option.None<HeroCard>();
        }

        public override bool Execute()
        {
            card_ = player_.PlayerDeck.Draw();
            if (!card_.HasValue) return false;

            player_.AddCardToSantasBag(Card_);
            return true;
        }

        public override void Undo()
        {
            if (!player_.RemoveCardFromSantasBag(Card_))
            {
                Console.WriteLine("Unable to remove card from Santa's bag");
                return;
            }

            player_.PlayerDeck.Insert(0, Card_);
        }
    }
}
