using UnityEngine;
using System.Collections;

using pathPlanner;

public class Entity : MonoBehaviour {

   [SerializeField]
   Node[] path;
   int currentPosition = 0;

   float speed = 1f;

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
            Debug.DrawLine(new Vector3((float)path[i].get_x(), (float)path[i].get_y(), -0.2f), new Vector3((float)path[i + 1].get_x(), (float)path[i + 1].get_y(), -0.2f), Color.red, 9999f);
         }

         if (currentPosition < path.Length)
         {
            Vector2 target = new Vector2((float)path[currentPosition].get_x(), (float)path[currentPosition].get_y());
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if ((Vector2)transform.position == target)
            {
               currentPosition++;
            }
            
         }
         else
         {
            currentPosition = 0;
            path = null;
         }
      }

	}

   public void GenerateNewPath()
   {

      int targetX = Random.Range(0, ProcTerrain.instance.GetWidth());
      int targetY = Random.Range(0, ProcTerrain.instance.GetHeight());

      Tile currentTile = ProcTerrain.instance.GetTile((int)transform.position.x, (int)transform.position.y);
      Tile targetTile = ProcTerrain.instance.GetTile(targetX, targetY);
      Array<object> newPath = ProcTerrain.pathfinder.FindPath(currentTile.GetNode(), targetTile.GetNode());

      path = new Node[newPath.length];

      for (int i = 0; i < path.Length; ++i)
      {
         path[i] = (Node)newPath[i];
      }

   }
}
