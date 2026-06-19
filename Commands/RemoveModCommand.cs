using System;

namespace SOTMDecks.Commands
{
    internal class RemoveModCommand : Command
    {
        private Modifier? mod_;
        private int index_ = -1;

        public RemoveModCommand(Player player) : base(player)
        {
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

            if (MiscHelpers.GetIntFromPlayer("") is not int index) return false;

            if (index < 0 || index >= player_.Modifiers.Count)
            {
                Console.WriteLine("Index out of range");
                return false;
            }

            index_ = index;
            mod_ = player_.Modifiers[index];
            player_.RemoveMod(index);
            return true;
        }

        public override void Undo()
        {
            // Restore the modifier at its original position.
            if (mod_ is not { } mod) return;

            player_.InsertMod(index_, mod);
        }
    }
}
