namespace Core.Agent;

public class Debouncer
{
    public Debouncer(TimeSpan duration)
    {
        _duration = duration;
    }

    private readonly TimeSpan _duration;
    private Timer? _timer;

    public void Call(Action action)
    {
        _timer?.Dispose(); // Cancela o timer anterior se ele existir
        _timer = new Timer(_ => action(), null, _duration, Timeout.InfiniteTimeSpan);
    }
}

public abstract class Agent
{
    private AgentStore Store { get; set; } = new ();
    private GameClient? Client { get; set; }
    protected abstract Command? Think(AgentStore store);

    private readonly Debouncer _debouncer = new(TimeSpan.FromMilliseconds(300));

    public void Start()
    {
        Client = new GameClient();
        Client.OnStoreChangedEvent += OnStoreChanged;
        Client.SendCommand('q');
    }

    public void Stop() => Client?.Disconnect();

    private void OnStoreChanged(IStoreDispatcher dispatcher)
    {
        dispatcher.Dispatch(Store);
        
        if (Store.AgentState.Dead) return;
        _debouncer.Call(() =>
        {
            Console.WriteLine($"{dispatcher.GetType().Name} >> {Store}");
            var command = Think(Store);
            if (command is null) return;
            SendCommand(command.Value);
        });
    }

    protected void SendCommand(Command command)
    {
        Store.LastCommand = command;
        if (Client is null) throw new Exception("Did you forget to call Start()?");
        Client.SendCommand((char) command);
    }

    public override string ToString() => Store.ToString();
}