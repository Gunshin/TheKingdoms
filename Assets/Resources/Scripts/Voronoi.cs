
using UnityEngine;
using System.Collections.Generic;

public class Voronoi
{

   // Use this for initialization

   int width, height;

   public int Width
   {
      get { return width; }
   }

   public int Height
   {
      get { return height; }
   }

   VoronoiPoint[,] map;

   List<VoronoiPolygon> polygons = new List<VoronoiPolygon>();

   public Voronoi(int width_, int height_, List<Vector2> points_)
   {
      width = width_;
      height = height_;

      generateRegions(width, height, points_);

   }

   public void lloydRelaxation()
   {
      List<Vector2> newPoints = new List<Vector2>();

      foreach (VoronoiPolygon polygon in polygons)
      {
         newPoints.Add(polygon.getCalculatedCentre());
      }

      generateRegions(width, height, newPoints);
   }

   public void lloydRelaxation(int amount_)
   {
      for (int i = 0; i < amount_; ++i)
      {
         lloydRelaxation();
      }
   }

   void generateRegions(int width_, int height_, List<Vector2> points_)
   {
      polygons.Clear();

      map = new VoronoiPoint[width, height];

      for (int i = 0; i < points_.Count; ++i)
      {
         VoronoiPolygon polygon = new VoronoiPolygon(this, new VoronoiPoint(points_[i]), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
         polygons.Add(polygon);
      }

      for (int i = 0; i < height; ++i)
      {
         for (int j = 0; j < width; ++j)
         {
            map[j, i] = new VoronoiPoint(new Vector2(j, i));
            VoronoiPolygon closestPoly = getClosestPolygonToPoint(map[j, i]);

            closestPoly.addPoint(map[j, i]);
         }
      }

      foreach (VoronoiPolygon polygon in polygons)
      {
         polygon.generateEdges();
      }

   }

   public void toTexture2D(out Texture2D tex_)
   {
      tex_ = new Texture2D(width, height);

      foreach (VoronoiPolygon polygon in polygons)
      {
         polygon.applyToTexture2D(tex_);
      }
   }

   VoronoiPolygon getClosestPolygonToPoint(VoronoiPoint point_)
   {
      VoronoiPolygon returnee = null;
      float currentDistance = -1;
      foreach (VoronoiPolygon polygon in polygons)
      {
         float tempDistance = Vector3.Distance(point_.Position, polygon.OriginalPoint.Position);
         if (currentDistance == -1 || currentDistance > tempDistance)
         {
            returnee = polygon;
            currentDistance = tempDistance;
         }
      }

      return returnee;
   }

   public List<VoronoiPolygon> Polygons
   {
      get { return polygons; }
   }

   public List<VoronoiPolygon> EdgePolygons
   {
      get
      {
         List<VoronoiPolygon> edgePolygons = new List<VoronoiPolygon>();

         for (int i = 0; i < width; ++i)
         {
            for (int j = 0; j < height; ++j)
            {
               VoronoiPoint point = map[i, j];
               if ((0 == i || 0 == j || width - 1 == i || height - 1 == j) && !edgePolygons.Contains(point.Parent))
               {
                  edgePolygons.Add(point.Parent);
               }
            }
         }

         return edgePolygons;
      }
   }

   public VoronoiPoint getPoint(Vector2 position_)
   {
      if (position_.x >= 0 && position_.y >= 0 && position_.x < width && position_.y < height)
      {
         return map[(int)position_.x, (int)position_.y];
      }

      return null;
   }

}
