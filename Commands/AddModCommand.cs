using Optional;
using System;

namespace SOTMDecks.Commands
{
    internal class AddModCommand : Command
    {
        private Option<Modifier> mod_;

        public AddModCommand(Player player) : base(player)
        {
            mod_ = Option.None<Modifier>();
        }

        public override bool Execute()
        {
            Option<string> descOpt = MiscHelpers.GetStringFromPlayer("Description");
            if (!descOpt.HasValue) return false;

            Modifier mod = new Modifier(descOpt.ValueOrThrow(), ConsoleColor.Cyan);
            player_.AddMod(mod);
            mod_ = Option.Some(mod);
            return true;
        }

        public override void Undo()
        {
            if (mod_.HasValue) player_.RemoveMod(mod_.ValueOrThrow());
        }
    }
}
