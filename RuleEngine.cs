using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

public class RuleEngine
{
    // Diccionario de operadores y sus implementaciones
    private static readonly Dictionary<string, Func<object, object, bool>> Operators = new()
    {
        { ">", (left, right) => Compare(left, right, (x, y) => x > y) },
        { "<", (left, right) => Compare(left, right, (x, y) => x < y) },
        { ">=", (left, right) => Compare(left, right, (x, y) => x >= y) },
        { "<=", (left, right) => Compare(left, right, (x, y) => x <= y) },
        { "==", (left, right) => Equals(left, right) },
        { "!=", (left, right) => !Equals(left, right) },
        { "IN", (left, right) => InOperator(left, right) },
        { "BETWEEN", (left, right) => BetweenOperator(left, right) }
    };

    private static bool Compare(object left, object right, Func<double, double, bool> comparison)
    {
        if (TryConvertToDouble(left, out double leftValue) && TryConvertToDouble(right, out double rightValue))
        {
            return comparison(leftValue, rightValue);
        }

        throw new InvalidOperationException($"No se pudo comparar los valores: {left}, {right}");
    }

    private static bool TryConvertToDouble(object value, out double result)
    {
        result = 0;
        if (value is JValue jValue) value = jValue.Value;
        return double.TryParse(value?.ToString(), out result);
    }

    // Método para evaluar condiciones (como antes)
    public bool EvaluateConditions(Condition condition, JObject data)
    {
        // Si la condición tiene subcondiciones, evaluarlas recursivamente
        if (condition.SubConditions != null && condition.SubConditions.Count > 0)
        {
            var logicalOperator = condition.LogicalOperator ?? "AND"; // Operador lógico predeterminado: AND
            bool result = logicalOperator == "AND";

            foreach (var subCondition in condition.SubConditions)
            {
                var subResult = EvaluateConditions(subCondition, data);
                result = logicalOperator switch
                {
                    "AND" => result && subResult,
                    "OR" => result || subResult,
                    "NOT" => !subResult,
                    _ => throw new InvalidOperationException($"Operador lógico no reconocido: {logicalOperator}")
                };
            }
            return result;
        }
        else
        {
            // Evaluar condición simple
            var leftValue = ExtractSimpleValue(condition.Left, data);
            var rightValue = ExtractSimpleValue(condition.Right, data);

            if (Operators.ContainsKey(condition.Operator))
            {
                return Operators[condition.Operator](leftValue, rightValue);
            }
            throw new InvalidOperationException($"Operador no reconocido: {condition.Operator}");
        }
    }

    private static object ExtractSimpleValue(object input, JObject data)
    {
        if (input is JObject jObject && jObject.ContainsKey("var"))
        {
            string pathStr = jObject["var"]?.ToString();
            if (!string.IsNullOrEmpty(pathStr))
            {
                var tokens = pathStr.Split('.');
                JToken current = data;

                foreach (var token in tokens)
                {
                    current = current[token];
                    if (current == null) return null;
                }

                return current is JValue jValue ? jValue.Value : current;
            }
        }

        return input is JValue jVal ? jVal.Value : input;
    }

    private static bool InOperator(object left, object right)
    {
        if (right is JArray array)
        {
            foreach (var item in array)
            {
                if (Equals(left, item.ToObject<object>()))
                {
                    return true;
                }
            }
            return false;
        }

        throw new InvalidOperationException($"Operador IN no soporta este tipo de dato para 'right': {right?.GetType()}");
    }

    private static bool BetweenOperator(object left, object right)
    {
        if (right is JArray range && range.Count == 2)
        {
            var min = Convert.ToDouble(range[0].ToObject<object>());
            var max = Convert.ToDouble(range[1].ToObject<object>());
            var value = Convert.ToDouble(left);
            return value >= min && value <= max;
        }

        throw new InvalidOperationException($"Operador BETWEEN no soporta este tipo de dato para 'right': {right?.GetType()}");
    }

    private void ExecuteActions(List<Action> actions, JObject data)
    {
        // Implementación del método para ejecutar acciones
    }
}
