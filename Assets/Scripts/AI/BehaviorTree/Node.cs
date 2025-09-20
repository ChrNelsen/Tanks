using UnityEngine;

// Base class for all Behavior Tree nodes
public abstract class Node                      // Every custom node (like Sequence, Selector, Action, Condition) will inherit from this
{
    protected NodeState state;                  // Stores the current state of this node (Success, Failure, or Running)
    public NodeState State => state;            // Getter public way to return State

    // Every node must have an Evaluate method
    public abstract NodeState Evaluate();       // When called, it should return either Running, Success, or Failure
}