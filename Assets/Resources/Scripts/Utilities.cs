using UnityEngine;
using System.Collections;

public class Utilities {

   //public static Mesh createSphere(string name_, int radius)
   //{

   //}

   //public static Mesh createCircleMesh(string name_, int vertexCount_)
   //{
   //   Mesh mesh = new Mesh();
		
   //   mesh.name = name_;

   //   Vector3[] vertices = new Vector3[vertexCount_];
   //   Vector2[] uvs = new Vector2[vertexCount_];
   //   int[] elements = new int[vertexCount_ * 3];

   //   vertices[0] = Vector3.zero;
   //   uvs[0] = new Vector2(0.5f, 0.5f);

   //   float angleStep = 360.0f / (float)(vertexCount_ - 1);

   //   for (int i = 1; i < vertexCount_; ++i)
   //   {
   //      vertices[i] = Quaternion.AngleAxis(-angleStep * (float)(i - 1), Vector3.back) * Vector3.up;
   //      float normedHorizontal = (vertices[i].x + 1.0f) * 0.5f;
   //      float normedVertical = (vertices[i].x + 1.0f) * 0.5f;
   //      uvs[i] = new Vector2(normedHorizontal, normedVertical);
   //   }

   //   for (int i = 0; i + 2 < vertexCount_; ++i)
   //   {
   //      int index = i * 3;
   //      elements[index + 0] = 0;
   //      elements[index + 1] = i + 1;
   //      elements[index + 2] = i + 2;
   //   }

   //   int lastTriangleIndex = elements.Length - 3;  
   //   elements[lastTriangleIndex + 0] = 0;  
   //   elements[lastTriangleIndex + 1] = vertexCount_ - 1;  
   //   elements[lastTriangleIndex + 2] = 1;

   //   mesh.vertices = vertices;
   //   mesh.uv = uvs;
   //   mesh.triangles = elements;
   //   mesh.RecalculateNormals();

   //   return mesh;
   //}

   public static Mesh createSquareMesh(string name_)
   {
      Mesh mesh = new Mesh();

      mesh.name = name_;

      Vector3[] vertices = new Vector3[4];
      vertices[0] = new Vector3(-0.5f, -0.5f, 0);
      vertices[1] = new Vector3(0.5f, -0.5f, 0);
      vertices[2] = new Vector3(-0.5f, 0.5f, 0);
      vertices[3] = new Vector3(0.5f, 0.5f, 0);
      mesh.vertices = vertices;

      Vector2[] uvs = new Vector2[4];
      uvs[0] = new Vector2(0, 0);
      uvs[1] = new Vector2(1, 0);
      uvs[2] = new Vector2(0, 1);
      uvs[3] = new Vector2(1, 1);
      mesh.uv = uvs;

      int[] elements = new int[6];
      elements[0] = 0;
      elements[1] = 1;
      elements[2] = 2;

      elements[3] = 2;
      elements[4] = 1;
      elements[5] = 3;

      mesh.triangles = elements;

      mesh.RecalculateNormals();

      return mesh;
   }

}
