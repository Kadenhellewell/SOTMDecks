using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class EnvironmentCard : Card
    {
        public EnvironmentCard(KeyValuePair<string, JToken?> json) : base(json)
        {
            Name = json.Key;
            var typeStr = json.Value["type"]?.ToString() ?? throw new Exception($"Card {Name} doesn't provide a type");
            Type = typeStr.Split(',')
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList();

            Text = json.Value["text"] is null ? "" : json.Value["text"].ToString();
            OnDestroy = json.Value["on destroy"] is null ? "" : json.Value["on destroy"].ToString();
            StartOfTurn = json.Value["start of turn"] is null ? "" : json.Value["start of turn"].ToString();
            EndOfTurn = json.Value["end of turn"] is null ? "" : json.Value["end of turn"].ToString();

            if (json.Value["custom"] is JArray customArray)
            {
                foreach (JObject cm in customArray)
                {
                    CustomMechanics.Add(new CustomMechanic(cm));
                }
            }


            MaxHP = json.Value["HP"] is null ? 0 : int.Parse(json.Value["HP"].ToString());
            hp_ = MaxHP;

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

        

        public override void PrintText()
        {

            if (StartOfTurn != "")
            {
                Console.Write($"{StartOfTurn} ");
            }
            PrintMechanics(MiscHelpers.Timing.START);

            if (Text != "")
            {
                Console.Write($" {Text}");
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

            if (StartOfTurn != "")
            {
                Console.WriteLine($"\t{StartOfTurn}");
            }
            PrintMechanics(MiscHelpers.Timing.START, newline: true);

            if (Text != "")
            {
                Console.WriteLine($"\t{Text}");
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

        public List<Modifier> Modifiers { get; } = new List<Modifier>();
    }
}
