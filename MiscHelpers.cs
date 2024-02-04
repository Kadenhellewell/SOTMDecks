using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    static internal class MiscHelpers
    {
        public static void ColorPrint(ConsoleColor coler, string message, bool newLine = false)
        {
            Console.ForegroundColor = coler;
            Console.Write(message);
            Console.ResetColor();
            if (newLine) Console.WriteLine();
        }

        public static int? GetIntFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? intStr = Console.ReadLine();
            if (intStr is null)
            {
                Console.WriteLine("No input provided");
                return null;
            }

            int? num;
            try
            {
                num = int.Parse(intStr);
            }
            catch
            {
                Console.WriteLine("Need to provide an integer");
                num = null;
            }

            return num;
        }

        public static string? GetStringFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? modStr = Console.ReadLine();
            if (modStr is null)
            {
                Console.WriteLine("No description provided");
                return null;
            }

            return modStr;
        }


        public static Card? GetCardFromIndex(CardCollection col, bool verbose = false)
        {
            Console.WriteLine("Card?");
            col.ListPrint(verbose);

            int? index = GetIntFromPlayer("");
            if (index is null) return null;

            if (index >= col.GetCount())
            {
                Console.WriteLine("Index out of range");
                return null;
            }

            return col.GetCards()[index.Value];
        }

        public static Location? GetLocationFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);

            Console.WriteLine("1. Hand");
            Console.WriteLine("2. Play Area");
            Console.WriteLine("3. Discard Pile");

            int? loc = MiscHelpers.GetIntFromPlayer("");
            if (loc is null)
            {
                Console.WriteLine("Must give an integer 1-3");
                return null;
            }

            switch (loc)
            {
                case 1: return Location.Hand;
                case 2: return Location.PlayArea;
                case 3: return Location.DiscardPile;
                default: return null;
            }
        }
    }
}
