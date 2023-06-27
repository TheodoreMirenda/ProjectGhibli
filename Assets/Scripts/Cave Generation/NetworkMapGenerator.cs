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

public static class NetworkMapGenerator {
    public static MapDataFinal GenerateMap(int seed, int rings, AnimationCurve meshHeightCurve, float noiseScale = 1000, int octaves = 10, float persistance = 0.45f, float lacunarity = 2, Vector2 offset = new Vector2(), 
        int mapWidth = 3, int mapHeight = 3, float meshHeightMultiplier = 1,  MeshGenerationShape meshGenerationShape = MeshGenerationShape.Hexagon,
        int iterrations = 200, float movementAmount = 100)
    {
        List<Vector2> globalVerticies = new List<Vector2>();
        List<Vector2> globalVerticiesList = new List<Vector2>();
        List<Vector2> innerVertices = new List<Vector2>();
        Dictionary<Vector2, bool> outerVerticies = new Dictionary<Vector2, bool>();
        MeshStruct meshStruct;
        List<TriangleNeighborsGroups> triangleNeighborsGroups = new List<TriangleNeighborsGroups>();
        List<Vector3> nonCollapsedTriangles = new List<Vector3>();
        List<Vector3> collapsedTriangles = new List<Vector3>();
        SerializableDictionary <Vector2, List<Vector3>> triangleNeighborsGroupsDict = new SerializableDictionary<Vector2, List<Vector3>>();
        List<GameObject> vertexTexts = new List<GameObject>();
        List<Face> faces = new List<Face>();
        List<BabyFace> babyFaces = new List<BabyFace>();
        float[,] noiseMap;
        MeshData meshData;
        Dictionary<GameObject, Vector2> vertexDict = new Dictionary<GameObject, Vector2>();
        Dictionary<Vector2, GameObject> vertDict2 = new Dictionary<Vector2, GameObject>();


        noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
		// CleanUp();
		meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshGenerationShape, meshHeightMultiplier, meshHeightCurve, rings);

        #region meshStruct for debugging values

        meshStruct.triangles = meshData.triangles;
        meshStruct.vertices = meshData.vertices;
        Vector2 offset2 = meshStruct.vertices[0];
        foreach(Vector3 vertice in meshStruct.vertices)
        {
            globalVerticies.Add(new Vector2(vertice.x - offset2.x, vertice.z - offset2.y));
        }
        
        meshStruct.triangle_Origional = meshData.triangle_Origional;
        meshStruct.uvs = meshData.uvs;
        #endregion

        outerVerticies = GetOuterVertices(rings, globalVerticies, outerVerticies);

        #region collapse triangels
        //generate a list of all edges, then shuffle up the order
            foreach(Vector3 triangle in meshData.triangle_Origional)
            {
                nonCollapsedTriangles.Add(triangle);
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
        #endregion

        
        #region subdivide 
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
            face.centroidCode = globalVerticies.Count - 1;

            face.fourCorners.Add(globalVerticies.Count - 1);
        }

        //subdivide each quad by adding a new vertice at the centroid of each quad
        foreach(Face face in faces)
        {
            //new vertices
            Vector2 ab = Vector2.Lerp(globalVerticies[(int)face.edges[0].x], globalVerticies[(int)face.edges[0].y], 0.5f);
            // Debug.Log($"ab: {ab}");
            int abCode  = CheckIfVertexExists(ab, globalVerticies);
            if(abCode == -1)
            {
                globalVerticies.Add(new Vector2(ab.x, ab.y));
                outerVerticies = OuterVerticeCheck((int)face.edges[0].x, (int)face.edges[0].y, ab, outerVerticies, globalVerticies);
                abCode = globalVerticies.Count - 1;
            }
            face.newEdges.Add(new Vector2 { x = (int)face.edges[0].x, y= abCode });
            face.newEdges.Add(new Vector2 { x = abCode, y = (int)face.edges[0].y });

            Vector2 bc = Vector2.Lerp(globalVerticies[(int)face.edges[1].x], globalVerticies[(int)face.edges[1].y], 0.5f);
            // Debug.Log($"bc: {bc}");
            int bcCode = CheckIfVertexExists(bc, globalVerticies);
            if(bcCode == -1)
            {
                globalVerticies.Add(new Vector2(bc.x, bc.y));
                outerVerticies = OuterVerticeCheck((int)face.edges[1].x, (int)face.edges[1].y, bc, outerVerticies, globalVerticies);
                bcCode = globalVerticies.Count - 1;
            }
            face.newEdges.Add(new Vector2 { x = (int)face.edges[1].x, y = bcCode });
            face.newEdges.Add(new Vector2 { x = bcCode, y = (int)face.edges[1].y });

            Vector2 cd = Vector2.Lerp(globalVerticies[(int)face.edges[2].x], globalVerticies[(int)face.edges[2].y], 0.5f);
            // Debug.Log($"cd: {cd}");
            int cdCode = CheckIfVertexExists(cd, globalVerticies);
            if(cdCode == -1)
            {
                globalVerticies.Add(new Vector2(cd.x, cd.y));
                outerVerticies = OuterVerticeCheck((int)face.edges[2].x, (int)face.edges[2].y, cd, outerVerticies, globalVerticies);
                cdCode = globalVerticies.Count - 1;
            }
            face.newEdges.Add(new Vector2 { x = (int)face.edges[2].x, y = cdCode });
            face.newEdges.Add(new Vector2 { x = cdCode, y = (int)face.edges[2].y });

            if(face.edges.Count == 4)
            {
                Vector2 da = Vector2.Lerp(globalVerticies[(int)face.edges[3].x], globalVerticies[(int)face.edges[3].y], 0.5f);
                // Debug.Log($"da: {da}");
                int daCode = CheckIfVertexExists(da, globalVerticies);
                if(daCode == -1)
                {
                    globalVerticies.Add(new Vector2(da.x, da.y));
                    outerVerticies = OuterVerticeCheck((int)face.edges[3].x, (int)face.edges[3].y, da, outerVerticies, globalVerticies);
                    daCode = globalVerticies.Count - 1;
                }
                face.newEdges.Add(new Vector2 { x = (int)face.edges[3].x, y = daCode });
                face.newEdges.Add(new Vector2 { x = daCode, y = (int)face.edges[3].y });
            }
        }
        
        //calculate the new triangles that were created by subdividing the quads
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

            }
        }
        #endregion

        for(int j = 0; j < iterrations; j++)
        {
            //smooth them out
            #region smooth baby faces
            for(int t = 0; t < babyFaces.Count; t++)
        {
            Vector4 babyFace = babyFaces[t].face;
            babyFaces[t].lowestForce = Mathf.Infinity;
            babyFaces[t].forcesToApply.Clear();

            Vector2 a = globalVerticies[(int)babyFace.x];
            Vector2 b = globalVerticies[(int)babyFace.y];
            Vector2 c = globalVerticies[(int)babyFace.z];
            Vector2 d = globalVerticies[(int)babyFace.w];

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
            #endregion

            //move the corners of the quads to the new vertice positions
            for(int i = 0; i < babyFaces.Count; i++)
            {
                BabyFace babf = babyFaces[i];
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
            }
        }

        //add new triangles and verticies to the mesh
        meshData = ReCalculateTriangles(meshData, noiseMap, globalVerticies, meshGenerationShape, faces);

        // Debug.Log($"offset is {offset}");

        // meshData.vertices = Noise.GenerateNoiseMapHex (meshData.vertices, seed, noiseScale, octaves, persistance, lacunarity, offset, rings);

        //assign grid values to all verticies
        // for(int i = 0; i<meshData.vertices.Length; i++)
        //     meshData.vertices[i] = new Vector3(meshData.vertices[i].x, meshHeightCurve.Evaluate(meshData.vertices[i].y) * meshHeightMultiplier,meshData.vertices[i].z);

        MapDataFinal final = new MapDataFinal();
        List<Vector4> facesNew = new List<Vector4>();

        foreach(BabyFace face in babyFaces)
            facesNew.Add(new Vector4(face.face.x, face.face.y, face.face.z, face.face.w));
       
        final.faces = facesNew;
        final.globalVerticies = globalVerticies;

        return final;
	}
    public static Dictionary<Vector2, bool> GetOuterVertices(int rings, List<Vector2> globalVerticies, Dictionary<Vector2, bool> outerVerticies)
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
        return outerVerticies;
    }
    public static int CheckIfVertexExists(Vector2 vertex, List<Vector2> globalVerticies)
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
    public static MeshData ReCalculateTriangles(MeshData meshData, float[,] noiseMap, List<Vector2>globalVerticies, MeshGenerationShape meshGenerationShape, List<Face> faces)
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

        if(meshGenerationShape == MeshGenerationShape.Square)
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
    public static Vector2 FindClosest(Vector2 og, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
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
    private static Dictionary<Vector2, bool> OuterVerticeCheck(int x, int y, Vector2 a, Dictionary<Vector2, bool> outerVerticies, List<Vector2> globalVerticies)
    {
        if(outerVerticies.ContainsKey(globalVerticies[x]) && outerVerticies.ContainsKey(globalVerticies[y]))
        {
            // Debug.Log($"adding outer vertice: {a}");
            outerVerticies.Add(a, true);
        }
        return outerVerticies;
    }
    // public List<int> Sort(List<int> pointers)
    // {
    //     List<Vector2> points = new List<Vector2>();
    //     foreach(int pointer in pointers)
    //     {
    //         points.Add(new Vector2(globalVerticies[pointer].x, globalVerticies[pointer].y));
    //     }

    //     Vector2 centroid = new Vector2(
    //         points.Average(p => p.x),
    //         points.Average(p => p.y)
    //     );

    //     // points.Sort((a, b) => Mathf.Atan2(a.y - centroid.y, a.x - centroid.x)
    //     //     .CompareTo(Mathf.Atan2(b.y - centroid.y, b.x - centroid.x)));

    //     points.Sort((a, b) => Mathf.Atan2(b.y - centroid.y, b.x - centroid.x)
    //         .CompareTo(Mathf.Atan2(a.y - centroid.y, a.x - centroid.x)));

    //     for(int i = 0; i < points.Count; i++)
    //     {
    //         pointers[i] = globalVerticies.IndexOf(new Vector2(points[i].x, points[i].y));
    //     }
    //     return pointers;
    // }
    public static Vector2 GetCentroid(Vector2[] vertices)
    {
        Vector2 centroid = Vector2.zero;
        foreach(Vector2 point in vertices)
        {
            centroid += point;
        }
        centroid /= vertices.Length;
        return centroid;
    }
    // public async Task<bool> SpawnVertexAtPoint(int name, Vector2 point)
    // {
    //     GameObject vertex = Instantiate(edgePrefab, new Vector3(point.x, 0, point.y), Quaternion.identity, step0Transform);
    //     // vertexTexts.Add(vertex);
    //     vertexDict.Add(vertex, point);
    //     vertDict2.Add(point, vertex);
    //     vertex.name = name.ToString();
    //     if(demoMode)
    //         await Task.Delay(15);
        
    //     return true;
    // }
    // public async Task<bool> SpawnTriangle(Vector3 triangle)
    // {
    //     GameObject a = Instantiate(edgePrefab, new Vector3(globalVerticies[(int)triangle.x].x, 0, globalVerticies[(int)triangle.x].y), Quaternion.identity, step1Transform);
    //     if(a.GetComponent<LineRenderer>() == null)
    //     {
    //         a.AddComponent<LineRenderer>();
    //         a.GetComponent<LineRenderer>().positionCount = 0;
    //         a.GetComponent<LineRenderer>().material = lineMat3;
    //     }

    //     LineRenderer l = a.GetComponent<LineRenderer>();
    //     List<Vector3> points = new List<Vector3>();
    //     for(int i = 0; i < l.positionCount; i++)
    //         points.Add(l.GetPosition(i));

    //     points.Add(a.transform.position);
    //     points.Add(new Vector3(globalVerticies[(int)triangle.y].x, 0, globalVerticies[(int)triangle.y].y));
    //     points.Add(new Vector3(globalVerticies[(int)triangle.z].x, 0, globalVerticies[(int)triangle.z].y));
    //     points.Add(a.transform.position);

    //     l.positionCount = 4+l.positionCount;
    //     l.SetPositions(points.ToArray());
    //     l.startWidth = 0.025f;
    //     l.endWidth = 0.025f;
    //     l.startColor = Color.blue;
    //     l.endColor = Color.blue;
    //     l.textureMode = LineTextureMode.RepeatPerSegment;
    //     l.material.mainTextureScale = new Vector2(1f / 0.025f, 1.0f);
    //     l.useWorldSpace = true;
    //     await Task.Delay(15);
    //     return true;
    // }
    // public async Task<bool> DrawEdge(Vector2 edge, float height, int delayAmount)
    // {
    //     GameObject a = Instantiate(edgePrefab, new Vector3(globalVerticies[(int)edge.x].x, 0, globalVerticies[(int)edge.x].y), Quaternion.identity, step2Transform);

    //     if(a.GetComponent<LineRenderer>() == null)
    //     {
    //         a.AddComponent<LineRenderer>();
    //         a.GetComponent<LineRenderer>().positionCount = 0;
    //         a.GetComponent<LineRenderer>().material = lineMat2;
    //     }

    //     LineRenderer l = a.GetComponent<LineRenderer>();
    //     List<Vector3> points = new List<Vector3>();
    //     for(int i = 0; i < l.positionCount; i++)
    //         points.Add(l.GetPosition(i));

    //     points.Add(new Vector3(a.transform.position.x,height, a.transform.position.z));
    //     points.Add(new Vector3(globalVerticies[(int)edge.y].x, height, globalVerticies[(int)edge.y].y));
    //     // Debug.Log($"corners.Count: {corners.Count}");
    //     l.positionCount = 2+l.positionCount;
    //     l.SetPositions(points.ToArray());
    //     l.startWidth = 0.05f;
    //     l.endWidth = 0.05f;

    //     l.useWorldSpace = true;
    //     if(delayAmount > 0)
    //         await Task.Delay(delayAmount);

    //     return true;
    // }
    // public async Task<bool> DrawFinalEdge(Vector2 edge, float height, int delayAmount)
    // {
    //     GameObject a = Instantiate(finalEdgePrefab, new Vector3(globalVerticies[(int)edge.x].x, 0, globalVerticies[(int)edge.x].y), Quaternion.identity, step2Transform);

    //     if(a.GetComponent<LineRenderer>() == null)
    //     {
    //         a.AddComponent<LineRenderer>();
    //         a.GetComponent<LineRenderer>().positionCount = 0;
    //         a.GetComponent<LineRenderer>().material = lineMat2;
    //     }

    //     LineRenderer l = a.GetComponent<LineRenderer>();
    //     List<Vector3> points = new List<Vector3>();
    //     for(int i = 0; i < l.positionCount; i++)
    //         points.Add(l.GetPosition(i));

    //     points.Add(new Vector3(a.transform.position.x,height, a.transform.position.z));
    //     points.Add(new Vector3(globalVerticies[(int)edge.y].x, height, globalVerticies[(int)edge.y].y));
    //     // Debug.Log($"corners.Count: {corners.Count}");
    //     l.positionCount = 2+l.positionCount;
    //     l.SetPositions(points.ToArray());
    //     l.startWidth = 0.05f;
    //     l.endWidth = 0.05f;

    //     l.useWorldSpace = true;
    //     if(delayAmount > 0)
    //         await Task.Delay(delayAmount);

    //     return true;
    // }
    // List<GameObject> newQuads = new List<GameObject>();
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

}
[System.Serializable] public struct MapDataFinal
{
    public List<Vector2> globalVerticies;
    public List<Vector4> faces;
    public List<Cell> cells;
}
}
