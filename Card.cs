using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class Card
    {
        public Card(KeyValuePair<string, JToken?> json) 
        {
            Name = json.Key;
            Type = json.Value["type"].ToString();
            Text = json.Value["text"] is null ? "" : json.Value["text"].ToString();
            OnEntry = json.Value["on entry"] is null ? "" : json.Value["on entry"].ToString();
            OnDestroy = json.Value["on destroy"] is null ? "" : json.Value["on destroy"].ToString();
            StartOfTurn = json.Value["start of turn"] is null ? "" : json.Value["start of turn"].ToString();
            Power = json.Value["power"] is null ? "" : json.Value["power"].ToString();
            EndOfTurn = json.Value["end of turn"] is null ? "" : json.Value["end of turn"].ToString();

            if (json.Value["starting HP"] is null)
            {
                MaxHP = json.Value["HP"] is null ? 0 : int.Parse(json.Value["HP"].ToString());
                hp_ = MaxHP;
            }
            else
            {
                MaxHP = json.Value["max HP"] is null ? 0 : int.Parse(json.Value["max HP"].ToString());
                hp_ = int.Parse(json.Value["starting HP"].ToString());
            }
            
            if (json.Value["modifiers"] is not null)
            {
                foreach (JObject mod in ((JArray)json.Value["modifiers"]))
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
                        Console.WriteLine($"'{mod.GetValue("color").ToString()}' is not a valid color");
                        Environment.Exit(1);
                    }
                }
            }
        }

        public bool IsOneshot()
        {
            return Type.Contains("Oneshot") || Type.Contains("One-Shot");
        }

        public void PrintHP()
        {
            Console.Write(" (");
            ConsoleColor hpColor = HP == MaxHP ? ConsoleColor.Green : ConsoleColor.Red;
            MiscHelpers.ColorPrint(hpColor, $"{HP}");
            Console.Write("/");
            MiscHelpers.ColorPrint(ConsoleColor.Green, $"{MaxHP}");
            Console.WriteLine(")");
        }

        public void PrintText()
        {
            if (OnEntry != "")
            {
                Console.Write($" {OnEntry}");
            }

            if (StartOfTurn != "")
            {
                Console.Write($" {StartOfTurn}");
            }

            if (Text != "")
            {
                Console.Write($" {Text}");
            }

            if (Power != "")
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, $" Power: ");
                Console.Write($"{Power}");
            }

            if (EndOfTurn != "")
            {
                Console.Write($" {EndOfTurn}");
            }

            if (OnDestroy != "")
            {
                Console.Write($" {OnDestroy}");
            }

            if (Count != 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.Magenta, " Count: ");
                Console.Write($"{Count}");
            }
            Console.WriteLine();
        }

        public void PrettyPrint(bool brief = false)
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

            Console.WriteLine($"\t{Type}");

            if (OnEntry != "")
            {
                Console.WriteLine($"\t{OnEntry}");
            }

            if (StartOfTurn != "")
            {
                Console.WriteLine($"\t{StartOfTurn}");
            }

            if (Text != "")
            {
                Console.WriteLine($"\t{Text}");
            }

            if (Power != "")
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, $"\tPower: ");
                Console.WriteLine($"{Power}");
            }

            if (EndOfTurn != "")
            {
                Console.WriteLine($"\t{EndOfTurn}");
            }

            if (OnDestroy != "")
            {
                Console.WriteLine($"\t{OnDestroy}");
            }

            if (Count != 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.Magenta, "\tCount: ");
                Console.WriteLine($"{Count}");
            }
            Console.WriteLine();
        }

        public string Name { get; }
        public string Type { get; }
        public string OnEntry { get; }
        public string OnDestroy { get; }
        public string StartOfTurn { get; }
        public string EndOfTurn { get; }
        public string Text { get; }
        public string Power { get; }
        public int Count { get; set; }

        private int hp_;
        public int HP 
        {
            get { return hp_;  }
            
            set 
            {
                if (value >= MaxHP) 
                {
                    hp_ = MaxHP; 
                }
                else if (value <= 0)
                {
                    hp_ = 0;
                }
                else
                {
                    hp_ = value;
                }
            } 
        }

        public int MaxHP { get; set; }
        public List<Modifier> Modifiers { get; } = new List<Modifier>();
    }
}
