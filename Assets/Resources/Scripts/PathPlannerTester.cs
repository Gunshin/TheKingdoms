using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using pathPlanner;

public class PathPlannerTester : MonoBehaviour
{

    public class Scenario
    {
        public int width = 0, height = 0, sx = -1, sy = -1, gx = -1, gy = -1;
        public float optimalLength = 0;

        public Scenario()
        {

        }
    }

    [SerializeField]
    string mapPath;

    [SerializeField]
    string scenarioPath;

    GraphGridMap map = null;
    Scenario[] scenarios = null;

    [SerializeField]
    Chunk prefabChunk;
    Chunk[][] chunks;

    pathPlanner.IPathfinder astar, jpso, jpsm, jpsp;

    [SerializeField]
    LineRenderer prefabLinerenderer;

    [SerializeField]
    Slider actionSlider;

    [SerializeField]
    GameObject start;
    [SerializeField]
    GameObject end;

    Scenario currentScenario;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        map = LoadMap(mapPath);
        scenarios = LoadScenarios(scenarioPath);

        astar = new pathPlanner.AStar(
            (x, y) =>
            {
                return Mathf.Sqrt(Mathf.Pow((float)(x.GetX() - y.GetX()), 2) + Mathf.Pow((float)(x.GetY() - y.GetY()), 2));
            });

        jpso = new pathPlanner.JPSO(map,
            (x, y) =>
            {
                return Mathf.Sqrt(Mathf.Pow((float)(x.GetX() - y.GetX()), 2) + Mathf.Pow((float)(x.GetY() - y.GetY()), 2));
            });

        jpsm = new pathPlanner.JPSM(map,
            (x, y) =>
            {
                return Mathf.Sqrt(Mathf.Pow((float)(x.GetX() - y.GetX()), 2) + Mathf.Pow((float)(x.GetY() - y.GetY()), 2));
            });

        jpsp = new pathPlanner.JPSPlus(map.GenerateGraphGridMapMinimalist(),
            (x, y) =>
            {
                return Mathf.Sqrt(Mathf.Pow((float)(x.GetX() - y.GetX()), 2) + Mathf.Pow((float)(x.GetY() - y.GetY()), 2));
            });

        ((JPSPlus)jpsp).AttachPrint(Debug.Log);

        #region generatemap
        int chunkIndexWidth = Mathf.CeilToInt(((float)map.GetWidth()) / Chunk.GetWidth());
        int chunkIndexHeight = Mathf.CeilToInt(((float)map.GetHeight()) / Chunk.GetHeight());

        Debug.Log("chunk: " + chunkIndexWidth + " _ " + chunkIndexHeight);

        chunks = new Chunk[chunkIndexWidth][];
        for (int i = 0; i < chunkIndexWidth; ++i)
        {
            chunks[i] = new Chunk[chunkIndexHeight];
        }

        for (int j = 0; j < chunkIndexHeight; ++j)
        {
            for (int i = 0; i < chunkIndexWidth; ++i)
            {
                chunks[i][j] = Instantiate(prefabChunk, new Vector3(i * Chunk.GetWidth(), j * Chunk.GetWidth(), 0), Quaternion.identity) as Chunk;

                chunks[i][j].GenerateTiles(
                    (chunk, tiles) =>
                    {
                        for (int y = 0; y < Chunk.GetHeight(); ++y)
                        {
                            for (int x = 0; x < Chunk.GetWidth(); ++x)
                            {
                                Node node = map.GetNodeByIndex(i * Chunk.GetWidth() + x, j * Chunk.GetHeight() + y);
                                if (node != null)
                                {
                                    if (node.GetTraversable() == true)
                                    {
                                        tiles[x][y] = Tile.GetTile("Grass").Clone();
                                    }
                                    else
                                    {
                                        tiles[x][y] = Tile.GetTile("Water").Clone();
                                    }
                                }
                            }
                        }

                        chunk.GenerateTexture();

                        for (int x = 0; x < tiles.Length; ++x)
                        {
                            for (int y = 0; y < tiles[x].Length; ++y)
                            {
                                if (tiles[x][y] != null)
                                {
                                    chunk.UpdateGraphicsOnTileIndex(x, y);
                                }
                            }
                        }
                    });

            }
        }
        #endregion generatemap

        currentScenario = scenarios[219];

        Debug.Log("scen: " + currentScenario.sx + " _ " + currentScenario.sy + " _ " + currentScenario.gx + " _ " + currentScenario.gy);
        GenerateAStar();

        //GenerateNewPath();


        //for (int i = 0; i < DebugLogger.get_instance().closedSet.length; ++i)
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

    }

    public void SliderOnValueChanged(float value_)
    {
        int index = (int)value_;

        for(int i = 0; i < actions.Length; ++i)
        {
            if(i < index)
            {
                actions[i].SetActive(true);
            }
            else
            {
                actions[i].SetActive(false);
            }
        }

    }

    public void SetRandomScenario()
    {

        currentScenario = scenarios[UnityEngine.Random.Range(0, scenarios.Length)];
        GenerateAStar();

    }

    public void GenerateAStar()
    {
        RunPath(astar, currentScenario);
    }

    public void GenerateJPSO()
    {
        RunPath(jpso, currentScenario);
    }

    public void GenerateJPSM()
    {
        RunPath(jpsm, currentScenario);
    }

    public void GenerateJPSP()
    {
        RunPath(jpsp, currentScenario);
    }

    void RunPath(pathPlanner.IPathfinder pather_, Scenario scen_)
    {
        pathPlanner.PathplannerParameter param = new pathPlanner.PathplannerParameter();
        param.startNode = map.GetNodeByIndex(scen_.sx, scen_.sy);
        param.goalNode = map.GetNodeByIndex(scen_.gx, scen_.gy);
        param.startX = scen_.sx;
        param.startY = scen_.sy;
        param.goalX = scen_.gx;
        param.goalY = scen_.gy;

        start.transform.position = new Vector3((float)param.startNode.GetPosition().GetX() + 0.5f, (float)param.startNode.GetPosition().GetY() + 0.5f, -0.1f);
        end.transform.position = new Vector3((float)param.goalNode.GetPosition().GetX() + 0.5f, (float)param.goalNode.GetPosition().GetY() + 0.5f, -0.1f);

        actionOn = true;
        if (actions != null)
        {
            for (int i = 0; i < actions.Length; ++i)
            {
                Destroy(actions[i]);
            }
        }

        pathOn = false;
        if (path != null)
        {
            for (int i = 0; i < path.Length; ++i)
            {
                Destroy(path[i]);
            }
        }

        Array<object> newPath = pather_.FindPath(param);
        if (newPath != null)
        {
            List<GameObject> tempPathList = new List<GameObject>();
            for (int i = 0; i < newPath.length - 1; ++i)
            {
                LineRenderer rend = Instantiate(prefabLinerenderer) as LineRenderer;
                rend.gameObject.SetActive(false);
                tempPathList.Add(rend.gameObject);
                rend.name = "path: " + i;

                Position child = (Position)newPath[i];
                Position parent = (Position)newPath[i + 1];

                rend.SetPosition(0, new Vector3((float)child.GetX() + 0.5f, (float)child.GetY() + 0.5f, -0.05f));
                rend.SetPosition(1, new Vector3((float)parent.GetX() + 0.5f, (float)parent.GetY() + 0.5f, -0.05f));
            }

            path = tempPathList.ToArray();
        }
        //lines = new LineRenderer[newPath.length - 1];
        Array<object> actionList = pather_.GetActionOutput().GetActionList();
        Array<object> actionTypes = pather_.GetActionOutput().GetActionTypes();

        List<GameObject> tempObjectList = new List<GameObject>();

        int ato = 0, explo = 0, expa = 0, sp = 0;

        for (int i = 0; i < actionList.length; ++i)
        {
            Action action = ((Action)actionList[i]);
            string actionType = action.actionType;
            if (actionType == "AddToOpen")
            {
                ato++;
            }
            else if (actionType == "Explored")
            {
                explo++;
            }
            else if (actionType == "Expand")
            {
                expa++;
            }
            else if (actionType == "SetParent")
            {
                sp++;
                LineRenderer rend = Instantiate(prefabLinerenderer) as LineRenderer;
                tempObjectList.Add(rend.gameObject);
                rend.name = "actionParent: " + i;

                Position child = action.primary;
                Position parent = action.secondary;

                rend.SetPosition(0, new Vector3((float)child.GetX() + 0.5f, (float)child.GetY() + 0.5f, -0.05f));
                rend.SetPosition(1, new Vector3((float)parent.GetX() + 0.5f, (float)parent.GetY() + 0.5f, -0.05f));
            }
        }

        actions = tempObjectList.ToArray();

        actionSlider.maxValue = actions.Length;
        //Debug.LogError("valid path: " + (newPath != null) + " _ " + path.Length);
    }

    GameObject[] actions;
    GameObject[] path;
    //List<LineRenderer> lines = new List<LineRenderer>();

    bool pathOn = false;
    public void TogglePath()
    {
        pathOn = !pathOn;
        for (int i = 0; i < path.Length; ++i)
        {
            path[i].SetActive(pathOn);
        }
    }

    bool actionOn = true;
    public void ToggleActions()
    {
        actionOn = !actionOn;
        for (int i = 0; i < actions.Length; ++i)
        {
            actions[i].SetActive(actionOn);
        }
    }

    Node GetRandomTraversableNode(GraphGridMap map_)
    {
        Debug.Log("width: " + map.GetWidth() + " _ height: " + map.GetHeight());
        Node node = null;
        while(node == null)
        {
            Node temp = map.GetNodeByIndex(Random.Range(0, map.GetWidth()), Random.Range(0, map.GetHeight()));
            if(temp.GetTraversable() == true)
            {
                node = temp;
            }
        }

        Debug.Log("x: " + node.GetPosition().GetX() + " _ y: " + node.GetPosition().GetY());
        return node;
    }

    // Update is called once per frame
    void Update()
    {

    }

    string[] ReadLines(string filePath_)
    {
        System.IO.File.SetAttributes(Application.dataPath + filePath_, System.IO.File.GetAttributes(Application.dataPath + filePath_) & ~System.IO.FileAttributes.ReadOnly);

        List<string> lines = new List<string>();
        System.IO.StreamReader sr = new System.IO.StreamReader(Application.dataPath + filePath_);

        while(!sr.EndOfStream)
        {
            lines.Add(sr.ReadLine());
        }

        return lines.ToArray();
    }

    public pathPlanner.GraphGridMap LoadMap(string filePath_)
    {
        string[] fin = ReadLines(filePath_);

        int height = int.Parse(fin[1].Split(' ')[1]);
        int width = int.Parse(fin[2].Split(' ')[1]);

        pathPlanner.GraphGridMap map = new pathPlanner.GraphGridMap(width, height);

        for (int y = 4; y < fin.Length; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                char value = fin[y][x];
                switch (value)
                {
                    case '.':
                    case 'G':
                    case 'S':
                        map.GetNodeByIndex(x, y - 4).SetTraversable(true);
                        break;

                    case '@':
                    case 'O':
                    case 'W':
                    case 'T':
                        map.GetNodeByIndex(x, y - 4).SetTraversable(false);
                        break;

                    default:
                        Debug.Log("something went wrong in loading map: " + filePath_ + " : " + value);
                        break;

                }

            }
        }

        return map;
    }

    public Scenario[] LoadScenarios(string filePath_)
    {
        string[] fin = ReadLines(filePath_);

        // not all lines in the scenarios file are scenarios
        List<Scenario> segmentList = new List<Scenario>();

        for (int i = 1; i < fin.Length; ++i)
        {

            /* each line in the scenario file is split up into segements seperated by a whitespace which represent different things.
             * [0] Bucket (not sure)
             * [1] map file path
             * [2] map width
             * [3] map height
             * [4] start x coord
             * [5] start y coord
             * [6] goal x coord
             * [7] goal y coord
             * [8] optimal length
             */
            if (fin[i].Length > 0)
            {
                string[] segments = fin[i].Split('\t');
                if (segments.Length == 9) // scenarios files sometimes have empty lines
                {
                    segmentList.Add(new Scenario
                    {
                        width = int.Parse(segments[2]),
                        height = int.Parse(segments[3]),
                        sx = int.Parse(segments[4]),
                        sy = int.Parse(segments[5]),
                        gx = int.Parse(segments[6]),
                        gy = int.Parse(segments[7]),
                        optimalLength = float.Parse(segments[8])
                    });
                }
            }
        }

        return segmentList.ToArray();
    }
}
