namespace Core.Agent;

public class DecisionNode
{
    public DecisionNode? LeftChild { get; set; }
    public DecisionNode? RightChild { get; set; }
    public Command? Command { get; set; }
    public int ScoreThreshold { get; set; }

    public DecisionNode FindClosestNode(AgentState state)
    {
        if (LeftChild == null && RightChild == null)
        {
            return this;
        }

        return state.Score <= ScoreThreshold 
            ? LeftChild?.FindClosestNode(state) ?? this
            : RightChild?.FindClosestNode(state) ?? this;
    }
}


public class DecisionTree
{
    private DecisionNode Root { get; set; }

    public DecisionTree(DecisionNode root)
    {
        Root = root;
    }

    public Command? Predict(AgentState state)
    {
        var node = Root.FindClosestNode(state);
        return node.Command ?? GetAlternativeCommand(state);
    }

    public void UpdateTree(AgentState state, AgentState lastState, bool actionWasSuccessful)
    {
        var currentNode = Root.FindClosestNode(state);
        var scoreDifference = state.Score - lastState.Score;

        if (!actionWasSuccessful && scoreDifference <= 0)
        {
            if (currentNode.LeftChild is null)
            {
                currentNode.LeftChild = new DecisionNode()
                {
                    Command = GetAlternativeCommand(state),
                };
            }
            else if (currentNode.RightChild is null)
            {
                currentNode.RightChild = new DecisionNode()
                {
                    Command = GetAlternativeCommand(state),
                };
            }
        }

        // Atualize o ScoreThreshold para refletir a experiência
        currentNode.ScoreThreshold = state.Score;
    }

    private static Command GetAlternativeCommand(AgentState state)
    {
        var random = new Random();

        // Exemplo de lógica refinada
        if (state.Observations.Contains(Observation.Blocked))
        {
            // Se bloqueado, vire ou recue
            return random.NextDouble() > 0.5 ? Command.TurnLeft : Command.TurnRight;
        }

        // Se o agente encontrou um item, tente pegá-lo
        if (state.Observations.Contains(Observation.BlueLight) || state.Observations.Contains(Observation.RedLight) ||
            state.Observations.Contains(Observation.GreenLight) || state.Observations.Contains(Observation.Weaklight))
        {
            return Command.PickUp;
        }

        // Se o agente detectou um inimigo próximo e tem munição suficiente
        if (state.Observations.Contains(Observation.Enemy) && state.Energy > 50)
        {
            // Se há um inimigo e energia suficiente, ataque ou recue
            return random.NextDouble() > 0.5 ? Command.Shoot : Command.MoveBackward;
        }

        // Se não há informações suficientes ou o agente está explorando
        var commands = new[] { Command.TurnLeft, Command.TurnRight, Command.MoveForward };
        return commands[random.Next(commands.Length)];
    }
}


public class DecisionTreeAgent : Agent
{
    private readonly DecisionTree _tree = new (new DecisionNode());
    
    private AgentState? LastState { get; set; }

    protected override Command? Think(AgentStore store)
    {
        var state = store.AgentState;
        var command = _tree.Predict(state);

        if (LastState is null)
        {
            LastState = state;
            return command;
        }
        
        var actionWasSuccessful = state.Score > LastState.Score;
        _tree.UpdateTree(state, LastState, actionWasSuccessful);

        return command;
    }
}

