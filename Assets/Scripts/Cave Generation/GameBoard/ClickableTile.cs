using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
[System.Serializable] public class ClickableTile : MonoBehaviour
{
    // public int vertexId;
    public MeshFilter clickableTileMesh;
    public MeshCollider clickableTileCollider;
    public List<Vector3> corners;
    public LineRenderer lineRenderer;
    // public Vector4 cornerFaces;

    private void Awake()
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    public void HighlightTile(bool _highlight){

        // Debug.Log("Highlighting tile: " + _highlight.ToString());
        lineRenderer.enabled = _highlight;
    }
}
}
