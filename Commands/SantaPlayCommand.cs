using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks.Commands
{
    internal class SantaPlayCommand : Command
    {
        private CardCollection bag_;

        public SantaPlayCommand(Player player) : base(player)
        {
            bag_ = new CardCollection("Temp Santa's bag");
        }

        public override bool Execute()
        {
            bag_.AddCollection(player_.SantasBag());
            if (bag_.GetCount() <= 0)
            {
                Console.WriteLine("Nothing in Santa's bag");
                return false;
            }

            bool success = true;
            foreach(Card card in bag_.GetCards())
            {
                Console.Write("Playing ");
                MiscHelpers.ColorPrint(ConsoleColor.Green, card.Name, newLine: false);
                Console.WriteLine(" from Santa's bag");
                if (card.Type.Contains("Oneshot"))
                {
                    Console.WriteLine($"-- oneshot: {card.Text}");
                    success &= player_.MoveCard(card, Location.SantasBag, Location.DiscardPile);
                }
                else
                {
                    success &= player_.MoveCard(card, Location.SantasBag, Location.PlayArea);
                }
            }
            return success;
        }

        public override void Undo()
        {
            foreach (Card card in bag_.GetCards())
            {
                if (card.Type.Contains("Oneshot"))
                {
                    player_.MoveCard(card, Location.DiscardPile, Location.SantasBag);
                }
                else
                {
                    player_.MoveCard(card, Location.PlayArea, Location.SantasBag);
                }
            }
        }
    }
}
