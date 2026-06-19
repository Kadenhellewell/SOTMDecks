using System;
using System.Collections.Generic;

namespace SOTMDecks.Input
{
    /// <summary>
    /// A command that runs an action and isn't recorded for undo. Covers both
    /// non-undoable state changes (e.g. shuffle) and control commands. Set
    /// <paramref name="keepGoing"/> to false to end the game loop (quit) — that is
    /// the only "control flow" the dispatch layer needs.
    /// </summary>
    internal class SimpleCommand : IInputCommand
    {
        private readonly Action<GameContext> run_;
        private readonly bool keepGoing_;

        public SimpleCommand(string[] triggers, string help, Action<GameContext> run, bool keepGoing = true)
        {
            Triggers = triggers;
            Help = help;
            run_ = run;
            keepGoing_ = keepGoing;
        }

        public IReadOnlyList<string> Triggers { get; }
        public string Help { get; }

        public bool Run(GameContext ctx)
        {
            run_(ctx);
            return keepGoing_;
        }
    }
}
