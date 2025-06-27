using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static List<int>? GetIntsFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? intListStr = Console.ReadLine();
            if (intListStr is null)
            {
                Console.WriteLine("No input provided");
                return null;
            }

            List<int>? intList = StringOfIntsToListOfInts(intListStr);
            if (intList is null)
            {
                Console.WriteLine("Input must be space-separated integers");
                return null;
            }

            return intList;
        }

        public static bool YesOrNo()
        {
            string? str = Console.ReadLine();
            if (str is null)
            {
                Console.WriteLine("No input provided - providing no");
                return false;
            }

            str = str.ToLower();
            return str == "y" || str == "yes";
        }

        public static List<int>? StringOfIntsToListOfInts(string ints)
        {
            try
            {
                return ints.Split(' ') // Split by spaces
                    .Where(s => !string.IsNullOrWhiteSpace(s)) // Remove empty entries
                    .Select(int.Parse) // Convert to integers
                    .ToList(); // Convert to list
            }
            catch
            {
                return null;
            }
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

        public static List<Card>? GetCardsFromInput(CardCollection col, bool verbose = false)
        {
            Console.WriteLine("Select cards space-separated numbers");
            col.ListPrint(verbose);

            List<int>? intList = GetIntsFromPlayer("");
            if (intList is null) return null;

            List<Card>? cards = new List<Card>();
            foreach (var i in intList) 
            {
                if (i >= col.GetCount())
                {
                    Console.WriteLine($"Index {i} out of range");
                    return null;
                }

                cards.Add(col.GetCards()[i]);
            }

            return cards;
        }

        public static Location? GetLocationFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);

            Console.WriteLine("1. Hand");
            Console.WriteLine("2. Play Area");
            Console.WriteLine("3. Discard Pile");
            Console.WriteLine("4. Top of Deck");
            Console.WriteLine("5. Bottom of Deck");

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
                case 4: return Location.TopOfDeck;
                case 5: return Location.BottomOfDeck;
                default: return null;
            }
        }
    }
}
