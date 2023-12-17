using INF1771_GameClient.Socket;

namespace Core;

public sealed class GameClient
{
    private const string Host = "atari.icad.puc-rio.br";
    private readonly HandleClient _client = new ();
    private Agent Agent { get; init; }
    
    public GameClient(Agent agent)
    {
        Agent = agent;
        
        HandleClient.CommandEvent += ReceiveCommand;

        _client.connect(Host);
        _client.sendColor(Agent.Color.R, Agent.Color.G, Agent.Color.B);
        _client.sendName(Agent.Name);
    }
    
    private void ReceiveCommand(object? sender, EventArgs args)
    {
        var response = Agent.Controller.React(new AgentState()
        {
            Ammo = 10,
            Direction = Direction.East,
            Energy = 100,
            Position = (10, 10),
            Score = 100,
        });
        
        _client.sendMsg(((char) response).ToString());
    }

    public void SendCommand(Command command)
    {
        Console.WriteLine($"Sending: {((char)command).ToString()}");
        _client.sendMsg(((char)command).ToString());
    }
}

public class Game
{
    private const string Host = "atari.icad.puc-rio.br";
    private readonly HandleClient _client = new ();
    
    private void ReceiveCommand(object? sender, EventArgs args)
    {
        var messages = (args as CommandEventArgs)?.cmd;

        if (messages is null) return;
        
        foreach (var message in messages)
        {
            Console.Write(message + ";");
        }
        Console.WriteLine("");
        
    }

    public Game()
    {
        HandleClient.CommandEvent += ReceiveCommand;

        _client.connect(Host);
        _client.sendColor(184, 51, 106);
        _client.sendName($"Tony Stark #{DateTime.Now.GetHashCode()}");
    }
    
    public void RunCommand(string command)
    {
        _client.sendMsg(command);
    }
}