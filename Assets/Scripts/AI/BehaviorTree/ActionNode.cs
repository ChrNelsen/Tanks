using System;

// Leaf node that performs an action
public class ActionNode : Node
{
    private Func<NodeState> action; // The action logic passed in

    // Method: When the tree ticks this node, run the action
    public override NodeState Evaluate()
    {
        state = action(); // Run the function and store the result
        return state;
    }

    // Constructor: provide a function that returns NodeState
    public ActionNode(Func<NodeState> action)
    {
        this.action = action;
    }
}