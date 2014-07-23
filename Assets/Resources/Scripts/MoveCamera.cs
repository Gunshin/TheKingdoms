using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

   public float XSpeed, YSpeed, ZSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
      if(Input.GetKey("w"))
      {
         transform.Translate(new Vector3(0, YSpeed, 0));
      }

      if (Input.GetKey("a"))
      {
         transform.Translate(new Vector3(-XSpeed, 0, 0));
         
      }

      if (Input.GetKey("s"))
      {
         transform.Translate(new Vector3(0, -YSpeed, 0));
      }

      if (Input.GetKey("d"))
      {
         transform.Translate(new Vector3(XSpeed, 0, 0));
      }

      if (Input.GetKey("d"))
      {
         transform.Translate(new Vector3(XSpeed, 0, 0));
      }

      if (Input.GetKey("q"))
      {
         transform.Translate(new Vector3(0, 0, ZSpeed));
      }

      if (Input.GetKey("e"))
      {
         transform.Translate(new Vector3(0, 0, -ZSpeed));
      }

	}
}
