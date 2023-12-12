using Core;

var game = new Game();

void SendData()
{
    while (true)
    {
        var i = Console.ReadLine();

        if (i is not null)
        {
            game.RunCommand(i);
        }
        else
        {
            return;
        }
    }
}

var sender = new Thread(SendData);

sender.Start();
sender.Join();

