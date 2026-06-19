using Optional;
using System;

namespace SOTMDecks.Commands
{
    internal class RemoveModCommand : Command
    {
        private Option<Modifier> mod_;
        private int index_;

        public RemoveModCommand(Player player) : base(player)
        {
            mod_ = Option.None<Modifier>();
            index_ = -1;
        }

        public override bool Execute()
        {
            if (player_.Modifiers.Count == 0)
            {
                Console.WriteLine("No modifiers to remove");
                return false;
            }

            Console.WriteLine("Which one?");
            for (int i = 0; i < player_.Modifiers.Count; i++)
            {
                Console.WriteLine($"\t{i}: {player_.Modifiers[i]}");
            }

            Option<int> indexOpt = MiscHelpers.GetIntFromPlayer("");
            if (!indexOpt.HasValue) return false;

            int index = indexOpt.ValueOrThrow();
            if (index < 0 || index >= player_.Modifiers.Count)
            {
                Console.WriteLine("Index out of range");
                return false;
            }

            index_ = index;
            mod_ = Option.Some(player_.Modifiers[index]);
            player_.RemoveMod(index);
            return true;
        }

        public override void Undo()
        {
            // Restore the modifier at its original position.
            if (mod_.HasValue) player_.InsertMod(index_, mod_.ValueOrThrow());
        }
    }
}
