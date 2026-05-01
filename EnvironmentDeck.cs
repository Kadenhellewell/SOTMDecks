using Newtonsoft.Json.Linq;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class EnvironmentDeck : Deck<EnvironmentCard>
    {
        public EnvironmentDeck(string fileName) : base("Environment Deck")
        {
            string file = File.ReadAllText(fileName);
            JObject json = JObject.Parse(file);

            if (json.TryGetValue("Deck", out JToken? deckToken) && deckToken is JObject deckObject)
            {
                cards_ = ParseDeck(deckObject);
            }
            else
            {
                throw new Exception("JSON does not contain a valid 'Deck' object.");
            }
        }

        protected override List<EnvironmentCard> ParseDeck(JObject json)
        {
            List<EnvironmentCard> cards = new List<EnvironmentCard>();
            foreach (var card in json)
            {
                for (int i = 0; i < int.Parse(card.Value["frequency"].ToString()); i++)
                {
                    cards.Add(new EnvironmentCard(card));
                }
            }

            return cards;
        }
    }
}
