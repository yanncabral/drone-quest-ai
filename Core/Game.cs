using INF1771_GameClient.Socket;

namespace Core;

public sealed class GameClient
{
    private const string Host = "atari.icad.puc-rio.br";
    private readonly HandleClient _client = new ();
    
    public delegate void StoreStateEvent(IStoreDispatcher dispatcher);
    public event StoreStateEvent? OnStoreChangedEvent;
    public delegate void ChatEvent(IChatDispatcher dispatcher);
    public event ChatEvent OnChatEvent;
    
    private static void OnChat(IChatDispatcher dispatcher)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(dispatcher.Say());
        Console.ForegroundColor = ConsoleColor.White;
    }


    public GameClient()
    {
        HandleClient.CommandEvent += HandleCommandEvent;
        OnChatEvent += OnChat;
        _client.connect(Host);
    }

    public void SetName(string name)
    {
        _client.sendName(name);
    }
    
    public void Disconnect()
    {
        _client.disconnect();
    }
    
    private void HandleCommandEvent(object? sender, EventArgs args)
    {
        if (args is not CommandEventArgs eventArgs) return;
        
        var message = eventArgs.cmd!;

        var handler = CommandHandler.Factory(message);

        switch (handler)
        {
            case IStoreDispatcher dispatcher:
                OnStoreChangedEvent?.Invoke(dispatcher);
                break;
            case IChatDispatcher chatDispatcher:
                OnChatEvent.Invoke(chatDispatcher);
                break;
        }
    }

    public void SendCommand(char command)
    {
        _client.sendMsg(command.ToString());
        _client.sendMsg("q");
        _client.sendMsg("o");
    }

    
}