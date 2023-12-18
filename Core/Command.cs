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
}

static class CommandExtensions
{
    public static Command? ToCommand(this char c)
    {
        return c switch
        {
            'w' => Command.MoveForward,
            's' => Command.MoveBackward,
            'a' => Command.TurnLeft,
            'd' => Command.TurnRight,
            't' => Command.PickUp,
            'e' => Command.Shoot,
            _ => null
        };
    }
}