﻿using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System.Collections;

public class ResourceRequirement
{
   string type, value;

   public string Type
   {
      get { return type; }
   }

   public string Value
   {
      get { return this.value; }
   }

   public ResourceRequirement(string type_, string value_)
   {
      type = type_;
      value = value_;
   }

}

public class GameResource
{

   static GameObject resourceParent = new GameObject("GameResources");

   public static void OnStartup()
   {
      Load("Data/Resources");
   }

   static List<GameResource> listGameResources = new List<GameResource>();
   public static List<GameResource> ListGameResources
   {
      get { return GameResource.listGameResources; }
   }

   static Dictionary<string, List<GameResource>> dictGameResources = new Dictionary<string, List<GameResource>>();
   public static Dictionary<string, List<GameResource>> DictGameResources
   {
      get { return dictGameResources; }
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

         GameObject resourceObj = (GameObject)GameObject.Instantiate(prefab);
         resourceObj.name = name;
         resourceObj.SetActive(false);
         resourceObj.transform.parent = resourceParent.transform;

         MeshRenderer render = resourceObj.GetComponent<MeshRenderer>();
         render.material = new Material(Shader.Find("Transparent/Diffuse"));

         Texture tex = Resources.Load(jsonData[i]["ImagePath"]) as Texture2D;
         render.material.mainTexture = tex;

         float time = jsonData[i]["Action"]["Time"].AsFloat;
         string productionResource = jsonData[i]["Action"]["ProductionResource"];
         int productionAmount = jsonData[i]["Action"]["ProductionAmount"].AsInt;

         List<Tile> validTiles = new List<Tile>();
         for (int j = 0; j < jsonData[i]["Placement"].Count; ++j)
         {
            string tileName = jsonData[i]["Placement"][j];
            validTiles.Add(Tile.GetTile(tileName));
         }

         GameResource resource = new GameResource(name, resourceObj, validTiles, time, productionResource, productionAmount);

         for (int j = 0; j < jsonData[i]["Placement"].Count; ++j)
         {
            string tileName = jsonData[i]["Placement"][j];
            List<GameResource> resources = null;
            DictGameResources.TryGetValue(tileName, out resources);
            Debug.Log("GameResources.Load dictionary key " + tileName + " found as null = " + (resources == null));
            if (resources != null)
            {
               resources.Add(resource);
            }
            else
            {
               resources = new List<GameResource>();
               resources.Add(resource);
               DictGameResources.Add(tileName, resources);
            }
         }

         for (int j = 0; j < jsonData[i]["Action"]["Requirements"].Count; ++j)
         {
            string type = jsonData[i]["Action"]["Requirements"][j]["Type"];
            string value = jsonData[i]["Action"]["Requirements"][j]["Value"];
            resource.addRequirement(type, value);
         }

         ListGameResources.Add(resource);
      }

      Debug.Log(ListGameResources.Count + " GameResources loaded from '" + filePath_ + "'");
      return true;

   }

   public static GameResource GetResource(string name_)
   {
      foreach (GameResource resource in ListGameResources)
      {
         if (resource.Name.Equals(name_))
         {
            return resource;
         }
      }
      return null;
   }

   public static List<GameResource> GetResourcesForTile(Tile tile_)
   {
      List<GameResource> resources = null;
      DictGameResources.TryGetValue(tile_.GetName(), out resources);
      return resources;
   }

   string name;
   public string Name
   {
      get { return name; }
   }

   GameObject resourcePrefab;
   public GameObject ResourcePrefab
   {
      get { return resourcePrefab; }
   }

   List<ResourceRequirement> requirements = new List<ResourceRequirement>();
   public List<ResourceRequirement> Requirements
   {
      get { return requirements; }
   }

   List<Tile> validTiles;
   public List<Tile> ValidTiles
   {
      get { return validTiles; }
   }

   float actionTime;
   public float ActionTime
   {
      get { return actionTime; }
   }

   string productionResource;
   public string ProductionResource
   {
      get { return productionResource; }
   }

   int productionAmount;
   public int ProductionAmount
   {
      get { return productionAmount; }
   }

   public GameResource(string name_, GameObject resourcePrefab_, List<Tile> validTiles_, float actionTime_, string productionResource_, int productionAmount_)
   {
      name = name_;
      resourcePrefab = resourcePrefab_;
      validTiles = validTiles_;
      actionTime = actionTime_;
      productionResource = productionResource_;
      productionAmount = productionAmount_;
   }

   public void addRequirement(string type_, string value_)
   {
      requirements.Add(new ResourceRequirement(type_, value_));
   }

}
