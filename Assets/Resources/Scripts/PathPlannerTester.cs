using UnityEngine;
using System.Collections;

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

    Map map = null;
    Scenario[] scenarios = null;

    // Use this for initialization
    void Start()
    {
        //map = LoadMap(mapPath);
        //scenarios = LoadScenarios(scenarioPath);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public pathPlanner.Map LoadMap(string filePath_)
	{
		string[] fin = System.IO.File.ReadAllLines(filePath_);

        int height = int.Parse(fin[1].Split(' ')[1]);
		int width = int.Parse(fin[2].Split(' ')[1]);

		Map map = new Map(width, height, 1, 1);

			for(int y = 4; y < fin.Length; ++y)
			{
                
				for (int x = 0; x < width; ++x)
				{
					char value = fin[y][x];
					switch(value)
					{
						case '.':
						case 'G':
						case 'S':
							map.GetNodeByIndex(x, y - 4).traversable = true;
                            break;
							
						case '@':
						case 'O':
						case 'W':
						case 'T':
							map.GetNodeByIndex(x, y - 4).traversable = false;
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
		string[] fin = System.IO.File.ReadAllLines(filePath_);
		
        // only the top line of the scenarios file is not a scenario
		Scenario[] segmentArray = new Scenario[fin.Length - 1];

        for(int i = 1; i < fin.Length; ++i)
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
				string[] segments = fin[i].Split(' ');
				segmentArray[i - 1] = new Scenario{
					width = int.Parse(segments[2]),
					height = int.Parse(segments[3]),
					sx = int.Parse(segments[4]),
					sy = int.Parse(segments[5]),
					gx = int.Parse(segments[6]),
					gy = int.Parse(segments[7]),
					optimalLength = float.Parse(segments[8])
				};
				
			}
		
		return segmentArray;
	}
}
