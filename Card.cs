using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class Card
    {
        public Card(string name, string type, string text) 
        {
            Name = name;
            Type = type;
            Text = text;
        }

        public void PrettyPrint()
        {
            MiscHelpers.ColorPrint(ConsoleColor.Green, Name, newLine: true);
            Console.WriteLine($"\t{Type}");
            Console.WriteLine($"\t{Text}");

            if (Count != 0)
            {
                MiscHelpers.ColorPrint(ConsoleColor.Magenta, "\tCount: ");
                Console.WriteLine($"{Count}");
            }
            Console.WriteLine();
        }

        public string Name { get; }
        public string Type { get; }
        public string Text { get; }
        public int Count { get; set;  }
    }
}
