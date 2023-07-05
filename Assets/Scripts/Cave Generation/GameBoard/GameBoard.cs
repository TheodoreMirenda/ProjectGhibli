using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TJ
{
public class GameBoard : MonoBehaviour
{
    [SerializeField] private ChunkGenerator chunkGenerator;
    // [SerializeField] private Chunk[] chunks;
    [SerializeField] private List<Chunk> chunks;
    [SerializeField] private GameObject clickableTilePrefab;
    [SerializeField] private float sizeMultiplier = 5f;
    [SerializeField] private SerializableDictionary<int, int5> vertexToFaceMap = new SerializableDictionary<int, int5>();
    [SerializeField] private Transform hexSpot;
    [SerializeField] private Transform playerGameObject;
    [SerializeField] private int playerLocationChunkIndex;
    private void Update()
    {
        UpdateVisibleChunks();
    }
    public void CreateInitialChunk()
    {
        ClearAllChunks();
        Chunk chunk = new Chunk();
        Transform chunkHolder = new GameObject().transform;
        chunk.chunkTransform = chunkHolder;

        chunkGenerator.RequestChunk(OnMapDataRecieved, chunk.centroid);
    }
    [ContextMenu("Generate Neighbors")]
    public void GenerateNeighbors()
    {
        chunks = HexGridLayout.CreateBorderingChunks(chunks, playerLocationChunkIndex);
        for(int i = 0; i < chunks.Count; i++){
            if(chunks[i].chunkTransform == null){
                // Debug.Log($"Creating chunk {i}");
                chunkGenerator.RequestChunk(OnMapDataRecieved, chunks[i].centroid);
            
                // chunks[i] = chunkGenerator.CreateChunk(chunks[i]);
                // CreateClickableTiles(chunks[i]);
            }
        }
    }
    public void OnMapDataRecieved(Chunk chunk){
        Debug.Log($"Recieved map data for chunk {chunk.centroid}");

        //find the chunk that matches the centroid
        for(int i = 0; i < chunks.Count; i++){
            if(chunks[i].centroid == chunk.centroid){
                chunks[i] = chunk;
                chunk = chunkGenerator.CreateChunk(chunk);
                CreateClickableTiles(chunks[i]);
                return;
            }
        }
    }
    [ContextMenu("DemoHex")]
    public void DemoHex()
    {
        chunks = HexGridLayout.CreateBorderingChunks(chunks, playerLocationChunkIndex);
        for(int i = 0; i < chunks.Count; i++){
            Instantiate(hexSpot, new Vector3(chunks[i].centroid.x*sizeMultiplier, 0, chunks[i].centroid.y*sizeMultiplier), Quaternion.identity);
        }
    }
    public void ClearAllChunks(){
        for(int i = chunks.Count-1; i >= 0; i--) {
            if(chunks[i].chunkTransform != null)
                DestroyImmediate(chunks[i].chunkTransform.gameObject);

            chunks.RemoveAt(i);
        }
        chunks.Clear();
        vertexToFaceMap.Clear();
    }
    // [ContextMenu("Create Clickable Tiles")]
    public void CreateClickableTiles(Chunk chunk)
    {
        chunk.clickableTiles?.Clear();
        chunk.clickableTiles = new List<ClickableTile>();

        if(chunk.chunkClickableTilesTransform != null)
            DestroyImmediate(chunk.chunkClickableTilesTransform.gameObject);

        Transform chunkClickableTilesTransform = new GameObject().transform;
        chunkClickableTilesTransform.name = "Clickable Tiles";
        chunkClickableTilesTransform.SetParent(chunk.chunkTransform);

        chunk.chunkClickableTilesTransform = chunkClickableTilesTransform;

        //get each verticies of each hexagon
        for(int i = 0; i < chunk.mapData.globalVerticies.Count; i++)
        {
            List<Vector3> corners = new List<Vector3>();
            //get faces containing each verticie
            foreach(Vector4 face in chunk.mapData.faces)
            {
                if((int)face.w == i || (int)face.x == i || (int)face.y == i || (int)face.z == i)
                {
                    //get the centroid of the face
                    Vector3 centroid = (chunk.mapData.globalVerticies[(int)face.w] + 
                                        chunk.mapData.globalVerticies[(int)face.x] + 
                                        chunk.mapData.globalVerticies[(int)face.y] + 
                                        chunk.mapData.globalVerticies[(int)face.z]) / 4;
                    
                    corners.Add(new Vector3(centroid.x*sizeMultiplier, 0, centroid.y*sizeMultiplier));
                }
            }
            
            ClickableTile ct = Instantiate(clickableTilePrefab, 
                                        new Vector3(chunk.mapData.globalVerticies[i].x*sizeMultiplier, 0, chunk.mapData.globalVerticies[i].y*sizeMultiplier),
                                        Quaternion.identity, 
                                        chunk.chunkClickableTilesTransform).GetComponent<ClickableTile>();

            ct.corners = corners;
            ct.vertexId = i;
            chunk.clickableTiles.Add(ct);
        }

        foreach(ClickableTile tile in chunk.clickableTiles)
        {
            //create mesh using 4 corners
            Mesh mesh = new Mesh();

            //we need to sort the corners in a clockwise order
            Vector3 center = Vector3.zero;
            foreach(Vector3 corner in tile.corners)
            {
                center += corner;
            }
            center /= tile.corners.Count;

            //sort the corners in clockwise order
            tile.corners.Sort((a, b) => {
                if (Mathf.Atan2(a.x - center.x, a.z - center.z) < Mathf.Atan2(b.x - center.x, b.z - center.z))
                    return -1;
                else
                    return 1;
            });

            //create triangles
            mesh.vertices = tile.corners.ToArray();

            //create triangles
            if(tile.corners.Count < 3)
            {
                tile.clickableTileMesh.sharedMesh = null;
                tile.clickableTileCollider.sharedMesh = null;
                continue;
            }

            if(tile.corners.Count == 3)
            {
                mesh.triangles = new int[] {
                    0, 1, 2,
                };
            }
            else if(tile.corners.Count == 4)
            {
                mesh.triangles = new int[] {
                    0, 1, 2,
                    0, 2, 3
                };
            }
            else if(tile.corners.Count == 5)
            {
                mesh.triangles = new int[] {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4
                };
            }

            mesh.RecalculateBounds();
            //get mesh center offset from transform.position
            Vector3 offset = tile.transform.position - tile.transform.TransformPoint(mesh.bounds.center);
            //apply offset to vertices
            mesh.vertices = mesh.vertices.Select(v => v + offset).ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            tile.clickableTileMesh.sharedMesh = mesh;

            //shrink the collider
            mesh.vertices = mesh.vertices.Select(v => v * 0.9f).ToArray();
            tile.clickableTileCollider.sharedMesh = mesh;

            //create line renderer
            tile.lineRenderer.positionCount = tile.corners.Count+1;
            tile.lineRenderer.SetPositions(tile.corners.ToArray());
            tile.lineRenderer.SetPosition(tile.corners.Count, tile.corners[0]);
            tile.lineRenderer.enabled = false;
        }
        SetUpNodes(chunk);
    }
    // [ContextMenu("SetUpNodes")]
    public void SetUpNodes(Chunk chunk)
    {
        // if(landParent != null)
        //     DestroyImmediate(landParent.gameObject);
        
        // landParent = new GameObject().transform;

        Node[] nodes = chunk.chunkTransform.GetComponentsInChildren<Node>();
        foreach(Node n in nodes)
        {
            Cell cell = n.gameObject.GetComponent<Cell>();
            n.AssignSides(cell, chunk.mapData.globalVerticies, sizeMultiplier);            
        }

        // landParent.localScale = new Vector3(1, 1, 1);
        // landParent.localPosition = new Vector3(0, 0, 0);
        // CreateVertexToFaceMap();
    }
    // [ContextMenu("SetUpVertexToFaceMap")]
    public void CreateVertexToFaceMap()
    {
        foreach(Vector2 vertex in chunks[0].mapData.globalVerticies)
        {
            int vertexId = chunks[0].mapData.globalVerticies.IndexOf(vertex);
            int5 faces = new int5(new int[]{-1,-1,-1,-1,-1});
            for(int i = 0; i < chunks[0].mapData.cells.Count; i++)
            {
                Vector4 face = chunks[0].mapData.cells[i].face;
                if ((int)face.w == vertexId || (int)face.x == vertexId || (int)face.y == vertexId || (int)face.z == vertexId)
                {
                    //add to first one that doesnt == -1
                    if(faces.v == -1)
                        faces.v = i;
                    else if(faces.w == -1)
                        faces.w = i;
                    else if(faces.x == -1)
                        faces.x = i;
                    else if(faces.y == -1)
                        faces.y = i;
                    else if(faces.z == -1)
                        faces.z = i;
                }
            }
            vertexToFaceMap.Add(vertexId, faces);
        }

        //log out all
        // foreach(KeyValuePair<int, int5> kvp in vertexToFaceMap)
        // {
        //     Debug.Log(kvp.Key + " " + kvp.Value.v +" " + kvp.Value.w + " " + kvp.Value.x + " " + kvp.Value.y + " " + kvp.Value.z);
        // }
    }
    public void MarkVertexAsClicked(int vertexId)
    {
        // Debug.Log($"Marking vertex {vertexId} as clicked");
        int5 faces = vertexToFaceMap[vertexId];

        // Debug.Log($"faces: {faces.w} {faces.x} {faces.y} {faces.z}");

        //get the cell that contains the vertex
        if(faces.v != -1)
            chunks[0].mapData.cells[faces.v]?.gameObject.GetComponent<Node>().MarkCorner(vertexId, 1);
        if(faces.w != -1)
            chunks[0].mapData.cells[faces.w]?.gameObject.GetComponent<Node>().MarkCorner(vertexId, 1);
        if(faces.x != -1)
            chunks[0].mapData.cells[faces.x]?.gameObject.GetComponent<Node>().MarkCorner(vertexId, 1);
        if(faces.y != -1)
            chunks[0].mapData.cells[faces.y]?.gameObject.GetComponent<Node>().MarkCorner(vertexId, 1);
        if(faces.z != -1)
            chunks[0].mapData.cells[faces.z]?.gameObject.GetComponent<Node>().MarkCorner(vertexId, 1);
    }
    private void UpdateVisibleChunks(){
        //get the chunk that is closest to the player
        int closestChunkIndex = 0;
        float closestDistance = Mathf.Infinity;
        for(int i = 0; i < chunks.Count; i++){
            float distance = Vector2.Distance(new Vector2(playerGameObject.position.x/5, playerGameObject.position.z/5), chunks[i].centroid);
            if(distance < closestDistance){
                closestDistance = distance;
                closestChunkIndex = i;
            }
        }
        UpdatePlayerLocationChunk(closestChunkIndex);
        // Debug.Log($"closest chunk is {chunkToAddNeighborsToIndex} at {closestChunk.centroid}");
    }
    private void UpdatePlayerLocationChunk(int newChunkIndex){
        if(playerLocationChunkIndex == newChunkIndex) return;

        // Debug.Log($"Player moved to chunk {newChunkIndex}");
        playerLocationChunkIndex = newChunkIndex;
        GenerateNeighbors();
    }
}
[System.Serializable] public struct int5
{
    public int v, w, x, y, z;
    public int5(int[] values)
    {
        v = values[0];
        w = values[1];
        x = values[2];
        y = values[3];
        z = values[4];
    }
}
}