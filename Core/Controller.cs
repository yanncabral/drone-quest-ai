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
    Observe = 'o',

    GameStatus = 'g', //sendRequestGameStatus(); - receber o status do jogo (estado, tempo atual)
    UserStatus = 'q', //sendRequestUserStatus(); - receber status do usuário (posição, estado do agente, pontos e energia)
    SendRequestPosition = 'p', // sendRequestPosition(); - receber posição do agente
    SendRequestScoreboard = 'u', // (); - lista de usuários logados e pontos
    //quit sendGoodbye(); - desconectar do jogo
}

public interface IController
{
    public Command? React(AgentState state);
}