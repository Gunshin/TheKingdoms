using UnityEngine;
using System.Collections;

public class SquareGeometry : MonoBehaviour
{

   public GameObject preFabSquareFace = (GameObject)Resources.Load("PreFabs/Geometry/PreFabSquareFace");
   public int width = 1, height = 1;

   SquareFace[,] faces;
   // Use this for initialization
   void Start()
   {
      faces = new SquareFace[width, height];
      for (int i = 0; i < height; ++i)
      {
         for (int j = 0; j < width; ++j)
         {
            faces[j, i] = ((GameObject)Instantiate(preFabSquareFace, this.transform.position, this.transform.rotation)).GetComponent<SquareFace>();
            faces[j, i].transform.parent = this.transform;
            faces[j, i].transform.localPosition = new Vector3(j - ((width - 1) / 2.0f), i - ((height - 1) / 2.0f), 0);
         }
      }

      Debug.Log("transform.parent.position centre: " + transform.parent.position.x + " " + transform.parent.position.y + " " + transform.parent.position.z);
      Debug.Log("faces[0, 0].transform.position centre: " + faces[0, 0].transform.position.x + " " + faces[0, 0].transform.position.y + " " + faces[0, 0].transform.position.z);
      float radius = 2;//Vector3.Distance(transform.parent.position, faces[0, 0].transform.position);
      Debug.Log("radius = " + radius);
      Vector3 centre = transform.parent.position;

      //Vector3 point = new Vector3(0.5f, 0.5f, 0.5f);

      //float distance = Vector3.Distance(centre, point);
      //Debug.Log("distance = " + distance);

      //Vector3 normalisedDirection = Vector3.Normalize(point - centre);
      //Debug.Log("normalisedDirection : __" + normalisedDirection.x + " " + normalisedDirection.y + " " + normalisedDirection.z);

      //Vector3 newPos = (normalisedDirection * radius) + centre;
      //Debug.Log("newPos : __" + newPos.x + " " + newPos.y + " " + newPos.z);

      //float newdistance = Vector3.Distance(centre, newPos);
      //Debug.Log("newdistance = " + newdistance);

      for (int i = 0; i < height; ++i)
      {
         for (int j = 0; j < width; ++j)
         {
            Mesh mesh = faces[j, i].Mesh;
            Vector3[] vertices = mesh.vertices;

            Vector3 localPos = faces[j, i].transform.position;

            Vector3 normalisedFaceDirection = Vector3.Normalize(localPos - centre);

            faces[j, i].transform.localPosition = (normalisedFaceDirection * radius);

            faces[j, i].transform.localScale = new Vector3(2, 2, 2);

            for (int a = 0; a < vertices.Length; ++a)
            {

               Vector3 verticePos = vertices[a] + localPos;

               //Vector3 normalisedDirection = verticePos - centre;
               Vector3 normalisedDirection = Vector3.Normalize(verticePos - centre);

               //Debug.Log("before: " + vertices[a].x + " " + vertices[a].y + " " + vertices[a].z);
               vertices[a] = (normalisedDirection * radius);
               Debug.Log("after a: " + a + " : __" + vertices[a].x + " " + vertices[a].y + " " + vertices[a].z);



               //LineRenderer line = new GameObject("LineRenderer").AddComponent(typeof(LineRenderer)) as LineRenderer;
               //line.SetVertexCount(2);
               //Vector3 primPos = centre;
               //primPos.z = 0.01f;
               //line.SetPosition(0, primPos);
               //Vector3 secondPos = verticePos;
               //secondPos.z = 0.01f;
               //line.SetPosition(1, secondPos);
               //line.SetWidth(1, 1);


            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
         }
      }
   }

   // Update is called once per frame
   void Update()
   {
   }
}
