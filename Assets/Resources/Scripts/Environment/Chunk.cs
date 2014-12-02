using UnityEngine;
using System.Collections;

using pathPlanner;

public class Chunk : MonoBehaviour
{

    Tile[][] tiles;
    public Tile SetTile(int indexX_, int indexY_, Tile tile_)
    {
        return indexX_ < 0 || indexX_ >= Chunk.GetWidth() || indexY_ < 0 || indexY_ >= Chunk.GetHeight() ? null : tiles[indexX_][indexY_] = tile_;
    }
    public Tile GetTile(int x_, int y_)
    {
        return x_ < 0 || y_ < 0 || x_ >= GetWidth() || y_ >= GetHeight() ? null : tiles[x_][y_];
    }

    Material cachedMat;

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

        transform.localScale = new Vector3(width, height, 1);
    }

    // Use this for initialization
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTiles(Environment e_)
    {

        e_.GenerateChunk(this);

        cachedMat = GetComponent<MeshRenderer>().material;
        cachedMat.mainTexture = new Texture2D(Chunk.GetWidth() * tiles[0][0].GetOriginalTexture().width, Chunk.GetHeight() * tiles[0][0].GetOriginalTexture().height, TextureFormat.RGBA32, false);
        cachedMat.mainTexture.wrapMode = TextureWrapMode.Clamp;
        cachedMat.mainTexture.filterMode = FilterMode.Point;

        for (int i = 0; i < tiles.Length; ++i)
        {
            for (int j = 0; j < tiles.Length; ++j)
            {
                UpdateGraphicsOnTileIndex(i, j);
            }
        }

        Resources.UnloadUnusedAssets();

    }

    public void UpdateGraphicsOnTileIndex(int indexX_, int indexY_)
    {
        Texture2D tileTex = tiles[indexX_][indexY_].GetFreshTileTexture();
        Color[] pixels = tileTex.GetPixels();

        Texture2D chunkTex = cachedMat.mainTexture as Texture2D;

        int current = 0;
        for (int j = 0; j < tileTex.height; ++j)
        {

            for (int i = 0; i < tileTex.width; ++i)
            {
                chunkTex.SetPixel(i + indexX_ * tileTex.width, j + indexY_ * tileTex.height, pixels[current++]);
            }
        }

        chunkTex.Apply(false);

        Resources.UnloadAsset(tileTex);
    }

    public void SetBaseNodeConnections(System.Action<Chunk, Tile, int, int> funcForApplyingStructure_)
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {

                funcForApplyingStructure_(this, tiles[i][j], i, j);


                //Debug.Log("node = " + tiles[i][j].transform.position);
                //for (int a = -1; a < 2; ++a)
                //{
                //    for (int b = -1; b < 2; ++b)
                //    {
                //        if (!(a == 0 && b == 0))
                //        {
                //            int neighbourX = (int)tiles[i][j].transform.position.x + a;
                //            int neighbourY = (int)tiles[i][j].transform.position.y + b;

                //            //Debug.Log("node = " + tiles[i][j].transform.position + " attempting neighbour at " + new Vector2(neighbourX, neighbourY));

                //            Tile tile = ProcTerrain.instance.GetTile(neighbourX, neighbourY);

                //            if (tile != null)
                //            {
                //                tiles[i][j].GetNode().AddNeighbour(tile.GetNode(), new haxe.lang.Null<double>());
                //            }
                //        }
                //    }
                //}


            }
        }
    }

    #region static
    static int width = 16, height = 16;

    public static int GetHeight()
    {
        return height;
    }

    public static int GetWidth()
    {
        return width;
    }

    #endregion static
}
