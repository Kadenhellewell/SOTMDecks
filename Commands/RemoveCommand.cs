using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class RemoveCommand : Command
    {
        private CardCollection? src_;
        private Card? card_;
        private CardCollection ko_;

        public RemoveCommand(CardCollection KO, Player player) : base(player) 
        {
            ko_ = KO;
        }

        public override bool Execute()
        {
            string? str = MiscHelpers.GetStringFromPlayer("From where?");
            if (str is null) return false;
            str = str.ToLower();

            switch (str)
            {
                case "hand":
                    src_ = player_.Hand();
                    break;
                case "discard":
                case "discard pile":
                    src_ = player_.DiscardPile();
                    break;
                case "play area":
                    src_ = player_.PlayArea();
                    break;
                default:
                    Console.WriteLine("Not a valid location");
                    return false;
            }

            Card? card = MiscHelpers.GetCardFromIndex(src_, verbose: true);
            if (card is null) return false;

            card_ = card;
            src_.Remove(card_);
            ko_.Add(card_);
            return true;
        }

        public override void Undo()
        {
            ko_.Remove(card_);
            src_.Add(card_);
        }
    }
}
