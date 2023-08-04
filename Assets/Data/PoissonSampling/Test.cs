using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
public class Test : MonoBehaviour {

	public float radius = 25;

    [Tooltip("Set region size to the size of the image you are using.")]
	public Vector2 regionSize = new (2048,1662);
	public int rejectionSamples = 30;
	public float displayRadius =1;

	[SerializeField] private List<Vector2> oldPoints;
	[SerializeField] private List<float2> points = new ();
    [SerializeField] private int pointCount => oldPoints.Count;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private GameObject textPrefab;
    public bool spawnText;

    [Header("Regions ")]
    [SerializeField] private Sprite regionsSprite;
    [SerializeField] private Color beachColor, plainsColor, forestColor, highlandsColor, rocks;
    [SerializeField] private List<LandDeed> landDeeds = new ();

    [SerializeField] private int maxRoads = 5;
    [SerializeField] private Color debugColor;
    [SerializeField] private List<int> distanceIncrements = new () { 50, 100, 150, 300, 450, 600, 750, 900, 1050, 1200, 1350, 1500 };

    [System.Serializable]
    public struct VectorContainer {
        public List<Vector2> vectors;
    }

    [System.Serializable]
    public struct LandDeed {
        public int id;
        public float2 location;
        public List<string> tags;
    }

    [System.Serializable] public struct LandDeedStruct{
        public List<LandDeed> landDeeds;
    }

    // Dictionary<float2, List<string>> pointTags = new ();

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

        var vectorContainer = new VectorContainer {
            vectors = oldPoints
        };

        string json = JsonUtility.ToJson(vectorContainer, true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/Points.json", json);
        // Debug.Log($"json: {json}");
    }
    [ContextMenu("Save Tags")]
    void SaveTags() {
        for(int i = 0; i < this.landDeeds.Count; i++) {
            this.landDeeds[i] = new LandDeed {
                id = i,
                location = this.landDeeds[i].location,
                tags = this.landDeeds[i].tags
            };
        }

        LandDeedStruct landDeeds = new LandDeedStruct {
            landDeeds = this.landDeeds
        };
        string json = JsonUtility.ToJson(landDeeds, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/LandDeeds.json", json);
    }
    [ContextMenu("Whole Process")]
    void WholeProcess() {
        LoadLandDeeds();
        CreateSettlements();
        RegionTags();
        SpawnText();
        GenerateRoads();
    }
   
    [ContextMenu("Add Settlement Tags")]
    private void CreateSettlements(){
        //add settlements to 150 random points
        int settlementCount = 150;
        for(int i = 0; i < settlementCount; i++) {
            int randomIndex = UnityEngine.Random.Range(0, 2500);
            if(!landDeeds[randomIndex].tags.Contains("settlement"))
                landDeeds[randomIndex].tags.Add("settlement");
            else
                i--;
        }
    }
    [ContextMenu("Add Region Tags")]
    void RegionTags() {

        // LoadLandDeeds();
        
        List<Vector2> pointsToReColor = new List<Vector2>();

        //destroy all children
        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < landDeeds.Count; i++) {
            Color pixelColor = regionsSprite.texture.GetPixel((int)landDeeds[i].location.x, (int)landDeeds[i].location.y);
            //get color that matches pixel color
            if(HasSimilarPixels(pixelColor,plainsColor)) {
                landDeeds[i].tags.Add("plains");
            }
            else if(HasSimilarPixels(pixelColor,beachColor)) {
                landDeeds[i].tags.Add("beach");
            }
            else if(HasSimilarPixels(pixelColor,forestColor)) {
                landDeeds[i].tags.Add("forest");
            }
            else if(HasSimilarPixels(pixelColor,highlandsColor)) {
                landDeeds[i].tags.Add("highlands");
            }
            else if(HasSimilarPixels(pixelColor,rocks)) {
                landDeeds[i].tags.Add("rocks");
            }
            else {
                
                Debug.LogError($"points missing a color: {landDeeds[i].location}");
            }
        }
        // string taggedPointsJson = JsonUtility.ToJson(vectorContainer, true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/taggedPoints.json", taggedPointsJson);
    }

    private void LoadLandDeeds(){
        landDeeds?.Clear();

        string json = System.IO.File.ReadAllText(Application.dataPath + "/Data/PoissonSampling/Points.json");
        VectorContainer vectorContainer = JsonUtility.FromJson<VectorContainer>(json);

        for(int i = 0; i < vectorContainer.vectors.Count; i++) {
            landDeeds.Add( new LandDeed { location = new float2(vectorContainer.vectors[i].x, vectorContainer.vectors[i].y), tags = new List<string>() });
        }
    }
    [ContextMenu("Spawn Text")]
    private void SpawnText() {
        for(int i = 0; i < landDeeds.Count; i++) {
            //spawn text mesh pro at point
            GameObject textMesh = Instantiate(textPrefab);
            textMesh.transform.parent = this.transform;
            textMesh.transform.position = new Vector3(landDeeds[i].location.x, landDeeds[i].location.y, 0);
            TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
            textMeshComponent.text = (i+1).ToString();
            textMeshComponent.color = GetColorFromTag(landDeeds[i].tags[0]);

        }
    }
    private Color GetColorFromTag(string tag) {
        return tag switch
        {
            "settlement" => Color.white,
            "plains" => plainsColor,
            "beach" => Color.black,
            "forest" => forestColor,
            "highlands" => Color.magenta,
            "rocks" => Color.cyan,
            _ => Color.black,
        };
    }
    [ContextMenu("Generate Roads")]
    public void GenerateRoads(){
        //get all settlements
        List<float2> settlements = new();
        Dictionary<float2, int> roadDict = new();


        for(int i = 0; i < landDeeds.Count; i++) {
            if(landDeeds[i].tags.Contains("settlement")) {
                settlements.Add(landDeeds[i].location);
                roadDict.Add(landDeeds[i].location, 0);
            }
        }
        Debug.Log($"settlements: {settlements.Count}");
        

        //get settlement in middle of map
        // float2 middle = new float2(1024, 831);
        // float2 closestSettlement = new float2(0,0);
        // float closestDistance = 1000000;
        // for(int i = 0; i < settlements.Count; i++) {
        //     float distance = math.distance(settlements[i], middle);
        //     if(distance < closestDistance) {
        //         closestDistance = distance;
        //         closestSettlement = settlements[i];
        //     }
        // }
        // Debug.Log($"closest settlement: {closestSettlement}");
        List<float2> settlementsWithoutRoads = new();
        List<float2> settlementsWithRoads = new();
        settlementsWithoutRoads.AddRange(settlements);

        //remove closest settlement from list
        // settlementsWithoutRoads.Remove(closestSettlement);

        //for settlement, create a road to the 5 closest settlements
        // for(int i = 0; i < settlements.Count; i++) {
        //     float distance = math.distance(settlements[i], closestSettlement);
        //     if(distance < 150) {
        //         Debug.DrawLine(new Vector3(settlements[i].x, settlements[i].y, 0), new Vector3(closestSettlement.x, closestSettlement.y, 0), Color.red, 30);
        //     }
        //     settlementsWithRoads.Add(settlements[i]);
        //     settlementsWithoutRoads.Remove(settlements[i]);
        // }
        

        for(int k = 0; k < distanceIncrements.Count; k++) {
            for(int i = 0; i < settlements.Count; i++) {
                for(int j = 0; j < settlements.Count; j++) {
                    if(i != j) {
                        float distance = math.distance(settlements[i], settlements[j]);
                        if(distance < distanceIncrements[k] && roadDict[settlements[i]] < maxRoads && roadDict[settlements[j]] < maxRoads) {

                            Debug.DrawLine(new Vector3(settlements[i].x, settlements[i].y, 0), new Vector3(settlements[j].x, settlements[j].y, 0), debugColor, 30);

                            roadDict[settlements[i]]++;
                            roadDict[settlements[j]]++;
                        }
                    }
                }
            }
        }


        // Debug.Log($"settlementsWithoutRoads: {settlementsWithoutRoads.Count}");
        // // for each settlement, get closest settlement and draw a line there
        // for(int i = 0; i < settlementsWithoutRoads.Count; i++) {
        //     float2 closest = new();
        //     float closestDistance2 = Mathf.Infinity;

        //     for(int j = 0; j < settlements.Count; j++) {
        //         float distance = math.distance(settlementsWithoutRoads[i], settlements[j]);
        //         if(distance < closestDistance2) {
        //             closestDistance2 = distance;
        //             closest = settlements[j];
        //         }
        //     }
        //     Debug.Log($"closest: {closest}");
        //     Debug.DrawLine(new Vector3(settlementsWithoutRoads[i].x, settlements[i].y, 0), new Vector3(closest.x, closest.y, 0), Color.blue, 30);
        // }

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