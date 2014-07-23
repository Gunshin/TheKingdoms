using UnityEngine;
using System.Collections;

public class SquareFace : MonoBehaviour
{

   MeshFilter mf;

   void Awake()
   {
      mf = GetComponent<MeshFilter>();
      mf.mesh = Utilities.createSquareMesh("square");
      
      Mesh mesh = mf.mesh;
      Vector3[] vertices = mesh.vertices;
      Color[] colors = new Color[vertices.Length];
      int i = 0;
      while (i < vertices.Length)
      {
         colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);
         i++;
      }
      mesh.colors = colors;
   }

   // Use this for initialization
   void Start()
   {
      
   }

   // Update is called once per frame
   void Update()
   {
      
   }

   public Mesh Mesh
   {
      get { return mf.mesh; }
      set { mf.mesh = value; }
   }

}
