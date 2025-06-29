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
        }

        public void Print(bool newline = false)
        {
            MiscHelpers.ColorPrint(Color, $"\t{Name}: ");
            if (newline) Console.WriteLine(Text);
            else Console.Write(Text);
        }
    }
}
