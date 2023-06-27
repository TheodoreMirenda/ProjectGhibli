using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
public class GameCursor : MonoBehaviour
{
    public Transform goalPositon;
    public float lerpSpeed = 0.1f;
    [SerializeField] private LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
    private void Update()
    {
        if(transform.position != goalPositon.position)
        {
            transform.position = Vector3.Lerp(transform.position, goalPositon.position, lerpSpeed*Time.deltaTime);
        }
    }
    public void UpdateLineRenderer(List<Vector3> lines)
    {
        lineRenderer.positionCount = lines.Count+1;
        Vector3[] positions = new Vector3[lines.Count+1];
        for(int i = 0; i < lines.Count; i++)
            positions[i] = lines[i];

        positions[lines.Count] = lines[0];

        lineRenderer.SetPositions(positions);
        this.transform.position = lines[0];
    }
}
}
