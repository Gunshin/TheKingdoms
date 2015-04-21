using UnityEngine;
using System.Collections.Generic;



/// <summary>
/// Every action requires a precondition, cost and postcondition. This class is just a binding of the 3. This class is then grouped with other instances to form the HTNNode
/// which affects the HTNState.
/// </summary>
public class HTNAction
{

    string name;
    bool funcIsOperator = false;

    //public HTNAction(string name_)
    //{
    //    name = name_;
    //}

    //string name;
    System.Func<HTNState, Task, bool> precondition;
    System.Func<HTNState, Task, float> cost;
    System.Action<HTNState, Task, List<Task>> postcondition;

    public HTNAction(string name_, System.Func<HTNState, Task, bool> precondition_, System.Func<HTNState, Task, float> cost_, System.Action<HTNState, Task, List<Task>> postcondition_, bool funcIsOperator_)
    {
        name = name_;
        precondition = precondition_;
        cost = cost_;
        postcondition = postcondition_;
        funcIsOperator = funcIsOperator_;
    }

    public string GetName()
    {
        return name;
    }

    public bool CanExecute(HTNState state_, Task task_)
    {
        return precondition(state_, task_);
    }

    public float GetCost(HTNState state_, Task task_)
    {
        return cost(state_, task_);
    }

    /// <summary>
    /// This function currently executes with the assumption that the preconditions have been met. Does not modify the state,
    /// but returns a list of relations to be applied to the state. Allows for us to get the post-conditions without actually
    /// modifying the state.
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="modifications_"> All modifications to be applied to the state </param>
    public void Execute(HTNState state_, Task currentTask_, List<Task> subtasks_)
    {
        postcondition(state_, currentTask_, subtasks_);
    }

    public bool IsOperator()
    {
        return funcIsOperator;
    }

}
