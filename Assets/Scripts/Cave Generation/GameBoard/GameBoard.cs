using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TJ
{
public class GameBoard : MonoBehaviour
{
    [SerializeField] private LandGenerator landGenerator;
    [SerializeField] private Chunk[] chunks;
    [SerializeField] private List<Chunk> chunks2;
    [SerializeField] private GameObject clickableTilePrefab;
    [SerializeField] private List<ClickableTile> clickableTiles;
    [SerializeField] private float sizeMultiplier = 5f;

    [SerializeField] private SerializableDictionary<int, int5> vertexToFaceMap = new SerializableDictionary<int, int5>();
    public void LoadData()
    {
        chunks = landGenerator.hexagons.ToArray();
    }
    public void CreateChunk()
    {
        Chunk newChunk = landGenerator.CreateChunk();
        chunks2.Add(newChunk);
    }
    [ContextMenu("Create Clickable Tiles")]
    public void CreateClickableTiles()
    {
        clickableTiles.Clear();

        // destroy all children of the land parent
        for (int i = this.transform.childCount; i > 0; i--)
            DestroyImmediate(this.transform.GetChild(0).gameObject);

        foreach(Chunk chunk in chunks)
        {
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
                    Quaternion.identity, this.transform).GetComponent<ClickableTile>();

                ct.corners = corners;
                ct.vertexId = i;
                clickableTiles.Add(ct);
            }
        }

        foreach(ClickableTile tile in clickableTiles)
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
        SetUpNodes();
    }
    [ContextMenu("SetUpNodes")]
    public void SetUpNodes()
    {
        // if(landParent != null)
        //     DestroyImmediate(landParent.gameObject);
        
        // landParent = new GameObject().transform;

        Node[] nodes = FindObjectsOfType<Node>();
        foreach(Node n in nodes)
        {
            Cell cell = n.gameObject.GetComponent<Cell>();
            n.AssignSides(cell, chunks[0].mapData.globalVerticies, sizeMultiplier);            
        }

        // landParent.localScale = new Vector3(1, 1, 1);
        // landParent.localPosition = new Vector3(0, 0, 0);
        CreateVertexToFaceMap();
    }
    [ContextMenu("SetUpVertexToFaceMap")]
    public void CreateVertexToFaceMap()
    {
        vertexToFaceMap.Clear();
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