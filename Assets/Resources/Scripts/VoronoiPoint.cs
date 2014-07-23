using UnityEngine;
using System.Collections.Generic;

public class VoronoiPoint
{

   Vector2 position;

   public Vector2 Position
   {
      get { return position; }
   }

   VoronoiPolygon parent = null;

   public VoronoiPolygon Parent
   {
      get { return parent; }
      set { parent = value; }
   }

   public VoronoiPoint(Vector2 position_)
   {
      position = position_;
   }

}

