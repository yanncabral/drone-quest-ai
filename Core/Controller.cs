namespace Core;

public enum Command
{
    /// <summary>
    /// Move the player forward
    /// </summary>
    MoveForward = 'w',
    /// <summary>
    /// Move the player backward
    /// </summary>
    MoveBackward = 's',
    /// <summary>
    /// Turn the player left (90°)
    /// </summary>
    TurnLeft = 'a',
    /// <summary>
    /// Turn the player right (90°)
    /// </summary>
    TurnRight = 'd',
    /// <summary>
    /// Pick up an object if there is one in the player's position
    /// </summary>
    PickUp = 't',
    /// <summary>
    /// To shoot the ammunition in a straight line in the direction the agent is looking - The ammunition is unlimited and has a range until it collides with a blocked position
    /// </summary>
    Shoot = 'e',
    /// <summary>
    /// Receive information about the world around you.
    /// </summary>
    Observe = 'o'
}

public interface IController
{
    public Command React(AgentState state);
}

public class RandomController : IController
{
    public Command React(AgentState state)
    {
        var random = new Random();
        var values = Enum.GetValues(typeof(Command));
        var randomCommand = (Command) values.GetValue(random.Next(values.Length))!;
        
        Console.WriteLine($"Reacting with {randomCommand}");

        return randomCommand;
    }
}

public class ManualController : IController
{
    public Command React(AgentState state)
    {
        // Returns command from user input
        var input = Console.ReadKey().KeyChar;
        var command = (Command) input;

        return command;
    }
}

