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

    [SerializeField] private int maxRoads = 5;
    [SerializeField] private Color debugColor;
    [SerializeField] private List<int> distanceIncrements = new () { 50, 100, 150, 300, 450, 600, 750, 900, 1050, 1200, 1350, 1500 };

    [System.Serializable] public struct NFTCollection {
        public List<LandDeedMetadata> landDeeds;
    }
    [System.Serializable] public struct LandDeedMetadata {
        public int id;
        public float longitude;
        public float lattitude;
        public string landType;//mystic, beach plains, highlands, forest

        public string underlandLeft;
        public string underlandMid;
        public string underlandRight;
        public string northSpot;
        public string southSpot;
        public string eastSpot;
        public string westSpot;

        public bool waterSource;  
        public bool road;
        public bool settlement;

        public LandDeedMetadata (int id, float longitude, float lattitude, string underlandLeft, 
                                 string underlandMid, string underlandRight, string northSpot, string southSpot, 
                                 string eastSpot, string westSpot, bool waterSource, bool road, bool settlement, string landType) {
            this.id = id;
            this.longitude = longitude;
            this.lattitude = lattitude;

            this.underlandLeft = underlandLeft;
            this.underlandMid = underlandMid;
            this.underlandRight = underlandRight;
            this.northSpot =  northSpot;
            this.southSpot = southSpot;
            this.eastSpot = eastSpot;
            this.westSpot = westSpot;

            this.waterSource = waterSource;
            this.road = road;
            this.settlement = settlement;
            this.landType = landType;
        }
    }
    [System.Serializable] public struct Trait {
        public string trait_type;
        public string value;
        public string display_type;
        public Trait(string trait_type, string value, string displayType) {
            this.trait_type = trait_type;
            this.value = value;
            this.display_type = displayType;
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
    [System.Serializable] public struct FinalMetadata {
        public string image;
        public string name;
        public Trait[] attributes;
        public FinalMetadata(string image, string name, Trait[] attributes) {
            this.image = image;
            this.name = name;
            this.attributes = attributes;
        }
    }
    [System.Serializable] public struct FinalMetadataList {
        public List<FinalMetadata> finalMetadata;
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

    [ContextMenu("Save MetaData")]
    void SaveMetaData() {
        string json = JsonUtility.ToJson(nftCollection, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/NFTCollection.json", json);
        SeededRandom.Init(0);

        FinalMetadataList finalMetadata = new(){
            finalMetadata = new List<FinalMetadata>()
        };
        for (int i = 0; i < nftCollection.landDeeds.Count; i++) {

            List<Trait> traits = new();
            if(nftCollection.landDeeds[i].landType.Length > 0)
                traits.Add(new Trait("landType", nftCollection.landDeeds[i].landType, "LandType"));
            if(nftCollection.landDeeds[i].underlandLeft.Length > 0)
                traits.Add(new Trait("underlandLeft", nftCollection.landDeeds[i].underlandLeft, GetTraitType(nftCollection.landDeeds[i].underlandLeft)));
            if(nftCollection.landDeeds[i].underlandMid.Length > 0)
                traits.Add(new Trait("underlandMid", nftCollection.landDeeds[i].underlandMid, GetTraitType(nftCollection.landDeeds[i].underlandMid)));
            if(nftCollection.landDeeds[i].underlandRight.Length > 0)
                traits.Add(new Trait("underlandRight", nftCollection.landDeeds[i].underlandRight, GetTraitType(nftCollection.landDeeds[i].underlandRight)));
            if(nftCollection.landDeeds[i].northSpot.Length > 0)
                traits.Add(new Trait("northSpot", nftCollection.landDeeds[i].northSpot, GetTraitType(nftCollection.landDeeds[i].northSpot)));

            if(nftCollection.landDeeds[i].settlement)
                traits.Add(new Trait("southSpot", "Settlement", "PlaceOfInterest"));
            else if(nftCollection.landDeeds[i].southSpot.Length > 0){
                traits.Add(new Trait("southSpot", nftCollection.landDeeds[i].southSpot, GetTraitType(nftCollection.landDeeds[i].southSpot)));
            }

            if(nftCollection.landDeeds[i].eastSpot.Length > 0)
                traits.Add(new Trait("eastSpot", nftCollection.landDeeds[i].eastSpot, GetTraitType(nftCollection.landDeeds[i].eastSpot)));
            if(nftCollection.landDeeds[i].westSpot.Length > 0)
                traits.Add(new Trait("westSpot", nftCollection.landDeeds[i].westSpot, GetTraitType(nftCollection.landDeeds[i].westSpot)));
            if(nftCollection.landDeeds[i].waterSource)
                traits.Add(new Trait("waterSource", GetRandomAttribute("WaterSource"), "WaterSource"));
            if(nftCollection.landDeeds[i].road)
                traits.Add(new Trait("road", GetRandomAttribute("Road"), "Road"));
            
            
            Trait[] traitsArray = traits.ToArray();

            finalMetadata.finalMetadata.Add(new FinalMetadata(
                "",
                (nftCollection.landDeeds[i].id+1).ToString(),
                traitsArray
            ));
            // finalMetadata.finalMetadata[i].attributes = traits;

        }

        string finalJson = JsonUtility.ToJson(finalMetadata, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/FinalMetadata.json", finalJson);
        Debug.Log($"saved metadata: {finalJson}");
    }
    private string GetTraitType(string traitName) {
        for(int i = 0; i < attributeDictionaries.Length; i++) {
            for(int j = 0; j < attributeDictionaries[i].attributes.Count; j++) {
                if(attributeDictionaries[i].attributes[j].traitName == traitName)
                {
                    if(attributeDictionaries[i].aspect == "Resource" || attributeDictionaries[i].aspect == "ResourceUnderland"){
                        return "Resource";
                    }
                    else if(attributeDictionaries[i].aspect == "PlaceOfInterest"){
                        return "PlaceOfInterest";
                    }
                    else if(attributeDictionaries[i].aspect == "Artifact" || attributeDictionaries[i].aspect == "ArtifactUnderland"){
                        return "Artifact";
                    }
                    else if(attributeDictionaries[i].aspect =="PlaceOfInterestOther"){
                        return "PlaceOfInterest";
                    }
                }
                    
            }
        }
        // Debug.LogError($"could not find trait type for {traitName}");
        return "Resource";
        // return null;
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

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            Color pixelColor = regionsSprite.texture.GetPixel((int)nftCollection.landDeeds[i].longitude, 
                (int)nftCollection.landDeeds[i].lattitude);

            // Debug.Log($"pixel color: {pixelColor}");

            string landType = "";
            //get color that matches pixel color
            if(HasSimilarPixels(pixelColor,plainsColor)) {
                landType = "Rolling Plains";
            }
            else if(HasSimilarPixels(pixelColor,beachColor)) {
                landType = "Beach";
            }
            else if(HasSimilarPixels(pixelColor,forestColor)) {
                landType = "Verdant Forest";
            }
            else if(HasSimilarPixels(pixelColor,highlandsColor)) {
                landType = "Highlands";
            }
            else if(HasSimilarPixels(pixelColor,rocks)) {
                landType = "Celestial Cliffs";
            }
            else if(HasSimilarPixels(pixelColor,Color.black)) {
                landType = "Mystic Grove";
            }
            else {
                Debug.LogError($"points missing a color: ");
            }

            nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMid,
                nftCollection.landDeeds[i].underlandRight,
                nftCollection.landDeeds[i].northSpot,
                nftCollection.landDeeds[i].southSpot,
                nftCollection.landDeeds[i].eastSpot,
                nftCollection.landDeeds[i].westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                landType
            );
        }
        SaveMetaData();
    }
    [ContextMenu("Add Random Attributes")]
    public void AddRandomAttributes(){
        SeededRandom.Init(0);

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            string underlandLeft = GetRandomAttribute("Underland");
             if(underlandLeft!="")
                underlandLeft = SeededRandom.Range(0f, 1f) switch
                {
                    > 0.33f => underlandLeft,
                    _ => "Chest",
                };

            string underlandMid = GetRandomAttribute("Underland");
            string underlandRight = GetRandomAttribute("Underland");

            string northSpot = GetRandomAttribute("Top");
            string southSpot = GetRandomAttribute("Top");
            string eastSpot = GetRandomAttribute("Top");
            string westSpot = GetRandomAttribute("Top");

            nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                underlandLeft,
                underlandMid,
                underlandRight,
                northSpot,
                southSpot,
                eastSpot,
                westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType
            );
        }
        // SpawnText();
        // GenerateRoads();
        GenerateWaterSources();
        HandleTrees();
        HandleRocks();
        Handle1of1s();
        SaveMetaData();
    }
    public string GetRandomAttribute(string attributeName){

        if(attributeName == "Top") {
            switch (SeededRandom.Range(0f,1f))
            {
                case > 0.4f:
                    attributeName = "Resource";
                    break;
                case > 0.2f:
                    attributeName = "PlaceOfInterest";
                    break;
                case > 0.15f:
                    attributeName = "Artifact";
                    break;
                default:
                    return "";
            } 
        }
        else if(attributeName =="Underland"){
            switch (SeededRandom.Range(0f,1f))
            {
                case > 0.7f:
                    attributeName = "ResourceUnderland";
                    break;
                case > 0.66f:
                    attributeName = "PlaceOfInterestUnderland";
                    break;
                case > 0.60f:
                    attributeName = "ArtifactUnderland";
                    break;
                default:
                    return "";
            } 
            if(attributeName == "PlaceOfInterestUnderland"){
                return (float)SeededRandom.Range(0f, 1f) switch
                {
                    > 0.65f => "Subway",
                    > 0.25f => "Cave System",
                    _ => "Dungeon",
                };
            }
        }

        for(int i = 0; i < attributeDictionaries.Length; i++) {

            if(attributeDictionaries[i].aspect == attributeName) {
                float random = SeededRandom.Range(0,attributeDictionaries[i].attributes.Count);
                return attributeDictionaries[i].attributes[(int)random].traitName;
            }
        }
        Debug.LogError($"could not find attribute: {attributeName}");
        return null;
    }

    // [ContextMenu("Handle 1 of 1s")]
    public void Handle1of1s(){
        int felisgarde = 1519;
        int ebisusBay = 2155;
        int brambleThorn = 986;
        int[] lightHouses = new int[] {2440, 2495, 2484, 2497, 2271, 2165, 2340, 2281, 2466, 2225, 1194, 892, 624, 350, 213, 163, 125, 781, 2322};
        int[] tikiLounge = new int[] {2303, 2352, 2441,2488,2473,2457,2460,2448,2444,2420,2385,2307,2210,2147,2111,2110,2119,2161,1043,905,816,800,586,610,670,382,499,718,360,408,305,259,161,360,599,169};

        for(int i = 0; i<nftCollection.landDeeds.Count; i++){
            if(felisgarde == nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMid,
                nftCollection.landDeeds[i].underlandRight,
                "Felisgarde",
                nftCollection.landDeeds[i].southSpot,
                nftCollection.landDeeds[i].eastSpot,
                nftCollection.landDeeds[i].westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType
                );
            }
            if(brambleThorn == nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMid,
                nftCollection.landDeeds[i].underlandRight,
                "Bramblethorn Titan",
                nftCollection.landDeeds[i].southSpot,
                nftCollection.landDeeds[i].eastSpot,
                nftCollection.landDeeds[i].westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType
                );
            }
            if(ebisusBay== nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMid,
                nftCollection.landDeeds[i].underlandRight,
                "Ebisu's Bay",
                nftCollection.landDeeds[i].southSpot,
                nftCollection.landDeeds[i].eastSpot,
                nftCollection.landDeeds[i].westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType
                );
            }
            for(int j = 0; j < lightHouses.Length; j++){
                if(lightHouses[j] == nftCollection.landDeeds[i].id+1){
                    nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    "Lighthouse",
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                    );
                }
            }
            for(int j = 0; j < tikiLounge.Length; j++){
                if(tikiLounge[j] == nftCollection.landDeeds[i].id+1){
                    nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    "Tiki Lounge",
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                    );
                }
            }
        }
        SaveMetaData();
    } 
    public void HandleTrees(){
        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            if(nftCollection.landDeeds[i].northSpot=="tree"){
                string newTree = GetNewTree(nftCollection.landDeeds[i].landType);

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    newTree,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
            if(nftCollection.landDeeds[i].southSpot=="tree"){
                string newTree = GetNewTree(nftCollection.landDeeds[i].landType);

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    newTree,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            if(nftCollection.landDeeds[i].eastSpot == "tree"){
                string newTree = GetNewTree(nftCollection.landDeeds[i].landType);

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    newTree,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            if(nftCollection.landDeeds[i].westSpot == "tree"){
                string newTree = GetNewTree(nftCollection.landDeeds[i].landType);

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    newTree,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
        }
        SaveMetaData();
    }
    public void HandleRocks(){
         for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            if(nftCollection.landDeeds[i].landType=="Celestial Cliffs"){

                string northSpot = SeededRandom.Range(0f,1f)>0.7 ? "Rock" : nftCollection.landDeeds[i].northSpot;
                string southSpot = SeededRandom.Range(0f,1f)>0.7 ? "Rock" : nftCollection.landDeeds[i].southSpot;
                string eastSpot = SeededRandom.Range(0f,1f)>0.7 ? "Rock" : nftCollection.landDeeds[i].eastSpot;
                string westSpot = SeededRandom.Range(0f,1f)>0.7 ? "Rock" : nftCollection.landDeeds[i].westSpot;

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMid,
                    nftCollection.landDeeds[i].underlandRight,
                    northSpot,
                    southSpot,
                    eastSpot,
                    westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
        }
        SaveMetaData();
    }    
    private string GetNewTree(string landType){
        string newTree = "";
        switch (SeededRandom.Range(0,3))
        {
            case 0:
                newTree = "Tree (Ash)";
                break;
            case 1:
                newTree = "Tree (Oak)";
                break;
            case 2:
                newTree = "Tree (Cherry Blossom)";
                break;
        } 

        if(landType=="Beach")
            newTree = "Tree (Palm)";

        if(landType=="Verdant Forest")
            newTree = SeededRandom.FlipCoin() ? "Nightshade" : newTree;
            
        return newTree;
    }
    // [ContextMenu("Load Land Deeds")]
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
            textMeshComponent.color = GetColorFromTag(nftCollection.landDeeds[i].landType);
            textMesh.GetComponent<LandDeedID>().id = nftCollection.landDeeds[i].id;
        }
    }
    private Color GetColorFromTag(string tag) {
        return tag switch
        {
            "Settlement" => Color.white,
            "plains" => plainsColor,
            "beach" => Color.black,
            "forest" => forestColor,
            "highlands" => Color.magenta,
            "rocks" => Color.cyan,
            "mystic" => Color.blue,
            _ => Color.black,
        };
    }
    [ContextMenu("Generate Roads")]
    public void GenerateRoads(){
        //get all settlements
        List<float2> settlements = new();
        Dictionary<float2, int> roadDict = new();

        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            if(nftCollection.landDeeds[i].settlement) {
                settlements.Add(new float2(nftCollection.landDeeds[i].longitude, nftCollection.landDeeds[i].lattitude));
                roadDict.Add(new float2(nftCollection.landDeeds[i].longitude, nftCollection.landDeeds[i].lattitude), 0);
            }
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

                                    if(nftCollection.landDeeds[hitObject.id].landType=="Beach")
                                        continue;

                                    nftCollection.landDeeds[hitObject.id] = new (
                                        nftCollection.landDeeds[hitObject.id].id,
                                        nftCollection.landDeeds[hitObject.id].longitude,
                                        nftCollection.landDeeds[hitObject.id].lattitude,
                                        nftCollection.landDeeds[hitObject.id].underlandLeft,
                                        nftCollection.landDeeds[hitObject.id].underlandMid,
                                        nftCollection.landDeeds[hitObject.id].underlandRight,
                                        nftCollection.landDeeds[hitObject.id].northSpot,
                                        nftCollection.landDeeds[hitObject.id].southSpot,
                                        nftCollection.landDeeds[hitObject.id].eastSpot,
                                        nftCollection.landDeeds[hitObject.id].westSpot,
                                        nftCollection.landDeeds[hitObject.id].waterSource,
                                        true,
                                        nftCollection.landDeeds[hitObject.id].settlement,
                                        nftCollection.landDeeds[hitObject.id].landType
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
        SaveMetaData();
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

                if(nftCollection.landDeeds[hitObject.id].landType=="beach")
                    continue;

                nftCollection.landDeeds[hitObject.id] = new (
                    nftCollection.landDeeds[hitObject.id].id,
                    nftCollection.landDeeds[hitObject.id].longitude,
                    nftCollection.landDeeds[hitObject.id].lattitude,
                    nftCollection.landDeeds[hitObject.id].underlandLeft,
                    nftCollection.landDeeds[hitObject.id].underlandMid,
                    nftCollection.landDeeds[hitObject.id].underlandRight,
                    nftCollection.landDeeds[hitObject.id].northSpot,
                    nftCollection.landDeeds[hitObject.id].southSpot,
                    nftCollection.landDeeds[hitObject.id].eastSpot,
                    nftCollection.landDeeds[hitObject.id].westSpot,
                    true,
                    nftCollection.landDeeds[hitObject.id].road,
                    nftCollection.landDeeds[hitObject.id].settlement,
                    nftCollection.landDeeds[hitObject.id].landType
                );
            }
            Debug.Log($"hit something: {raycastHits[0].collider.gameObject.name}");
        }
        else {
            Debug.Log($"hit nothing");
        }
        }
        SaveMetaData();

    }
    private bool HasSimilarPixels(Color a, Color b) {
        float threshold = 0.2f;
        if(Mathf.Abs(a.r - b.r) < threshold && Mathf.Abs(a.g - b.g) < threshold && Mathf.Abs(a.b - b.b) < threshold) {
            return true;
        }
        return false;
    }
}