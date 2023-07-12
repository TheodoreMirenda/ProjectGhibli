using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Mathematics;

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
    Dictionary<int, int> vertexIdPairs = new Dictionary<int, int> ();
    [SerializeField] private int vertexToClick;
    private void Update()
    {
        UpdateVisibleChunks();
    }
    public void CreateInitialChunk()
    {
        ClearAllChunks();
        Chunk chunk = new Chunk(
            new Vector2(0, 0),
            new GameObject().transform,
            new GameObject().transform,
            new MapDataFinal(),
            new List<ClickableTile>()
        );

        if(Application.isPlaying)
            chunkGenerator.RequestChunk(OnMapDataRecieved, chunk.centroid);
        else
        {
            chunk.mapData = NetworkMapGenerator.GenerateMap(chunkGenerator.seed, chunkGenerator.rings, chunkGenerator.animationCurve);
            chunks.Add(chunk);
            OnMapDataRecieved(chunk);
        }
    }
    [ContextMenu("Generate Neighbors")]
    public void GenerateNeighbors()
    {
        chunks = HexGridLayout.CreateBorderingChunks(chunks, playerLocationChunkIndex, chunkGenerator.rings);
        for(int i = 0; i < chunks.Count; i++){
            if(chunks[i].chunkTransform == null){
            
                if(Application.isPlaying)
                    chunkGenerator.RequestChunk(OnMapDataRecieved, chunks[i].centroid);
                else
                {
                    chunks[i] = new Chunk(
                        chunks[i].centroid,
                        new GameObject().transform,
                        new GameObject().transform,
                        NetworkMapGenerator.GenerateMap(chunkGenerator.seed, chunkGenerator.rings, chunkGenerator.animationCurve),
                        new List<ClickableTile>()
                    );
                    OnMapDataRecieved(chunks[i]);
                }
            }
        }
    }
    public void OnMapDataRecieved(Chunk chunk){
        // Debug.Log($"Recieved map data for chunk {chunk.centroid}");

        //find the chunk that matches the centroid
        for(int i = 0; i < chunks.Count; i++){
            if(chunks[i].centroid == chunk.centroid){
                chunks[i] = chunk;
                chunk = chunkGenerator.CreateChunk(chunk);
                SetUpNodes(i);
                CreateClickableTiles(i);
                CheckForVertexOverlap(chunks[i]);
                foreach(Node n in chunks[i].mapData.nodes)
                {
                    Instantiate(hexSpot, n.GetCentroidAsVector3()*sizeMultiplier, Quaternion.identity, chunks[i].chunkTransform)
                        .transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                return;
            }
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
    public void CreateClickableTiles(int chunkIndex)
    {
        chunks[chunkIndex].chunkClickableTilesTransform.name = "Clickable Tiles";
        chunks[chunkIndex].chunkClickableTilesTransform.SetParent(chunks[chunkIndex].chunkTransform);

        //get each verticies of each hexagon
        for(int i = 0; i < chunks[chunkIndex].mapData.globalVerticies.Count; i++)
        {
            List<Vector3> corners = new List<Vector3>();
            //get faces containing each verticie
            foreach(Node node in chunks[chunkIndex].mapData.nodes)
            {
                if(node.side[0].vertexId == i || node.side[1].vertexId  == i || node.side[2].vertexId  == i || node.side[3].vertexId  == i)
                {
                    //get the centroid of the face
                    Vector3 centroid = node.GetCentroid();
                    corners.Add(new Vector3(centroid.x*sizeMultiplier, 0, centroid.y*sizeMultiplier));
            }}

            if(corners.Count >= 3){
                ClickableTile ct = CreateClickableTile(chunkIndex, corners);
                chunks[chunkIndex].clickableTiles.Add(ct);
            };
        }
    }
    private ClickableTile CreateClickableTile(int chunkIndex, List<Vector3> corners){
        ClickableTile tile = Instantiate(clickableTilePrefab, 
                                        new Vector3(0, 0, 
                                                    0),
                                        Quaternion.identity, 
                                        chunks[chunkIndex].chunkClickableTilesTransform).GetComponent<ClickableTile>();

        tile.gameObject.name = corners.Count.ToString();
        tile.corners = corners;
        // tile.vertexId = globalVerticieIndex;

        //create mesh using 4 corners
        Mesh mesh = new Mesh();

        //we need to sort the corners in a clockwise order
        Vector3 center = Vector3.zero;
        foreach(Vector3 corner in tile.corners)
            center += corner;

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
        tile.clickableTileMesh.sharedMesh = null;
        tile.clickableTileCollider.sharedMesh = null;

        if(tile.corners.Count == 3){
            mesh.triangles = new int[] {
                0, 1, 2,
            };
        }
        else if(tile.corners.Count == 4){
            mesh.triangles = new int[] {
                0, 1, 2,
                0, 2, 3
            };
        }
        else if(tile.corners.Count == 5){
            mesh.triangles = new int[] {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4
            };
        }
        else if(tile.corners.Count == 6){
            mesh.triangles = new int[] {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5
            };
        }

        // mesh.RecalculateBounds();
        //get mesh center offset from transform.position
        // Vector3 offset = tile.transform.position - tile.transform.TransformPoint(mesh.bounds.center);
        //apply offset to vertices
        // mesh.vertices = mesh.vertices.Select(v => v + offset).ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        tile.clickableTileMesh.sharedMesh = mesh;

        //shrink the collider
        // mesh.vertices = mesh.vertices.Select(v => v * 0.9f).ToArray();
        // tile.clickableTileCollider.sharedMesh = mesh;

        //create line renderer
        // tile.lineRenderer.positionCount = tile.corners.Count+1;
        // tile.lineRenderer.SetPositions(tile.corners.ToArray());
        // tile.lineRenderer.SetPosition(tile.corners.Count, tile.corners[0]);
        // tile.lineRenderer.enabled = false;
        
        return tile;
    }
    public void SetUpNodes(int chunkIndex)
    {
        Chunk c = chunks[chunkIndex];
        c.mapData.nodes = chunks[chunkIndex].chunkTransform.GetComponentsInChildren<Node>();
        chunks[chunkIndex] = c;

        foreach(Node n in  chunks[chunkIndex].mapData.nodes)
        {
            Cell cell = n.gameObject.GetComponent<Cell>();
            n.AssignSides(cell,  chunks[chunkIndex].mapData.globalVerticies, sizeMultiplier);            
        }

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
    [ContextMenu("MarkVertexAsClicked")]
    public void MarkVertexAsClicked()
    {
        // Debug.Log($"Marking vertex {vertexId} as clicked");
        int vertexId = vertexToClick;
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
    public void MarkVertexAsClicked(ClickableTile clickableTile)
    {
        // Debug.Log($"Marking vertex {vertexId} as clicked");
        int vertexId = chunks[0].mapData.globalVerticies.IndexOf(clickableTile.corners[0]/sizeMultiplier);
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
            float distance = Vector2.Distance(new Vector2(playerGameObject.position.x/sizeMultiplier, playerGameObject.position.z/sizeMultiplier), chunks[i].centroid);
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
    private Node GetNodeInOtherChunk(Chunk chunk, int side0, int side1, Node n){
        for(int i = 0; i < chunk.mapData.nodes.Length; i++){
            if(chunk.mapData.nodes[i].side[0].vertexId == side1 && chunk.mapData.nodes[i].side[1].vertexId == side0){
                return chunk.mapData.nodes[i];
            }
            else if(chunk.mapData.nodes[i].side[1].vertexId == side1 && chunk.mapData.nodes[i].side[2].vertexId == side0){
                chunk.mapData.nodes[i].neighbor_12 = n;
                return chunk.mapData.nodes[i];
            }
            else if(chunk.mapData.nodes[i].side[2].vertexId == side1 && chunk.mapData.nodes[i].side[3].vertexId == side0){
                chunk.mapData.nodes[i].neighbor_23 = n;
                return chunk.mapData.nodes[i];
            }
            else if(chunk.mapData.nodes[i].side[3].vertexId == side1 && chunk.mapData.nodes[i].side[0].vertexId == side0){
                chunk.mapData.nodes[i].neighbor_30 = n;
                return chunk.mapData.nodes[i];
            }
        }
        return null;
    }
    private void CheckForVertexOverlap(Chunk chunk){
        for(int chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++){
            if(chunks[chunkIndex].centroid == chunk.centroid) continue;
            vertexIdPairs?.Clear();
            if(Vector2.Distance(chunks[chunkIndex].centroid, chunk.centroid) < 1.5f*sizeMultiplier){
                //check for any overlapping verticies
                List<float2> verticiesOverlapping = new List<float2>();
                List<int> vertexIDOverllaps = new List<int>();
                List<int> vertexIDOverllapsOtherChunk = new List<int>();

                for(int j = 0; j < chunk.mapData.globalVerticies.Count; j++){
                    for(int k = 0; k < chunks[chunkIndex].mapData.globalVerticies?.Count; k++){
                        if(Vector2.Distance(chunk.mapData.globalVerticies[j], chunks[chunkIndex].mapData.globalVerticies[k]) < 0.1f){

                            GameObject trash = Instantiate(hexSpot, new Vector3(chunk.mapData.globalVerticies[j].x*sizeMultiplier, 0, 
                                                            chunk.mapData.globalVerticies[j].y*sizeMultiplier), Quaternion.identity, chunk.chunkTransform).gameObject;
                                        
                            trash.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                            trash.name = j.ToString();
                            vertexIDOverllaps.Add(j);
                            vertexIDOverllapsOtherChunk.Add(k);
                            vertexIdPairs.Add(j,k);
                }}}

                //run it down mid getting each side of each node, if the side contains >1 overlapping vertex, its an edge
                foreach(Node node in chunk.mapData.nodes){

                    if(vertexIDOverllaps.Contains(node.side[0].vertexId) && vertexIDOverllaps.Contains(node.side[1].vertexId)){
                        // Debug.Log($"found overlapping edge  {node.side[0].vertexId} {node.side[1].vertexId}");
                        // Debug.Log($"Gonna need: {vertexIdPairs[node.side[0].vertexId]} and {vertexIdPairs[node.side[1].vertexId]}");
                        node.neighbor_01 = GetNodeInOtherChunk(chunks[chunkIndex], vertexIdPairs[node.side[0].vertexId], 
                                                                vertexIdPairs[node.side[1].vertexId], node);
                    }
                    else if(vertexIDOverllaps.Contains(node.side[1].vertexId) && vertexIDOverllaps.Contains(node.side[2].vertexId)){
                        // Debug.Log($"Gonna need: {vertexIdPairs[node.side[1].vertexId]} and {vertexIdPairs[node.side[2].vertexId]}");
                        node.neighbor_12 = GetNodeInOtherChunk(chunks[chunkIndex], vertexIdPairs[node.side[1].vertexId],
                                                                vertexIdPairs[node.side[2].vertexId], node);
                    }
                    else if(vertexIDOverllaps.Contains(node.side[2].vertexId) && vertexIDOverllaps.Contains(node.side[3].vertexId)){
                        // Debug.Log($"Gonna need: {vertexIdPairs[node.side[2].vertexId]} and {vertexIdPairs[node.side[3].vertexId]}");
                        node.neighbor_23 = GetNodeInOtherChunk(chunks[chunkIndex], vertexIdPairs[node.side[2].vertexId],
                                                                vertexIdPairs[node.side[3].vertexId], node);
                    }
                    else if(vertexIDOverllaps.Contains(node.side[3].vertexId) && vertexIDOverllaps.Contains(node.side[0].vertexId)){
                        // Debug.Log($"Gonna need: {vertexIdPairs[node.side[3].vertexId]} and {vertexIdPairs[node.side[0].vertexId]}");
                        node.neighbor_30 = GetNodeInOtherChunk(chunks[chunkIndex], vertexIdPairs[node.side[3].vertexId],
                                                                vertexIdPairs[node.side[0].vertexId], node);
                }}

                //Create new clickable tiles for the overlapping verticies
                for(int i = 0; i < vertexIDOverllaps.Count; i++){

                    //get nodes containing the overlapping verticies
                    List<Node> nodes = new List<Node>();
                    foreach(Node n in chunk.mapData.nodes){
                        if(n.side[0].vertexId == vertexIDOverllaps[i] || n.side[1].vertexId == vertexIDOverllaps[i] ||
                           n.side[2].vertexId == vertexIDOverllaps[i] || n.side[3].vertexId == vertexIDOverllaps[i]) {
                                nodes.Add(n);
                    }}
                    //get nodes on other chunk containing the overlapping verticies
                    foreach(Node n in chunks[chunkIndex].mapData.nodes){
                        if(n.side[0].vertexId == vertexIDOverllapsOtherChunk[i] || n.side[1].vertexId == vertexIDOverllapsOtherChunk[i] ||
                           n.side[2].vertexId == vertexIDOverllapsOtherChunk[i] || n.side[3].vertexId == vertexIDOverllapsOtherChunk[i]) {
                                nodes.Add(n);
                    }}

                    //if there are 3 or more nodes, create a clickable tile
                    if(nodes.Count >= 3){
                        //need to delete old clickable tile if one exists

                        List<Vector3> corners = new List<Vector3>();
                        foreach(Node node in nodes){
                            Vector3 centroid = node.GetCentroid();
                            corners.Add(new Vector3(centroid.x*sizeMultiplier, 0, centroid.y*sizeMultiplier));
                        }
                        ClickableTile ct = CreateClickableTile(chunkIndex, corners);
                        chunks[chunkIndex].clickableTiles.Add(ct);
                    }
                }
    }}}
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