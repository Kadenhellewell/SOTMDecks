using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Optional;

namespace SOTMDecks
{
    internal class HeroDeck : Deck<HeroCard>
    {
        /**
         Layout of json. One deck:
        {
            "card-name": {
                type:"some type",
                text:"some text",
                "frequency":num
            },
            ...
        }


         */
        public HeroDeck(string fileName) : base("Deck")
        {
            string file = File.ReadAllText(fileName);
            JObject json = JObject.Parse(file);

            // InnatePower2 isn't in most decks, whereas the other attributes are in all of them.
            // This assures that it doesn't get into a weird state
            InnatePower2 = "";

            Name = json["Name"]?.ToString() ?? Name;

            // If the JSON has a property named "Identities" and that property is a JSON array,
            // then proceed inside the block, with the variable identitiesArray ready to use.
            if (json.TryGetValue("Identities", out JToken? identitiesToken) && identitiesToken is JArray identitiesArray)
            {
                int choice;

                if (identitiesArray.Count == 1)
                {
                    choice = 0; // Only one identity, auto-select it
                }
                else
                {
                    for (int i = 0; i < identitiesArray.Count; i++)
                    {
                        var identity = identitiesArray[i];
                        string title = identity["Title"]?.ToString() ?? "title";
                        string innatePower = identity["Innate Power"]?.ToString() ?? "";
                        Console.WriteLine($"{i}: {title} - Power: {innatePower}");
                    }

                    Option<int> inputChoice;
                    do
                    {
                        inputChoice = MiscHelpers.GetIntFromPlayer("Select Identity by number:");
                    } while (!inputChoice.HasValue || inputChoice.ValueOr(-1) < 0 || inputChoice.ValueOr(0) >= identitiesArray.Count);

                    choice = inputChoice.ValueOr(0);
                }

                var chosen = identitiesArray[(int)choice];
                InnatePower = chosen["Innate Power"]?.ToString() ?? "";
                InnatePower2 = chosen["Innate Power 2"]?.ToString() ?? "";
                StartingHP = int.Parse(chosen["Starting HP"]?.ToString() ?? "0");
                IncapacitatedAbilities = chosen["Incapacitated"]?.ToObject<string[]>() ?? throw new Exception("Missing 'Incapacitated' abilities in chosen Identity.");
            }
            else
            {
                throw new Exception("JSON does not contain 'Identities' array.");
            }

            if (json.TryGetValue("Deck", out JToken? deckToken) && deckToken is JObject deckObject)
            {
                cards_ = ParseDeck(deckObject);
            }
            else
            {
                throw new Exception("JSON does not contain a valid 'Deck' object.");
            }
        }

        
        public string Name { get; private set; } = "Hero Deck";
        public string InnatePower { get; }
        public string InnatePower2 { get; }
        public int StartingHP { get; }
        public string[] IncapacitatedAbilities { get; }

        protected override List<HeroCard> ParseDeck(JObject json)
        {
            List<HeroCard> cards = new List<HeroCard>();
            foreach (var card in json)
            {
                for (int i = 0; i < int.Parse(card.Value["frequency"].ToString()); i++)
                {   
                    cards.Add(new HeroCard(card));
                }
            }

            return cards;
        }

        public void PrintInnatePower()
        {
            MiscHelpers.ColorPrint(ConsoleColor.Blue, "Innate Power: ");
            Console.WriteLine(InnatePower);

            if (InnatePower2 != "")
            {
                MiscHelpers.ColorPrint(ConsoleColor.Blue, "Innate Power: ");
                Console.WriteLine(InnatePower2);
            }
        }
    }
}
