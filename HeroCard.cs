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
            Name = json.Key;
            var typeStr = json.Value["type"]?.ToString() ?? throw new Exception($"Card {Name} doesn't provide a type");
            Type = typeStr.Split(',')
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList();

            Text = json.Value["text"] is null ? "" : json.Value["text"].ToString();
            OnEntry = json.Value["on entry"] is null ? "" : json.Value["on entry"].ToString();
            OnDestroy = json.Value["on destroy"] is null ? "" : json.Value["on destroy"].ToString();
            StartOfTurn = json.Value["start of turn"] is null ? "" : json.Value["start of turn"].ToString();
            Power = json.Value["power"] is null ? "" : json.Value["power"].ToString();
            EndOfTurn = json.Value["end of turn"] is null ? "" : json.Value["end of turn"].ToString();

            if (json.Value["custom"] is JArray customArray)
            {
                foreach (JObject cm in customArray)
                {
                    CustomMechanics.Add(new CustomMechanic(cm));
                }
            }

            if (json.Value["starting HP"] is null)
            {
                MaxHP = json.Value["HP"] is null ? 0 : int.Parse(json.Value["HP"].ToString());
                hp_ = MaxHP;
            }
            else
            {
                MaxHP = json.Value["max HP"] is null ? 0 : int.Parse(json.Value["max HP"].ToString());
                hp_ = int.Parse(json.Value["starting HP"].ToString());
                startingHP_ = hp_;
            }

            IsTarget = MaxHP > 0;
            
            if (json.Value["modifiers"] is not null)
            {
                foreach (JObject mod in (JArray)json.Value["modifiers"])
                {
                    string text = mod.GetValue("text").ToString();
                    try
                    {
                        Enum.TryParse(mod.GetValue("color").ToString(), out ConsoleColor color);
                        Modifiers.Add(new Modifier(text, color));
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"'{mod.GetValue("color")}' is not a valid color");
                        Environment.Exit(1);
                    }
                }
            }
        }

        public bool IsOneshot()
        {
            return Type.Any(t => t.Equals("Oneshot", StringComparison.OrdinalIgnoreCase)
                      || t.Equals("One-Shot", StringComparison.OrdinalIgnoreCase));
        }

        public bool HasCustomMechanicAtTime(MiscHelpers.Timing time)
        {
            foreach (var mech in CustomMechanics)
            {
                if (mech.Timing == time) return true;
            }

            return false;
        }

        

        public void PrintMechanics(MiscHelpers.Timing timing, bool newline = false)
        {
            foreach (var mechanic in CustomMechanics) 
            {
                mechanic.Print(timing, newline);
            }
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

            if (Count != 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.Magenta, " Count: ");
                Console.Write($"{Count}");
            }
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

            if (Count != 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.Magenta, "\tCount: ");
                Console.WriteLine($"{Count}");
            }
            Console.WriteLine();
        }

       
        public string OnEntry { get; }
        public string Power { get; }
        public int Count { get; set; }

        private int startingHP_;

        public List<Modifier> Modifiers { get; } = new List<Modifier>();
    }
}
