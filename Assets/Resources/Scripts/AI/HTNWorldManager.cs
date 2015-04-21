using UnityEngine;
using System.Collections.Generic;

using System;
using System.Reflection;

public class HTNWorldManager : MonoBehaviour
{

    public static HTNWorldManager instance;

    HTNState globalState;
    HTNPlanner planner;

    void Awake()
    {
        instance = this;

        globalState = new HTNState();
        planner = new HTNPlanner();
    }

    void Start()
    {

        Invoke("Print", 1);

        planner.AddAction(new HTNAction("Make", MakePrecondition, MakeCost, MakeExecution, false));
        planner.AddAction(new HTNAction("Get", GetPrecondition, GetCost, GetExecution, false));
        planner.AddAction(new HTNAction("Craft", CraftPrecondition, CraftCost, CraftExecution, false));
        planner.AddAction(new HTNAction("MoveTo", MoveToPrecondition, MoveToCost, MoveToExecution, true));
        planner.AddAction(new HTNAction("Pickup", PickupPrecondition, PickupCost, PickupExecution, true));
        planner.AddAction(new HTNAction("Create", CreatePrecondition, CreateCost, CreateExecution, true));
        planner.AddAction(new HTNAction("Destroy", DestroyPrecondition, DestroyCost, DestroyExecution, true));

        planner.AddAction(new HTNAction("CutDownTree", CutDownTreePrecondition, CutDownTreeCost, CutDownTreeExecution, true));
        planner.AddAction(new HTNAction("PickApple", PickApplePrecondition, PickAppleCost, PickAppleExecution, true));
        planner.AddAction(new HTNAction("Mine", MinePrecondition, MineCost, MineExecution, true));

    }

    void Print()
    {
        //globalState.Print();

        //string logID = HTNPlanner.GenerateUniqueID("Apple");
        //globalState.Add("At", logID, new Vector2(5, 5).ToString());
        //globalState.Add("Type", "Apple", logID);
    }

    void Update()
    {

        List<string> lazyAgents = globalState.GetRelations("HasTasks", "False");

        if (lazyAgents != null)
        {
            string agentID = lazyAgents[0];

            List<Task> finalPlan = planner.GetTaskPlan(globalState, new List<Task>(){
                new Task("Get", new Dictionary<string,string>(){{"entityID", agentID}, {"type", "Log"}}),
                new Task("Get", new Dictionary<string,string>() {{"entityID", agentID}, {"type", "Apple"}}),
                new Task("Get", new Dictionary<string,string>() {{"entityID", agentID}, {"type", "IronCrushed"}})
            });

            Entity ent = ((Entity)globalState.GetObject(agentID));

            foreach (Task task in finalPlan)
            {
                task.Print();
                ent.AddTask(task);
            }

            ent.AdvanceToNextTask();
        }
    }

    public HTNState GetGlobalState()
    {
        return globalState;
    }
    
    public HTNPlanner GetPlanner()
    {
        return planner;
    }

    /*
     * "At"
     * "Inventory"
     */

    /*
     * MakeTool
     * /Get ?Wood
     * //Find ?Wood
     * //MoveTo ?Wood
     * //Pickup ?Wood
     * 
     * /Get ?MetalBar
     * //Find ?MetalBar
     * //MoveTo ?MetalBar
     * //Pickup ?MetalBar
     * 
     * /CreateTool
     */

    // TODO: change deciduouswood to 'Wood'

    // hf

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <returns></returns>
    bool MakePrecondition(HTNState state_, Task task_)
    {

        return true;

    }

    float MakeCost(HTNState state_, Task task_)
    {
        return 0;
    }

    /// <summary>
    /// 'entityID'
    /// 'type'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    void MakeExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        switch (task_.associatedObjects["type"])
        {
            case "Tool":
                {
                    subList_.Add(new Task("Get", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "type", "Log" } }));
                    subList_.Add(new Task("Get", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "type", "MetalBar" } }));
                    subList_.Add(new Task("Craft", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "type", "Tool" } }));
                    break;
                }
            default:
                break;
        }
    }

    bool GetPrecondition(HTNState state_, Task task_)
    {

        return true;

    }

    float GetCost(HTNState state_, Task task_)
    {

        return 0;

    }

    /// <summary>
    /// 'entityID'
    /// 'type'
    /// 
    /// TODO: does not worry about no items existing.
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    void GetExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        // search through the relations to get a list of objects defined by 'task_.associatedObjects[1]', and select the first item
        string itemID = null;

        // get all objects of type
        List<string> items = state_.GetRelations("Type", task_.associatedObjects["type"]);

        if (items != null)
        {
            foreach (string i in items)
            {
                // if item is lying on the floor
                if (state_.HasRelation("At", i))
                {
                    itemID = i;
                }
            }
        }

        if (itemID != null)
        {
            // we have found an item of the correct type lying on the floor

            string ownerPosition = state_.GetRelations("At", itemID)[0];
            subList_.Add(new Task("MoveTo", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "position", ownerPosition } }));
            subList_.Add(new Task("Pickup", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "itemID", itemID } }));
        }
        else
        {
            // we have not found an item lying on the floor
            switch(task_.associatedObjects["type"])
            {
                case "Log":
                    {
                        List<string> treeList = state_.GetRelations("Type", "Tree");
                        string treeID = treeList[UnityEngine.Random.Range(0, treeList.Count)];
                        string treePosition = state_.GetRelations("At", treeID)[0];

                        subList_.Add(new Task("MoveTo", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "position", treePosition } }));
                        subList_.Add(new Task("CutDownTree", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "treeID", treeID } }));

                        break;
                    }
                case "Apple":
                    {
                        List<string> appleList = state_.GetRelations("Type", "TreeApple");
                        string treeID = appleList[UnityEngine.Random.Range(0, appleList.Count)];
                        string treePosition = state_.GetRelations("At", treeID)[0];

                        subList_.Add(new Task("MoveTo", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "position", treePosition } }));
                        subList_.Add(new Task("PickApple", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "treeID", treeID } }));

                        break;
                    }
                case "IronCrushed":
                    {
                        List<string> ironList = state_.GetRelations("Type", "IronOre");
                        string ironMineID = ironList[UnityEngine.Random.Range(0, ironList.Count)];
                        string ironPosition = state_.GetRelations("At", ironMineID)[0];

                        subList_.Add(new Task("MoveTo", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "position", ironPosition } }));
                        subList_.Add(new Task("Mine", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "mineID", ironMineID }, {"type", "ItronCrushed"} }));

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }
    }

    bool CraftPrecondition(HTNState state_, Task task_)
    {

        switch (task_.associatedObjects["itemID"])
        {
            case "Tool":
                {

                    List<string> inventoryWood = state_.Unify("Inventory", task_.associatedObjects["entityID"], "Type", "Wood");
                    List<string> inventoryMetalBar = state_.Unify("Inventory", task_.associatedObjects["entityID"], "Type", "MetalBar");

                    if(inventoryWood.Count > 0 && inventoryMetalBar.Count > 0)
                    {
                        return true;
                    }

                    break;
                }
            default:
                break;
        }

        return false;

    }

    float CraftCost(HTNState state_, Task task_)
    {

        return 0;

    }

    /// <summary>
    /// 'entityID'
    /// 'type'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <returns></returns>
    void CraftExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        switch (task_.associatedObjects["itemID"])
        {
            case "Tool":
                {
                    List<string> inventoryWood = state_.Unify("Inventory", task_.associatedObjects["entityID"], "Type", "Wood");
                    List<string> inventoryMetalBar = state_.Unify("Inventory", task_.associatedObjects["entityID"], "Type", "MetalBar");

                    string woodID = inventoryWood[0];
                    string metalBarID = inventoryMetalBar[0];

                    subList_.Add(new Task("Destroy", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "itemID", woodID } }));
                    subList_.Add(new Task("Destroy", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "itemID", metalBarID } }));
                    subList_.Add(new Task("Create", new Dictionary<string, string>() { { "entityID", task_.associatedObjects["entityID"] }, { "type", task_.associatedObjects["type"] } }));

                    break;
                }
            default:
                break;
        }

    }

    // operators

    bool MoveToPrecondition(HTNState state_, Task task_)
    {
        return true;
    }

    /// <summary>
    /// string = '(x, y)'
    /// </summary>
    /// <param name="position_"></param>
    /// <returns></returns>
    public static Vector2 Convert(string position_)
    {
        string[] strings = position_.Remove(position_.Length - 1, 1).Remove(0, 1).Split(',');

        float x = float.Parse(strings[0]);
        float y = float.Parse(strings[1]);

        return new Vector2(x, y);
    }

    float MoveToCost(HTNState state_, Task task_)
    {

        Entity entity = ((Entity)state_.GetObject(task_.associatedObjects["entityID"]));
        Vector2 targetPosition = Convert(task_.associatedObjects["position"]);

        return Vector2.Distance(entity.transform.position, targetPosition);

    }

    /// <summary>
    /// 'entityID'
    /// 'position'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <returns></returns>
    void MoveToExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        // an entity can only be at one place at a time, so only the first element will be used
        string currentEntityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];
        state_.UpdateRelation("At", task_.associatedObjects["entityID"], currentEntityPosition, false);
        state_.UpdateRelation("At", task_.associatedObjects["entityID"], task_.associatedObjects["position"], true);

    }

    bool PickupPrecondition(HTNState state_, Task task_)
    {

        string currentEntityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];
        string currentItemPosition = state_.GetRelations("At", task_.associatedObjects["itemID"])[0];

        return currentEntityPosition.Equals(currentEntityPosition);
    }

    float PickupCost(HTNState state_, Task task_)
    {

        return 0;

    }

    /// <summary>
    /// 'entityID'
    /// 'itemID'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <returns></returns>
    void PickupExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        string currentEntityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];
        string currentItemPosition = state_.GetRelations("At", task_.associatedObjects["itemID"])[0];

        state_.UpdateRelation("Inventory", task_.associatedObjects["entityID"], task_.associatedObjects["itemID"], true);
        state_.UpdateRelation("At", task_.associatedObjects["itemID"], currentItemPosition, false);

        // commented out because we assume that we will always be at the item when we pick it up

        //if(currentEntityPosition.Equals(currentItemPosition))
        //{
        //    state_.UpdateRelation("Inventory", task_.associatedObjects["entityID"], task_.associatedObjects["itemID"], true);
        //    state_.UpdateRelation("At", task_.associatedObjects["itemID"], currentItemPosition, false);
        //}

    }

    bool CreatePrecondition(HTNState state_, Task task_)
    {

        return true;

    }

    float CreateCost(HTNState state_, Task task_)
    {

        return 0;

    }

    /// <summary>
    /// 'entityID'
    /// 'type'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    void CreateExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        string itemID = HTNPlanner.GenerateUniqueID(task_.associatedObjects["type"]);

        state_.UpdateRelation("Type", task_.associatedObjects["type"], itemID, true);
        state_.UpdateRelation("Inventory", task_.associatedObjects["entityID"], itemID, true);
    }

    bool DestroyPrecondition(HTNState state_, Task task_)
    {

        return true;

    }

    float DestroyCost(HTNState state_, Task task_)
    {

        return 0;

    }

    /// <summary>
    /// 'entityID'
    /// 'itemID'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    void DestroyExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        string type = task_.associatedObjects["itemID"].Split('/')[0];
        state_.UpdateRelation("Type", type, task_.associatedObjects["itemID"], false);
        state_.UpdateRelation("Inventory", task_.associatedObjects["entityID"], task_.associatedObjects["itemID"], false);
    }

    bool CutDownTreePrecondition(HTNState state_, Task task_)
    {
        string treePosition = state_.GetRelations("At", task_.associatedObjects["treeID"])[0];
        string entityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];

        return treePosition.Equals(entityPosition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="Task_"></param>
    /// <returns></returns>
    float CutDownTreeCost(HTNState state_, Task Task_)
    {
        return 0;
    }

    /// <summary>
    /// 'entityID'
    /// 'treeID'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    /// <returns></returns>
    void CutDownTreeExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        subList_.Add(new Task("Create", new Dictionary<string, string>() { { "type", "Log" }, { "entityID", task_.associatedObjects["entityID"] } }));
        state_.Remove("At", task_.associatedObjects["treeID"]);
        state_.Remove("Type", "Tree", task_.associatedObjects["treeID"]);
    }

    bool PickApplePrecondition(HTNState state_, Task task_)
    {
        string treePosition = state_.GetRelations("At", task_.associatedObjects["treeID"])[0];
        string entityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];

        return treePosition.Equals(entityPosition);
    }

    float PickAppleCost(HTNState state_, Task Task_)
    {
        return 0;
    }

    /// <summary>
    /// 'entityID'
    /// 'treeID'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    /// <returns></returns>
    void PickAppleExecution(HTNState state_, Task task_, List<Task> subList_)
    {
        subList_.Add(new Task("Create", new Dictionary<string, string>() { { "type", "Apple" }, { "entityID", task_.associatedObjects["entityID"] } }));
    }

    bool MinePrecondition(HTNState state_, Task task_)
    {

        string minePosition = state_.GetRelations("At", task_.associatedObjects["mineID"])[0];
        string entityPosition = state_.GetRelations("At", task_.associatedObjects["entityID"])[0];

        return minePosition.Equals(entityPosition);
    }

    float MineCost(HTNState state_, Task task_)
    {
        return 0;
    }

    /// <summary>
    /// 'entityID'
    /// 'type'
    /// </summary>
    /// <param name="state_"></param>
    /// <param name="task_"></param>
    /// <param name="subList_"></param>
    void MineExecution(HTNState state_, Task task_, List<Task> subList_)
    {

        subList_.Add(new Task("Create", new Dictionary<string, string>() { { "type", task_.associatedObjects["type"] }, { "entityID", task_.associatedObjects["entityID"] } }));

    }

}
