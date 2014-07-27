using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using UnityEditor;

using pathPlanner;

public class Tile : MonoBehaviour
{

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
   public Node GetNode()
   {
      if (node == null)
      {
         node = new Node(transform.position.x, transform.position.y, false);
      }
      return node;
   }

   void Start()
   {



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

      if (asset == null)
      {
         Debug.Log("file '" + filePath_ + "' could not be loaded");
         return false;
      }

      JSONNode jsonData = JSON.Parse(asset.text);

      for (int i = 0; i < jsonData.Count; ++i)
      {
         string name = jsonData[i]["Name"];

         GameObject prefab = Resources.Load<GameObject>("Prefabs/Quad");

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