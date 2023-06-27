using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;

namespace TJ
{
public enum MeshGenerationShape { Square, Hexagon };
public static class MeshGenerator {
	public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshGenerationShape shape,  float heightMultiplier, AnimationCurve heightCurve, int rings)
	{
		if(shape == MeshGenerationShape.Square) {
			return GenerateSquareTerrainMesh(heightMap);
		} else if(shape == MeshGenerationShape.Hexagon) {
			return GenerateHexagonTerrainMesh(heightMap, heightMultiplier, heightCurve, rings);
		}
		return null;
	}
	public static MeshData GenerateSquareTerrainMesh(float[,] heightMap)
	{
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		MeshData meshData = new MeshData();
		meshData.SetUpMeshData (width, height);
		int vertexIndex = 0;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

                float offset = ShouldOffset(x) ? -0.25f : 0.25f;
				meshData.vertices [vertexIndex] = new Vector3 ((topLeftX + x)*Mathf.Sqrt(3)/2, 0, topLeftZ - y + offset);
                // Debug.Log($"meshdata.vertices[{vertexIndex}] = {meshData.vertices[vertexIndex]}");

				meshData.uvs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

                //ignore the last row and column of vertices
				if (x < width - 1 && y < height - 1)
                {
                    // Debug.Log($"added triangle: {vertexIndex}, {vertexIndex + width + 1}, {vertexIndex + width}");
                    if(x%2 == 0) {
                        meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    } else {
                        // int A = vertexIndex, C = vertexIndex + width + 1, D = vertexIndex + width, B = vertexIndex + 1;
                        // We have to use a different layout for triangles below even vertex rows: ACD and ADB.
                        // https://catlikecoding.com/unity/tutorials/procedural-meshes/triangle-grid/
                        // var tA = int3(iA, iC, iD);
                        // var tB = int3(iA, iD, iB);

                        meshData.AddTriangle (vertexIndex, vertexIndex + 1, vertexIndex + width);
                        meshData.AddTriangle (vertexIndex + width + 1, vertexIndex + width, vertexIndex + 1);
                    }
				}
				vertexIndex++;
			}
		}

		return meshData;
	}
	public static MeshData GenerateHexagonTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int rings)
	{
		int vertexIndex = 0;
		int ringVerticies = 0;
		int triangles = 0;
		MeshData meshData = new MeshData();
		
		//int ringVertices = Enumerable.Range(0, rings + 1).Sum(i => 6 * i);
		for(int i = 0; i<=rings; i++)
			ringVerticies += 6*i;

		triangles = (int)Mathf.Pow(rings, 2) * 6;
		
		meshData.vertices = new Vector3[1+(ringVerticies)];
		meshData.uvs = new Vector2[meshData.vertices.Length];
        meshData.triangle_Origional = new Vector3[triangles];
		meshData.triangles = new int[meshData.triangle_Origional.Length*3];

		List<Face> faces = new List<Face>();

		//generate vertices
		for(int ring = 1; ring <= rings; ring++)
		{
			//centerpoint
			if(ring==1)
			{
				Vector3 point0 = GetPoint(0, 1, 0);
				meshData.vertices [0] = new Vector3 (point0.x, 0, point0.z);
				meshData.uvs [0] = new Vector2 (0, 0);
				vertexIndex+=1;
			}

			for(int i = 0; i < 6; i++)
			{
				Vector3 point1 = GetPoint(ring, 1, 1-i);
				meshData.vertices [vertexIndex] = new Vector3 (point1.x, 0, point1.z);
				meshData.uvs [vertexIndex] = new Vector2 (0, 0);
				meshData.vertices = GenerateInterVerticies(vertexIndex, ring, meshData.vertices, meshData.uvs, 1-i);
				vertexIndex+=(ring-1)+1;
			}
		}

		//generate triangles
		for(int ring = 1; ring <= rings; ring++)
		{
			int startIndex = 0;

			// startIndex = (ring-1)*6+1;
			for(int i = 0; i < ring; i++)
			{
				if(i==0)
					startIndex +=1;
				else
					startIndex += 6*i;
			}

			int trianglesToAdd = 6*ring;
			int pointsToAddCount = ring-1;
			int innerVertex = (ring-1)*6;
			int itterations = trianglesToAdd+startIndex;
			int pointNumber = 0;


			if(ring==1)
			{
				//iterates through 1-6
				for(int i = startIndex; i < itterations; i++)
				{
					// Debug.Log($"i: {i}");
					if(i%trianglesToAdd != 0)
					{
						meshData.AddTriangle(innerVertex, i, i+1);

						//add an outer triangle
						if(ring!=rings)
							meshData.AddTriangle(i+1, i, GetOuterPoint(i, ring, pointNumber));
					}
					else
					{
						meshData.AddTriangle(innerVertex, i, startIndex);

						if(ring!=rings)
							meshData.AddTriangle(startIndex, i,  GetOuterPoint(i, ring, pointNumber));
					}
					pointNumber++;
				}
			}
			else
			{
				innerVertex = startIndex - 6*(ring-1);
				// Debug.Log($"startIndex: {startIndex}, innerVertex: {innerVertex}");
				// itterations = 12;
				// Debug.Log($"innerVertex: {innerVertex}, itterations: {itterations}, startIndex: {startIndex}");

				for(int i = startIndex; i < itterations; i++)
				{
					// Debug.Log($"i: {i}");
					if(i%(itterations-1) != 0)
					{
						// Debug.Log($"Give it up for: {i}");
						// Debug.Log($"innerVertex: {innerVertex}");
						meshData.AddTriangle(innerVertex, i, i+1);

						//add an outer triangle
						if(ring!=rings)
							meshData.AddTriangle(i+1, i, GetOuterPoint(i, ring, pointNumber));
					}
					else
					{
						innerVertex = 0;
						for(int k = 0; k < ring-1; k++)
						{
							if(k==0)
								innerVertex +=1;
							else
								innerVertex += 6*k;
						}
						meshData.AddTriangle(innerVertex, i, startIndex);
						// Debug.Log($"yo hoe: {innerVertex}, {i}, {startIndex}");
						// Debug.Log($"this is for 18");
						// Debug.Log($"innerVertex: {innerVertex}, i: {i}, startIndex: {startIndex}");

						if(ring!=rings)
							meshData.AddTriangle(startIndex, i,  GetOuterPoint(i, ring, pointNumber));
					}

					if(i%ring==0)
					{
					}
					else
					{
						//move to next inner vertex
						// Debug.Log($"not innerVertex: {innerVertex}");
						innerVertex++;
					}
					pointNumber++;
				}
			}
			// Debug.Log($"done ring: {ring}");
		}
		
		return meshData;
	}
	public static int GetOuterPoint(int i, int ring, int pointNumber)
	{
		int outerPoint = 0;
		// int pointsToSkipby = ring+1;
		// outerPoint = (ring*6)+pointsToSkipby*i;
		//outer points = 18
		//corners to skip = 6
		//starting index = 19
		//skip 19, hit 20 and 21, skip 22
		
		//get next ring's starting index
		int nextRingStartIndex = 0;
		for(int j = 0; j <= ring; j++)
		{
			if(j==0)
				nextRingStartIndex +=1;
			else
				nextRingStartIndex += 6*j;
		}
		// Debug.Log($"nextRingStartIndex: {nextRingStartIndex}");
		//skip every 3rd point
		int numBoxes = (pointNumber+ring) / ring;
		outerPoint = nextRingStartIndex+numBoxes+pointNumber;
		// Debug.Log($"i:{i}, ring {ring}, outerPoint: {outerPoint}, skips: {numBoxes}, pointNumber: {pointNumber}");
	
		return outerPoint;
	}
	public static Vector3[] GenerateInterVerticies(int vertexNumber, int ringNumber, Vector3[] vertices, Vector2[] uvs, int x)
	{
		List<Vector3> pointsToAdd = new List<Vector3>();

		//start point
		Vector3 point1 = GetPoint(ringNumber, 1, x);
		Vector3 point2 = GetPoint(ringNumber, 1, x-1);

		//get amount of points to add
		int pointsToAddCount = ringNumber-1;

		if(pointsToAddCount == 0)
			return vertices;

		//get the distance between the points
		float distance = Vector3.Distance(point1, point2);

		//get the distance between each point
		float segmentsBetweenPoints = distance / (pointsToAddCount+1);

		//get the direction of the points
		Vector3 direction = (point2 - point1).normalized;

		//add points
		for(int i = 0; i < pointsToAddCount; i++)
		{
			Vector3 point = point1 + (direction * (segmentsBetweenPoints * (i+1)));
			pointsToAdd.Add(point);
		}

		//add points to vertices
		for(int i = 1; i <= pointsToAdd.Count; i++)
		{
			vertices[vertexNumber+i] = new Vector3 (pointsToAdd[i-1].x, 0, pointsToAdd[i-1].z);
		}
		return vertices;
	}
	private static void DrawFaces(float innerRadian, float outerRadian, float heightA, float heightB)
	{
		List<Face> faces = new List<Face>();
		
		for(int point = 0; point < 6; point++)
		{
			faces.Add(CreateFace(innerRadian, outerRadian, heightA, heightB, point));
		}
	}
	public static Face CreateFace(float innerRadian, float outerRadian, float heightA, float heightB, int point, bool reverse = false)
	{
		Vector3 pointA = GetPoint(innerRadian, heightA, point);
		Vector3 pointB = GetPoint(outerRadian, heightB, (point<5) ? point+1:0);
		Vector3 pointC = GetPoint(outerRadian, heightB, (point<0) ? point+1:0);
		Vector3 pointD = GetPoint(innerRadian, heightA, point);

		List<Vector3> vertices = new List<Vector3>{pointA, pointB, pointC, pointD};
		List<int> triangles = new List<int>{0, 1, 2, 2, 3, 0};
		List<Vector2> uvs = new List<Vector2>{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

		if(reverse)
			vertices.Reverse();

		return new Face(vertices, triangles, uvs);
	}
	public static Vector3 GetPoint(float size, float height, int index, float ring = 1)
	{
		float angleDegrees = 60/ring * index;
		float angleRadians = Mathf.PI / 180f * angleDegrees;
		return new Vector3(size * Mathf.Cos(angleRadians), height, size * Mathf.Sin(angleRadians));
	}
	public struct Face
	{
		public List<Vector3> vertices;
		public List<int> triangles;
		public List<Vector2> uvs;
		public Face (List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
		{
			this.vertices = vertices;
			this.triangles = triangles;
			this.uvs = uvs;
		}
	}

    public static bool ShouldOffset(int x) {
        return x % 2 == 0;
    }
}

[System.Serializable] public struct MeshStruct {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] triangle_Origional;
}

public class MeshData {
	public Vector3[] vertices;
	public int[] triangles;
    public Vector3[] triangle_Origional;
	public Vector2[] uvs;
	public int triangleIndex;

	public void SetUpMeshData(int meshWidth, int meshHeight) {
		vertices = new Vector3[meshWidth * meshHeight];
		uvs = new Vector2[meshWidth * meshHeight];
		triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
        triangle_Origional = new Vector3[triangles.Length/3];
	}

	public void AddTriangle(int a, int b, int c) {
        triangle_Origional[triangleIndex/3] = new Vector3(a,b,c);
		triangles [triangleIndex] = a;
		triangles [triangleIndex + 1] = b;
		triangles [triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();
		return mesh;
	}

}
}