// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// public class TriangleGridTests : MonoBehaviour
// {
// //     # UpDown Triangle Co-ordinates
// // # This module provides sample code for working with equilateral triangles in an up-down configuration, i.e.
// // #        ____________
// // #       /\    /\    /
// // #      /  \  /  \  /
// // #     /____\/____\/
// // #    /\    /\    /
// // #   /  \  /  \  /
// // #  /____\/____\/

// // # Each triangle is defined by three co-ordinates, a, b, c.
// // # b determines which row the triangle is in, and a and c the two diagonals.
// // # a + b + c always sums to either 1 or 2.
// // # There are many other possible co-ordinate schemes, but this one seems to have the simplest maths.

// // # Thus, the origin is a vertex, and it has 6 triangles around it:
// // # (1, 0, 0), (1, 1, 0), (0, 1, 0), (0, 1, 1), (0, 0, 1), (1, 0, 1)

// // # To find the neighbours of a down triangle, add 1 to a co-ordinate, and subtract one for neighbours of an up triangle.

// public static (double x, double y) TriCenter((double x, double y) a, (double x, double y) b, (double x, double y) c, double edge_length)
// {
//     // Calculate square root of 3 and other constants
//     double sqrt3 = Mathf.Sqrt(3);
//     double inv_edge_length = 1.0 / edge_length;

//     // Calculate the center of the triangle
//     double x_center = 0.5 * a.x + -0.5 * c.x;
//     double y_center = -sqrt3 / 6 * a.x + sqrt3 / 3 * b.x - sqrt3 / 6 * c.x;

//     return (x_center * edge_length, y_center * edge_length);
// }

// public static bool PointsUp(int a, int b, int c)
// {
//     // Check if the sum of the three points is equal to 2
//     return a + b + c == 2;
// }
// public static List<(double, double)> TriCorners(int a, int b, int c, double edge_length)
// {
//     // Check if the triangle points up
//     if (PointsUp(a, b, c))
//     {
//         // Return the corners for the upwards pointing triangle
//         return new List<(double, double)> {
//             TriCenter(1 + a, b, c, edge_length),
//             TriCenter(a, b, 1 + c, edge_length),
//             TriCenter(a, 1 + b, c, edge_length)
//         };
//     }
//     else
//     {
//         // Return the corners for the downwards pointing triangle
//         return new List<(double, double)> {
//             TriCenter(-1 + a, b, c, edge_length),
//             TriCenter(a, b, -1 + c, edge_length),
//             TriCenter(a, -1 + b, c, edge_length)
//         };
//     }
// }

// public static (int, int, int) PickTri(double x, double y, double edge_length)
// {
//     // Calculate square root of 3 and other constants
//     double sqrt3 = Mathf.Sqrt(3);
//     double inv_edge_length = 1.0 / edge_length;

//     // Using dot product, measures which row and diagonals a given point occupies.
//     // Or equivalently, multiply by the inverse matrix to tri_center
//     // Note we have to break symmetry, using floor(...)+1 instead of ceil, in order
//     // to deal with corner vertices like (0, 0) correctly.
//     int a = (int) Mathf.Max((1 * x - sqrt3 / 3 * y) * inv_edge_length);
//     int b = (int) Mathf.Min((sqrt3 * 2 / 3 * y) * inv_edge_length) + 1;
//     int c = (int) Mathf.Max((-1 * x - sqrt3 / 3 * y) * inv_edge_length);

//     return (a, b, c);
// }



// public static List<(int, int, int)> TriNeighbours(int a, int b, int c)
// {
//     // Check if the triangle points up
//     if (PointsUp(a, b, c))
//     {
//         // Return the neighbors for the upwards pointing triangle
//         return new List<(int, int, int)> {
//             (a - 1, b    , c    ),
//             (a    , b - 1, c    ),
//             (a    , b    , c - 1)
//         };
//     }
//     else
//     {
//         // Return the neighbors for the downwards pointing triangle
//         return new List<(int, int, int)> {
//             (a + 1, b    , c    ),
//             (a    , b + 1, c    ),
//             (a    , b    , c + 1)
//         };
//     }
// }

// }
