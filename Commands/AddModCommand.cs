using System;

namespace SOTMDecks.Commands
{
    internal class AddModCommand : Command
    {
        private Modifier? mod_;

        public AddModCommand(Player player) : base(player)
        {
        }

        public override bool Execute()
        {
            string? desc = MiscHelpers.GetStringFromPlayer("Description");
            if (desc is null) return false;

            Modifier mod = new Modifier(desc, ConsoleColor.Cyan);
            player_.AddMod(mod);
            mod_ = mod;
            return true;
        }

        public override void Undo()
        {
            if (mod_ is not { } mod) return;

            player_.RemoveMod(mod);
        }
    }
}
