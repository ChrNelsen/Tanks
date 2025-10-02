using System;

// Leaf node that checks a condition
public class ConditionNode : Node
{
    private Func<bool> condition; // Condition logic passed in

    // Evaluate returns Success if condition is true, else Failure
    public override NodeState Evaluate()
    {
        state = condition() ? NodeState.Success : NodeState.Failure;
        return state;
    }

    // Constructor: provide a function that returns true/false
    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }
}