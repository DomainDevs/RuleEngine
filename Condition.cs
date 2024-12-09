public class Condition
{
    public object Left { get; set; }
    public string Operator { get; set; }
    public object Right { get; set; }
    public string LogicalOperator { get; set; }  // Operador lógico, como "AND", "OR"
    public List<Condition> SubConditions { get; set; }  // Subcondiciones para evaluaciones más complejas
}