using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class CustomMechanic
    {
        public string Name { get; }
        public ConsoleColor Color { get; }
        public string Text { get; }
        public MiscHelpers.Timing Timing { get; }

        public CustomMechanic(JObject json)
        {
            Name = json.GetValue("name")?.ToString() ?? "Custom";
            Text = json.GetValue("text")?.ToString() ?? "";

            string colorStr = json.GetValue("color")?.ToString() ?? "Cyan";
            if (!Enum.TryParse(colorStr, out ConsoleColor parsedColor))
            {
                Console.WriteLine($"'{colorStr}' is not a valid ConsoleColor for custom mechanic '{Name}'. Defaulting to Cyan.");
                parsedColor = ConsoleColor.Cyan;
            }
            Color = parsedColor;

            string timingStr = (json.GetValue("timing")?.ToString() ?? "none").ToUpper();
            if (!Enum.TryParse(timingStr, out MiscHelpers.Timing parsedTiming))
            {
                Console.WriteLine($"'{timingStr}' is not a vlid Timing for custom mechanic '{Name}'. Defaulting to NONE.");
                parsedTiming = MiscHelpers.Timing.NONE;
            }
            Timing = parsedTiming;
        }

        public void Print(MiscHelpers.Timing timing, bool newline = false)
        {
            if (timing != Timing) return; // Only print at the right time

            MiscHelpers.ColorPrint(Color, $"\t{Name}: ");
            if (newline) Console.WriteLine($"{Text} ");
            else Console.Write($"{Text} ");
        }
    }
}
