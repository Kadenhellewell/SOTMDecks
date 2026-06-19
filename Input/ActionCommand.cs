using System;
using System.Collections.Generic;
using SOTMDecks.Commands;

namespace SOTMDecks.Input
{
    /// <summary>
    /// The bridge between the dispatch layer and the undoable <c>Command</c> layer:
    /// builds a <c>Command</c> and runs it through the undo stack. This is the only
    /// kind of input command that records anything for undo. An optional post-print
    /// runs afterward (e.g. re-showing HP or the target list).
    /// </summary>
    internal class ActionCommand : IInputCommand
    {
        private readonly Func<GameContext, Command> build_;
        private readonly Action<GameContext>? postPrint_;

        public ActionCommand(string[] triggers, string help,
                             Func<GameContext, Command> build,
                             Action<GameContext>? postPrint = null)
        {
            Triggers = triggers;
            Help = help;
            build_ = build;
            postPrint_ = postPrint;
        }

        public IReadOnlyList<string> Triggers { get; }
        public string Help { get; }

        public bool Run(GameContext ctx)
        {
            ctx.RunCommand(build_(ctx));
            postPrint_?.Invoke(ctx);
            return true;
        }
    }
}
