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
    [SerializeField] private Material material;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private List<Color> colorSpaces = new List<Color>(), bannedColors = new List<Color>();
    [SerializeField] private GameObject textPrefab;
    public bool spawnText;

	// void OnValidate() {
	// 	points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
	// }
    [ContextMenu("Generate Points")]
    void GeneratePoints() {
        //delete all child objects of this.transform
        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples, 2500, mapSprite);

        //sort points numerically so top left is 1
        points.Sort((a, b) => b.x.CompareTo(a.x));
        points.Sort((a, b) => b.y.CompareTo(a.y));
        
        if(!spawnText) {
            return;
        }

        for(int i = 0; i < points.Count; i++) {
            //spawn text mesh pro at point
            GameObject textMesh = Instantiate(textPrefab);
            textMesh.transform.parent = this.transform;
            textMesh.transform.position = new Vector3(points[i].x, points[i].y, 0);
            TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
            textMeshComponent.text = (i+1).ToString();
        }
    }

    [ContextMenu("Create Mesh from coords")]
    void CreateMeshFromPoints() {
        string coords="1016,2448,1010,2529,1146,2578,1369,2567,1792,2524,1836,2453,1586,2333,1809,2111,2059,2046,2347,2051,2520,2105,2423,2198,2504,2192,2499,2246,2575,2279,2580,2322,2662,2366,2743,2333,2819,2355,2928,2322,2857,2268,2873,2225,3004,2176,2911,2056,3009,1953,3118,1867,3096,1747,3004,1715,3020,1633,2868,1611,2971,1503,3036,1411,3107,1356,3085,1291,2933,1259,2841,1232,2922,1161,2906,1096,2819,1058,2667,884,2526,732,2423,662,2227,629,1945,580,1782,559,1640,553,1483,521,1391,466,1434,385,1450,304,1380,168,1217,157,1075,206,1005,282,1021,374,1103,396,1119,483,956,564,842,624,663,700,358,846,244,955,315,1020,293,1069,223,1134,348,1177,462,1210,462,1313,576,1389,663,1465,608,1585,592,1672,690,1786,815,1851,880,1889,869,1948,972,1992,1141,2046,1179,2117,1048,2182,826,2231,717,2279,907,2334,1070,2377";
        string[] coordsArray = coords.Split(',');

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
        Vector3[] vertices = new Vector3[coordsArray.Length/2];
        int[] triangles = new int[coordsArray.Length/2];
        for (int i = 0; i < coordsArray.Length; i+=2)
        {
            vertices[i/2] = new Vector3(float.Parse(coordsArray[i]), 0, float.Parse(coordsArray[i+1]));
            triangles[i/2] = i/2;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Debug.Log("Mesh created");
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