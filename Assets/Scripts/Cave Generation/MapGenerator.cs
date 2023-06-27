using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TJ.Supplements;
using System.Linq;
using TMPro;
using System;
using System.Threading.Tasks;

namespace TJ
{

public class MapGenerator : MonoBehaviour {

	public bool autoUpdate, demoMode;
    public Material lineMat1, lineMat2, lineMat3;
    public Transform step0Transform, step1Transform, step2Transform;
	public enum DrawMode {NoiseMap, ColourMap, Mesh};
	public DrawMode drawMode;
    public GameObject vertexPrefab, edgePrefab, finalEdgePrefab, VertexText;
    public MeshGenerationShape meshGenerationShape;
	public int mapWidth;
	public int mapHeight;
    [Range(1,10)] public int rings;

    [Header("Noise Generation")]
	[SerializeField] private float noiseScale = 1;
	[SerializeField] private int octaves;
	[SerializeField] [Range(0,1)] private float persistance;
	[SerializeField] private float lacunarity;
	[SerializeField] private int seed;

	[HideInInspector] public Vector2 offset;
	[HideInInspector] public TerrainType[] regions;
    
    [Header("Triangle Grid Generation")]
    public MeshStruct meshStruct;
    public List<TriangleNeighborsGroups> triangleNeighborsGroups;
    public List<Vector3> nonCollapsedTriangles;
    public List<Vector3> collapsedTriangles;
    public SerializableDictionary <Vector2, List<Vector3>> triangleNeighborsGroupsDict = new SerializableDictionary<Vector2, List<Vector3>>();
     List<GameObject> vertices;
     List<GameObject> xedvertices;
     List<GameObject> vertexTexts = new List<GameObject>();

    [Header("Collapsed Quads")]
    public List<Face> faces = new List<Face>();
    public List<BabyFace> babyFaces = new List<BabyFace>();
    public List<Vector2> globalVerticies = new List<Vector2>();
    public SerializableDictionary<Vector2, bool> outerVerticies = new SerializableDictionary<Vector2, bool>();

    [Header("Show Vertex Numbers")]
    public bool showVertexNumbers = false, spawnVerticesPrefabs = false, spawnEdgesPrefabs = false, drawLines;
    // public GameObject quadPrefab;
    // public int smoothAmount;
    public float debugLineTime = 1f, debugLineTime2 = 1f;
    public float iterrations;
    public float movementAmount;
    float[,] noiseMap;
    Color[] colourMap;
    MeshData meshData;
    MapDisplay display;

    [Header("Mesh Generation Height")]
    public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;
    Dictionary<GameObject, Vector2> vertexDict = new Dictionary<GameObject, Vector2>();
    Dictionary<Vector2, GameObject> vertDict2 = new Dictionary<Vector2, GameObject>();
    private async void Start()
    {
        if(demoMode)
        {
            await GenerateMap();
            await Smooth();
        }
    }
    public async Task GenerateMap()
    {
        noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
		// CleanUp();

		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, mapWidth, mapHeight));
		} else if (drawMode == DrawMode.Mesh) 
        {
            Debug.Log($"meshdata 1");
            meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshGenerationShape, meshHeightMultiplier, meshHeightCurve, rings);
            Debug.Log($"meshdata");
            #region meshStruct for debugging values
            meshStruct.triangles = meshData.triangles;
            meshStruct.vertices = meshData.vertices;
            foreach(Vector3 vertice in meshStruct.vertices)
            {
                Debug.Log($"verts");
                globalVerticies.Add(new Vector2(vertice.x, vertice.z));
                // SpawnVertexPoint(globalVerticies.Count-1, new Vector2(vertice.x, vertice.z));
                if(demoMode)
                    await SpawnVertexAtPoint(globalVerticies.Count-1, new Vector2(vertice.x, vertice.z));
            }
            
            meshStruct.triangle_Origional = meshData.triangle_Origional;
            meshStruct.uvs = meshData.uvs;
            #endregion

            if(demoMode)
            {
                for(int i = 0; i < meshStruct.triangles.Length; i+=3)
                    await SpawnTriangle(new Vector3 (meshStruct.triangles[i], meshStruct.triangles[i+1], meshStruct.triangles[i+2]));
            }
            
            GetOuterVertices();

            CollapseTriangles();

            //draw all remaining edges
            if(demoMode)
            {
                foreach(Face face in faces)
                {
                    foreach(Vector2 edge in face.edges)
                    {
                        await DrawEdge(edge, 0.05f, 10);
                    }
                }
            }

            if(demoMode)
            {
                for(int i = 0; i < step0Transform.childCount; i++)
                    Destroy(step0Transform.GetChild(i).gameObject);

                Destroy(step1Transform.gameObject);
                await Task.Delay(1000);
            }
            
            Subdivide();

            if(demoMode)
            {
                foreach(Face face in faces)
                {
                    foreach(BabyFace babyFace in face.babyFaces)
                    {
                        if(!face.fourCorners.Contains((int)babyFace.face.x))
                            await DrawEdge(new Vector2(babyFace.face.x, face.centroidCode), 0.1f, 10);
                        if(!face.fourCorners.Contains((int)babyFace.face.y))
                            await DrawEdge(new Vector2(babyFace.face.y, face.centroidCode), 0.1f, 10);
                        if(!face.fourCorners.Contains((int)babyFace.face.z))
                            await DrawEdge(new Vector2(babyFace.face.z, face.centroidCode), 0.1f, 10);
                        if(!face.fourCorners.Contains((int)babyFace.face.w))
                            await DrawEdge(new Vector2(babyFace.face.w, face.centroidCode), 0.1f, 10);
                    }
                }
            }
            
            if(drawLines)
                OutlineFaces();

            display.DrawMesh (meshData, TextureGenerator.TextureFromColourMap (colourMap, mapWidth, mapHeight));
        }

        SaveMapData();
        return;
	}
    private void SaveMapData()
    {
        List<Vector4> facesNew = new List<Vector4>();
        foreach(BabyFace face in babyFaces)
            facesNew.Add(new Vector4(face.face.x, face.face.y, face.face.z, face.face.w));
       
        GetComponent<LandGenerator>().SaveMapData(globalVerticies, facesNew);
    }
    public void CleanUp()
    {
        Debug.Log($"cleanup");
        // noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
		colourMap = new Color[mapWidth * mapHeight];
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				float currentHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colourMap [y * mapWidth + x] = regions [i].colour;
						break;
					}
				}
			}
		}
        
		display = FindObjectOfType<MapDisplay> ();
        display.meshFilter.sharedMesh = null;
        faces.Clear();
        triangleNeighborsGroups.Clear();
        triangleNeighborsGroupsDict.Clear();
        collapsedTriangles.Clear();
        nonCollapsedTriangles.Clear();
        globalVerticies.Clear();
        outerVerticies.Clear();
        faces.Clear();
        babyFaces.Clear();

        if(vertices != null)
        {
            foreach(GameObject vertice in vertices)
                DestroyImmediate(vertice.gameObject);
            foreach(GameObject vertice in xedvertices)
                DestroyImmediate(vertice.gameObject);
            foreach(GameObject vertice in vertexTexts)
                DestroyImmediate(vertice.gameObject);

            vertices.Clear();
            xedvertices.Clear();
            vertexTexts.Clear();
        }

        for (int i = step0Transform.transform.childCount; i > 0; --i)
            DestroyImmediate(step0Transform.GetChild(0).gameObject);
        
        for (int i = step1Transform.transform.childCount; i > 0; --i)
            DestroyImmediate(step1Transform.GetChild(0).gameObject);

        for (int i = step2Transform.transform.childCount; i > 0; --i)
            DestroyImmediate(step2Transform.GetChild(0).gameObject);
    }
    public void GetOuterVertices()
    {
        int outerVertsCount = 0;
        for(int i = 0; i<rings; i++)
			outerVertsCount += 6;

        outerVerticies.Clear();

        //get the outer vertices from the end of global verticies list
        for(int i = globalVerticies.Count-1; i>=globalVerticies.Count-outerVertsCount; i--)
        {
            outerVerticies.Add(globalVerticies[i], true);
        }
    }
    public void CollapseTriangles()
    {
        //generate a list of all edges, then shuffle up the order
            foreach(Vector3 triangle in meshData.triangle_Origional)
            {
                nonCollapsedTriangles.Add(triangle);
                Debug.Log($"added new group");
                triangleNeighborsGroups.Add(new TriangleNeighborsGroups { 
                    triangle = triangle, 
                    edges = new List<Vector2> { SortEdges(triangle.x, triangle.y), 
                                                SortEdges(triangle.y, triangle.z), 
                                                SortEdges(triangle.z, triangle.x)}});
            }
            triangleNeighborsGroups.Shuffle();

            //get the edges that appear in multiple triangles
            foreach(TriangleNeighborsGroups triangle in triangleNeighborsGroups)
            {
                foreach(Vector2 edge in triangle.edges)
                {
                    if(triangleNeighborsGroupsDict.ContainsKey(edge))
                        triangleNeighborsGroupsDict[edge].Add(triangle.triangle);
                    else
                        triangleNeighborsGroupsDict.Add(edge, new List<Vector3> { triangle.triangle });
                }
            }

            //get triangles that share and edge
            List<Vector2> sharedEdges = new List<Vector2>();
            foreach(KeyValuePair<Vector2, List<Vector3>> edge in triangleNeighborsGroupsDict)
            {
                if(edge.Value.Count > 1)
                    sharedEdges.Add(edge.Key);
            }
            sharedEdges.Shuffle();

            //collapse triangles that share an edge and remove them from the list of triangles to be collapsed
            foreach(Vector2 edge in sharedEdges)
            {
                List<Vector3> triangles = triangleNeighborsGroupsDict[edge];
                if(nonCollapsedTriangles.Contains(triangles[0]) && nonCollapsedTriangles.Contains(triangles[1]))
                {
                    //adding triangles to new quad
                    faces.Add( new Face { fourCorners = new List<int> { new int() }, 
                                          triangles = new List<Vector3> { triangles[0], triangles[1] }, 
                                          centroid = new Vector2() });

                    collapsedTriangles.Add(triangles[0]);
                    collapsedTriangles.Add(triangles[1]);
                    nonCollapsedTriangles.Remove(triangles[0]);
                    nonCollapsedTriangles.Remove(triangles[1]);
                }
            }

            // add leftover triangles that were not collapsed to faces
            foreach(Vector3 triangle in nonCollapsedTriangles)
            {
                faces.Add(new Face { fourCorners = new List<int> { new int() }, 
                                     triangles = new List<Vector3> { triangle }, 
                                     centroid = new Vector2() });
            }

            //remove edges that are shared between faces, whether they are triangles or quads
            foreach(Face face in faces)
            {
                face.fourCorners.Clear();

                //add corners to faces
                foreach(Vector3 triangle in face.triangles)
                {
                    if(!face.fourCorners.Contains((int)triangle.x))
                    {
                        face.fourCorners.Add((int)triangle.x);
                    }
                    if(!face.fourCorners.Contains((int)triangle.y))
                    {
                        face.fourCorners.Add((int)triangle.y);
                    }
                    if(!face.fourCorners.Contains((int)triangle.z))
                    {
                        face.fourCorners.Add((int)triangle.z);
                    }

                    //add edges to faces
                    face.edges.Add(new Vector2 { x = triangle.x, y = triangle.y });
                    face.edges.Add(new Vector2 { x = triangle.y, y = triangle.z });
                    face.edges.Add(new Vector2 { x = triangle.z, y = triangle.x });
                }

                //find overlapping edges
                for(int i = 0; i < face.edges.Count; i++)
                {
                    for(int j = 0; j < face.edges.Count; j++)
                    {
                        if(i != j)
                        {
                            Vector2 edge1 = SortEdges(face.edges[i].x, face.edges[i].y);
                            Vector2 edge2 = SortEdges(face.edges[j].x, face.edges[j].y);
                            
                            if(edge1 == edge2)
                            {
                                // Debug.Log($"overlapping edge found: {face.edges[i]}");
                                face.overlappingEdges.Add(face.edges[i]);
                                
                            }
                        }
                    }
                }

                //remove overlapping edges
                face.edges.RemoveAll(edge => face.overlappingEdges.Contains(edge));
            }

    }
    public async Task Smooth()
    {
        for(int j = 0; j < iterrations; j++)
        {
            //smooth them out
            SmoothBabyFaces();

            if(demoMode)
            {
                //destroy all child objects under step2Transform 
                for (int i = step2Transform.transform.childCount; i > 0; --i)
                    DestroyImmediate(step2Transform.GetChild(0).gameObject);

                await Task.Delay(1);
                Debug.Log($"smooth iterration: {j}");
            }

            //move the corners of the quads to the new vertice positions
            for(int i = 0; i < babyFaces.Count; i++)
                MoveCorners(i);
        }
        //add new triangles and verticies to the mesh
        meshData = ReCalculateTriangles(meshData, noiseMap);

        meshData.vertices = Noise.GenerateNoiseMapHex (meshData.vertices, seed, noiseScale, octaves, persistance, lacunarity, offset, rings);

        //assign grid values to all verticies
        for(int i = 0; i<meshData.vertices.Length; i++)
            meshData.vertices[i] = new Vector3(meshData.vertices[i].x, meshHeightCurve.Evaluate(meshData.vertices[i].y) * meshHeightMultiplier,meshData.vertices[i].z);

        //finally, draw the mesh
        display.DrawMesh (meshData, TextureGenerator.TextureFromColourMap (colourMap, mapWidth, mapHeight));
        if(drawLines)
            OutlineFaces();
        
        return;
    }
    public void OutlineFaces()
    {
        foreach(BabyFace babyFace in babyFaces)
        {
            Vector2 a = globalVerticies[(int)babyFace.face.x];
            Vector2 b = globalVerticies[(int)babyFace.face.y];
            Vector2 c = globalVerticies[(int)babyFace.face.z];
            Vector2 d = globalVerticies[(int)babyFace.face.w];

            Debug.DrawLine(new Vector3(a.x, 0, a.y), new Vector3(b.x, 0, b.y), Color.cyan, debugLineTime2);
            Debug.DrawLine(new Vector3(b.x, 0, b.y), new Vector3(c.x, 0, c.y), Color.cyan, debugLineTime2);
            Debug.DrawLine(new Vector3(c.x, 0, c.y), new Vector3(d.x, 0, d.y), Color.cyan, debugLineTime2);
            Debug.DrawLine(new Vector3(d.x, 0, d.y), new Vector3(a.x, 0, a.y), Color.cyan, debugLineTime2);
        }
    }
    public void Subdivide()
    {
        //find centroid of each quad
        foreach(Face face in faces)
        {
            Vector2[] vertices = new Vector2[face.fourCorners.Count];
            for(int i = 0; i < face.fourCorners.Count; i++)
            {
                vertices[i] = new Vector2(meshStruct.vertices[face.fourCorners[i]].x, meshStruct.vertices[face.fourCorners[i]].z);
            }
            face.vertices = new List<Vector2>(vertices);
            face.centroid = GetCentroid(vertices);

            globalVerticies.Add(new Vector2(face.centroid.x, face.centroid.y));
            SpawnVertexPoint(globalVerticies.Count-1, new Vector2(face.centroid.x, face.centroid.y));
            face.centroidCode = globalVerticies.Count - 1;

            face.fourCorners.Add(globalVerticies.Count - 1);

            if(spawnVerticesPrefabs)
            {
                xedvertices.Add(
                    Instantiate(vertexPrefab, 
                    new Vector3(face.centroid.x, 0, face.centroid.y), 
                    Quaternion.identity, this.transform));
            }
        }

        //subdivide each quad by adding a new vertice at the centroid of each quad
        foreach(Face face in faces)
            SubDivideFace(face);
        
        //calculate the new triangles that were created by subdividing the quads
        CalculateBabyFaces();
    }
    public void SubDivideFace(Face face)
    {
        //new vertices
        Vector2 ab = Vector2.Lerp(globalVerticies[(int)face.edges[0].x], globalVerticies[(int)face.edges[0].y], 0.5f);
        // Debug.Log($"ab: {ab}");
        int abCode  = CheckIfVertexExists(ab);
        if(abCode == -1)
        {
            globalVerticies.Add(new Vector2(ab.x, ab.y));
            OuterVerticeCheck((int)face.edges[0].x, (int)face.edges[0].y, ab);

            if(spawnEdgesPrefabs)
                xedvertices.Add(Instantiate(edgePrefab, new Vector3(ab.x, 0, ab.y), Quaternion.identity));

            abCode = globalVerticies.Count - 1;
            SpawnVertexPoint(abCode, ab);
        }
        face.newEdges.Add(new Vector2 { x = (int)face.edges[0].x, y= abCode });
        face.newEdges.Add(new Vector2 { x = abCode, y = (int)face.edges[0].y });

        Vector2 bc = Vector2.Lerp(globalVerticies[(int)face.edges[1].x], globalVerticies[(int)face.edges[1].y], 0.5f);
        // Debug.Log($"bc: {bc}");
        int bcCode = CheckIfVertexExists(bc);
        if(bcCode == -1)
        {
            globalVerticies.Add(new Vector2(bc.x, bc.y));
            OuterVerticeCheck((int)face.edges[1].x, (int)face.edges[1].y, bc);

            if(spawnEdgesPrefabs)
                xedvertices.Add(Instantiate(edgePrefab, new Vector3(bc.x, 0, bc.y), Quaternion.identity));

            bcCode = globalVerticies.Count - 1;
            SpawnVertexPoint(bcCode, bc);
        }
        face.newEdges.Add(new Vector2 { x = (int)face.edges[1].x, y = bcCode });
        face.newEdges.Add(new Vector2 { x = bcCode, y = (int)face.edges[1].y });

        Vector2 cd = Vector2.Lerp(globalVerticies[(int)face.edges[2].x], globalVerticies[(int)face.edges[2].y], 0.5f);
        // Debug.Log($"cd: {cd}");
        int cdCode = CheckIfVertexExists(cd);
        if(cdCode == -1)
        {
            globalVerticies.Add(new Vector2(cd.x, cd.y));
            OuterVerticeCheck((int)face.edges[2].x, (int)face.edges[2].y, cd);

            if(spawnEdgesPrefabs)
                xedvertices.Add(Instantiate(edgePrefab, new Vector3(cd.x, 0, cd.y), Quaternion.identity));

            cdCode = globalVerticies.Count - 1;
            SpawnVertexPoint(cdCode, cd);
        }
        face.newEdges.Add(new Vector2 { x = (int)face.edges[2].x, y = cdCode });
        face.newEdges.Add(new Vector2 { x = cdCode, y = (int)face.edges[2].y });

        if(face.edges.Count == 4)
        {
            Vector2 da = Vector2.Lerp(globalVerticies[(int)face.edges[3].x], globalVerticies[(int)face.edges[3].y], 0.5f);
            // Debug.Log($"da: {da}");
            int daCode = CheckIfVertexExists(da);
            if(daCode == -1)
            {
                globalVerticies.Add(new Vector2(da.x, da.y));
                OuterVerticeCheck((int)face.edges[3].x, (int)face.edges[3].y, da);

                if(spawnEdgesPrefabs)
                    xedvertices.Add(Instantiate(edgePrefab, new Vector3(da.x, 0, da.y), Quaternion.identity));

                daCode = globalVerticies.Count - 1;
                SpawnVertexPoint(daCode, da);
            }
            face.newEdges.Add(new Vector2 { x = (int)face.edges[3].x, y = daCode });
            face.newEdges.Add(new Vector2 { x = daCode, y = (int)face.edges[3].y });
        }
    }
    public int CheckIfVertexExists(Vector2 vertex)
    {
        for(int i = 0; i < globalVerticies.Count; i++)
        {
            if(globalVerticies[i] == vertex)
            {
                // Debug.Log($"vertex exists at {i}");
                // Debug.Log($"There are currently {globalVerticies.Count} verticies");
                return i;
            }
        }
        return -1;
    }
    public MeshData ReCalculateTriangles(MeshData meshData, float[,] noiseMap)
    {
        meshData.vertices = new Vector3[globalVerticies.Count];
        meshData.uvs = new Vector2[globalVerticies.Count];
        // meshData.vertices = new Vector3[(int)(noiseMap.GetLength(0) * noiseMap.GetLength(1) * 4f)];
        // meshData.uvs = new Vector2[(int)(noiseMap.GetLength(0) * noiseMap.GetLength(1) * 4f)];
        
        for(int i = 0; i < globalVerticies.Count; i++)
        {
            // Debug.Log($"broke at {i}");
            meshData.vertices[i] = new Vector3(globalVerticies[i].x, 0, globalVerticies[i].y);
        }

        if(meshGenerationShape== MeshGenerationShape.Square)
        {
            meshData.triangles = new int[(noiseMap.GetLength(0) -1)*(noiseMap.GetLength(1) -1)*6*6];
        }
        else if(meshGenerationShape == MeshGenerationShape.Hexagon)
        {
            int x =0;
            foreach(Face face in faces)
                foreach(Vector2 edge in face.newEdges)
                    x+=3;

            // Debug.Log($"There are {x} triangles");
            meshData.triangles = new int[x];
        }
        
        meshData.triangle_Origional = new Vector3[meshData.triangles.Length/3];
        meshData.triangleIndex = 0;
        // Debug.Log($"There are {meshData.triangles.Length} triangles, and {meshData.triangle_Origional.Length} origional triangles");
        foreach(Face face in faces)
        {
            for(int i = 0; i < face.newEdges.Count; i++)
            {
                int a = (int)face.newEdges[i].x;
                int b = (int)face.newEdges[i].y;
                int c = (int)face.centroidCode;
                meshData.AddTriangle(a,b,c);
            }
        }
        return meshData;
    }
    public async void CalculateBabyFaces()
    {
        foreach(Face face in faces)
        {
            for(int i = 0; i < face.fourCorners.Count-1; i+=1)
            {
                //get new edges that contian that corner
                float x = 0, y = 0;
                foreach(Vector2 edge in face.newEdges)
                {
                    if(edge.x == face.fourCorners[i])
                        y = edge.y;
                    if(edge.y == face.fourCorners[i])
                        x = edge.x;
                }
                Vector4 newFace = new Vector4(face.fourCorners[i], x, face.centroidCode,y);

                babyFaces.Add(new BabyFace { face = newFace });
                face.babyFaces.Add(new BabyFace { face = newFace });

                // if(demoMode)
                //     await DisplayBabyface(newFace);
            }
        }
    }
    public void SmoothBabyFaces()
    {
        for(int t = 0; t < babyFaces.Count; t++)
        {
            Vector4 babyFace = babyFaces[t].face;
            babyFaces[t].lowestForce = Mathf.Infinity;
            babyFaces[t].forcesToApply.Clear();

            Vector2 a = globalVerticies[(int)babyFace.x];
            Vector2 b = globalVerticies[(int)babyFace.y];
            Vector2 c = globalVerticies[(int)babyFace.z];
            Vector2 d = globalVerticies[(int)babyFace.w];

            if(drawLines)
            {
                Debug.DrawLine(new Vector3(a.x, 0, a.y), new Vector3(b.x, 0, b.y), Color.green, debugLineTime);
                Debug.DrawLine(new Vector3(b.x, 0, b.y), new Vector3(c.x, 0, c.y), Color.green, debugLineTime);
                Debug.DrawLine(new Vector3(c.x, 0, c.y), new Vector3(d.x, 0, d.y), Color.green, debugLineTime);
                Debug.DrawLine(new Vector3(d.x, 0, d.y), new Vector3(a.x, 0, a.y), Color.green, debugLineTime);
            }

            //Get area of the quad
            float area = Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
            // Debug.Log($"area is {area}");
            // float 
            area = 0.10825f;

            //get half of the diagonal of a square with the same area
            float halfDiagonal = Mathf.Sqrt(area);
            
            //get the centroid of the quad
            Vector2 centroid = new Vector2(
                (a.x + b.x + c.x + d.x) / 4f,
                (a.y + b.y + c.y + d.y) / 4f
            );

            // if(drawLines)
            //     Debug.DrawRay(new Vector3(centroid.x, 0, centroid.y), Vector3.up, Color.blue, debugLineTime);

            Vector2[] corners = new Vector2[4] { a, b, c, d };

            bool OuterVerticeCheck(int x)
            {
                if(outerVerticies.ContainsKey(globalVerticies[x]))
                    return true;

                return false;
            }

            bool[] outerV = new bool[4] { OuterVerticeCheck((int)babyFace.x), OuterVerticeCheck((int)babyFace.y), 
                OuterVerticeCheck((int)babyFace.z), OuterVerticeCheck((int)babyFace.w) };

            // Debug.Log($"outerV is {outerV[0]}, {outerV[1]}, {outerV[2]}, {outerV[3]}");

            if(!outerV[0] && !outerV[1] && !outerV[2] && !outerV[3])
            {
                // Debug.Log("All inner verticies");
            }
            else
            {
                // Debug.Log($"There are one or more outer verticies");
                //only calc forces for outer verticies
                List<Vector2> outerCorners = new List<Vector2>();
                for(int i = 0; i < corners.Length; i++)
                {
                    if(outerV[i])
                        outerCorners.Add(corners[i]);
                }
                corners = outerCorners.ToArray();
                // Debug.Log($"There are {corners.Length} outer corners");
            }

            for(int i = 0; i < corners.Length; i++)
            {
                //get the direction from the centroid to the corner
                Vector2 direction = corners[i] - centroid;

                //get the direction 90 degrees to the corner
                Vector2 direction90 = new Vector2(-direction.y, direction.x);

                //get the new position of the corner
                Vector2 newA = centroid + direction.normalized * halfDiagonal;
                Vector2 newB = centroid + direction90.normalized * halfDiagonal;
                Vector2 newC = centroid - direction.normalized * halfDiagonal;
                Vector2 newD = centroid - direction90.normalized * halfDiagonal;
                
                if(drawLines)
                {
                    Vector3 newA3 = new Vector3(newA.x, 0, newA.y);
                    Vector3 newB3 = new Vector3(newB.x, 0, newB.y);
                    Vector3 newC3 = new Vector3(newC.x, 0, newC.y);
                    Vector3 newD3 = new Vector3(newD.x, 0, newD.y);

                    Debug.DrawLine(newA3, newB3, Color.red, debugLineTime);
                    Debug.DrawLine(newB3, newC3, Color.red, debugLineTime);
                    Debug.DrawLine(newC3, newD3, Color.red, debugLineTime);
                    Debug.DrawLine(newD3, newA3, Color.red, debugLineTime);
                }

                //get the closest new corner from each of the origional corners
                Vector2 closestA = FindClosest(a, newA, newB, newC, newD);
                Vector2 closestB = FindClosest(b, newA, newB, newC, newD);
                Vector2 closestC = FindClosest(c, newA, newB, newC, newD);
                Vector2 closestD = FindClosest(d, newA, newB, newC, newD);

                //get the directional force needed to move the corner to the new position
                Vector2 forceA = closestA - a;
                Vector2 forceB = closestB - b;
                Vector2 forceC = closestC - c;
                Vector2 forceD = closestD - d;

                //get the direction the force must be applied in
                Vector2 directionA = forceA.normalized;
                Vector2 directionB = forceB.normalized;
                Vector2 directionC = forceC.normalized;
                Vector2 directionD = forceD.normalized;

                //get total force needed to move the quad to the new position, the absolute value of each force
                Vector2 totalForce = new Vector2(
                    Mathf.Abs(forceA.x) + Mathf.Abs(forceB.x) + Mathf.Abs(forceC.x) + Mathf.Abs(forceD.x),
                    Mathf.Abs(forceA.y) + Mathf.Abs(forceB.y) + Mathf.Abs(forceC.y) + Mathf.Abs(forceD.y)
                );

                if(totalForce.magnitude < babyFaces[t].lowestForce)
                {
                    // Debug.Log($"lowest force: {totalForce.magnitude} i: {i}");
                    babyFaces[t].lowestForce = totalForce.magnitude;

                    babyFaces[t].forcesToApply.Clear();
                    babyFaces[t].forcesToApply.Add(forceA);
                    babyFaces[t].forcesToApply.Add(forceB);
                    babyFaces[t].forcesToApply.Add(forceC);
                    babyFaces[t].forcesToApply.Add(forceD);
                }
            }
        }
    }
    public Vector2 FindClosest(Vector2 og, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        Vector2[] points = new Vector2[4] {a, b, c, d };
        Vector2 closest = points[0];
        float closestDistance = Vector2.Distance(og, points[0]);
        for(int i = 1; i < points.Length; i++)
        {
            float distance = Vector2.Distance(og, points[i]);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = points[i];
            }
        }
        return closest;
    }
    public async void MoveCorners(int babyFaceIndex)
    {
        BabyFace babf = babyFaces[babyFaceIndex];
        Vector2 a = globalVerticies[(int)babf.face.x];
        Vector2 b = globalVerticies[(int)babf.face.y];
        Vector2 c = globalVerticies[(int)babf.face.z];
        Vector2 d = globalVerticies[(int)babf.face.w];
        
        //appling less force than needed to prevent the quad from moving too fast
        if(!outerVerticies.ContainsKey(globalVerticies[(int)babf.face.x]))
            globalVerticies[(int)babf.face.x] += babf.forcesToApply[0]/movementAmount;
        if(!outerVerticies.ContainsKey(globalVerticies[(int)babf.face.y]))
            globalVerticies[(int)babf.face.y] += babf.forcesToApply[1]/movementAmount;
        if(!outerVerticies.ContainsKey(globalVerticies[(int)babf.face.z]))
            globalVerticies[(int)babf.face.z] += babf.forcesToApply[2]/movementAmount;
        if(!outerVerticies.ContainsKey(globalVerticies[(int)babf.face.w]))
            globalVerticies[(int)babf.face.w] += babf.forcesToApply[3]/movementAmount;

        if(demoMode)
        {
            DrawFinalEdge(new Vector2(babf.face.x, babf.face.y), 0.2f, 0);
            DrawFinalEdge(new Vector2(babf.face.y, babf.face.z), 0.2f, 0);
            DrawFinalEdge(new Vector2(babf.face.z, babf.face.w), 0.2f, 0);
            DrawFinalEdge(new Vector2(babf.face.w, babf.face.x), 0.2f, 0);
        }
        // Debug.DrawLine(new Vector3(a.x, 0, a.y), new Vector3(globalVerticies[(int)babf.face.x].x, 0, globalVerticies[(int)babf.face.x].y), Color.blue, debugLineTime, false);
        // Debug.DrawLine(new Vector3(b.x, 0, b.y), new Vector3(globalVerticies[(int)babf.face.y].x, 0, globalVerticies[(int)babf.face.y].y), Color.blue, debugLineTime, false);
        // Debug.DrawLine(new Vector3(c.x, 0, c.y), new Vector3(globalVerticies[(int)babf.face.z].x, 0, globalVerticies[(int)babf.face.z].y), Color.blue, debugLineTime, false);
        // Debug.DrawLine(new Vector3(d.x, 0, d.y), new Vector3(globalVerticies[(int)babf.face.w].x, 0, globalVerticies[(int)babf.face.w].y), Color.blue, debugLineTime, false);
    }
    private void IsThisAnOuterVertice(int index)
    {
        if(!outerVerticies.ContainsKey(globalVerticies[index]))
        {
            outerVerticies.Add(globalVerticies[index], true);
        }
    }
    private void OuterVerticeCheck(int x, int y, Vector2 a)
    {
        if(outerVerticies.ContainsKey(globalVerticies[x]) && outerVerticies.ContainsKey(globalVerticies[y]))
        {
            // Debug.Log($"adding outer vertice: {a}");
            outerVerticies.Add(a, true);
        }
    }
    public List<int> Sort(List<int> pointers)
    {
        List<Vector2> points = new List<Vector2>();
        foreach(int pointer in pointers)
        {
            points.Add(new Vector2(globalVerticies[pointer].x, globalVerticies[pointer].y));
        }

        Vector2 centroid = new Vector2(
            points.Average(p => p.x),
            points.Average(p => p.y)
        );

        // points.Sort((a, b) => Mathf.Atan2(a.y - centroid.y, a.x - centroid.x)
        //     .CompareTo(Mathf.Atan2(b.y - centroid.y, b.x - centroid.x)));

        points.Sort((a, b) => Mathf.Atan2(b.y - centroid.y, b.x - centroid.x)
            .CompareTo(Mathf.Atan2(a.y - centroid.y, a.x - centroid.x)));

        for(int i = 0; i < points.Count; i++)
        {
            pointers[i] = globalVerticies.IndexOf(new Vector2(points[i].x, points[i].y));
        }
        return pointers;
    }
    public Vector2 GetCentroid(Vector2[] vertices)
    {
        Vector2 centroid = Vector2.zero;
        foreach(Vector2 point in vertices)
        {
            centroid += point;
        }
        centroid /= vertices.Length;
        return centroid;
    }
    public void SpawnVertexPoint(int name, Vector2 point)
    {
        if(!showVertexNumbers)
            return;

        GameObject vertex = Instantiate(VertexText, new Vector3(point.x, 0, point.y), Quaternion.identity, this.transform);
        vertex.name = name.ToString();
        vertex.GetComponentInChildren<TMP_Text>().text = name.ToString();
        vertexTexts.Add(vertex);
    }
    public async Task<bool> SpawnVertexAtPoint(int name, Vector2 point)
    {
        GameObject vertex = Instantiate(edgePrefab, new Vector3(point.x, 0, point.y), Quaternion.identity, step0Transform);
        // vertexTexts.Add(vertex);
        vertexDict.Add(vertex, point);
        vertDict2.Add(point, vertex);
        vertex.name = name.ToString();
        if(demoMode)
            await Task.Delay(15);
        
        return true;
    }
    public async Task<bool> SpawnTriangle(Vector3 triangle)
    {
        GameObject a = Instantiate(edgePrefab, new Vector3(globalVerticies[(int)triangle.x].x, 0, globalVerticies[(int)triangle.x].y), Quaternion.identity, step1Transform);
        if(a.GetComponent<LineRenderer>() == null)
        {
            a.AddComponent<LineRenderer>();
            a.GetComponent<LineRenderer>().positionCount = 0;
            a.GetComponent<LineRenderer>().material = lineMat3;
        }

        LineRenderer l = a.GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i < l.positionCount; i++)
            points.Add(l.GetPosition(i));

        points.Add(a.transform.position);
        points.Add(new Vector3(globalVerticies[(int)triangle.y].x, 0, globalVerticies[(int)triangle.y].y));
        points.Add(new Vector3(globalVerticies[(int)triangle.z].x, 0, globalVerticies[(int)triangle.z].y));
        points.Add(a.transform.position);

        l.positionCount = 4+l.positionCount;
        l.SetPositions(points.ToArray());
        l.startWidth = 0.025f;
        l.endWidth = 0.025f;
        l.startColor = Color.blue;
        l.endColor = Color.blue;
        l.textureMode = LineTextureMode.RepeatPerSegment;
        l.material.mainTextureScale = new Vector2(1f / 0.025f, 1.0f);
        l.useWorldSpace = true;
        await Task.Delay(15);
        return true;
    }
    public async Task<bool> DrawEdge(Vector2 edge, float height, int delayAmount)
    {
        GameObject a = Instantiate(edgePrefab, new Vector3(globalVerticies[(int)edge.x].x, 0, globalVerticies[(int)edge.x].y), Quaternion.identity, step2Transform);

        if(a.GetComponent<LineRenderer>() == null)
        {
            a.AddComponent<LineRenderer>();
            a.GetComponent<LineRenderer>().positionCount = 0;
            a.GetComponent<LineRenderer>().material = lineMat2;
        }

        LineRenderer l = a.GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i < l.positionCount; i++)
            points.Add(l.GetPosition(i));

        points.Add(new Vector3(a.transform.position.x,height, a.transform.position.z));
        points.Add(new Vector3(globalVerticies[(int)edge.y].x, height, globalVerticies[(int)edge.y].y));
        // Debug.Log($"corners.Count: {corners.Count}");
        l.positionCount = 2+l.positionCount;
        l.SetPositions(points.ToArray());
        l.startWidth = 0.05f;
        l.endWidth = 0.05f;

        l.useWorldSpace = true;
        if(delayAmount > 0)
            await Task.Delay(delayAmount);

        return true;
    }
    public async Task<bool> DrawFinalEdge(Vector2 edge, float height, int delayAmount)
    {
        GameObject a = Instantiate(finalEdgePrefab, new Vector3(globalVerticies[(int)edge.x].x, 0, globalVerticies[(int)edge.x].y), Quaternion.identity, step2Transform);

        if(a.GetComponent<LineRenderer>() == null)
        {
            a.AddComponent<LineRenderer>();
            a.GetComponent<LineRenderer>().positionCount = 0;
            a.GetComponent<LineRenderer>().material = lineMat2;
        }

        LineRenderer l = a.GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i < l.positionCount; i++)
            points.Add(l.GetPosition(i));

        points.Add(new Vector3(a.transform.position.x,height, a.transform.position.z));
        points.Add(new Vector3(globalVerticies[(int)edge.y].x, height, globalVerticies[(int)edge.y].y));
        // Debug.Log($"corners.Count: {corners.Count}");
        l.positionCount = 2+l.positionCount;
        l.SetPositions(points.ToArray());
        l.startWidth = 0.05f;
        l.endWidth = 0.05f;

        l.useWorldSpace = true;
        if(delayAmount > 0)
            await Task.Delay(delayAmount);

        return true;
    }
    List<GameObject> newQuads = new List<GameObject>();
    // public void GenerateMeshes(Face face)
    // {
    //     MeshData meshData = new MeshData ();
    //     GameObject newQuad = Instantiate(quadPrefab);
    //     MeshFilter meshFilter = newQuad.GetComponent<MeshFilter>();
    //     newQuads.Add(newQuad);
	//     //  MeshRenderer meshRenderer;

    //     for(int i = 0; i < globalVerticies.Count; i++)
    //     {
    //         // Debug.Log($"broke at {i}");
    //         meshData.vertices[i] = new Vector3(globalVerticies[i].x, 0, globalVerticies[i].y);
    //     }

    //     for(int i = 0; i < face.newEdges.Count; i++)
    //     {
    //         int a = (int)face.newEdges[i].x;
    //         int b = (int)face.newEdges[i].y;
    //         int c = (int)face.centroidCode;
    //         meshData.AddTriangle(a,b,c);
    //         // Debug.Log($"a: {a}, b: {b}, c: {c}");

    //     }
	// 	// textureRender.sharedMaterial.mainTexture = texture;
	// 	// textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	

    //     Material generatedMaterial = new Material(Shader.Find("Standard"));
    //     generatedMaterial.SetColor("_Color",Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

    //     newQuad.GetComponent<MeshRenderer>().material = generatedMaterial;

	// 	meshFilter.sharedMesh = meshData.CreateMesh();
	// 	// meshRenderer.sharedMaterial.mainTexture = texture;
    // }
    
    public static Vector2 SortEdges(float x, float y)
    {
        return new Vector2(Mathf.Min(x, y), Mathf.Max(x, y));
    }
    public static List<Vector3> TriNeighbours(int a, int b, int c)
    {
        // Check if the triangle points up
        if (PointsUp(a, b, c))
        {
            // Return the neighbors for the upwards pointing triangle
            List<Vector3> pointsToReturn = new List<Vector3>();
            pointsToReturn.AddRange(new List<Vector3> {
                new Vector3(a - 1, b, c),
                new Vector3(a, b - 1, c),
                new Vector3(a, b, c - 1)
            });
            return pointsToReturn;
        }
        else
        {
            // Return the neighbors for the downwards pointing triangle
            List<Vector3> pointsToReturn = new List<Vector3>();
            pointsToReturn.AddRange(new List<Vector3> {
                new Vector3(a + 1, b, c),
                new Vector3(a, b + 1, c),
                new Vector3(a, b, c + 1)
            });
            return pointsToReturn;
        }
    }
    public static bool PointsUp(int a, int b, int c)
    {
        // Check if the sum of the three points is equal to 2
        return a + b + c == 2;
    }
    void OnDrawGizmosSelected()
    {
        for(int i = 0; i < babyFaces.Count; i++)
        {
            BabyFace babf = babyFaces[i];
            Vector2 a = globalVerticies[(int)babf.face.x];
            Vector2 b = globalVerticies[(int)babf.face.y];
            Vector2 c = globalVerticies[(int)babf.face.z];
            Vector2 d = globalVerticies[(int)babf.face.w];

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(a.x, 0, a.y), new Vector3(globalVerticies[(int)babf.face.x].x, 0, globalVerticies[(int)babf.face.x].y));
            Gizmos.DrawLine(new Vector3(b.x, 0, b.y), new Vector3(globalVerticies[(int)babf.face.y].x, 0, globalVerticies[(int)babf.face.y].y));
            Gizmos.DrawLine(new Vector3(c.x, 0, c.y), new Vector3(globalVerticies[(int)babf.face.z].x, 0, globalVerticies[(int)babf.face.z].y));
            Gizmos.DrawLine(new Vector3(d.x, 0, d.y), new Vector3(globalVerticies[(int)babf.face.w].x, 0, globalVerticies[(int)babf.face.w].y));
        }
            
    }
	void OnValidate() {
		if (mapWidth < 1) {
			mapWidth = 1;
		}
		if (mapHeight < 1) {
			mapHeight = 1;
		}
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}
	}
}
[System.Serializable] public class Face
    {
        public List<int> fourCorners;
        public List<Vector2> vertices;
        public List<Vector3> triangles;
        public List<Vector2> edges = new List<Vector2>();
        public Vector2 centroid;
        public int centroidCode;
        public List<Vector2> overlappingEdges = new List<Vector2>();
        public List<Vector2> newEdges = new List<Vector2>();
        public List<BabyFace> babyFaces = new List<BabyFace>();
    }
    [System.Serializable] public struct TriangleNeighborsGroups
    {
        public Vector3 triangle;
        public List<Vector2> edges;
    }
[System.Serializable] public class BabyFace
    {
        public Vector4 face;
        public List<Vector2> forcesToApply = new List<Vector2>();
        public float lowestForce = Mathf.Infinity;
    }
public static class ListExtensions  
{
        public static void Shuffle<T>(this IList<T> list) 
        {
            System.Random rnd = new System.Random();
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }
 
        public static void Swap<T>(this IList<T> list, int i, int j) 
        {
            (list[i], list[j]) = (list[j], list[i]);
        }
}
[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}
public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(float[,] heightMap) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, heightMap [x, y]);
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}
}
}
