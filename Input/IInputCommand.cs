using System.Collections.Generic;

namespace SOTMDecks.Input
{
    /// <summary>
    /// Something the player can type at the prompt. This is the dispatch layer:
    /// a command knows its trigger word(s) and how to run, but says nothing about
    /// undo. Undoable actions reach the undo stack via <see cref="GameContext.RunCommand"/>;
    /// views and other handlers simply never do — so "undo doesn't apply here" is
    /// handled by omission, not by a flag or an empty Undo().
    /// </summary>
    internal interface IInputCommand
    {
        /// <summary>One or more strings that invoke this command (aliases).</summary>
        IReadOnlyList<string> Triggers { get; }

        /// <summary>One-line description, used by the `help` listing.</summary>
        string Help { get; }

        /// <summary>Runs the command. Returns whether the game loop should keep going.</summary>
        bool Run(GameContext ctx);
    }
}
