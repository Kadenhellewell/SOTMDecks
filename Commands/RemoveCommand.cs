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
        private Option<Card> card_;
        private CardCollection ko_;

        public RemoveCommand(CardCollection KO, Player player) : base(player)
        {
            ko_ = KO;
            src_ = Option.None<CardCollection>();
            card_ = Option.None<Card>();
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

            var srcVal = src_.ValueOr(() => throw new InvalidOperationException("Source not set"));
            card_ = MiscHelpers.GetCardFromIndex(srcVal, verbose: true);
            if (!card_.HasValue) return false;

            var cardVal = card_.ValueOr(() => throw new InvalidOperationException("No card selected"));
            srcVal.Remove(cardVal);
            ko_.Add(cardVal);
            return true;
        }

        public override void Undo()
        {
            var srcVal = src_.ValueOr(() => throw new InvalidOperationException("Source not set"));
            var cardVal = card_.ValueOr(() => throw new InvalidOperationException("No card to undo"));

            ko_.Remove(cardVal);
            srcVal.Add(cardVal);
        }
    }
}
