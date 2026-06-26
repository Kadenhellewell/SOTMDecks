using System;

namespace SOTMDecks.View
{
    /// <summary>
    /// The production <see cref="IOutput"/>: writes to the console. Mirrors the behavior of
    /// the old MiscHelpers.ColorPrint (set color, write, reset).
    /// </summary>
    internal class ConsoleOutput : IOutput
    {
        public void Write(string text) => Console.Write(text);

        public void WriteLine(string text = "") => Console.WriteLine(text);

        public void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
