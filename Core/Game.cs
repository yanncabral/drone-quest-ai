using System.Text.RegularExpressions;
using INF1771_GameClient.Socket;

namespace Core;

public sealed class GameClient
{
    private const string Host = "atari.icad.puc-rio.br";
    private readonly HandleClient _client = new ();

    private Agent Agent { get; set; } = new ();

    public Player[] ScoreBoard { get; set; } = new Player[] { };

    public GameClient(Agent agent)
    {
        HandleClient.CommandEvent += HandleCommandEvent;
        HandleClient.ChangeStatusEvent += HandleStatusEvent;
        Agent = agent;
        _client.connect(Host);
        agent.Client = this;
        _client.sendRequestUserStatus();
    }

    private static void HandleStatusEvent(object? sender, EventArgs args)
    {
        Console.WriteLine($"Receiving status event: {args}");
    }
    
    private void HandleCommandEvent(object? sender, EventArgs args)
    {
        if (args is CommandEventArgs eventArgs)
        {
            var command = eventArgs.cmd!;

            if (command[0] == "s" && command.Length > 1)
            {
                var state = ParseState(command);
                Agent.State = state;
                var response = Agent.Controller.React(state);

                _client.sendMsg(((char) response).ToString());
            }

            if (command[0] == "u")
            {
                ScoreBoard = ParseScoreBoard(command);
            }

            if (command[0] == "p" && Agent.State is not null)
            {
                Agent.State.Position = (int.Parse(command[1]), int.Parse(command[2]));
            }
        }

        _client.sendRequestUserStatus();
        _client.sendRequestScoreboard();
    }

    private static Player[] ParseScoreBoard(string[] command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        
        if (command.Length <= 1)
            return Array.Empty<Player>();

        var players = new List<Player>();
        const string playerInfoPattern = @"^(.*?)#(connected|offline)#(-?\d+)#(\d+)#Color \[A=255, R=(\d+), G=(\d+), B=(\d+)\]$";

        for (var i = 1; i < command.Length; i++)
        {
            var match = Regex.Match(command[i], playerInfoPattern);
            if (!match.Success) continue;
            
            var name = match.Groups[1].Value;
            var isConnected = match.Groups[2].Value == "connected";
            var score = int.Parse(match.Groups[3].Value);
            var energy = int.Parse(match.Groups[4].Value);
            var r = int.Parse(match.Groups[5].Value);
            var g = int.Parse(match.Groups[6].Value);
            var b = int.Parse(match.Groups[7].Value);

            players.Add(new Player(name, isConnected, score, energy, new Color(r, g, b)));
        }

        return players.ToArray();
    }

    private AgentState ParseState(string[] command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        
        var state = new AgentState()
        {
            Position = (int.Parse(command[1]), int.Parse(command[2])),
            Direction = command[3] switch
            {
                "north" => Direction.North,
                "east" => Direction.East,
                "west" => Direction.West,
                "south" => Direction.South,
                _ => throw new ArgumentOutOfRangeException(),
            },
            // State = command[4] (Game or death)
            Score = int.Parse(command[5]),
            Energy = int.Parse(command[6]),
        };

        return state;
    }

    public void SendCommand(Command command)
    {
        _client.sendMsg(((char)command).ToString());
    }
}