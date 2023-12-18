using Core.Agent;

namespace Core;

public class GameState
{
    public GameStatus Status { get; set; }
    public List<Player> Scoreboard { get; set; } = new ();
}