using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Environment
{

   PerlinNoise temperature;
   PerlinNoise rainfall;

   public Environment(int seed_)
   {
      temperature = new PerlinNoise(seed_);
      rainfall = new PerlinNoise(seed_ + 1);
   }

   int zoom = 32, octaves = 7;

   public Biome GetBiomeType(int x_, int y_)
   {

      float t = temperature.getPerlinNoise(x_, y_, 0.4f, octaves, zoom) / 2 + 0.5f;
      float r = rainfall.getPerlinNoise(x_, y_, 0.4f, octaves, zoom) / 2 + 0.5f;

      return Biome.GetBiome(t, r);
   }

   public Tile GetTile(int x_, int y_)
   {

      Biome biome = GetBiomeType(x_, y_);

      List<BiomeTile> tiles = biome.ValidTiles;
      float chance = Random.Range(0.0f, 1.0f);
      float currentValue = 0;
      for (int i = 0; i < tiles.Count; ++i)
      {
         currentValue += tiles[i].Chance;
         if (currentValue >= chance)
         {
            return tiles[i].Tile;
         }
      }

      return tiles[0].Tile;

   }

   /// <summary>
   /// Returns a resource from x y coordinates
   /// </summary>
   /// <param name="x_"></param>
   /// <param name="y_"></param>
   /// <param name="tile_">Tile to cull resources that cant be placed on it</param>
   /// <returns>Returns a prefab, or null if the tile is supposed to be empty</returns>
   public GameResource GetResource(int x_, int y_, Tile tile_)
   {
      Biome biome = GetBiomeType(x_, y_);

      List<BiomeResource> resources = biome.GetBiomeResources(tile_);

      float chance = Random.Range(0.0f, 1.0f);
      float currentValue = 0;
      for (int i = 0; i < resources.Count; ++i)
      {
         currentValue += resources[i].Chance;
         if (currentValue >= chance)
         {
            return resources[i].Resource;
         }
      }

      return null;
   }

   public Tile GenerateLocation(int x_, int y_, GameObject parent_)
   {

      Tile tile = GetTile(x_, y_);

      GameObject tileObj = (GameObject)GameObject.Instantiate(tile.gameObject, new Vector3(x_, y_, 0), Quaternion.identity);
      tileObj.transform.parent = parent_.transform;
      tileObj.SetActive(true);

      GameResource resource = GetResource(x_, y_, tile);

      if (resource != null)
      {
         GameObject resourceObj = (GameObject)GameObject.Instantiate(resource.ResourcePrefab, tileObj.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity);
         resourceObj.transform.parent = parent_.transform;
         resourceObj.SetActive(true);
      }

      return tileObj.GetComponent<Tile>();

   }

}
