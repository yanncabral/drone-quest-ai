namespace Core.Agent;

public class AgentState
{
    public string Name { get; set; } = $"Tony Stark #{DateTime.Now.GetHashCode()}";
    public Color Color { get; set; } = new (33, 33, 33);
    public Position Position { get; set; } = new Position(0, 0);
    public Direction Direction { get; set; }
    
    public int Score { get; set; }
    public int Energy { get; set; }

    public bool Dead { get; set; }
    
    public HashSet<Observation> Observations { get; set; } = new();
    
    private string ObservationsString => "{ " + string.Join(", ", Observations.Select(o => o.ToString())) + " }";

    public override string ToString()
    {
        return $"AgentState({Position}, Direction: {Direction}, Score: {Score}, Energy: {Energy}, Observations: {ObservationsString})";
    }
}