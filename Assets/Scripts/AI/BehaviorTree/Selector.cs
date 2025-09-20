using System.Collections.Generic;

// Selector Node: tries children one by one, succeeds if any child succeeds
public class Selector : Node
{
    private List<Node> children; // Child nodes of this selector

    // Evaluate runs each child and decides overall state
    public override NodeState Evaluate()
    {
        foreach (Node node in children)
        {
            NodeState result = node.Evaluate(); // Run the child

            if (result == NodeState.Success)
            {
                state = NodeState.Success; // Selector succeeds immediately if any child succeeds
                return state;
            }
            if (result == NodeState.Running)
            {
                state = NodeState.Running; // Keep running if a child is still in progress
                return state;
            }
        }

        // All children failed
        state = NodeState.Failure;
        return state;
    }

    // Constructor: give the selector its children
    public Selector(List<Node> nodes) => children = nodes;
}
