public class Condition
{
    public string Operator { get; set; }
    public object Left { get; set; }
    public object Right { get; set; }
    public string LogicalOperator { get; set; }
    public List<Condition> SubConditions { get; set; }
}