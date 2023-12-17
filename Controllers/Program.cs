using Core;

void Loop(Agent agent)
{
    while (true)
    {
        var i = Console.ReadLine();

        if (i is not null)
        {
            agent.SendCommand((Command) i[0]);
        }
        else
        {
            return;
        }
    }
}


// var agent = new Agent(new RandomController());
var agent = new Agent(new ManualController());

Loop(agent);

// var sender = new Thread(SendData);
//
// sender.Start();
// sender.Join();



