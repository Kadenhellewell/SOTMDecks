using System;
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

            foreach (var item in json) 
            {
                if (item.Key == "Innate Power")
                    InnatePower = item.Value.ToString();
                else if (item.Key == "Starting HP")
                    StartingHP = int.Parse(item.Value.ToString());
                else if (item.Key == "Name")
                    Name = item.Value.ToString();
                else if (item.Key == "Incapacitated")
                    IncapacitatedAbilities = ((JArray)item.Value).ToObject<string[]>();
                else if (item.Key == "Deck")
                {
                    cards_ = ParseDeck((JObject)item.Value);
                }
            }
        }

        
        public string Name { get; }
        public string InnatePower { get; }
        public int StartingHP { get; }
        public string[] IncapacitatedAbilities { get; }

        private static List<Card> ParseDeck(JObject json)
        {
            List<Card> cards = new List<Card>();
            foreach (var card in json)
            {
                for (int i = 0; i < int.Parse(card.Value["frequency"].ToString()); i++)
                {
                    cards.Add(new Card(card.Key, card.Value["type"].ToString(), card.Value["text"].ToString()));
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
