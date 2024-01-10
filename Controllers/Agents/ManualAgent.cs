using Core;
using Core.Agent;

namespace Controllers.Agents;

public class ManualAgent : Agent
{
    public ManualAgent()
    {
        var sender = new Thread(Loop);

        sender.Start();
        sender.Join();
    }

    private void Loop()
    {
        while (true)
        {
            var i = Console.ReadLine();

            if (i is not null && i.Length > 0)
            {
                SendCommand((Command) i[0]);
            }
            else
            {
                return;
            }
        }
    }

    protected override Command? Think(AgentStore store)
    {
        return null;
    }
}