using UnityEngine;
using System.Collections;

using pathPlanner;

public class Entity : MonoBehaviour {

   [SerializeField]
   Node[] path;

	// Use this for initialization
	void Start () {

      

	}
	
	// Update is called once per frame
	void Update () {
	   
      if(path == null)
      {
         GenerateNewPath();
      }
      else
      {
         for(int i = 0; i < path.Length - 1; ++i)
         {
            Debug.DrawLine(new Vector3((float)path[i].get_x(), (float)path[i].get_y(), -0.2f), new Vector3((float)path[i + 1].get_x(), (float)path[i + 1].get_y(), -0.2f), Color.red, 99999f);
         }
      }

	}

   public void GenerateNewPath()
   {
      //Debug.Log("running");

      int targetX = Random.Range(0, ProcTerrain.instance.GetWidth());
      int targetY = Random.Range(0, ProcTerrain.instance.GetHeight());

      //Debug.Log("proc terrain = " + targetX + " _____ " + targetY);

      Tile currentTile = ProcTerrain.instance.GetTile((int)transform.position.x, (int)transform.position.y);
      Tile targetTile = ProcTerrain.instance.GetTile(targetX, targetY);

      //Debug.Log("check: " + (currentTile == targetTile) + " and node: " + (currentTile.GetNode() == targetTile.GetNode()));

      Debug.Log("finding path from " + currentTile.GetNode().get_x() + " to " + targetTile.transform.position);

      Debug.Log("pathfinder = " + (ProcTerrain.pathfinder == null) + " ____ " + (targetTile.GetNode() == null));

      Array<object> newPath = ProcTerrain.pathfinder.FindPath(currentTile.GetNode(), targetTile.GetNode());

      Debug.Log("found path of length = " + newPath.length);

      path = new Node[newPath.length];

      for(int i = 0; i < path.Length; ++i)
      {
         path[i] = (Node)newPath[i];
      }
   }
}
