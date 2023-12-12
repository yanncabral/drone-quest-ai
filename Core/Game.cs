using INF1771_GameClient.Socket;

namespace Core;

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
        _client.connect(Host);


        HandleClient.CommandEvent += ReceiveCommand;
        
        _client.sendColor(184, 51, 106);
        _client.sendName("Yann & Zepa");
        
        _client.sendRequestObservation();
    }
    
    public void RunCommand(string command)
    {
        _client.sendMsg(command);
    }
}