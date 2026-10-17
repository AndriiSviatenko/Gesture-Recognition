using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GestureNormalizer
    {
        public static List<Vector2> Center(IReadOnlyList<Vector2> points)
        {
            var res = new List<Vector2>();
            if (points == null || points.Count == 0)
            {
                return new List<Vector2>();
            }
            else
            {
                res.Capacity = (points.Count);
            }

            Vector2 sum = Vector2.zero;
            foreach (var point in points)
            {
                sum += point;
            }
            Vector2 center = sum / points.Count;
            foreach (var point in points)
            {
                res.Add(point - center);
            }
            return res;
        }

        public static List<Vector2> NormalizeScale(IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count == 0)
            {
                return new List<Vector2>();
            }

            float minX = points[0].x;
            float maxX = points[0].x;
            float minY = points[0].y;
            float maxY = points[0].y;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].x < minX)
                {
                    minX = points[i].x;
                }
                if (points[i].x > maxX)
                {
                    maxX = points[i].x;
                }
                if (points[i].y < minY)
                {
                    minY = points[i].y;
                }
                if (points[i].y > maxY)
                {
                    maxY = points[i].y;
                }
            }

            var weight = (maxX - minX);
            var height = (maxY - minY);

            var wOrigin = weight > height ? weight : height;

            if (wOrigin < 0.00001f)
            {
                return new List<Vector2>();
            }

            var res = new List<Vector2>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                var newX = points[i].x / wOrigin;
                var newY = points[i].y / wOrigin;
                res.Add(new Vector2(newX, newY));
            }
            return res;
        }
    }
}
