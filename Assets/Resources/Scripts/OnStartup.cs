using UnityEngine;
using System.Collections;

public class OnStartup : MonoBehaviour {

	// Use this for initialization
	void Start () {
      this.enabled = false;

      Tile.OnStartup();

      GameResource.OnStartup();

      Biome.OnStartup();
      
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
