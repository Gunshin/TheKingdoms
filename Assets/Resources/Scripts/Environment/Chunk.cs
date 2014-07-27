using UnityEngine;
using System.Collections;

using pathPlanner;

public class Chunk{

   static int width = 16, height = 16;

   public static int GetHeight()
   {
      return height;
   }

   public static int GetWidth()
   {
      return width;
   }

   GameObject chunkObject;

   int x, y;

   Environment environment;

   Tile[][] tiles;

   public Chunk(int x_, int y_, Environment env_)
   {
      x = x_;
      y = y_;

      environment = env_;

      chunkObject = new GameObject("Chunk " + x + " _ " + y);
      chunkObject.transform.Translate(new Vector3(x * (GetWidth() - 1), y * (GetHeight() - 1)));

      Start();
   }

	// Use this for initialization
	void Start () {

      tiles = new Tile[width][];
      for (int i = 0; i < width; ++i )
         tiles[i] = new Tile[height];

      for(int i = 0; i < width; ++i)
      {
         for (int j = 0; j < height; ++j)
         {
            tiles[i][j] = environment.GenerateLocation(i + (x * (GetWidth() - 1)), j + (y * (GetHeight() - 1)), chunkObject);
         }
      }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public Tile GetTile(int x_, int y_)
   {
      if (x_ < 0 || y_ < 0 || x_ >= GetWidth() || y_ >= GetHeight())
         return null;

      return tiles[x_][y_];
   }

   public void SetBaseNodeConnections()
   {
      for (int i = 0; i < width; ++i)
      {
         for (int j = 0; j < height; ++j)
         {
            
            
            for(int a = -1; a < 2; ++a)
            {
               for(int b = -1; b < 2; ++b)
               {
                  if (!(a == 0 && b == 0))
                  {
                     int neighbourX = (int)tiles[i][j].transform.position.x + a;
                     int neighbourY = (int)tiles[i][j].transform.position.y + b;

                     Tile tile = ProcTerrain.instance.GetTile(neighbourX, neighbourY);

                     if (tile != null)
                     {
                        tiles[i][j].GetNode().AddNeighbour(tile.GetNode(), new haxe.lang.Null<double>());
                     }
                  }
               }
            }


         }
      }
   }
}
