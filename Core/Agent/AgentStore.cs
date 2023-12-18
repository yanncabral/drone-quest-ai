namespace Core.Agent;

public class AgentStore : IEqualityComparer<AgentStore>
{
    public AgentState AgentState { get; set; } = new();
    public GameState GameState { get; set; } = new();
    public Command? LastCommand { get; set; }

    public override string ToString()
    {
        return $"AgentStore({AgentState}, LastCommand: {(char) LastCommand.GetValueOrDefault()})";
    }


    public bool Equals(AgentStore? x, AgentStore? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return Equals(x.AgentState, y.AgentState) && Equals(x.GameState, y.GameState) && x.LastCommand == y.LastCommand;
    }

    public int GetHashCode(AgentStore obj)
    {
        return HashCode.Combine(obj.AgentState, obj.GameState, obj.LastCommand);
    }
}