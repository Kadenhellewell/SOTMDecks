using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ConsoleColor Color { get; }

        public override string ToString()
        {
            return $"\t{Description}";
        }
    }
}
