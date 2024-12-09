public class Rule
{
    public int Id { get; set; }  // Identificador único de la regla
    public bool IsActive { get; set; }  // Indica si la regla está activa
    public string Name { get; set; }  // Nombre de la regla
    public Condition Conditions { get; set; }  // Condiciones de la regla
    public List<Action> Actions { get; set; }  // Acciones que ejecutará la regla
}
