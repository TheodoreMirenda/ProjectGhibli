using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCollapse : MonoBehaviour
{
    public static List<(int, int, int)> TriNeighbours(int a, int b, int c)
    {
        // Check if the triangle points up
        if (PointsUp(a, b, c))
        {
            // Return the neighbors for the upwards pointing triangle
            return new List<(int, int, int)> {
                (a - 1, b, c),
                (a, b - 1, c),
                (a, b, c - 1)
            };
        }
        else
        {
            // Return the neighbors for the downwards pointing triangle
            return new List<(int, int, int)> {
                (a + 1, b, c),
                (a, b + 1, c),
                (a, b, c + 1)
            };
        }
    }
    public static bool PointsUp(int a, int b, int c)
    {
        // Check if the sum of the three points is equal to 2
        return a + b + c == 2;
    }
}
