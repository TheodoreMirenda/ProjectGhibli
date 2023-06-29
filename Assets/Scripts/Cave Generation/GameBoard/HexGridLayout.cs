using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
public static class HexGridLayout
{
    public static List<Chunk> CreateBorderingChunks(List<Chunk> chunks, int activeChunkIndex)
    {
        // Chunk activeHex = chunks[activeChunkIndex];

        for(int i = 0; i < 6; i++)
        {
            //Sqrt(3) is the distance between the center of a hexagon and the center of a neighboring hexagon 1.73205080757
            Vector2 newHexagonCentroid = GetPoint(2*Mathf.Sqrt(3), 1-i);
            bool hexagonExists = false;
            foreach(Chunk hex in chunks)
            {
                if(hex.centroid == newHexagonCentroid){
                    Debug.Log($"neighbor {i} already exists");
                    hexagonExists = true;
                    break;
                }
            }

            if(!hexagonExists)
            {
                // Debug.Log($"neighbor not found at {i}");
                Chunk newHexagon = new Chunk();
                newHexagon.centroid = newHexagonCentroid;
                newHexagon.mapData = new MapDataFinal();
                chunks.Add(newHexagon);
            }
        }
        return chunks;
    }
    public static Vector3 GetPoint(float size, int index, float ring = 1)
	{
		float angleDegrees = 60/ring * index;
		float angleRadians = Mathf.PI / 180f * angleDegrees;
		return new Vector2(size * Mathf.Sin(angleRadians), size * Mathf.Cos(angleRadians));
	}
}
[System.Serializable] public struct Chunk
{
    public Vector2 centroid;
    public Transform chunkTransform;
    public MapDataFinal mapData;
}
}
