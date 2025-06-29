using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Optional;

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

        public static Option<int> GetIntFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? intStr = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(intStr))
            {
                Console.WriteLine("No input provided");
                return Option.None<int>();
            }

            if (int.TryParse(intStr, out int result))
            {
                return Option.Some(result);
            }
            else
            {
                Console.WriteLine("Need to provide an integer");
                return Option.None<int>();
            }
        }

        public static Option<List<int>> GetIntsFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? intListStr = Console.ReadLine();
            if (intListStr is null)
            {
                Console.WriteLine("No input provided");
                return Option.None<List<int>>();
            }

            Option<List<int>> intList = StringOfIntsToListOfInts(intListStr);
            if (!intList.HasValue)
            {
                Console.WriteLine("Input must be space-separated integers");
                return Option.None<List<int>>();
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

        public static Option<List<int>> StringOfIntsToListOfInts(string ints)
        {
            try
            {
                return Option.Some(ints.Split(' ') // Split by spaces
                    .Where(s => !string.IsNullOrWhiteSpace(s)) // Remove empty entries
                    .Select(int.Parse) // Convert to integers
                    .ToList()); // Convert to list
            }
            catch
            {
                return Option.None<List<int>>();
            }
        }

        public static Option<string> GetStringFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);
            string? modStr = Console.ReadLine();
            if (modStr is null)
            {
                Console.WriteLine("No string provided");
                return Option.None<string>();
            }

            return Option.Some(modStr);
        }


        public static Option<Card> GetCardFromIndex(CardCollection col, bool verbose = false)
        {
            Console.WriteLine("Card?");
            col.ListPrint(verbose);

            Option<int> indexOpt = GetIntFromPlayer("");
            if (!indexOpt.HasValue) return Option.None<Card>();

            int index = indexOpt.ValueOr(-1);

            if (index >= col.GetCount() || index < 0)
            {
                Console.WriteLine("Index out of range");
                return Option.None<Card>();
            }

            return Option.Some(col.GetCards()[index]);
        }

        public static Option<List<Card>> GetCardsFromInput(CardCollection col, bool verbose = false)
        {

            Console.WriteLine("Select cards space-separated numbers");
            col.ListPrint(verbose);

            Option<List<int>> intList = GetIntsFromPlayer("");
            if (!intList.HasValue) return Option.None<List<Card>>();

            List<Card> cards = new List<Card>();
            foreach (var i in intList.ValueOrThrow()) 
            {
                if (i >= col.GetCount())
                {
                    Console.WriteLine($"Index {i} out of range");
                    return Option.None<List<Card>>();
                }

                cards.Add(col.GetCards()[i]);
            }

            if (cards.Count == 0)
            {
                Console.WriteLine("Not cards were selected");
                return Option.None<List<Card>>();
            }
            return Option.Some(cards);
        }

        public static Option<Location> GetLocationFromPlayer(string prompt)
        {
            Console.WriteLine(prompt);

            Console.WriteLine("1. Hand");
            Console.WriteLine("2. Play Area");
            Console.WriteLine("3. Discard Pile");
            Console.WriteLine("4. Top of Deck");
            Console.WriteLine("5. Bottom of Deck");

            Option<int> locOpt = MiscHelpers.GetIntFromPlayer("");
            if (!locOpt.HasValue)
            {
                Console.WriteLine("Must give an integer 1-3");
                return Option.None<Location>();
            }

            switch (locOpt.ValueOr(0))
            {
                case 1: return Option.Some(Location.Hand);
                case 2: return Option.Some(Location.PlayArea);
                case 3: return Option.Some(Location.DiscardPile);
                case 4: return Option.Some(Location.TopOfDeck);
                case 5: return Option.Some(Location.BottomOfDeck);
                default:
                    Console.WriteLine("Must provided integer 1-5");
                    return Option.None<Location>();
            }
        }

        // This functions extends the Option class to allow me to grab values from options without needing to pass crap in
        public static T ValueOrThrow<T>(this Option<T> option)
        {
            return option.ValueOr(() => throw new InvalidOperationException("Option did not have a value"));
        }
    }
}
