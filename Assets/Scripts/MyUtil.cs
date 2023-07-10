using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtil
{
    public static Vector2 BezierCalCulate(float lerpValue, params Vector2[] points)
    {
        while (points.Length > 1)
        {
            Vector2[] newPoints = new Vector2[points.Length - 1];
            for (int i = 0; i < newPoints.Length; i++)
            {
                newPoints[i] = Vector2.Lerp(points[i], points[i + 1], lerpValue);
            }
            points = newPoints;
        }
        return points[0];
    }
}
