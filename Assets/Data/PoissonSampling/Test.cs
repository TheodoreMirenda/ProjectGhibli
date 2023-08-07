using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
public class Test : MonoBehaviour {

    [SerializeField] NFTCollection nftCollection = new();
	public float radius = 25;

    [Tooltip("Set region size to the size of the image you are using.")]
	public Vector2 regionSize = new (2048,1662);
	public int rejectionSamples = 30;
	public float displayRadius =1;

	// [SerializeField] private List<Vector2> oldPoints;
	// [SerializeField] private List<float2> points = new ();
    // [SerializeField] private int pointCount => oldPoints.Count;
    [SerializeField] private Sprite mapSprite;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Material selectedMaterial, offMaterial;
    public bool spawnText;

    [Header("Regions")]
    [SerializeField] private Sprite regionsSprite;
    [SerializeField] private Color beachColor, plainsColor, forestColor, highlandsColor, rocks;
    // [SerializeField] private LandDeedStruct landDeeds = new ();

    [SerializeField] private int maxRoads = 5;
    [SerializeField] private Color debugColor;
    [SerializeField] private List<int> distanceIncrements = new () { 50, 100, 150, 300, 450, 600, 750, 900, 1050, 1200, 1350, 1500 };

    // [System.Serializable] public struct VectorContainer {
    //     public List<Vector2> vectors;
    // }
    // [System.Serializable] public struct LandDeed {
    //     public int id;
    //     public float2 location;
    //     public float lattitude;
    //     public float longitude;
    //     public List<string> tags;
    // }
    [System.Serializable] public struct NFTCollection {
        public List<LandDeedMetadata> landDeeds;
    }
    [System.Serializable] public struct LandDeedMetadata {
        public int id;
        public float longitude;
        public float lattitude;
        public string region;//mystic, beach plains, highlands, forest
        public string underlandLeft;
        public string underlandMid;
        public string underlandRight;
        public string artifact;
        public string placeOfInterest;
        public string resource;
        public string waterSource;  
        public string road;

        public LandDeedMetadata (int id, float longitude, float lattitude, string region, string underlandLeft, string underlandMid, 
                                 string underlandRight, string artifact, string placeOfInterest, string resource, string waterSource, string road) {
            this.id = id;
            this.longitude = longitude;
            this.lattitude = lattitude;
            this.region = region;
            this.underlandLeft = underlandLeft;
            this.underlandMid = underlandMid;
            this.underlandRight = underlandRight;
            this.artifact = artifact;
            this.placeOfInterest = placeOfInterest;
            this.resource = resource;
            this.waterSource = waterSource;
            this.road = road;
        }
    }
    
    public AttributeDictionary[] attributeDictionaries;
    [System.Serializable] public struct AttributeDictionary {
        public string aspect;
        public List<Attribute> attributes;
    }
    [System.Serializable] public struct Attribute {
        public string traitName;
        public float likelyhood;
    }
    public string thingToSearchFor;
    public void SearchForThing(){
        if(thingToSearchFor == "") return;

        // if(thingToSearchFor=="road"){
        //     for()
        // }
    }
    [ContextMenu("Generate Attributes")]
    public void GenerateAttributes(){
        for(int i = 0; i < attributeDictionaries.Length; i++) {
            for(int j = 0; j < attributeDictionaries[i].attributes.Count; j++) {
                Debug.Log($"aspect: {attributeDictionaries[i].aspect}, trait: {attributeDictionaries[i].attributes[j].traitName}, likelyhood: {attributeDictionaries[i].attributes[j].likelyhood}");
            }
        }
    }

    void GeneratePoints() {

        // for(int i = this.transform.childCount-1; i > 0; i--) {
        //     DestroyImmediate(this.transform.GetChild(i).gameObject);
        // }

        // oldPoints = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples, 2500, mapSprite);
            
        // //sort points numerically so top left is 1
        // oldPoints.Sort((a, b) => b.x.CompareTo(a.x));
        // oldPoints.Sort((a, b) => b.y.CompareTo(a.y));
        
        // if(spawnText) {
        //     for(int i = 0; i < oldPoints.Count; i++) {
        //         //spawn text mesh pro at point
        //         GameObject textMesh = Instantiate(textPrefab);
        //         textMesh.transform.parent = this.transform;
        //         textMesh.transform.position = new Vector3(oldPoints[i].x, oldPoints[i].y, 0);
        //         TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
        //         textMeshComponent.text = (i+1).ToString();
        //     }
        // }

        // var vectorContainer = new VectorContainer {
        //     vectors = oldPoints
        // };

        // string json = JsonUtility.ToJson(vectorContainer, true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/Points.json", json);
        // Debug.Log($"json: {json}");
    }

    [ContextMenu("Save Tags")]
    void SaveTags() {
        string json = JsonUtility.ToJson(nftCollection, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/NFTCollectionNew.json", json);
    }
    [ContextMenu("Whole Process")]
    void WholeProcess() {
        LoadLandDeeds();
        // CreateSettlements();
        RegionTags();
        SpawnText();
        GenerateRoads();
    }
   
    // [ContextMenu("Add Settlement Tags")]
    // private void CreateSettlements(){
    //     //add settlements to 150 random points
    //     // int settlementCount = 150;
    //     // for(int i = 0; i < settlementCount; i++) {
    //     //     int randomIndex = UnityEngine.Random.Range(0, 2500);
    //     //     if(!landDeeds[randomIndex].tags.Contains("settlement"))
    //     //         landDeeds[randomIndex].tags.Add("settlement");
    //     //     else
    //     //         i--;
    //     // }
    // }
    [ContextMenu("Add Region Tags")]
    void RegionTags() {

        // LoadLandDeeds();
        
        // List<Vector2> pointsToReColor = new List<Vector2>();

        // //destroy all children
        // for(int i = this.transform.childCount-1; i > 0; i--) {
        //     DestroyImmediate(this.transform.GetChild(i).gameObject);
        // }

        // for(int i = 0; i < landDeeds.Count; i++) {
        //     Color pixelColor = regionsSprite.texture.GetPixel((int)landDeeds[i].location.x, (int)landDeeds[i].location.y);
        //     //get color that matches pixel color
        //     if(HasSimilarPixels(pixelColor,plainsColor)) {
        //         landDeeds[i].tags.Add("plains");
        //     }
        //     else if(HasSimilarPixels(pixelColor,beachColor)) {
        //         landDeeds[i].tags.Add("beach");
        //     }
        //     else if(HasSimilarPixels(pixelColor,forestColor)) {
        //         landDeeds[i].tags.Add("forest");
        //     }
        //     else if(HasSimilarPixels(pixelColor,highlandsColor)) {
        //         landDeeds[i].tags.Add("highlands");
        //     }
        //     else if(HasSimilarPixels(pixelColor,rocks)) {
        //         landDeeds[i].tags.Add("rocks");
        //     }
        //     else {
                
        //         Debug.LogError($"points missing a color: {landDeeds[i].location}");
        //     }
        // }
        // string taggedPointsJson = JsonUtility.ToJson(vectorContainer, true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/taggedPoints.json", taggedPointsJson);
    }

    [ContextMenu("Load Land Deeds")]
    private void LoadLandDeeds(){
        string json = System.IO.File.ReadAllText(Application.dataPath + "/Data/PoissonSampling/NFTCollection.json");
        nftCollection = JsonUtility.FromJson<NFTCollection>(json);
    }
    [ContextMenu("Spawn Text")]
    private void SpawnText() {

        // //destroy all children
        for(int i = this.transform.childCount-1; i > 0; i--) {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            GameObject textMesh = Instantiate(textPrefab);
            textMesh.transform.SetParent(this.transform);
            textMesh.transform.position = new Vector3(nftCollection.landDeeds[i].longitude,
                 nftCollection.landDeeds[i].lattitude, 0);
            TMP_Text textMeshComponent = textMesh.GetComponent<TMP_Text>();
            textMeshComponent.text = (i+1).ToString();
            textMeshComponent.color = GetColorFromTag(nftCollection.landDeeds[i].region);
            textMesh.GetComponent<LandDeedID>().id = nftCollection.landDeeds[i].id;
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

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            if(nftCollection.landDeeds[i].region.Contains("settlement")) {
                settlements.Add(new float2(nftCollection.landDeeds[i].longitude, nftCollection.landDeeds[i].lattitude));
                roadDict.Add(new float2(nftCollection.landDeeds[i].longitude, nftCollection.landDeeds[i].lattitude), 0);
            }
            nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].region,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMid,
                nftCollection.landDeeds[i].underlandRight,
                nftCollection.landDeeds[i].artifact,
                nftCollection.landDeeds[i].placeOfInterest,
                nftCollection.landDeeds[i].resource,
                nftCollection.landDeeds[i].waterSource,
                ""
            );
        }
        Debug.Log($"settlements: {settlements.Count}");
        

        // get settlement in middle of map
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
        // List<float2> settlementsWithoutRoads = new();
        // List<float2> settlementsWithRoads = new();
        // settlementsWithoutRoads.AddRange(settlements);

        // // remove closest settlement from list
        // settlementsWithoutRoads.Remove(closestSettlement);

        // for settlement, create a road to the 5 closest settlements
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
                        if(distance < distanceIncrements[k] && roadDict[settlements[i]] < 
                            maxRoads && roadDict[settlements[j]] < maxRoads) {

                            Debug.DrawLine(new Vector3(settlements[i].x, settlements[i].y, 
                            0), new Vector3(settlements[j].x, settlements[j].y, 0), debugColor, 30);

                            roadDict[settlements[i]]++;
                            roadDict[settlements[j]]++;

                            //spherecast along line to see if it hits anything
                            Vector3 direction = new Vector3(settlements[j].x, settlements[j].y, 0) -  new Vector3(settlements[i].x, settlements[i].y, 0);
                            RaycastHit[] raycastHits = Physics.SphereCastAll(new Vector3(settlements[i].x, settlements[i].y, 0), 5, direction, distance);
                            if(raycastHits.Length > 0) {
                                Debug.Log($"amount of hits {raycastHits.Length}");
                                foreach(RaycastHit hit in raycastHits) {
                                    LandDeedID hitObject = hit.collider.gameObject.GetComponentInParent<LandDeedID>();
                                    hitObject.GetComponent<TMP_Text>().color = debugColor;

                                    if(nftCollection.landDeeds[hitObject.id].region=="beach")
                                        continue;

                                    nftCollection.landDeeds[hitObject.id] = new LandDeedMetadata(
                                        nftCollection.landDeeds[hitObject.id].id,
                                        nftCollection.landDeeds[hitObject.id].longitude,
                                        nftCollection.landDeeds[hitObject.id].lattitude,
                                        nftCollection.landDeeds[hitObject.id].region,
                                        nftCollection.landDeeds[hitObject.id].underlandLeft,
                                        nftCollection.landDeeds[hitObject.id].underlandMid,
                                        nftCollection.landDeeds[hitObject.id].underlandRight,
                                        nftCollection.landDeeds[hitObject.id].artifact,
                                        nftCollection.landDeeds[hitObject.id].placeOfInterest,
                                        nftCollection.landDeeds[hitObject.id].resource,
                                        nftCollection.landDeeds[hitObject.id].waterSource,
                                        "dirt path"
                                    );
                                }
                                // Debug.Log($"hit something: {raycastHits[0].collider.gameObject.name}");
                            }
                            else {
                                // Debug.Log($"hit nothing");
                            }
                        }
                    }
                }
            }
        }
    }
    
    [ContextMenu("Generate WaterSources")]
    public void GenerateWaterSources() {

    for(int i = 0; i < 5; i++) {
        //get point on left edge of canvas
        Vector3 point = new (0, UnityEngine.Random.Range(0, 1646),0);
        //get point on oposite edge of canvas
        Vector3 point2 = new (2048, UnityEngine.Random.Range(0, 1646),0);
        Debug.DrawLine(point, point2, debugColor, 30);
        //spherecast along line to see if it hits anything
        Vector3 direction = point2 -  point;
        float distance = Vector3.Distance(point, point2);
        RaycastHit[] raycastHits = Physics.SphereCastAll(point, 5, direction, distance);

        if(raycastHits.Length > 0) {
            Debug.Log($"amount of hits {raycastHits.Length}");
            foreach(RaycastHit hit in raycastHits) {
                LandDeedID hitObject = hit.collider.gameObject.GetComponentInParent<LandDeedID>();
                hitObject.GetComponent<TMP_Text>().color = debugColor;

                if(nftCollection.landDeeds[hitObject.id].region=="beach")
                    continue;

                nftCollection.landDeeds[hitObject.id] = new LandDeedMetadata(
                    nftCollection.landDeeds[hitObject.id].id,
                    nftCollection.landDeeds[hitObject.id].longitude,
                    nftCollection.landDeeds[hitObject.id].lattitude,
                    nftCollection.landDeeds[hitObject.id].region,
                    nftCollection.landDeeds[hitObject.id].underlandLeft,
                    nftCollection.landDeeds[hitObject.id].underlandMid,
                    nftCollection.landDeeds[hitObject.id].underlandRight,
                    nftCollection.landDeeds[hitObject.id].artifact,
                    nftCollection.landDeeds[hitObject.id].placeOfInterest,
                    nftCollection.landDeeds[hitObject.id].resource,
                    "river",
                    nftCollection.landDeeds[hitObject.id].road
                );
            }
            Debug.Log($"hit something: {raycastHits[0].collider.gameObject.name}");
        }
        else {
            Debug.Log($"hit nothing");
        }
        }
        

    }
    [ContextMenu("Generate Resources")]
    public void GenerateResources() {
        

    }
    [ContextMenu("Provide Summary")]
    public void ProvideSummary() {
        // get the counts of each reagion type
        int beachCount = 0;
        int plainsCount = 0;
        int forestCount = 0;
        int highlandsCount = 0;
        int rocksCount = 0;
        int settlementCount = 0;
        int dirtPathCount = 0;
        int waterSourceCount = 0;
        int resourceCount = 0;

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            if(nftCollection.landDeeds[i].region.Contains("beach")) {
                beachCount++;
            }
            else if(nftCollection.landDeeds[i].region.Contains("plains")) {
                plainsCount++;
            }
            else if(nftCollection.landDeeds[i].region.Contains("forest")) {
                forestCount++;
            }
            else if(nftCollection.landDeeds[i].region.Contains("highlands")) {
                highlandsCount++;
            }
            else if(nftCollection.landDeeds[i].region.Contains("rocks")) {
                rocksCount++;
            }
            if(nftCollection.landDeeds[i].region.Contains("settlement")) {
                settlementCount++;
            }
            if(nftCollection.landDeeds[i].road.Contains("dirt path")) {
                dirtPathCount++;
            }
            if(nftCollection.landDeeds[i].waterSource.Contains("river")) {
                waterSourceCount++;
            }
            if(nftCollection.landDeeds[i].resource.Contains("iron")) {
                resourceCount++;
            }
        }

        Debug.Log($"beach: {beachCount}, plains: {plainsCount}, forest: {forestCount}, highlands: {highlandsCount}, rocks: {rocksCount}");
        Debug.Log($"settlements: {settlementCount}, dirt paths: {dirtPathCount}, water sources: {waterSourceCount}, resources: {resourceCount}");
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