using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GestureResampler
    {
        public static float CalculatePathLength(IReadOnlyList<Vector2> points)
        {
            float length = 0f;

            for (int i = 1; i < points.Count; i++)
            {
                length += Vector2.Distance(points[i], points[i - 1]);
            }

            return length;
        }

        public static Vector2 PointAlongSegment(Vector2 start, Vector2 end, float distanceFromStart)
        {
            var dist = Vector2.Distance(start, end);

            if (dist < 0.00001f)
            {
                return start;
            }

            var step = distanceFromStart / dist;

            return Vector2.Lerp(start, end, step);
        }

        public static List<Vector2> Resample(IReadOnlyList<Vector2> points, int targetCount)
        {
            if (targetCount < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(targetCount));
            }

            var res = new List<Vector2>(targetCount);
            if (points == null || points.Count == 0)
            {
                return res;
            }

            float length = CalculatePathLength(points);

            if (length < 0.00001f)
            {
                return res;
            }

            float traversed = 0f;
            float step = 0f;
            float segmentLength = 0f;
            float segmentEndDist = 0f;

            step = length / (targetCount - 1);
            float nextDist = step;
            res.Add(points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                segmentLength = Vector2.Distance(points[i], points[i - 1]);
                segmentEndDist = traversed + segmentLength;
                while (nextDist <= segmentEndDist && res.Count < targetCount - 1)
                {
                    var start = points[i - 1];
                    var end = points[i];

                    var newDist = nextDist - traversed;
                    var point = PointAlongSegment(start, end, newDist);
                    res.Add(point);
                    nextDist += step;
                }
                traversed = segmentEndDist;
            }
            res.Add(points[points.Count - 1]);
            return res;
        }

    }
}
