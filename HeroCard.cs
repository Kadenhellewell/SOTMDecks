using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class HeroCard : Card
    {
        public HeroCard(KeyValuePair<string, JToken?> json) : base(json)
        {
            JToken value = json.Value ?? throw new Exception($"Card '{json.Key}' has no definition");

            Name = json.Key;
            string typeStr = value["type"]?.ToString() ?? throw new Exception($"Card {Name} doesn't provide a type");
            Type = typeStr.Split(',')
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList();

            Text = value["text"]?.ToString() ?? "";
            OnEntry = value["on entry"]?.ToString() ?? "";
            OnDestroy = value["on destroy"]?.ToString() ?? "";
            StartOfTurn = value["start of turn"]?.ToString() ?? "";
            Power = value["power"]?.ToString() ?? "";
            EndOfTurn = value["end of turn"]?.ToString() ?? "";

            if (value["custom"] is JArray customArray)
            {
                foreach (JObject cm in customArray)
                {
                    CustomMechanics.Add(new CustomMechanic(cm));
                }
            }

            JToken? startingHp = value["starting HP"];
            if (startingHp is null)
            {
                MaxHP = value["HP"] is { } hp ? int.Parse(hp.ToString()) : 0;
                hp_ = MaxHP;
            }
            else
            {
                MaxHP = value["max HP"] is { } maxHp ? int.Parse(maxHp.ToString()) : 0;
                hp_ = int.Parse(startingHp.ToString());
                startingHP_ = hp_;
            }

            IsTarget = MaxHP > 0;

            if (value["modifiers"] is JArray modifierArray)
            {
                foreach (JObject mod in modifierArray)
                {
                    string text = mod.GetValue("text")?.ToString() ?? "";
                    string colorStr = mod.GetValue("color")?.ToString() ?? "Cyan";
                    if (!Enum.TryParse(colorStr, out ConsoleColor color))
                    {
                        Console.WriteLine($"'{colorStr}' is not a valid color for a modifier on '{Name}'. Defaulting to Cyan.");
                        color = ConsoleColor.Cyan;
                    }
                    Modifiers.Add(new Modifier(text, color));
                }
            }
        }

        public bool IsOneshot()
        {
            return Type.Any(t => t.Equals("Oneshot", StringComparison.OrdinalIgnoreCase)
                      || t.Equals("One-Shot", StringComparison.OrdinalIgnoreCase));
        }

        public override void PrintText()
        {
            if (OnEntry != "")
            {
                Console.Write($" {OnEntry} ");
            }
            PrintMechanics(MiscHelpers.Timing.ENTRY);

            if (StartOfTurn != "")
            {
                Console.Write($"{StartOfTurn} ");
            }
            PrintMechanics(MiscHelpers.Timing.START);

            if (Text != "")
            {
                Console.Write($" {Text}");
            }

            if (Power != "")
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, $" Power: ");
                Console.Write($"{Power}");
            }

            PrintMechanics(MiscHelpers.Timing.NONE);

            if (EndOfTurn != "")
            {
                Console.Write($" {EndOfTurn}");
            }
            PrintMechanics(MiscHelpers.Timing.END);

            if (OnDestroy != "")
            {
                Console.Write($" {OnDestroy}");
            }
            PrintMechanics(MiscHelpers.Timing.DESTROY);

            Console.WriteLine();
        }

        public override void PrettyPrint(bool brief = false)
        {
            MiscHelpers.ColorPrint(ConsoleColor.Green, Name);
            if (MaxHP > 0)
            {
                PrintHP();
            }
            else
            {
                Console.WriteLine("");
            }

            if (brief) return;

            Console.WriteLine($"\t{TypeAsString()}");

            if (OnEntry != "")
            {
                Console.WriteLine($"\t{OnEntry}");
            }
            PrintMechanics(MiscHelpers.Timing.ENTRY, newline: true);

            if (StartOfTurn != "")
            {
                Console.WriteLine($"\t{StartOfTurn}");
            }
            PrintMechanics(MiscHelpers.Timing.START, newline: true);

            if (Text != "")
            {
                Console.WriteLine($"\t{Text}");
            }

            if (Power != "")
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, $"\tPower: ");
                Console.WriteLine($"{Power}");
            }

            PrintMechanics(MiscHelpers.Timing.NONE, newline: true);

            if (EndOfTurn != "")
            {
                Console.WriteLine($"\t{EndOfTurn}");
            }
            PrintMechanics(MiscHelpers.Timing.END, newline: true);

            if (OnDestroy != "")
            {
                Console.WriteLine($"\t{OnDestroy}");
            }
            PrintMechanics(MiscHelpers.Timing.DESTROY, newline: true);

            Console.WriteLine();
        }


        public string OnEntry { get; }
        public string Power { get; }

        private int startingHP_;
    }
}
