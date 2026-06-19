using System;
using System.Collections.Generic;

namespace SOTMDecks.Input
{
    /// <summary>
    /// A read-only command that just shows information. It never builds an
    /// undoable <c>Command</c> and never touches the undo stack — there is simply
    /// nothing to undo.
    /// </summary>
    internal class ViewCommand : IInputCommand
    {
        private readonly Action<GameContext> show_;

        public ViewCommand(string[] triggers, string help, Action<GameContext> show)
        {
            Triggers = triggers;
            Help = help;
            show_ = show;
        }

        public IReadOnlyList<string> Triggers { get; }
        public string Help { get; }

        public bool Run(GameContext ctx)
        {
            show_(ctx);
            return true;
        }
    }
}
