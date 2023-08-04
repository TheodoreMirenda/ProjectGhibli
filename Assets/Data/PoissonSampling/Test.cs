using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
public class Test : MonoBehaviour {

	public float radius = 25;

    [Tooltip("Set region size to the size of the image you are using.")]
	public Vector2 regionSize = new Vector2(2048,1662);
	public int rejectionSamples = 30;
	public float displayRadius =1;

	[SerializeField] private List<Vector2> oldPoints;
	[SerializeField] private List<float2> points = new List<float2>();
    [SerializeField] private int pointCount => oldPoints.Count;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private GameObject textPrefab;
    public bool spawnText;

    [Header("Regions ")]
    [SerializeField] private Sprite regionsSprite;
    [SerializeField] private Color beachColor, plainsColor, forestColor, highlandsColor, rocks;


    [System.Serializable]
    public struct VectorContainer {
        public List<Vector2> vectors;
    }

    private Dictionary<float2, List<string>> pointTags = new Dictionary<float2, List<string>>();

	// void OnValidate() {
	// 	points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
	// }
    // [ContextMenu("Generate Points")]
    void GeneratePoints() {

        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        oldPoints = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples, 2500, mapSprite);

        //sort points numerically so top left is 1
        oldPoints.Sort((a, b) => b.x.CompareTo(a.x));
        oldPoints.Sort((a, b) => b.y.CompareTo(a.y));
        
        if(spawnText) {
            for(int i = 0; i < oldPoints.Count; i++) {
                //spawn text mesh pro at point
                GameObject textMesh = Instantiate(textPrefab);
                textMesh.transform.parent = this.transform;
                textMesh.transform.position = new Vector3(oldPoints[i].x, oldPoints[i].y, 0);
                TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
                textMeshComponent.text = (i+1).ToString();
            }
        }

        var vectorContainer = new VectorContainer();
        vectorContainer.vectors = oldPoints; 

        string json = JsonUtility.ToJson(vectorContainer, true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/Points.json", json);
        // Debug.Log($"json: {json}");
    }
    [ContextMenu("Tag Points")]
    void TagPoints() {

        //load json
        string json = System.IO.File.ReadAllText(Application.dataPath + "/Data/PoissonSampling/Points.json");
        VectorContainer vectorContainer = JsonUtility.FromJson<VectorContainer>(json);
        points?.Clear();

        for(int i = 0; i < vectorContainer.vectors.Count; i++) {
            points.Add(new float2(vectorContainer.vectors[i].x, vectorContainer.vectors[i].y));
            pointTags.Add(new float2(points[i].x, points[i].y), new List<string>());
        }
        
        List<Vector2> pointsToReColor = new List<Vector2>();

        //destroy all children
        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        CreateSettlements();

        for(int i = 0; i < points.Count; i++) {
            Color pixelColor = regionsSprite.texture.GetPixel((int)points[i].x, (int)points[i].y);
            //get color that matches pixel color
            if(HasSimilarPixels(pixelColor,plainsColor)) {
                pointTags[points[i]].Add("plains");
            }
            else if(HasSimilarPixels(pixelColor,beachColor)) {
                pointTags[points[i]].Add("beach");
            }
            else if(HasSimilarPixels(pixelColor,forestColor)) {
                pointTags[points[i]].Add("forest");
            }
            else if(HasSimilarPixels(pixelColor,highlandsColor)) {
                pointTags[points[i]].Add("highlands");
            }
            else if(HasSimilarPixels(pixelColor,rocks)) {
                pointTags[points[i]].Add("rocks");
            }
            else {
                pointsToReColor.Add(points[i]);
            }
        }

        SpawnText();
        
        Debug.Log($"points missing a color: {pointsToReColor.Count}");

        string taggedPointsJson = JsonUtility.ToJson(vectorContainer, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/taggedPoints.json", taggedPointsJson);
    }
    private void CreateSettlements(){
        //add settlements to 150 random points
        int settlementCount = 150;
        for(int i = 0; i < settlementCount; i++) {
            int randomIndex = UnityEngine.Random.Range(0, 2500);
            pointTags[points[randomIndex]].Add("settlement");
        }
    }
    private void SpawnText() {
        for(int i = 0; i < pointTags.Count; i++) {
            //spawn text mesh pro at point
            GameObject textMesh = Instantiate(textPrefab);
            textMesh.transform.parent = this.transform;
            textMesh.transform.position = new Vector3(points[i].x, points[i].y, 0);
            TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
            textMeshComponent.text = (i+1).ToString();
            textMeshComponent.color = GetColorFromTag(pointTags[points[i]][0]);
        }
    }
    private Color GetColorFromTag(string tag) {
        switch(tag) {
            case "settlement":
                return Color.white;
            case "plains":
                return plainsColor;
            case "beach":
                return Color.black;
            case "forest":
                return forestColor;
            case "highlands":
                return Color.magenta;
            case "rocks":
                return Color.cyan;
            default:
                return Color.black;
        }
    }
	// void OnDrawGizmos() {
	// 	Gizmos.DrawWireCube(regionSize/2,regionSize);
	// 	if (points != null) {
	// 		foreach (Vector2 point in points) {
	// 			Gizmos.DrawSphere(point, displayRadius);
	// 		}
	// 	}
	// }
    private bool HasSimilarPixels(Color a, Color b) {
        float threshold = 0.2f;
        if(Mathf.Abs(a.r - b.r) < threshold && Mathf.Abs(a.g - b.g) < threshold && Mathf.Abs(a.b - b.b) < threshold) {
            return true;
        }
        return false;
    }
}