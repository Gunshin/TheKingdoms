using UnityEngine;
using System.Collections;

using pathPlanner;

public class Chunk : MonoBehaviour
{

    static int width = 16, height = 16;

    public static int GetHeight()
    {
        return height;
    }

    public static int GetWidth()
    {
        return width;
    }

    Tile[][] tiles;

    //public Chunk(int x_, int y_, Environment env_)
    //{
    //    x = x_;
    //    y = y_;

    //    environment = env_;

    //    chunkObject = new GameObject("Chunk " + x + " _ " + y);
    //    chunkObject.transform.Translate(new Vector3(x * GetWidth(), y * GetHeight()));

    //    Start();
    //}

    void Awake()
    {
        tiles = new Tile[width][];
        for (int i = 0; i < width; ++i)
            tiles[i] = new Tile[height];

        
    }

    // Use this for initialization
    void Start()
    {

        SetBaseNodeConnections();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTiles(Environment e_)
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                tiles[i][j] = e_.GenerateLocation(i + (int)transform.position.x, j + (int)transform.position.y, gameObject);
            }
        }
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

                Debug.Log("node = " + tiles[i][j].transform.position);
                for (int a = -1; a < 2; ++a)
                {
                    for (int b = -1; b < 2; ++b)
                    {
                        if (!(a == 0 && b == 0))
                        {
                            int neighbourX = (int)tiles[i][j].transform.position.x + a;
                            int neighbourY = (int)tiles[i][j].transform.position.y + b;

                            //Debug.Log("node = " + tiles[i][j].transform.position + " attempting neighbour at " + new Vector2(neighbourX, neighbourY));

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
