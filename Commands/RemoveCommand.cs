using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class RemoveCommand : Command
    {
        private CardCollection<HeroCard>? src_;
        private HeroCard? card_;
        private CardCollection<HeroCard> ko_;

        public RemoveCommand(CardCollection<HeroCard> KO, Player player) : base(player)
        {
            ko_ = KO;
        }

        public override bool Execute()
        {
            string? str = MiscHelpers.GetStringFromPlayer("From where?");
            if (str is null) return false;

            switch (str.ToLower())
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

            HeroCard? card = MiscHelpers.GetCardFromIndex(src_, verbose: true);
            if (card is null) return false;

            card_ = card;
            src_.Remove(card);
            ko_.Add(card);
            return true;
        }

        public override void Undo()
        {
            if (src_ is not { } src || card_ is not { } card) return;

            ko_.Remove(card);
            src.Add(card);
        }
    }
}
