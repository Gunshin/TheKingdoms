﻿using UnityEngine;
using System.Collections.Generic;

using pathPlanner;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class ProcTerrain : MonoBehaviour
{

    public static ProcTerrain instance = null;

    public static pathPlanner.GraphGridMap pathMap = new GraphGridMap(128, 128);
    public static IPathfinder pathfinder;

    public int lloydRelaxCount = 0;

    [SerializeField]
    Chunk prefabChunk;
    public int chunkIndexWidth, chunkIndexHeight;
    Chunk[][] chunks;

    public int pointsCount = 64;

    int seed = 231;

    Voronoi voronoi;

    Texture2D map;

    void Start()
    {
        instance = this;

        pathfinder = new pathPlanner.JPSO(pathMap, (x, y) =>
        {
            return Mathf.Sqrt(Mathf.Pow((float)(x.GetX() - y.GetX()), 2) + Mathf.Pow((float)(x.GetY() - y.GetY()), 2));
        });

        //pathPlanner.DebugLogger.GetInstance().loggingFunction = Debug.Log;

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

        for (int i = 0; i < chunkIndexWidth; ++i)
        {
            chunks[i] = new Chunk[chunkIndexHeight];
            for (int j = 0; j < chunkIndexHeight; ++j)
            {
                chunks[i][j] = Instantiate(prefabChunk, new Vector3(i * Chunk.GetWidth(), j * Chunk.GetWidth(), 0), Quaternion.identity) as Chunk;

                chunks[i][j].GenerateTiles(
                    (chunk, tiles) => 
                    {
                        environment.GenerateChunk(chunk);

                        chunk.GenerateTexture();

                        for (int x = 0; x < tiles.Length; ++x)
                        {
                            for (int y = 0; y < tiles[x].Length; ++y)
                            {
                                chunk.UpdateGraphicsOnTileIndex(x, y);
                            }
                        }
                    });
                //chunks[i][j].SetBaseNodeConnections(
                //(chunk, tile, indexX, indexY) =>
                //{
                //    tile.SetNode(pathMap.GetNodeByIndex(indexX, indexY), true);
                //}
                //);

            }
        }

        GameObject entityPrefab = Resources.Load<GameObject>("Prefabs/Entity/Entity");

        int enemyCount = 100;

        for (int i = 0; i < enemyCount; ++i)
            Instantiate(entityPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        //Array<object> newPath = ProcTerrain.pathfinder.FindPath(pathMap.GetNodeByIndex(0, 0), pathMap.GetNodeByIndex(100, 50),
        //    (x, y) =>
        //    {
        //        return Mathf.Sqrt(Mathf.Pow((float)(x.x - y.x), 2) + Mathf.Pow((float)(x.y - y.y), 2));
        //    }
        //);

        //for (int i = 0; i < newPath.length; ++i)
        //{
        //    Node node = (Node)newPath[i];
        //    Debug.Log("path: " + i + " : " + new Vector2((float)node.get_x(), (float)node.get_y()));
        //}

        //for (int i = 0; i < DebugLogger.get_instance().closedSet.length; ++i )
        //{
        //    Node node = (Node)DebugLogger.get_instance().closedSet[i];
        //    GetTile((int)node.get_x(), (int)node.get_y()).debugColourShow = true;
        //    GetTile((int)node.get_x(), (int)node.get_y()).debugColour = Color.black;
        //}

        //for (int i = 0; i < DebugLogger.get_instance().openSet.length; ++i)
        //{
        //    Node node = (Node)DebugLogger.get_instance().openSet[i];
        //    GetTile((int)node.get_x(), (int)node.get_y()).debugColourShow = true;
        //    GetTile((int)node.get_x(), (int)node.get_y()).debugColour = Color.white;
        //}

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

    public Chunk GetChunk(Tile tile_)
    {

        int indexX = ((int)tile_.Position.x) / Chunk.GetWidth();
        int indexY = ((int)tile_.Position.y) / Chunk.GetHeight();

        if (indexX >= 0 && indexX < chunkIndexWidth && indexY >= 0 && indexY < chunkIndexHeight)
        {
            return chunks[indexX][indexY];
        }

        return null;
    }

    public Tile GetTile(int x_, int y_)
    {
        //Debug.Log(x_ + " _____ " + y_);

        int indexX = x_ / Chunk.GetWidth();
        int indexY = y_ / Chunk.GetHeight();

        if (indexX >= 0 && indexX < chunkIndexWidth && indexY >= 0 && indexY < chunkIndexHeight)
        {

            int tileXOffset = x_ % Chunk.GetWidth();
            int tileYOffset = y_ % Chunk.GetHeight();

            //Debug.Log(indexX + " ___ " + indexY + " ________________ " + tileXOffset + " ______ " + tileYOffset);

            return chunks[indexX][indexY].GetTile(tileXOffset, tileYOffset);
        }
        return null;
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
