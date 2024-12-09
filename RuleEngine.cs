using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; //Es mejor: System.Text.Json, por eficiencia

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
        { "!=", (left, right) => !Equals(left, right) }
    };


    // Método para comparar valores numéricos
    /*
    private static bool Compare(object left, object right, Func<double, double, bool> comparison)
    {
        if (TryConvertToDouble(left, out double leftValue) && TryConvertToDouble(right, out double rightValue))
        {
            return comparison(leftValue, rightValue);
        }

        throw new InvalidOperationException($"No se pudo comparar los valores: {left}, {right}");
    }
    */
    private static bool Compare(object left, object right, Func<double, double, bool> comparison)
    {
        if (TryConvertToDouble(left, out double leftValue) && TryConvertToDouble(right, out double rightValue))
        {
            return comparison(leftValue, rightValue);
        }

        throw new InvalidOperationException($"No se pudo comparar los valores: {left}, {right}");
    }

    // Intentar convertir un objeto a double
    /*
    private static bool TryConvertToDouble(object value, out double result)
    {
        result = 0;

        if (value == null)
            return false;

        if (value is JValue jValue)
        {
            value = jValue.Value; // Extraer el valor del JValue
        }

        if (double.TryParse(value?.ToString(), out result))
        {
            return true;
        }

        return false;
    }
    */
    // Intentar convertir un objeto a double
    /*
    private static bool TryConvertToDouble(object value, out double result)
    {
        result = 0;

        if (value == null)
            return false;

        if (value is JValue jValue)
        {
            value = jValue.Value; // Extraer el valor del JValue
        }

        if (double.TryParse(value?.ToString(), out result))
        {
            return true;
        }

        return false;
    }
    */
    // Intentar convertir un objeto a double
    private static bool TryConvertToDouble(object value, out double result)
    {
        result = 0;
        if (value is JValue jValue) value = jValue.Value;
        return double.TryParse(value?.ToString(), out result);
    }


    // Método principal para evaluar condiciones
    /*
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
    */
    // Método principal para evaluar condiciones
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


    // Obtener el valor de una variable desde los datos o usar el valor literal
    private static object GetValue(object path, JObject data)
    {
        if (path is JObject jObject)
        {
            return jObject; // Devuelve el objeto JSON si es uno
        }

        if (path is string pathStr && pathStr.StartsWith("var"))
        {
            var tokens = pathStr.Split('.');
            JToken current = data;

            foreach (var token in tokens[1..])
            {
                current = current[token];
                if (current == null)
                    return null;
            }

            return current is JValue jValue ? jValue.Value : current;
        }

        return path;
    }

    // Extraer un valor simple desde una variable o un dato literal
    /*
    private static object ExtractSimpleValue(object input, JObject data)
    {
        if (input is string pathStr && pathStr.StartsWith("var"))
        {
            var tokens = pathStr.Split('.');
            JToken current = data;

            foreach (var token in tokens[1..])
            {
                current = current[token];
                if (current == null)
                    return null;
            }

            return current is JValue jValue ? jValue.Value : current;
        }

        if (input is JValue jVal)
        {
            return jVal.Value;
        }

        return input; // Devolver el literal si no es un objeto JSON o una variable
    }
    */
    // Extraer un valor simple desde una variable o un dato literal
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
                    if (current == null)
                        return null;
                }

                return current is JValue jValue ? jValue.Value : current;
            }
        }

        if (input is JValue jVal)
        {
            return jVal.Value;
        }

        return input;
    }

    private void ValidateCondition(Condition condition)
    {
        if (condition == null)
        {
            throw new ArgumentNullException(nameof(condition), "La condición no puede ser nula.");
        }

        // Si tiene subcondiciones, validamos la estructura lógica
        if (condition.SubConditions != null && condition.SubConditions.Count > 0)
        {
            if (string.IsNullOrEmpty(condition.LogicalOperator))
            {
                throw new InvalidOperationException("Las subcondiciones requieren un operador lógico.");
            }

            // Validar operador lógico permitido
            var validLogicalOperators = new[] { "AND", "OR", "NOT" };
            if (!validLogicalOperators.Contains(condition.LogicalOperator.ToUpper()))
            {
                throw new InvalidOperationException($"Operador lógico no reconocido: {condition.LogicalOperator}");
            }

            // Validar recursivamente las subcondiciones
            foreach (var subCondition in condition.SubConditions)
            {
                ValidateCondition(subCondition);
            }
        }
        else
        {
            // Si no hay subcondiciones, validamos como una condición simple
            if (string.IsNullOrEmpty(condition.Operator))
            {
                throw new InvalidOperationException("Una condición simple requiere un operador.");
            }

            // Validar que el operador sea reconocido
            if (!Operators.ContainsKey(condition.Operator))
            {
                throw new InvalidOperationException($"Operador no reconocido: {condition.Operator}");
            }

            // Validar que ambas partes (Left y Right) estén definidas
            if (condition.Left == null || condition.Right == null)
            {
                throw new InvalidOperationException("Una condición simple debe tener valores definidos para 'Left' y 'Right'.");
            }
        }
    }

    private static void ValidateData(Condition condition, JObject data)
    {
        if (condition == null || data == null) return;

        if (condition.Left is JObject leftObj && leftObj.ContainsKey("var"))
        {
            string path = leftObj["var"]?.ToString();
            if (!PathExistsInData(path, data))
            {
                throw new InvalidOperationException($"La clave '{path}' no existe en los datos proporcionados.");
            }
        }

        if (condition.Right is JObject rightObj && rightObj.ContainsKey("var"))
        {
            string path = rightObj["var"]?.ToString();
            if (!PathExistsInData(path, data))
            {
                throw new InvalidOperationException($"La clave '{path}' no existe en los datos proporcionados.");
            }
        }

        if (condition.SubConditions != null)
        {
            foreach (var subCondition in condition.SubConditions)
            {
                ValidateData(subCondition, data);
            }
        }
    }

    private static bool PathExistsInData(string path, JObject data)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var tokens = path.Split('.');
        JToken current = data;

        foreach (var token in tokens)
        {
            current = current[token];
            if (current == null) return false;
        }

        return true;
    }

}
