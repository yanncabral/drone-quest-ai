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
        Start();
        // _client.sendRequestUserStatus();
    }
    
    public void Start()
    {
        _client.sendName(this.Agent.Name);
        _client.sendRequestScoreboard();
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

            if (command[0] != "u")
            {
                Console.Write($"Receiving command event: [");

                foreach (var c in command)
                {
                    Console.Write($"{c}, ");
                }
                Console.WriteLine($"\b\b]");
            }

            if (command[0] == "hello")
            {
                Console.WriteLine($"{command[1]} has entered the game");
            }
            
            if (command[0] == "goodbye")
            {
                Console.WriteLine($"{command[1]} has exited the game");
            }

            if (command[0] == "changename")
            {
                Console.WriteLine($"{command[1]} has changed name to {command[2]}");
            }
            
            if (command[0] == "d")
            {
                Console.WriteLine($"{command[1]} hit you");
            }
            
            if (command[0] == "h")
            {
                Console.WriteLine($"You hit {command[1]}");
            }
            
            if (command[0] == "u")
            {
                ScoreBoard = ParseScoreBoard(command);
            }
            
            if (command[0] == "s" && command.Length > 1)
            {
                var state = ParseState(command);
                Agent.State = state;
            }
            
            if (command[0] == "p" && Agent.State is not null)
            {
                Agent.State.Position = (int.Parse(command[1]), int.Parse(command[2]));
            }

            if (command[0] == "o" && Agent.State is not null)
            {
                var observation = command[1] switch
                {
                    "blocked" => Observation.Blocked,
                    "steps" => Observation.Steps,
                    "breeze" => Observation.Breeze,
                    "flash" => Observation.Flash,
                    "blueLight" => Observation.BlueLight,
                    "redLight" => Observation.RedLight,
                    "greenLight" => Observation.GreenLight,
                    "weakLight" => Observation.Weaklight,
                    "enemy" => Observation.Enemy,
                    "damage" => Observation.Damage,
                    "hit" => Observation.Hit,
                    _ => throw new ArgumentOutOfRangeException(),
                };

                Agent.State.Observations.Add(observation);
            }
        }
        else
        {
            Console.WriteLine($"HandleCommandEvent emmited: {args}");
        }
            
        
        // var response = Agent.Controller.React(null);
        //         
        // if (response is not null) 
        //     _client.sendMsg(((char) response).ToString());

        // _client.sendRequestUserStatus();
        // _client.sendRequestScoreboard();
    }

    public void SendCommand(char command)
    {
        Console.WriteLine($"Sending command: ${command}");
        _client.sendMsg(command.ToString());
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
}