using UnityEngine;
using System.Collections.Generic;

using pathPlanner;

public class Entity : MonoBehaviour
{

    [SerializeField]
    List<Vector2> path;

    float speed = 1f;

    // Use this for initialization
    void Start()
    {

        path = new List<Vector2>();

    }

    // Update is called once per frame
    void Update()
    {

        if (path.Count == 0)
        {
            GenerateNewPath();
        }
        else
        {
            for (int i = 0; i < path.Count - 1; ++i)
            {
                Debug.DrawLine(new Vector3(path[i].x, path[i].y, -0.2f), new Vector3(path[i + 1].x, path[i + 1].y, -0.2f), Color.red);
            }

            transform.position = Vector2.MoveTowards(transform.position, path[0], speed * Time.deltaTime);

            if ((Vector2)transform.position == path[0])
            {
                path.RemoveAt(0);
            }
        }

    }

    public void GenerateNewPath()
    {

        int targetX = Random.Range(0, ProcTerrain.instance.GetWidth());
        int targetY = Random.Range(0, ProcTerrain.instance.GetHeight());

        Tile currentTile = ProcTerrain.instance.GetTile((int)transform.position.x, (int)transform.position.y);
        Tile targetTile = ProcTerrain.instance.GetTile(targetX, targetY);

        Debug.Log("generating from: " + transform.position + " to: " + transform.position);
        Debug.Log("neighbourstruc: " + (currentTile.GetNode().GetNeighboursStructure() != null));
        Debug.Log("Generate = " + (ProcTerrain.pathfinder != null) + " _ " + (currentTile.GetNode() != null) + " _ " + (targetTile.GetNode() != null));

        pathPlanner.PathplannerParameter param = new pathPlanner.PathplannerParameter();
        param.startNode = currentTile.GetNode();
        param.goalNode = targetTile.GetNode();

        Array<object> newPath = ProcTerrain.pathfinder.FindPath(param);

        path.Clear();

        for (int i = 0; i < newPath.length; ++i)
        {
            Node node = (Node)newPath[i];
            path.Add(new Vector2((float)node.GetPosition().GetX(), (float)node.GetPosition().GetY()));
        }

        //Debug.Log("target = " + targetX + " " + targetY);
    }
}
