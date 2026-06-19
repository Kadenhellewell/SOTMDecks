using System;
using System.Collections.Generic;

namespace SOTMDecks.Input
{
    /// <summary>
    /// Maps trigger strings to input commands. Replaces the giant dispatch switch:
    /// commands are data you can register, enumerate (for `help`), and check for
    /// collisions — and the same registry can be shared across game modes.
    /// </summary>
    internal class CommandRegistry
    {
        private readonly Dictionary<string, IInputCommand> byTrigger_ =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly List<IInputCommand> all_ = new();

        public void Register(IInputCommand command)
        {
            all_.Add(command);
            foreach (string trigger in command.Triggers)
            {
                if (byTrigger_.ContainsKey(trigger))
                {
                    throw new InvalidOperationException($"Duplicate command trigger: '{trigger}'");
                }
                byTrigger_[trigger] = command;
            }
        }

        public bool TryGet(string trigger, out IInputCommand command)
            => byTrigger_.TryGetValue(trigger, out command!);

        /// <summary>All registered commands, in registration order (for `help`).</summary>
        public IReadOnlyList<IInputCommand> All => all_;
    }
}
