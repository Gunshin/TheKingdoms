using UnityEngine;
using System.Collections.Generic;

public class VoronoiPolygon
{

   // Use this for initialization

   Voronoi parentDiagram;

   VoronoiPoint originalPoint;
   Color color;

   public Color Color
   {
      get { return color; }
      set { color = value; }
   }

   List<VoronoiPoint> points = new List<VoronoiPoint>();

   public List<VoronoiPoint> Points
   {
      get { return points; }
   }

   List<VoronoiPoint> edges = new List<VoronoiPoint>();
   List<VoronoiPoint> corners = new List<VoronoiPoint>();

   public List<VoronoiPoint> Corners
   {
      get { return corners; }
   }

   List<VoronoiPolygon> neighbours = new List<VoronoiPolygon>();

   public List<VoronoiPolygon> Neighbours
   {
      get { return neighbours; }
   }

   public VoronoiPolygon(Voronoi parentDiagram_, VoronoiPoint original_, Color color_)
   {
      parentDiagram = parentDiagram_;
      originalPoint = original_;
      color = color_;
   }

   public Vector2 getCalculatedCentre()
   {
      Vector2 returnee = new Vector2();
      foreach (VoronoiPoint point in points)
      {
         returnee += point.Position;
      }

      returnee /= points.Count;

      return returnee;
   }

   public void addPoint(VoronoiPoint point_)
   {
      foreach (VoronoiPoint point in points)
      {
         if (point.Position == point_.Position)
         {
            return;
         }
      }
      point_.Parent = this;
      points.Add(point_);
   }

   public void generateEdges()
   {
      foreach (VoronoiPoint point in points)
      {

         List<VoronoiPolygon> tempNeighours = new List<VoronoiPolygon>();

         for (int i = -1; i < 2; ++i)
         {
            for (int j = -1; j < 2; ++j)
            {
               VoronoiPoint neighbour = parentDiagram.getPoint(new Vector2((int)point.Position.x + j, (int)point.Position.y + i));

               if (neighbour != null && !tempNeighours.Contains(neighbour.Parent) && neighbour != point && neighbour.Parent != point.Parent)
               {
                  tempNeighours.Add(neighbour.Parent);
                  if (!neighbours.Contains(neighbour.Parent))
                  {
                     neighbours.Add(neighbour.Parent);
                  }
               }
            }
         }

         ////north
         //{
         //   VoronoiPoint neighbour = parentDiagram.getPoint(new Vector2((int)point.Position.x, (int)point.Position.y + 1));

         //   if (neighbour != null && neighbour != point && !adjacencyPolygons.Contains(neighbour.Parent) && neighbour.Parent != point.Parent)
         //   {
         //      adjacencyPolygons.Add(neighbour.Parent);
         //   }
         //}

         ////east
         //{
         //   VoronoiPoint neighbour = parentDiagram.getPoint(new Vector2((int)point.Position.x + 1, (int)point.Position.y));

         //   if (neighbour != null && neighbour != point && !adjacencyPolygons.Contains(neighbour.Parent) && neighbour.Parent != point.Parent)
         //   {
         //      adjacencyPolygons.Add(neighbour.Parent);
         //   }
         //}

         ////south
         //{
         //   VoronoiPoint neighbour = parentDiagram.getPoint(new Vector2((int)point.Position.x, (int)point.Position.y - 1));

         //   if (neighbour != null && neighbour != point && !adjacencyPolygons.Contains(neighbour.Parent) && neighbour.Parent != point.Parent)
         //   {
         //      adjacencyPolygons.Add(neighbour.Parent);
         //   }
         //}

         ////west
         //{
         //   VoronoiPoint neighbour = parentDiagram.getPoint(new Vector2((int)point.Position.x - 1, (int)point.Position.y));

         //   if (neighbour != null && neighbour != point && !adjacencyPolygons.Contains(neighbour.Parent) && neighbour.Parent != point.Parent)
         //   {
         //      adjacencyPolygons.Add(neighbour.Parent);
         //   }
         //}

         if (tempNeighours.Count >= 1 && (
                              point.Position.x == 0 ||
                              point.Position.y == 0 ||
                              point.Position.x == parentDiagram.Width - 1 ||
                              point.Position.y == parentDiagram.Height - 1
                              )
                     )
         {
            //edges.Add(point);
            corners.Add(point);
         }
         else if (tempNeighours.Count > 1)
         {
            //edges.Add(point);
            corners.Add(point);
         }
         else if (tempNeighours.Count == 1)
         {
            edges.Add(point);
         }

      }
   }

   public void applyToTexture2D(Texture2D tex_)
   {


      foreach (VoronoiPoint point in points)
      {
         tex_.SetPixel((int)point.Position.x, (int)point.Position.y, color);
      }

      foreach (VoronoiPoint point in edges)
      {
         tex_.SetPixel((int)point.Position.x, (int)point.Position.y, Color.black);
      }

      foreach (VoronoiPoint point in corners)
      {
         tex_.SetPixel((int)point.Position.x, (int)point.Position.y, Color.white);
      }

      tex_.SetPixel((int)originalPoint.Position.x, (int)originalPoint.Position.y, Color.red);

      tex_.Apply();
   }

   public VoronoiPoint OriginalPoint
   {
      get { return originalPoint; }
   }

   public int Area
   {
      get
      {
         return points.Count;
      }
   }

   public List<Vector2> PointsAsVector
   {
      get
      {
         List<Vector2> returnee = new List<Vector2>();
         foreach(VoronoiPoint point in points)
         {
            returnee.Add(point.Position);
         }

         return returnee;
      }
   }
}
