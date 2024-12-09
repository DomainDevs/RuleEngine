public class Rule
{
    public string Name { get; set; }
    public Condition Conditions { get; set; }
    public List<Action> Actions { get; set; }
}