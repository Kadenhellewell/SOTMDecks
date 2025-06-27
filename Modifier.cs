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
        }

        public string Description { get; }
        ConsoleColor Color;

        public override string ToString()
        {
            return $"\t{Description}";
        }

        public void Print()
        {
            MiscHelpers.ColorPrint(Color, ToString(), newLine: true);
        }
    }
}
