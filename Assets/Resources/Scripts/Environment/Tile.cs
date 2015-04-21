using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

using pathPlanner;

public class Tile
{
    string tileType;
    Texture2D originalTexture;
    public Texture2D GetOriginalTexture()
    {
        return originalTexture;
    }

    // we combine the tile texture and its owned game resource by overlaying the game resource onto a fresh version of the original texture
    public Texture2D GetFreshTileTexture()
    {
        Color[] pixels = originalTexture.GetPixels();

        if (tileResource != null)
        {
            Color[] gameResourcePixels = tileResource.GetOriginalTexture().GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
            {
                if (gameResourcePixels[i].a != 0)
                {
                    pixels[i] = gameResourcePixels[i];
                }
            }
        }

        Texture2D freshTex = new Texture2D(originalTexture.width, originalTexture.height);
        freshTex.SetPixels(0, 0, freshTex.width, freshTex.height, pixels);

        freshTex.Apply(false);

        return freshTex;
    }

    bool debugShowNeighbours = false;

    public bool debugColourShow = false;
    public Color debugColour = Color.red;

    
    public string GetName()
    {
        return tileType;
    }

    //Node node;
    public bool cachedTraversable = true; // only used at beginning to cache traversable. needs cleaning up so i dont have to do this shit
    //public Node GetNode()
    //{
    //    return node;
    //}

    //public Node SetNode(Node node_, bool useCachedTraversable_)
    //{
    //    node = node_;
    //    if(useCachedTraversable_)
    //        node.SetTraversable(cachedTraversable);
    //    return node;
    //}

    GameResource tileResource;
    public GameResource GetResource()
    {
        return tileResource;
    }

    public GameResource SetResource(GameResource resource_)
    {
        return tileResource = resource_;
    }

    Vector2 position;
    public Vector2 Position
    {
        get { return position; }
        set 
        { 
            position = value;
            tileResource.Position = value;
        }
    }

    public Tile(string type_, Texture2D tex_)
    {
        tileType = type_;
        originalTexture = tex_;
    }

    public Tile Clone()
    {
        return new Tile(tileType, originalTexture);
    }

    //void Update()
    //{
    //    if (debugShowNeighbours)
    //    {
    //        for (int i = 0; i < node.GetNeighbours().length; ++i)
    //        {
    //            Node neighbour = ((DistanceNode)node.GetNeighbours()[i]).connectedNode;
    //            Debug.DrawLine(transform.position, new Vector3((float)neighbour.get_x(), (float)neighbour.get_y(), transform.position.z));
    //        }
    //    }

    //    if(debugColourShow)
    //    {
    //        Debug.DrawLine(new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z), new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z), debugColour);
    //        Debug.DrawLine(new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z), new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z), debugColour);
    //        Debug.DrawLine(new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), debugColour);
    //        Debug.DrawLine(new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z), debugColour);
    //    }

    //    if(GetNode().get_parent() != null)
    //    {
    //        Debug.DrawLine(transform.position, new Vector3((float)GetNode().get_parent().get_x(), (float)GetNode().get_parent().get_y(), transform.position.z));
    //    }
    //}

    #region static
    static GameObject tileParent = new GameObject("Tiles");

    public static void OnStartup()
    {
        Load("Data/Tiles");
    }

    static Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

    public static void Add(Tile tile_)
    {
        tiles.Add(tile_.GetName(), tile_);
    }

    public static bool Load(string filePath_)
    {
        TextAsset asset = (TextAsset)Resources.Load(filePath_);
		
		//GameObject prefab = Resources.Load<GameObject>("Prefabs/Quad");

        if (asset == null)
        {
            Debug.Log("file '" + filePath_ + "' could not be loaded");
            return false;
        }

        JSONNode jsonData = JSON.Parse(asset.text);

        for (int i = 0; i < jsonData.Count; ++i)
        {
            string name = jsonData[i]["Name"];

            //GameObject tileObj = (GameObject)GameObject.Instantiate(prefab);
            //tileObj.name = name;
            //tileObj.SetActive(false);
            //tileObj.transform.parent = tileParent.transform;

            //MeshRenderer render = tileObj.GetComponent<MeshRenderer>();
            //render.material = new Material(Shader.Find("Transparent/Diffuse"));

            Texture2D tex = Resources.Load(jsonData[i]["ImagePath"]) as Texture2D;
            //render.material.mainTexture = tex;

            Tile tile = new Tile(name, tex);

            string traversableValue = jsonData[i]["Traversable"];
            //tile.SetNode(new Node(new Position(0, 0), traversableValue.Equals("True"), new GraphStructureIndirect()), false);
            tile.cachedTraversable = traversableValue.Equals("True");

            Add(tile);

        }

        Debug.Log(tiles.Count + " Tiles loaded from '" + filePath_ + "'");
        return true;

    }


    public static Tile GetTile(string name_)
    {
        Tile tile = null;
        tiles.TryGetValue(name_, out tile);
        return tile;
    }
    #endregion

}