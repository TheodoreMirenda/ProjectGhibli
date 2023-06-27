using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector4 face;
    public bool isCollapsed, rootRoad;
    public GameObject emptyPrefab;
    public List<Prototype> possiblePrototypes;
    public List<int> prototypeWeights;
    public Vector2 coords = new Vector2();
    public Cell Neighbor_ZW;
    public Cell Neighbor_WX;
    public Cell Neighbor_XY;
    public Cell Neighbor_YZ;
    public SocketSO socket_ZW;
    public SocketSO socket_WX;
    public SocketSO socket_XY;
    public SocketSO socket_YZ;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public GameObject collapsedGameObject;
    public Vector3 centroid;
    public void GenerateWeight(Weights weights)
    {
        prototypeWeights = new List<int>(new int[possiblePrototypes.Count]);
        int i = 0;
        foreach(Prototype p in possiblePrototypes)
        {
            prototypeWeights[i] += weights.GetWeight(p.basePrototype);
            // prototypeWeights[i] = (int)((float)prototypeWeights[i]/ (float)p.attributes.Count);
            i++;
        }
    }
}