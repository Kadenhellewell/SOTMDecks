using System;

namespace SOTMDecks.View
{
    /// <summary>
    /// Output port for the view layer. Renderers write through this instead of touching
    /// <see cref="Console"/> directly, so output can be redirected or captured in tests.
    /// </summary>
    internal interface IOutput
    {
        void Write(string text);
        void WriteLine(string text = "");

        /// <summary>Writes colored text, then restores the default color (no newline).</summary>
        void Write(string text, ConsoleColor color);

        /// <summary>Writes colored text and a newline; the newline itself is uncolored.</summary>
        void WriteLine(string text, ConsoleColor color);
    }
}
