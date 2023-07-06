using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiscSampling {

	public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30, int desiredSpawnPoints = 0) {
		float cellSize = radius/Mathf.Sqrt(2);
        string coords="1016,2448,1010,2529,1146,2578,1369,2567,1792,2524,1836,2453,1586,2333,1809,2111,2059,2046,2347,2051,2520,2105,2423,2198,2504,2192,2499,2246,2575,2279,2580,2322,2662,2366,2743,2333,2819,2355,2928,2322,2857,2268,2873,2225,3004,2176,2911,2056,3009,1953,3118,1867,3096,1747,3004,1715,3020,1633,2868,1611,2971,1503,3036,1411,3107,1356,3085,1291,2933,1259,2841,1232,2922,1161,2906,1096,2819,1058,2667,884,2526,732,2423,662,2227,629,1945,580,1782,559,1640,553,1483,521,1391,466,1434,385,1450,304,1380,168,1217,157,1075,206,1005,282,1021,374,1103,396,1119,483,956,564,842,624,663,700,358,846,244,955,315,1020,293,1069,223,1134,348,1177,462,1210,462,1313,576,1389,663,1465,608,1585,592,1672,690,1786,815,1851,880,1889,869,1948,972,1992,1141,2046,1179,2117,1048,2182,826,2231,717,2279,907,2334,1070,2377";
        string[] coordsArray = coords.Split(',');

		int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize)];
		List<Vector2> points = new List<Vector2>();
		List<Vector2> spawnPoints = new List<Vector2>();
		spawnPoints.Add(sampleRegionSize/2);

		while (spawnPoints.Count > 0) {
			int spawnIndex = Random.Range(0,spawnPoints.Count);
			Vector2 spawnCentre = spawnPoints[spawnIndex];
			bool candidateAccepted = false;

			for (int i = 0; i < numSamplesBeforeRejection; i++)
			{
                if(desiredSpawnPoints > 0 && points.Count >= desiredSpawnPoints) {
                    return points;
                }
				float angle = Random.value * Mathf.PI * 2;
				Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
				Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2*radius);
				if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) {
					points.Add(candidate);
					spawnPoints.Add(candidate);
					grid[(int)(candidate.x/cellSize),(int)(candidate.y/cellSize)] = points.Count;
					candidateAccepted = true;
					break;
				}
			}
			if (!candidateAccepted) {
				spawnPoints.RemoveAt(spawnIndex);
			}

		}

		return points;
	}

	static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
		if (candidate.x >=0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y) {
			int cellX = (int)(candidate.x/cellSize);
			int cellY = (int)(candidate.y/cellSize);
			int searchStartX = Mathf.Max(0,cellX -2);
			int searchEndX = Mathf.Min(cellX+2,grid.GetLength(0)-1);
			int searchStartY = Mathf.Max(0,cellY -2);
			int searchEndY = Mathf.Min(cellY+2,grid.GetLength(1)-1);

			for (int x = searchStartX; x <= searchEndX; x++) {
				for (int y = searchStartY; y <= searchEndY; y++) {
					int pointIndex = grid[x,y]-1;
					if (pointIndex != -1) {
						float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
						if (sqrDst < radius*radius) {
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}
}