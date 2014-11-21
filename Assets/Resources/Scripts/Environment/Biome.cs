using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class BiomeResource
{
   GameResource resource;

   public GameResource Resource
   {
      get { return resource; }
   }

   float chance;

   public float Chance
   {
      get { return chance; }
   }

   public BiomeResource(GameResource resource_, float chance_)
   {
      resource = resource_;
      chance = chance_;
   }
}

public class BiomeTile
{
   Tile tile;

   public Tile Tile
   {
      get { return tile; }
   }

   float chance;

   public float Chance
   {
      get { return chance; }
   }

   public BiomeTile(Tile tile_, float chance_)
   {
      tile = tile_;
      chance = chance_;
   }
}

public class BiomeRequirements
{
   float heightMin, heightMax, temperatureMin, temperatureMax, rainfallMin, rainfallMax;

   public float HeightMin
   {
      get { return heightMin; }
   }

   public float HeightMax
   {
      get { return heightMax; }
   }

   public float TemperatureMin
   {
      get { return temperatureMin; }
   }

   public float TemperatureMax
   {
      get { return temperatureMax; }
   }

   public float RainfallMin
   {
      get { return rainfallMin; }
   }

   public float RainfallMax
   {
      get { return rainfallMax; }
   }

   public BiomeRequirements(float heightMin_, float heightMax_, float temperatureMin_, float temperatureMax_, float rainfallMin_, float rainfallMax_)
   {
      heightMin = heightMin_;
      heightMax = heightMax_;
      temperatureMin = temperatureMin_;
      temperatureMax = temperatureMax_;
      rainfallMin = rainfallMin_;
      rainfallMax = rainfallMax_;
   }

   public bool HeightIsWithinBounds(float height_)
   {
      return height_ > heightMin && height_ <= heightMax ? true : false;
   }

   public bool TemperatureIsWithinBounds(float temperature_)
   {
      return temperature_ > temperatureMin && temperature_ <= temperatureMax ? true : false;
   }

   public bool RainfallIsWithinBounds(float rainfall_)
   {
      return rainfall_ > rainfallMin && rainfall_ <= rainfallMax ? true : false;
   }
}

public class Biome
{

   #region static

   public static void OnStartup()
   {
      Load("Data/Biomes");
      GenerateBiomeGraph(biomes, temperatureMax, rainfallMax, out whittakerGraph);
   }

   static List<Biome> biomes = new List<Biome>();

   public static List<Biome> Biomes
   {
      get { return Biome.biomes; }
   }

   static int temperatureMax = 70, rainfallMax = 400;
   public static int TemperatureMax
   {
      get { return Biome.temperatureMax; }
   }
   public static int RainfallMax
   {
      get { return Biome.rainfallMax; }
   }

   static Biome[][] whittakerGraph;
   public static Biome[][] WhittakerGraph
   {
      get { return Biome.whittakerGraph; }
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

      //for every biome in file
      for (int i = 0; i < jsonData.Count; ++i)
      {
         string name = jsonData[i]["Name"];

         List<BiomeTile> tempTiles = new List<BiomeTile>();
         for (int j = 0; j < jsonData[i]["ValidTiles"].Count; ++j)
         {
            Tile tile = Tile.GetTile(jsonData[i]["ValidTiles"][j]["Name"]);
            float chance = jsonData[i]["ValidTiles"][j]["Chance"].AsFloat;

            tempTiles.Add(new BiomeTile(tile, chance));
         }


         List<BiomeResource> tempResources = new List<BiomeResource>();
         for (int j = 0; j < jsonData[i]["ValidResources"].Count; ++j)
         {
            GameResource resource = GameResource.GetResource(jsonData[i]["ValidResources"][j]["Name"]);
            float chance = jsonData[i]["ValidResources"][j]["Chance"].AsFloat;

            tempResources.Add(new BiomeResource(resource, chance));
         }


         float hMin = jsonData[i]["Requirements"]["Height"]["Min"].AsFloat;
         float hMax = jsonData[i]["Requirements"]["Height"]["Max"].AsFloat;

         float tMin = jsonData[i]["Requirements"]["Temperature"]["Min"].AsFloat;
         float tMax = jsonData[i]["Requirements"]["Temperature"]["Max"].AsFloat;

         float rMin = jsonData[i]["Requirements"]["Rainfall"]["Min"].AsFloat;
         float rMax = jsonData[i]["Requirements"]["Rainfall"]["Max"].AsFloat;

         BiomeRequirements newRequirements = new BiomeRequirements(hMin, hMax, tMin, tMax, rMin, rMax);

         Biome biome = new Biome(name, tempTiles, tempResources, newRequirements);

         Biomes.Add(biome);

      }

      Debug.Log(Biomes.Count + " Biomes loaded from '" + filePath_ + "'");
      return true;

   }

   public static void GenerateBiomeGraph(List<Biome> biomes_, int temperatureMaxValue_, int rainfallMaxValue_, out Biome[][] graph_)
   {
      graph_ = new Biome[temperatureMaxValue_][];
      for (int temperatureLoopIndex = 0; temperatureLoopIndex < temperatureMaxValue_; ++temperatureLoopIndex)
      {

         graph_[temperatureLoopIndex] = new Biome[rainfallMaxValue_];
         for (int rainfallLoopIndex = 0; rainfallLoopIndex < rainfallMaxValue_; ++rainfallLoopIndex)
         {

            foreach (Biome biome in biomes_)
            {
               float currentTemperatureValue = (float)temperatureLoopIndex / temperatureMaxValue_;
               float currentRainfallValue = (float)rainfallLoopIndex / rainfallMaxValue_;
               if (biome.Requirements.TemperatureIsWithinBounds(currentTemperatureValue) &&
                  biome.Requirements.RainfallIsWithinBounds(currentRainfallValue))
               {
                  if (graph_[temperatureLoopIndex][rainfallLoopIndex] == null)
                  {
                     graph_[temperatureLoopIndex][rainfallLoopIndex] = biome;
                  }
                  else
                  {
                     Debug.LogWarning("Biome.GenerateBiomeGraph biome " + biome.Type + " is being ignored as location x = " +
                        temperatureLoopIndex + " y = " + rainfallLoopIndex + " is already occupied by " +
                        graph_[temperatureLoopIndex][rainfallLoopIndex].Type);
                  }
               }
            }

         }

      }

   }

   public static Biome GetBiome(float temperature_, float rainfall_)
   {

      int temperatureIndex = (int)(temperature_ * temperatureMax);
      int rainfallIndex = (int)(rainfall_ * rainfallMax);

      return whittakerGraph[temperatureIndex][rainfallIndex];
   }

   #endregion static

   string type;
   public string Type
   {
      get { return type; }
   }

   List<BiomeTile> validTiles;
   public List<BiomeTile> ValidTiles
   {
      get { return validTiles; }
   }

   List<BiomeResource> validResources;
   public List<BiomeResource> ValidResources
   {
      get { return validResources; }
   }

   Dictionary<string, List<BiomeResource>> dictValidResources = new Dictionary<string, List<BiomeResource>>();
   public Dictionary<string, List<BiomeResource>> DictValidResources
   {
      get { return dictValidResources; }
   }

   BiomeRequirements requirements;
   public BiomeRequirements Requirements
   {
      get { return requirements; }
   }

   public Biome(string type_, List<BiomeTile> validTiles_, List<BiomeResource> validResources_, BiomeRequirements requirements_)
   {
      type = type_;
      validTiles = validTiles_;
      validResources = validResources_;
      requirements = requirements_;

      foreach (BiomeResource res in validResources)
      {

         foreach (Tile tile in res.Resource.ValidTiles)
         {
            List<BiomeResource> resources = null;
            DictValidResources.TryGetValue(tile.GetName(), out resources);
            if (resources != null)
            {
               resources.Add(res);
            }
            else
            {
               resources = new List<BiomeResource>();
               resources.Add(res);
               DictValidResources.Add(tile.GetName(), resources);
            }
         }

      }

   }

   public List<BiomeResource> GetBiomeResources(Tile tile_)
   {
      List<BiomeResource> resources = null;
      DictValidResources.TryGetValue(tile_.GetName(), out resources);
      return resources;
   }

}
