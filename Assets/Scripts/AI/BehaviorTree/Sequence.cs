using System.Collections.Generic;

// All children must succeed for the sequence to succeed.
public class Sequence : Node
{
    // Class Variable
    private List<Node> children;

    // Method returns either Success, Failure, Running
    public override NodeState Evaluate()
    {
        bool anyChildRunning = false;

        foreach (Node node in children)
        {
            NodeState result = node.Evaluate();

            if (result == NodeState.Failure)
            {
                state = NodeState.Failure;
                return state;
            }

            if (result == NodeState.Running)
            {
                anyChildRunning = true;
            }
        }

        state = anyChildRunning ? NodeState.Running : NodeState.Success;
        return state;
    }

    // Constructor
    public Sequence(List<Node> nodes)
    {
        children = nodes; // store the children in this sequence
    }
}
