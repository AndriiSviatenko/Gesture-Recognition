using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GestureMatcher
    {
        public static float CalculateDistance(IReadOnlyList<Vector2> points1, IReadOnlyList<Vector2> points2)
        {
            if (points1 == null || points2 == null)
            {
                throw new System.ArgumentException("null");
            }

            if (points1.Count == 0 || points2.Count == 0)
            {
                throw new System.ArgumentException("count zero");
            }

            if (points1.Count != points2.Count)
            {
                throw new System.ArgumentException("Gestures must have the same number of points.");
            }

            float sum = 0f;
            for (int i = 0; i < points1.Count; i++)
            {
                sum += Vector2.Distance(points1[i], points2[i]);
            }
            return sum / points1.Count;
        }
    }
}
