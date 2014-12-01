using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

using pathPlanner;

public class Tile : MonoBehaviour
{
    [SerializeField]
    bool debugShowNeighbours = false;

    public bool debugColourShow = false;
    public Color debugColour = Color.red;

    string tileType;
    public string GetName()
    {
        return tileType;
    }
    public void SetName(string name_)
    {
        tileType = name_;
    }

    Node node;
    public bool cachedTraversable = true; // only used at beginning to cache traversable. needs cleaning up so i dont have to do this shit
    public Node GetNode()
    {
        return node;
    }

    public Node SetNode(Node node_)
    {
        node = node_;
        node.traversable = cachedTraversable;
        return node;
    }


    void Start()
    {



    }

    void Update()
    {
        if (debugShowNeighbours)
        {
            for (int i = 0; i < node.GetNeighbours().length; ++i)
            {
                Node neighbour = ((DistanceNode)node.GetNeighbours()[i]).connectedNode;
                Debug.DrawLine(transform.position, new Vector3((float)neighbour.get_x(), (float)neighbour.get_y(), transform.position.z));
            }
        }

        if(debugColourShow)
        {
            Debug.DrawLine(new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z), new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z), debugColour);
            Debug.DrawLine(new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z), new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z), debugColour);
            Debug.DrawLine(new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), debugColour);
            Debug.DrawLine(new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z), debugColour);
        }

        if(GetNode().get_parent() != null)
        {
            Debug.DrawLine(transform.position, new Vector3((float)GetNode().get_parent().get_x(), (float)GetNode().get_parent().get_y(), transform.position.z));
        }
    }

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
		
		GameObject prefab = Resources.Load<GameObject>("Prefabs/Quad");

        if (asset == null)
        {
            Debug.Log("file '" + filePath_ + "' could not be loaded");
            return false;
        }

        JSONNode jsonData = JSON.Parse(asset.text);

        for (int i = 0; i < jsonData.Count; ++i)
        {
            string name = jsonData[i]["Name"];

            GameObject tileObj = (GameObject)GameObject.Instantiate(prefab);
            tileObj.name = name;
            tileObj.SetActive(false);
            tileObj.transform.parent = tileParent.transform;

            MeshRenderer render = tileObj.GetComponent<MeshRenderer>();
            render.material = new Material(Shader.Find("Transparent/Diffuse"));

            Texture tex = Resources.Load(jsonData[i]["ImagePath"]) as Texture2D;
            render.material.mainTexture = tex;

            Tile tile = tileObj.AddComponent<Tile>();
            tile.SetName(name);

            string traversableValue = jsonData[i]["Traversable"];
            tile.SetNode(new Node(0, 0, true, new GraphStructureIndirect()));
            tile.GetNode().set_traversable(traversableValue.Equals("True"));

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