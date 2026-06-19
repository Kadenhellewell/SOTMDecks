using System;
using SOTMDecks.Commands;

namespace SOTMDecks.Input
{
    /// <summary>
    /// The services handed to every input command: the player, whether brief
    /// output was requested for this invocation, and access to the undo stack.
    /// Built once by <see cref="Game"/>; <see cref="Brief"/> is refreshed before
    /// each dispatch.
    /// </summary>
    internal class GameContext
    {
        private readonly Action<Command> runCommand_;
        private readonly Action undo_;
        private readonly Action printSetup_;

        public GameContext(Player player, Action<Command> runCommand, Action undo, Action printSetup)
        {
            Player = player;
            runCommand_ = runCommand;
            undo_ = undo;
            printSetup_ = printSetup;
        }

        public Player Player { get; }

        /// <summary>Whether the player appended the " b" brief flag to this command.</summary>
        public bool Brief { get; set; }

        /// <summary>Executes an undoable action and records it on the undo stack.</summary>
        public void RunCommand(Command command) => runCommand_(command);

        /// <summary>Undoes the most recent undoable action.</summary>
        public void Undo() => undo_();

        /// <summary>Prints the HP / innate power / modifiers summary.</summary>
        public void PrintSetup() => printSetup_();
    }
}
