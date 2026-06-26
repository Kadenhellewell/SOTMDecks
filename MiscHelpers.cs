using System;
using System.Collections.Generic;
using System.Linq;
using SOTMDecks.View;

namespace SOTMDecks
{
    static internal class MiscHelpers
    {
        public enum Timing
        {
            START,
            END,
            ENTRY,
            DESTROY,
            NONE
        }

        /// <summary>
        /// Strips a trailing " b" brief flag off a command string, setting <paramref name="brief"/> accordingly.
        /// </summary>
        public static string ExtractBrief(string commandStr, out bool brief)
        {
            brief = false;
            if (commandStr.EndsWith(" b"))
            {
                brief = true;
                commandStr = commandStr.Substring(0, commandStr.Length - 2).Trim();
            }
            return commandStr;
        }

        public static void ColorPrint(ConsoleColor color, string message, bool newLine = false)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
            if (newLine) Console.WriteLine();
        }

        public static int? GetIntFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? intStr = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(intStr))
            {
                Console.WriteLine("No input provided");
                return null;
            }

            if (int.TryParse(intStr, out int result))
            {
                return result;
            }

            Console.WriteLine("Need to provide an integer");
            return null;
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
                Console.WriteLine("No string provided");
                return null;
            }

            return modStr;
        }

        public static T? GetCardFromIndex<T>(CardCollection<T> col, bool verbose = false) where T : Card
        {
            Console.WriteLine("Card?");
            Display.List(col, verbose);

            if (GetIntFromPlayer("") is not int index) return null;

            if (index >= col.GetCount() || index < 0)
            {
                Console.WriteLine("Index out of range");
                return null;
            }

            return col.GetCards()[index];
        }

        public static List<T>? GetCardsFromInput<T>(CardCollection<T> col, bool verbose = false) where T : Card
        {
            Console.WriteLine("Select cards space-separated numbers");
            Display.List(col, verbose);

            List<int>? intList = GetIntsFromPlayer("");
            if (intList is null) return null;

            List<T> cards = new List<T>();
            foreach (var i in intList)
            {
                if (i >= col.GetCount())
                {
                    Console.WriteLine($"Index {i} out of range");
                    return null;
                }

                cards.Add(col.GetCards()[i]);
            }

            if (cards.Count == 0)
            {
                Console.WriteLine("No cards were selected");
                return null;
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

            if (GetIntFromPlayer("") is not int loc)
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
                default:
                    Console.WriteLine("Must provided integer 1-5");
                    return null;
            }
        }
    }
}
