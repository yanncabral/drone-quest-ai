namespace Core;

public class AgentState
{
    public (int x, int y) Position { get; init; }
    public Direction Direction { get; init; }
    
    public int Score { get; init; }
    public int Energy { get; init; }
    public int Ammo { get; init; }
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

public enum Observations
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
    
    // State
    public AgentState? State { get; set; }

    private GameClient Client { get; set; }
    public IController Controller { get; set; }
    
    public Agent(IController controller)
    {
        Controller = controller;
        Client = new GameClient(this);
    }
    
    public void SendCommand(Command command)
    {
        Console.WriteLine($"Sending {Enum.GetName(typeof(Command), command)}");
        Client.SendCommand(command);
        Client.SendCommand(Command.Observe);
    }
}