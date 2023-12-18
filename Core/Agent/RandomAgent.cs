namespace Core.Agent;

public class RandomAgent : Agent
{
    protected override Command? Think(AgentStore store)
    {
        var random = new Random();
        var values = Enum.GetValues(typeof(Command));
        var randomCommand = (Command) values.GetValue(random.Next(values.Length))!;
        
        Thread.Sleep(100);

        return randomCommand;
    }
}