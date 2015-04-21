using UnityEngine;
using System.Collections.Generic;

public class Task
{
    public string name;
    public Dictionary<string, string> associatedObjects;

    public Task(string name_, Dictionary<string, string> objects_)
    {
        name = name_;
        associatedObjects = objects_;
    }

    public void Print()
    {
        string assString = "";

        foreach (KeyValuePair<string, string> k in associatedObjects)
        {
            assString += "Key: " + k.Key + " Value: " + k.Value + " ";
        }

        Debug.Log("Task: " + name + " has associatedObjects: " + assString);
    }
}

public class HTNPlanner
{

    public class HTNPlannerNode
    {
        public HTNState state;
        public HTNAction action;

        public float value;

        public HTNPlannerNode parent = null;

        public HTNPlannerNode(HTNState state_, HTNAction action_, float value_)
        {
            state = state_;
            action = action_;
            value = value_;
        }
    }

    /// <summary>
    /// Key is the name of the action
    /// </summary>
    Dictionary<string, HTNAction> actions = new Dictionary<string, HTNAction>();

    public HTNPlanner()
    {

    }

    public void AddAction(HTNAction action_)
    {
        actions.Add(action_.GetName(), action_);
    }

    public HTNAction GetAction(string name_)
    {
        if(actions.ContainsKey(name_))
        {
            return actions[name_];
        }

        return null;
    }

    //public List<HTNAction> GetAllPossibleActions(HTNState state_)
    //{

    //    List<HTNAction> finalList = new List<HTNAction>();

    //    foreach(List<HTNAction> actionList in actions.Values)
    //    {
    //        foreach(HTNAction action in actionList)
    //        {
    //            if(action.CanExecute(state_))
    //            {
    //                finalList.Add(action);
    //            }
    //        }
    //    }

    //    return finalList;
    //}

    /// <summary>
    /// Gets all actions with the desired variable and type.
    /// 
    /// EG. variable = "Add", type = "Wood"
    /// </summary>
    /// <param name="variable_"></param>
    /// <param name="type_"></param>
    /// <returns></returns>
    //public List<HTNAction> GetActions(HTNState state_, string variable_, string type_)
    //{
    //    if(actions.ContainsKey(type_))
    //    {
    //        List<HTNAction> plausibleActions = actions[type_];

    //        List<HTNAction> finalList = new List<HTNAction>();
    //        foreach(HTNAction a in plausibleActions) // loop through all possible actions
    //        {
    //            if(a.CanExecute(state_)) // see if they can be executed on the state
    //            {
    //                List<Triple<string, string, string>> stateTransforms = a.Execute(state_); // get the transformations of this action on the state
    //                foreach(Triple<string, string, string> triple in stateTransforms)
    //                {
    //                    if(triple.valueA.Equals(variable_) && triple.valueB.Equals(type_)) // look for a transformation that matches the params andadd it to the final list
    //                    {
    //                        finalList.Add(a);
    //                    }
    //                }
    //            }
    //        }

    //        return finalList;
    //    }

    //    return null;
    //}

    public List<Task> GetTaskPlan(HTNState state_, List<Task> tasks_)
    {

        List<Task> finalPlan = new List<Task>(tasks_);
        HTNState copiedState = state_.DeepCopy();

        // we go through the tasks backwards so we can remove tasks
        for (int i = 0; i < finalPlan.Count; ++i)
        {

            Task currentTask = finalPlan[i];

            if (actions[currentTask.name].CanExecute(copiedState, currentTask))
            {

                List<Task> subtasks = new List<Task>();

                actions[currentTask.name].Execute(copiedState, currentTask, subtasks);

                if (subtasks.Count > 0)
                {
                    finalPlan.InsertRange(i + 1, subtasks);
                }

            }

        }

        return finalPlan;
    }

    //public List<Task> GetTaskPlan(HTNState state_, System.Func<HTNState, float> heuristic_, float maxIterations_, List<Task> tasks_)
    //{
    //    float iteration = 0;

    //    bool foundPlan = false;

    //    List<HTNPlannerNode> open = new List<HTNPlannerNode>();

    //    open.Add(new HTNPlannerNode(state_, null, 0)); // add initial state

    //    while(!foundPlan && open.Count > 0 && iteration < maxIterations_)
    //    {
    //        iteration++;

    //        HTNPlannerNode current = open[0];
    //        open.RemoveAt(0);

    //        List<HTNAction> possibleActions = GetAllPossibleActions(current.state);

    //        foreach(HTNAction action in possibleActions)
    //        {
    //            HTNState clonedState = current.state.DeepCopy();

    //            List<Task> subtasks = new List<Task>();

    //            action.Execute(clonedState, subtasks);


    //            foreach(Triple<string, string, string> relation in relationTransforms)
    //            {
    //                clonedState.UpdateRelation(relation);
    //            }

    //            float value = current.value + action.GetCost(current.state) + heuristic_(clonedState);

    //            HTNPlannerNode neighbour = new HTNPlannerNode(clonedState, action, value);
    //            neighbour.parent = current;

    //        }
    //    }

    //}

    //public void Insert(List<HTNPlannerNode> nodeList_, HTNPlannerNode node_)
    //{

    //    for(int i = 0; i < nodeList_.Count; ++i)
    //    {
    //        if(nodeList_[i].value > node_.value)
    //        {
    //            nodeList_.Insert(i, node_);
    //            return;
    //        }
    //    }

    //    nodeList_.Add(node_);
    //}

    static int idCount = 0;
    public static string GenerateUniqueID(string base_)
    {
        return base_ + "/" + idCount++;
    }
}
