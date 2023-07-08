using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
public class Node : MonoBehaviour
{
    public NodeSide[] side = new NodeSide[4];
    public Node neighbor_01, neighbor_12, neighbor_23, neighbor_30;
    public SocketSO socket_ZW;
    public SocketSO socket_WX;
    public SocketSO socket_XY;
    public SocketSO socket_YZ;
    MeshManager meshManager;
    public Prototype activePrototype;
    [SerializeField] float sizeMultiplier = 1f;

    public void AssignSides(Cell cell, List<Vector2> globalVerticies, float _sizeMultiplier)
    {
        //W0, X1, Y2, Z3
        side[0].vertexId = (int)cell.face.w;
        side[0].vertexPosition = globalVerticies[(int)cell.face.w];
        side[1].vertexId = (int)cell.face.x;
        side[1].vertexPosition = globalVerticies[(int)cell.face.x];
        side[2].vertexId = (int)cell.face.y;
        side[2].vertexPosition = globalVerticies[(int)cell.face.y];
        side[3].vertexId = (int)cell.face.z;
        side[3].vertexPosition = globalVerticies[(int)cell.face.z];

        neighbor_01 = cell.Neighbor_WX?.GetComponent<Node>();
        neighbor_12 = cell.Neighbor_XY?.GetComponent<Node>();
        neighbor_23 = cell.Neighbor_YZ?.GetComponent<Node>();
        neighbor_30 = cell.Neighbor_ZW?.GetComponent<Node>();

        sizeMultiplier = _sizeMultiplier;
        SquishCell();
    }

    public void MarkCorner(int cornerNumber, int value) {
        for(int i = 0; i < side.Length; i++) {
            if(side[i].vertexId == cornerNumber) {
                side[i].value = value;
                CalculateSockets();
                break;
    }}}

    public void CalculateSockets()
    {
        //W0, X1, Y2, Z3
        meshManager = FindObjectOfType<MeshManager>();
        socket_ZW = meshManager.GetSocket(side[3].value, side[0].value);
        socket_WX = meshManager.GetSocket(side[0].value, side[1].value);
        socket_XY = meshManager.GetSocket(side[1].value, side[2].value);
        socket_YZ = meshManager.GetSocket(side[2].value, side[3].value);

        activePrototype = meshManager.GetPrefab(socket_ZW, socket_WX, socket_XY, socket_YZ, this);
        if(activePrototype != null)
        {
            this.gameObject.name = activePrototype.name;
            Cell cell = gameObject.GetComponent<Cell>();
            cell.possiblePrototypes[0] = activePrototype;

            SquishCell(activePrototype.prefab);
        }
    }
    public void SquishCell(GameObject thingToSpawn = null)
    {
        Cell cell = gameObject.GetComponent<Cell>();

        if(thingToSpawn == null)
            thingToSpawn = FindObjectOfType<MeshManager>().GrassPrefab;

        for(int i = cell.transform.childCount; i > 0; i--)
            DestroyImmediate(cell.transform.GetChild(0).gameObject);

        cell.collapsedGameObject = Instantiate(thingToSpawn, cell.centroid, Quaternion.identity, cell.transform);
        activePrototype = cell.possiblePrototypes[0];
        SquishMesh(cell);
        // SquishOtherObject(cell);

        Tile tile = cell.collapsedGameObject.GetComponent<Tile>();

        //move the position to face x
        tile.tileMeshFilter.gameObject.transform.position = new Vector3(
            side[0].vertexPosition.x*sizeMultiplier, 
            0, 
            side[0].vertexPosition.y*sizeMultiplier);

        // Debug.Log($"sizeMultiplier: {sizeMultiplier}");
        // HandleDecorations(tile);
    }
    private void HandleDecorations(Tile tile)
    {
        SeededRandom.Init(69);
        if(tile.decorations==null)
            return;

        tile.decorations.gameObject.transform.localPosition = Vector3.zero;
        foreach(Transform child in tile.decorations.transform)
        {
            if(tile.trueNorth != null && child.name == "RotateMe")
            {
                child.transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(tile.trueNorth.position - 
                    child.transform.position, Vector3.up).eulerAngles.y, 0);
            }
        }

        //activate decorations
        for(int i = 0; i < tile.decorationData.Length; i++)
        {
            // SeededRandom.Init(seed+i);
            if(SeededRandom.Range(0f,1f) > tile.decorationData[i].activationChance)
                tile.decorationData[i].decorationGameObject.gameObject.SetActive(false);
        }
    }
    public void SquishMesh(Cell cell)
    {
        Vector4 face = cell.face;
        float desiredRotation = (activePrototype.meshRotation)*-90;
        Tile tile = cell.collapsedGameObject.GetComponent<Tile>();

        //Get mesh of land
        Mesh mesh = tile.tileMeshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;
        Vector3[] normals = mesh.normals;

        Quaternion newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0,desiredRotation,0);//the degrees the vertices are to be rotated, for example (0,90,0) 
        // Debug.Log($"desiredRotation: {desiredRotation}");
        
        //vertices being the array of vertices of your mesh
        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] = newRotation * (vertices[i] - mesh.bounds.center) + mesh.bounds.center;
            // vertices[i] = newRotation * (vertices[i] - new Vector3(0.25f, 0f, 0.25f)) + new Vector3(0.25f, 0f, 0.25f);
        }

        //rotate the decorations 
        if(tile.decorations!=null)
        {
            foreach(Transform child in tile.decorations?.transform)
                child.transform.localPosition = newRotation * (child.transform.localPosition  - mesh.bounds.center) + mesh.bounds.center;
        //     child.transform.localPosition = newRotation * (child.transform.localPosition - new Vector3(0.25f, 0, 0.25f)) +new Vector3(0.25f, 0, 0.25f);
        }
        
        //W0, X1, Y2, Z3
        //Get coords of the underlying face
        Vector3 a = new Vector3(side[0].vertexPosition.x, 0, side[0].vertexPosition.y);
        Vector3 b = new Vector3(side[3].vertexPosition.x, 0, side[3].vertexPosition.y);
        Vector3 c = new Vector3(side[2].vertexPosition.x, 0, side[2].vertexPosition.y);
        Vector3 d = new Vector3(side[1].vertexPosition.x, 0, side[1].vertexPosition.y);

        b-=a;
        c-=a;
        d-=a;
        a-=a;

        //get the x and y value of all verticies of the origional mesh
        for(int i = 0; i < vertices.Length; i++)
        {   
            //get the x and y value of the vertex
            float x = vertices[i].x*2;
            float y = vertices[i].y*2;
            float z = vertices[i].z*2;

            Vector3 q = Vector3.Lerp(a, b, x);
            Vector3 r = Vector3.Lerp(d, c, x);
            Vector3 p = Vector3.Lerp(r, q, z);

            vertices[i].x = p.x;
            vertices[i].z = p.z;
            vertices[i].y += p.y;
        }

        // //rotate decoration
        if(tile.decorations!=null)
        {
            //get each child gameobject of decorations
            foreach(Transform child in tile.decorations.transform)
            {
                //rotate the local rotation of the child
                //get the x and y value of the vertex
                float x = (child.localPosition.x)*2;
                float y = (child.localPosition.y)*2;
                float z = (child.localPosition.z)*2;

                Vector3 q = Vector3.Lerp(a, b, x);
                Vector3 r = Vector3.Lerp(d, c, x);
                Vector3 p = Vector3.Lerp(r, q, z);

                child.localPosition = new Vector3(p.x, child.localPosition.y+p.y, p.z);
            }
        }

        //create new mesh and assign it to the land
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        // mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        tile.tileMeshFilter.sharedMesh = mesh;
        tile.tileMeshFilter.GetComponent<MeshCollider>().sharedMesh = mesh;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    public Vector2 GetCentroid()
    {
        Vector2 centroid = Vector2.zero;
        for(int i = 0; i < side.Length; i++)
            centroid += side[i].vertexPosition;
            
        centroid /= side.Length;
        return centroid;
    }
    public Vector3 GetCentroidAsVector3(){
        Vector2 centroid = GetCentroid();
        return new Vector3(centroid.x, 0, centroid.y);
    }
}
[System.Serializable] public struct NodeSide
{
    public int vertexId, value;
    public Vector2 vertexPosition;
}
}
