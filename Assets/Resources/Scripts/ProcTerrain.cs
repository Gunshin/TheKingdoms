﻿using UnityEngine;
using System.Collections.Generic;

using pathPlanner;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class ProcTerrain : MonoBehaviour
{

   public static ProcTerrain instance = null;

   public static IPathfinder pathfinder = new AStar();

   public int lloydRelaxCount = 0;

   public int chunkIndexWidth, chunkIndexHeight;
   Chunk[][] chunks;

   public int pointsCount = 64;

   int seed = 231;

   Voronoi voronoi;

   Texture2D map;

   void Start()
   {
      instance = this;
      //tilePrefabs = Resources.Load<GameObject>("Prefabs/Tiles/Dirt/Dirt");
      //resources = new GameObject[1];
      //resources[0] = (GameObject)Resources.Load<GameObject>("Prefabs/GameResources/Tree/Prefab_Res_Tree");

      //tiles = new GameObject[width][];

      //for (int i = 0; i < tiles.Length; ++i)
      //{
      //   tiles[i] = new GameObject[height];
      //   for (int j = 0; j < tiles[i].Length; ++j)
      //   {
      //      tiles[i][j] = (GameObject)Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], new Vector3(i, j, 0) + transform.position, Quaternion.AngleAxis(0, new Vector3(0, 0, 1)));
      //      tiles[i][j].transform.parent = transform;
      //   }
      //}

      //for(int i = 0; i < 1000; ++i)
      //{
      //   GameObject obj = (GameObject)Instantiate(resources[0], new Vector3(Random.Range(0, width), Random.Range(0, height), -0.01f) + transform.position, Quaternion.identity);
      //   obj.transform.parent = transform;
      //}

      //List<Vector2> points = new List<Vector2>();

      //for (int i = 0; i < pointsCount; ++i)
      //{
      //   int x = Random.Range(0, width), y = Random.Range(0, height);
      //   points.Add(new Vector2(x, y));
      //}

      //voronoi = new Voronoi(width, height, points);
      //voronoi.lloydRelaxation(lloydRelaxCount);

      Environment environment = new Environment(seed);
      chunks = new Chunk[chunkIndexWidth][];

      for(int i = 0; i < chunkIndexWidth; ++i)
      {
         chunks[i] = new Chunk[chunkIndexHeight];
         for(int j = 0; j < chunkIndexHeight; ++j)
         {
            chunks[i][j] = new Chunk(i, j, environment);
         }
      }

      for (int i = 0; i < chunkIndexWidth; ++i)
      {
         for (int j = 0; j < chunkIndexHeight; ++j)
         {
            chunks[i][j].SetBaseNodeConnections();
         }
      }

      GameObject entityPrefab = Resources.Load<GameObject>("Prefabs/Entity/Entity");

      Instantiate(entityPrefab, new Vector3(10, 10, 0), Quaternion.identity);

      //foreach (VoronoiPolygon polygon in voronoi.Polygons)
      //{
      //   polygon.Color = new Color(255, 0, 0);

      //   GameObject biomeType = biomes[Random.Range(0, biomes.Length)];

      //   GameObject biomeObj = (GameObject)Instantiate(biomeType, polygon.getCalculatedCentre(), Quaternion.identity);
      //   biomeObj.transform.parent = transform;

      //   //Biome biome = biomeObj.GetComponent<Biome>();
      //   //biome.TilePositions = polygon.PointsAsVector;

         
      //}

      //voronoi.toTexture2D(out map);

      //MeshRenderer render = GetComponent<MeshRenderer>();
      //render.material.mainTexture = map;

   }

   // Update is called once per frame
   void Update()
   {

   }

   public Chunk GetChunk(int x_, int y_)
   {
      return chunks[x_][y_];
   }

   public Tile GetTile(int x_, int y_)
   {
      int indexX = x_ / Chunk.GetWidth();
      int indexY = y_ / Chunk.GetHeight();

      int tileXOffset = x_ % Chunk.GetWidth();
      int tileYOffset = y_ % Chunk.GetHeight();

      //Debug.Log(indexX + " ___ " + indexY + " ________________ " + tileXOffset + " ______ " + tileYOffset);

      return chunks[indexX][indexY].GetTile(tileXOffset, tileYOffset);
   }

   public int GetWidth()
   {
      return chunkIndexWidth * Chunk.GetWidth();
   }

   public int GetHeight()
   {
      return chunkIndexHeight * Chunk.GetHeight();
   }
}