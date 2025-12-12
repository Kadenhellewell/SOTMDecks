using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace SOTMDecks.Commands
{
    internal class RemoveCommand : Command
    {
        private Option<CardCollection> src_;
        private Option<HeroCard> card_;
        private HeroCard Card_ => card_.ValueOrThrow();
        private CardCollection ko_;

        public RemoveCommand(CardCollection KO, Player player) : base(player)
        {
            ko_ = KO;
            src_ = Option.None<CardCollection>();
            card_ = Option.None<HeroCard>();
        }

        public override bool Execute()
        {
            Option<string> strOpt = MiscHelpers.GetStringFromPlayer("From where?");
            if (!strOpt.HasValue) return false;

            string str = strOpt.ValueOr("").ToLower();

            switch (str)
            {
                case "hand":
                    src_ = Option.Some(player_.Hand());
                    break;
                case "discard":
                case "discard pile":
                    src_ = Option.Some(player_.DiscardPile());
                    break;
                case "play area":
                    src_ = Option.Some(player_.PlayArea());
                    break;
                default:
                    Console.WriteLine("Not a valid location");
                    return false;
            }

            var srcVal = src_.ValueOrThrow();
            card_ = MiscHelpers.GetCardFromIndex(src_.ValueOrThrow(), verbose: true);
            if (!card_.HasValue) return false;

            srcVal.Remove(Card_);
            ko_.Add(Card_);
            return true;
        }

        public override void Undo()
        {
            var srcVal = src_.ValueOrThrow();

            ko_.Remove(Card_);
            srcVal.Add(Card_);
        }
    }
}
