using System.Text.RegularExpressions;
using Core.Agent;

namespace Core;

public record CommandHandler(string CommandName, string[] Message)
{
    public static CommandHandler? Factory(string[] message)
    {
        var dispatchers = new CommandHandler[]
        {
            // Chat events
            new PlayerHasEnteredHandler(message),
            new PlayerHasExitedHandler(message),
            new PlayerHasRenamedHandler(message),
            new PlayerSaysHandler(message),

            // Game events
            new AgentWasHitHandler(message),
            new YouHitTheEnemyHandler(message),
            new ObservationHandler(message),

            // Agent update
            new AgentStateChangedHandler(message),
            new PositionChangedHandler(message),

            // Game update
            new ScoreboardUpdateHandler(message),
            new GameStatusUpdateHandler(message),
        };

        return Array.Find(dispatchers, dispatcher => dispatcher.CommandName == message[0]);
    }
}

public record GameStatusUpdateHandler(string[] Message) : CommandHandler("g", Message), IStoreDispatcher
{
    private GameStatus ParseStatus()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return Message[1] switch
        {
            "Game" => GameStatus.Game,
            "Gameover" => GameStatus.Gameover,
            "Ready" => GameStatus.Ready,
            _ => throw new ArgumentException("Invalid game status"), // TODO: Descobrir valores de outros estados
        };
    }

    public void Dispatch(AgentStore store) => store.GameState.Status = ParseStatus();
}

public record PlayerHasEnteredHandler(string[] Message) : CommandHandler("hello", Message), IChatDispatcher
{
    public string Say()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return $"{Message[1]} entrou no jogo!";
    }
}

public record PlayerHasExitedHandler(string[] Message) : CommandHandler("goodbye", Message), IChatDispatcher
{
    public string Say()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return $"{Message[1]} saiu do jogo!";
    }
}

public record PlayerSaysHandler(string[] Message) : CommandHandler("say", Message), IChatDispatcher
{
    public string Say()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return $"{Message[1]}: {Message[2]}";
    }
}

public record PlayerHasRenamedHandler(string[] Message) : CommandHandler("changename", Message), IChatDispatcher
{
    public string Say()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return $"{Message[1]} mudou seu nome para {Message[2]}!";
    }
}

public record PositionChangedHandler(string[] Message) : CommandHandler("p", Message), IStoreDispatcher
{
    public void Dispatch(AgentStore store) => store.AgentState.Position = ParsePosition();

    private Position ParsePosition()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        return new Position(int.Parse(Message[1]), int.Parse(Message[2]));
    }
}

public record AgentWasHitHandler(string[] Message) : CommandHandler("d", Message)
{
    public void Dispatch(AgentStore store)
    {
        // TODO: Implementar
    }
}

public record YouHitTheEnemyHandler(string[] Message) : CommandHandler("h", Message)
{
    public void Dispatch(AgentStore store)
    {
        // TODO: Implementar
    }
}

public record ObservationHandler(string[] Message) : CommandHandler("o", Message),
    IStoreDispatcher
{
    public void Dispatch(AgentStore store) => store.AgentState.Observations = ParseObservations();

    private HashSet<Observation> ParseObservations()
    {
        var sensors = Message[1].Split(",");
        var observations = new HashSet<Observation>();
        foreach (var sensor in sensors)
        {
            Observation? observation = sensor switch
            {
                "blocked" => Observation.Blocked,
                "steps" => Observation.Steps,
                "breeze" => Observation.Breeze,
                "flash" => Observation.Flash,
                "blueLight" => Observation.BlueLight,
                "redLight" => Observation.RedLight,
                "greenLight" => Observation.GreenLight,
                "weakLight" => Observation.Weaklight,
                _ => null
            };

            if (observation is not null)
                observations.Add(observation.Value);
        }

        return observations;
    }
}

public record AgentStateChangedHandler(string[] Message) : CommandHandler("s", Message), IStoreDispatcher
{
    public void Dispatch(AgentStore store) => store.AgentState = ParseState();

    private AgentState ParseState()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        var state = new AgentState()
        {
            Position = new Position(int.Parse(Message[1]), int.Parse(Message[2])),
            Direction = Message[3] switch
            {
                "north" => Direction.North,
                "east" => Direction.East,
                "west" => Direction.West,
                "south" => Direction.South,
                _ => throw new ArgumentException("Invalid direction"),
            },
            Dead = Message[4] == "dead",
            Score = int.Parse(Message[5]),
            Energy = int.Parse(Message[6]),
        };

        return state;
    }
}

public record ScoreboardUpdateHandler(string[] Message) : CommandHandler("u", Message), IStoreDispatcher
{
    public void Dispatch(AgentStore store) => store.GameState.Scoreboard = ParseScoreboard();

    private List<Player> ParseScoreboard()
    {
        if (Message == null) throw new ArgumentNullException(nameof(Message));

        var players = new List<Player>();

        if (Message.Length <= 1)
            return players;

        const string playerInfoPattern =
            @"^(.*?)#(connected|offline)#(-?\d+)#(\d+)#Color \[A=255, R=(\d+), G=(\d+), B=(\d+)\]$";

        for (var i = 1; i < Message.Length; i++)
        {
            var match = Regex.Match(Message[i], playerInfoPattern);
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

        return players;
    }
}