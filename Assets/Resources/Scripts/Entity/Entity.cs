using UnityEngine;
using System.Collections.Generic;

using pathPlanner;

public class Entity : MonoBehaviour
{

    [SerializeField]
    List<Vector2> path;

    float speed = 1f;

    List<Task> taskList = new List<Task>();
    Task currentTask = null;

    // Use this for initialization
    void Start()
    {

        path = new List<Vector2>();

        this.name = HTNPlanner.GenerateUniqueID("Entity");

        HTNWorldManager.instance.GetGlobalState().AddObject(this.name, this);
        HTNWorldManager.instance.GetGlobalState().Add("Type", "Entity", this.name);
        HTNWorldManager.instance.GetGlobalState().Add("At", this.name, ((Vector2)transform.position).ToString());
        HTNWorldManager.instance.GetGlobalState().Add("HasTasks", "False", this.name);

    }

    // Update is called once per frame
    void Update()
    {

        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; ++i)
            {
                Debug.DrawLine(new Vector3(path[i].x, path[i].y, -0.2f), new Vector3(path[i + 1].x, path[i + 1].y, -0.2f), Color.red);
            }

            transform.position = Vector2.MoveTowards(transform.position, path[0], speed * Time.deltaTime);

            if ((Vector2)transform.position == path[0])
            {
                HTNWorldManager.instance.GetGlobalState().Remove("At", this.name);
                path.RemoveAt(0);
                HTNWorldManager.instance.GetGlobalState().Add("At", this.name, ((Vector2)transform.position).ToString());
            }
        }
        else if(path.Count == 0 && currentTask != null && currentTask.name == "MoveTo")
        {
            AdvanceToNextTask();
        }

        if(currentTask == null && taskList.Count > 0 && HTNWorldManager.instance.GetGlobalState().HasRelation("HasTasks", "True", this.name))
        {
            AdvanceToNextTask();
        }

    }

    public void AddTask(Task task_)
    {
        if(taskList.Count == 0)
        {
            HTNWorldManager.instance.GetGlobalState().Add("HasTasks", "True", this.name);
            HTNWorldManager.instance.GetGlobalState().Remove("HasTasks", "False", this.name);
        }

        taskList.Add(task_);
    }

    public void MoveTo(Vector2 target_)
    {

        pathPlanner.PathplannerParameter param = new pathPlanner.PathplannerParameter();
        param.startX = (int)transform.position.x;
        param.startY = (int)transform.position.y;
        param.goalX = (int)target_.x;
        param.goalY = (int)target_.y;

        Array<object> newPath = ProcTerrain.pathfinder.FindPath(param);

        path.Clear();

        for (int i = 0; i < newPath.length; ++i)
        {
            Position node = (Position)newPath[i];
            path.Add(new Vector2((float)node.GetX(), (float)node.GetY()));
        }

    }

    public void SetTask(Task task_)
    {

        // some tasks like 'get' are not base level tasks. for now i am ignoring them
        if(!HTNWorldManager.instance.GetPlanner().GetAction(task_.name).IsOperator())
        {
            return;
        }

        currentTask = task_;

        switch (currentTask.name)
        {
            case "MoveTo":
                {
                    Vector2 target = HTNWorldManager.Convert(currentTask.associatedObjects["position"]);
                    MoveTo(target);
                    break;
                }
            case "CutDownTree":
                {
                    GameResource tree = HTNWorldManager.instance.GetGlobalState().GetObject(currentTask.associatedObjects["treeID"]) as GameResource;
                    Vector2 treePosition = tree.Position;

                    Tile containerTile = ProcTerrain.instance.GetTile((int)treePosition.x, (int)treePosition.y);
                    containerTile.SetResource(null);

                    Chunk containerChunk = ProcTerrain.instance.GetChunk(containerTile);
                    containerChunk.UpdateGraphicsOnTileIndex(((int)containerTile.Position.x) % Chunk.GetWidth(), ((int)containerTile.Position.y) % Chunk.GetHeight());

                    // need a better way of doing this but screw that, aint got time
                    HTNWorldManager.instance.GetPlanner().GetAction(currentTask.name).Execute(HTNWorldManager.instance.GetGlobalState(), task_, new List<Task>());

                    currentTask = null; // set currentTask to null so that a new task is issued. better than using recursion since we dont want any unwitted cyclic stuff
                    break;
                }
            case "PickApple":
                {
                    HTNWorldManager.instance.GetPlanner().GetAction(currentTask.name).Execute(HTNWorldManager.instance.GetGlobalState(), task_, new List<Task>());

                    currentTask = null; // set currentTask to null so that a new task is issued. better than using recursion since we dont want any unwitted cyclic stuff
                    break;
                }
            case "Mine":
                {
                    HTNWorldManager.instance.GetPlanner().GetAction(currentTask.name).Execute(HTNWorldManager.instance.GetGlobalState(), task_, new List<Task>());

                    currentTask = null; // set currentTask to null so that a new task is issued. better than using recursion since we dont want any unwitted cyclic stuff
                    break;
                }
            case "Create":
                {

                    HTNWorldManager.instance.GetPlanner().GetAction(currentTask.name).Execute(HTNWorldManager.instance.GetGlobalState(), task_, new List<Task>());
                    Debug.Log("ran create");
                    currentTask = null; // set currentTask to null so that a new task is issued. better than using recursion since we dont want any unwitted cyclic stuff
                    break;
                }
            default:
                Debug.LogError("DO NOT KNOW WHAT TO DO WITH TASK: " + currentTask.name);
                currentTask = null;
                break;

        }

    }

    public void AdvanceToNextTask()
    {
        if(taskList.Count > 0)
        {
            SetTask(taskList[0]);
            taskList.RemoveAt(0);
        }
        

        if(taskList.Count == 0)
        {
            HTNWorldManager.instance.GetGlobalState().Add("HasTasks", "False", this.name);
            HTNWorldManager.instance.GetGlobalState().Remove("HasTasks", "True", this.name);
        }
    }

    //public void GenerateNewPath()
    //{

    //    int targetX = Random.Range(0, ProcTerrain.instance.GetWidth());
    //    int targetY = Random.Range(0, ProcTerrain.instance.GetHeight());

    //    Tile currentTile = ProcTerrain.instance.GetTile((int)transform.position.x, (int)transform.position.y);
    //    Tile targetTile = ProcTerrain.instance.GetTile(targetX, targetY);

    //    Debug.Log("generating from: " + transform.position + " to: " + transform.position);
    //    Debug.Log("neighbourstruc: " + (currentTile.GetNode().GetNeighboursStructure() != null));
    //    Debug.Log("Generate = " + (ProcTerrain.pathfinder != null) + " _ " + (currentTile.GetNode() != null) + " _ " + (targetTile.GetNode() != null));

    //    pathPlanner.PathplannerParameter param = new pathPlanner.PathplannerParameter();
    //    param.startNode = currentTile.GetNode();
    //    param.goalNode = targetTile.GetNode();

    //    Array<object> newPath = ProcTerrain.pathfinder.FindPath(param);

    //    path.Clear();

    //    for (int i = 0; i < newPath.length; ++i)
    //    {
    //        Node node = (Node)newPath[i];
    //        path.Add(new Vector2((float)node.GetPosition().GetX(), (float)node.GetPosition().GetY()));
    //    }

    //    //Debug.Log("target = " + targetX + " " + targetY);
    //}
}
