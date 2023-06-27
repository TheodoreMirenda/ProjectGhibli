using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TJ
{
public static class Noise {

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, 
        float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[mapWidth,mapHeight];

		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;


		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
		
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}
				noiseMap [x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}
	public static Vector3[] GenerateNoiseMapHex(Vector3[] vertices, int seed, float scale, int octaves, 
        float persistance, float lacunarity, Vector2 offset, int rings) {
		Debug.Log($"offset x: {offset.x} offset y: {offset.y}");
		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float[] noiseMap = new float[vertices.Length];
		for(int j = 0; j < vertices.Length; j++)
		{
			float amplitude = 1;
			float frequency = 1;
			float noiseHeight = 0;

			for (int i = 0; i < octaves; i++)
			{
				float sampleX = (vertices[j].x*vertices.Length) / scale * frequency + octaveOffsets[i].x;
				float sampleY = (vertices[j].z*vertices.Length) / scale * frequency+ octaveOffsets[i].y;
				// Debug.Log($"Sample X: {sampleX} \n Sample Y: {sampleY}");

				float perlinValue = Mathf.PerlinNoise (sampleX, sampleY);
				noiseHeight += perlinValue * amplitude;

				amplitude *= persistance;
				frequency *= lacunarity;
			}

			if (noiseHeight > maxNoiseHeight) {
				// Debug.Log($"Max Noise Height: {noiseHeight}");
				maxNoiseHeight = noiseHeight;
			} else if (noiseHeight < minNoiseHeight) {
				// Debug.Log($"Min Noise Height: {noiseHeight}");
				minNoiseHeight = noiseHeight;
			}

			noiseMap[j] = noiseHeight;
		}

		for(int j = 0; j < vertices.Length; j++)
		{
			//wtf is this
			float noise = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap[j]);
			// Debug.Log($"noise: {noise}");
			vertices[j] = new Vector3(vertices[j].x, noise, vertices[j].z);
		}

		return vertices;
	}
	public static float GenerateGlobalNoiseMapHex(Vector2 vertice, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		
		//seeded random number generator
		SeededRandom.Init(seed);

		//create random offsets for each octave
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = SeededRandom.Range (-100000, 100000) + offset.x;
			float offsetY = SeededRandom.Range (-100000, 100000) + offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);
		}

		//check to prevent divide by 0
		noiseScale = noiseScale <= 0 ? 0.0001f : noiseScale;

		// float maxNoiseHeight = float.MinValue;
		// float minNoiseHeight = float.MaxValue;
	
		float amplitude = 1;
		float frequency = 1;
		float noiseHeight = 0;

		for (int i = 0; i < octaves; i++)
		{
			float sampleX = vertice.x / noiseScale * frequency + octaveOffsets[i].x;
			float sampleY = vertice.y / noiseScale * frequency+ octaveOffsets[i].y;

			float perlinValue = Mathf.PerlinNoise (sampleX, sampleY);
			noiseHeight += perlinValue * amplitude;

			amplitude *= persistance;
			frequency *= lacunarity;
		}

		//massaging the noise values to be between min and max
		// maxNoiseHeight = (noiseHeight > maxNoiseHeight) ? maxNoiseHeight : noiseHeight;
		// minNoiseHeight = (noiseHeight < minNoiseHeight) ? minNoiseHeight : noiseHeight;

		return noiseHeight;
	}
}
}