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

        Debug.Log("Generate = " + (ProcTerrain.pathfinder != null) + " _ " + (currentTile.GetNode() != null) + " _ " + (targetTile.GetNode() != null));

        Array<object> newPath = ProcTerrain.pathfinder.FindPath(currentTile.GetNode(), targetTile.GetNode(),
            (x, y) => 
            {
                return Mathf.Sqrt(Mathf.Pow((float)(x.x - y.x), 2) + Mathf.Pow((float)(x.y - y.y), 2));
            }
        );

        path.Clear();

        for (int i = 0; i < newPath.length; ++i)
        {
            Node node = (Node)newPath[i];
            path.Add(new Vector2((float)node.get_x(), (float)node.get_y()));
        }

        //Debug.Log("target = " + targetX + " " + targetY);
        Debug.Log("current = " + currentTile.transform.position + " target = " + targetTile.transform.position);
    }
}
