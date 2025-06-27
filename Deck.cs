﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SOTMDecks
{
    internal class Deck : CardCollection
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
        public Deck(string fileName) : base("Deck")
        {
            string file = File.ReadAllText(fileName);
            JObject json = JObject.Parse(file);

            // InnatePower2 isn't in most decks, whereas the other attributes are in all of them.
            // This assures that it doesn't get into a weird state
            InnatePower2 = "";

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

                    int? inputChoice = null;
                    do
                    {
                        inputChoice = MiscHelpers.GetIntFromPlayer("Select Identity by number:");
                    } while (inputChoice == null || inputChoice < 0 || inputChoice >= identitiesArray.Count);

                    choice = (int)inputChoice;
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

        
        public string Name { get; }
        public string InnatePower { get; }
        public string InnatePower2 { get; }
        public int StartingHP { get; }
        public string[] IncapacitatedAbilities { get; }

        private static List<Card> ParseDeck(JObject json)
        {
            List<Card> cards = new List<Card>();
            foreach (var card in json)
            {
                for (int i = 0; i < int.Parse(card.Value["frequency"].ToString()); i++)
                {   
                    cards.Add(new Card(card));
                }
            }

            return cards;
        }

        public void Shuffle()
        {
            cards_ = cards_.OrderBy(a => Guid.NewGuid()).ToList();
        }

        public Card? Draw(bool fromBottom = false)
        {
            if (cards_.Count == 0)
            {
                Console.WriteLine("No cards to draw");
                return null;
            }
            Card drawn;
            if (fromBottom)
            {
                drawn = cards_[cards_.Count - 1];
                cards_.RemoveAt(cards_.Count - 1);
            }
            else
            {
                drawn = cards_.First();
                cards_.RemoveAt(0);
            }
            return drawn;
        }

        public List<Card>? GetTopCards(int n)
        {
            if (cards_.Count < n)
            {
                Console.WriteLine($"Fewer than {n} cards");
                return null;
            }

            return cards_.Take(n).ToList();
        }

        public void RevealCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (i >= cards_.Count)
                {
                    Console.WriteLine("No more cards in deck");
                    return;
                }
                cards_[i].PrettyPrint();
                Console.WriteLine();
                Console.WriteLine("------");
            }
        }
    }
}
