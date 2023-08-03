using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour {

	public float radius = 25;

    [Tooltip("Set region size to the size of the image you are using.")]
	public Vector2 regionSize = new Vector2(2048,1662);
	public int rejectionSamples = 30;
	public float displayRadius =1;

	[SerializeField] private List<Vector2> points;
    [SerializeField] private int pointCount => points.Count;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private GameObject textPrefab;
    public bool spawnText;

    [System.Serializable]
    public struct VectorContainer {
        public List<Vector2> vectors;
    }

	// void OnValidate() {
	// 	points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
	// }
    [ContextMenu("Generate Points")]
    void GeneratePoints() {

        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples, 2500, mapSprite);

        //sort points numerically so top left is 1
        points.Sort((a, b) => b.x.CompareTo(a.x));
        points.Sort((a, b) => b.y.CompareTo(a.y));
        
        if(spawnText) {
            for(int i = 0; i < points.Count; i++) {
                //spawn text mesh pro at point
                GameObject textMesh = Instantiate(textPrefab);
                textMesh.transform.parent = this.transform;
                textMesh.transform.position = new Vector3(points[i].x, points[i].y, 0);
                TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
                textMeshComponent.text = (i+1).ToString();
            }
        }

        var vectorContainer = new VectorContainer();
        vectorContainer.vectors = points; 

        string json = JsonUtility.ToJson(vectorContainer, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/Points.json", json);
        // Debug.Log($"json: {json}");
    }

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(regionSize/2,regionSize);
		if (points != null) {
			foreach (Vector2 point in points) {
				Gizmos.DrawSphere(point, displayRadius);
			}
		}
	}
}