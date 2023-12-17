namespace Core;

public class ManualController : IController
{
    public Command? React(AgentState state)
    {
        Console.WriteLine($"Reacting to {state}");
        // Returns command from user input
        var input = Console.ReadLine();
        if (input is null || input.Length == 0) return null;
        
        var command = (Command) input[0];

        return command;
    }
}