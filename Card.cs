using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal abstract class Card
    {
        public Card(KeyValuePair<string, JToken?> _) 
        {
        
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

        public string TypeAsString()
        {
            return string.Join(", ", Type);
        }

        public bool IsType(string type)
        {
            return Type.Any(t => t.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public void OnDestroyed()
        {
            hp_ = MaxHP;
            Console.WriteLine($"{OnDestroy}");
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

        public abstract void PrintText();
        public abstract void PrettyPrint(bool brief = false);

        public string Name { get; protected init; }
        public List<string> Type { get; protected init; }
        public string OnDestroy { get; protected init; }
        public string StartOfTurn { get; protected init; }
        public string EndOfTurn { get; protected init; }
        public string Text { get; protected init; }
        public List<CustomMechanic> CustomMechanics { get; protected init; } = new();
        public bool IsTarget { get; protected init; }

        protected int hp_;
        public int HP
        {
            get { return hp_; }

            protected set
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
    }
}
