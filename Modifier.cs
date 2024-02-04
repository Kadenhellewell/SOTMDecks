using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SOTMDecks
{
    internal class Modifier
    {
        public Modifier(string desc, ConsoleColor color) 
        {
            Description = desc;
            Color = color;
            Number = 0;
        }

        public string Description { get; }
        public int Number;
        ConsoleColor Color;

        public void Increment()
        {
            Number++;
        }

        public void Decrement()
        {
            Number--;
        }

        public override string ToString()
        {
            return $"\t{Description} ({Number})";
        }

        public void Print()
        {
            MiscHelpers.ColorPrint(Color, ToString(), newLine: true);
        }
    }
}
