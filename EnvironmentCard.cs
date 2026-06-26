using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal class EnvironmentCard : Card
    {
        public EnvironmentCard(KeyValuePair<string, JToken?> json) : base(json)
        {
            JToken value = json.Value ?? throw new Exception($"Card '{json.Key}' has no definition");

            Name = json.Key;
            string typeStr = value["type"]?.ToString() ?? throw new Exception($"Card {Name} doesn't provide a type");
            Type = typeStr.Split(',')
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList();

            Text = value["text"]?.ToString() ?? "";
            OnDestroy = value["on destroy"]?.ToString() ?? "";
            StartOfTurn = value["start of turn"]?.ToString() ?? "";
            EndOfTurn = value["end of turn"]?.ToString() ?? "";

            if (value["custom"] is JArray customArray)
            {
                foreach (JObject cm in customArray)
                {
                    CustomMechanics.Add(new CustomMechanic(cm));
                }
            }

            MaxHP = value["HP"] is { } hp ? int.Parse(hp.ToString()) : 0;
            hp_ = MaxHP;

            IsTarget = MaxHP > 0;

            if (value["modifiers"] is JArray modifierArray)
            {
                foreach (JObject mod in modifierArray)
                {
                    string text = mod.GetValue("text")?.ToString() ?? "";
                    string colorStr = mod.GetValue("color")?.ToString() ?? "Cyan";
                    if (!Enum.TryParse(colorStr, out ConsoleColor color))
                    {
                        Console.WriteLine($"'{colorStr}' is not a valid color for a modifier on '{Name}'. Defaulting to Cyan.");
                        color = ConsoleColor.Cyan;
                    }
                    Modifiers.Add(new Modifier(text, color));
                }
            }
        }
    }
}
