using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        // Cargar reglas desde un archivo JSON
        //var rulesJson = File.ReadAllText("rules.json");
        //var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Rule>>(rulesJson);
        var rules = LoadRules(); // Asumimos que cargamos las reglas de un archivo JSON

        // Datos de entrada para probar las reglas
        JObject data = new JObject
        {
            ["compra"] = new JObject
            {
                ["total"] = 150
            },
            ["cliente"] = new JObject
            {
                ["tipo"] = "VIP",
                ["region"] = "norte"
            },
            ["compra"] = new JObject
            {
                ["total"] = 170,
                ["descuento"] = 10,
                ["metodo_pago"] = "tarjeta"
            },
            ["compra"] = new JObject { 
                ["total"] = 100 
            }
        };

        // Evaluar reglas
        var ruleEngine = new RuleEngine();
        foreach (var rule in rules)
        {
            var result = ruleEngine.EvaluateConditions(rule.Conditions, data);
            Console.WriteLine($"Regla '{rule.Name}' evaluada: {result}");
            if (result && rule.IsActive)
            {
                Console.WriteLine($"Acciones a ejecutar:");
                foreach (var action in rule.Actions)
                {
                    Console.WriteLine($"  - Tipo: {action.Type}, Valor: {action.Value}");
                }
            }
        }
    }
    public static List<Rule> LoadRules()
    {
        var json = File.ReadAllText("rules.json");
        return JsonConvert.DeserializeObject<List<Rule>>(json);
    }
}