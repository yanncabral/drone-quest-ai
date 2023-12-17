namespace Core;

public class AgentState
{
    public (int x, int y) Position { get; set; }
    public Direction Direction { get; set; }
    
    public int Score { get; set; }
    public int Energy { get; set; }
    public int Ammo { get; set; }
    
    public List<Observation> Observations { get; init; } = new();

    public override string ToString()
    {
        return $"AgentState(Position: {Position}, Direction: {Direction}, Score: {Score}, Energy: {Energy}, Ammo: {Ammo})";
    }
}

public enum State
{
    Dead,
    Game,
    Gameover,
    Ready,
}

public enum Direction
{
    East,
    North,
    South,
    West,
}

public enum Observation
{
    Blocked,
    Steps,
    Breeze,
    Flash,
    BlueLight,
    RedLight,
    GreenLight,
    Weaklight,
    Enemy,
    Damage,
    Hit,
}

public enum Items
{
    Falls,
    Coins,
    Rings,
    Potions,
    Obstacles,
    Teleports,
}

public record Color (int R, int G, int B);

public class Agent
{
    public string Name { get; set; } = $"Tony Stark #{DateTime.Now.GetHashCode()}";
    public Color Color { get; set; } = new (0, 0, 0);
    
    public AgentState? State { get; set; }

    public GameClient? Client { get; set; }
    public IController Controller { get; init; }

    public void Start()
    {
        // Agent = agent;
        // _client.sendColor(Agent.Color.R, Agent.Color.G, Agent.Color.B);
        // _client.sendName(Agent.Name);
    }
    
    public void SendCommand(Command command)
    {
        Console.WriteLine($"Sending {Enum.GetName(typeof(Command), command)}");
        if (Client is null)
            throw new NullReferenceException("Agent is not initialized.Have you added into a game client?");
        Client.SendCommand(command);
        Client.SendCommand(Command.Observe);
    }
}