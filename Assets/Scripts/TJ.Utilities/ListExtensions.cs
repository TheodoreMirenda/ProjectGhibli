using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TJ.Utilities{
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list) 
    {
        System.Random rnd = new();
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, rnd.Next(i, list.Count));
    }
    public static void Swap<T>(this IList<T> list, int i, int j) 
    {
        (list[i], list[j]) = (list[j], list[i]);
    }
}
}