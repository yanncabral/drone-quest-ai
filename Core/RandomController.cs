namespace Core;

public class RandomController : IController
{
    public Command? React(AgentState state)
    {
        var random = new Random();
        var values = Enum.GetValues(typeof(Command));
        var randomCommand = (Command) values.GetValue(random.Next(values.Length))!;

        Thread.Sleep(1000);
        
        Console.WriteLine($"Reacting {state} with {randomCommand}");

        return randomCommand;
    }
}