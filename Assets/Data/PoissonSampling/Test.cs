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
        public string underlandMiddle;
        public string underlandRight;
        public string northSpot;
        public string southSpot;
        public string eastSpot;
        public string westSpot;

        public bool waterSource;  
        public bool road;
        public bool settlement;
        public string legendary;

        public LandDeedMetadata (int id, float longitude, float lattitude, string underlandLeft, 
                                 string underlandMiddle, string underlandRight, string northSpot, string southSpot, 
                                 string eastSpot, string westSpot, bool waterSource, bool road, bool settlement, string landType, string legendary = "") {
            this.id = id;
            this.longitude = longitude;
            this.lattitude = lattitude;

            this.underlandLeft = underlandLeft;
            this.underlandMiddle = underlandMiddle;
            this.underlandRight = underlandRight;
            this.northSpot =  northSpot;
            this.southSpot = southSpot;
            this.eastSpot = eastSpot;
            this.westSpot = westSpot;

            this.waterSource = waterSource;
            this.road = road;
            this.settlement = settlement;
            this.landType = landType;
            this.legendary = legendary;
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
        // public float likelyhood;
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
            if(nftCollection.landDeeds[i].underlandMiddle.Length > 0)
                traits.Add(new Trait("underlandMiddle", nftCollection.landDeeds[i].underlandMiddle, GetTraitType(nftCollection.landDeeds[i].underlandMiddle)));
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
            if(nftCollection.landDeeds[i].legendary != "")
                traits.Add(new Trait("legendary", nftCollection.landDeeds[i].legendary, "Legendary"));
            
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
        Debug.Log($"saved metadata");
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
                landType = "Rolling-Plains";
            }
            else if(HasSimilarPixels(pixelColor,beachColor)) {
                landType = "Beach";
            }
            else if(HasSimilarPixels(pixelColor,forestColor)) {
                landType = "Verdant-Forest";
            }
            else if(HasSimilarPixels(pixelColor,highlandsColor)) {
                landType = "Highlands";
            }
            else if(HasSimilarPixels(pixelColor,rocks)) {
                landType = "Celestial-Cliffs";
            }
            else if(HasSimilarPixels(pixelColor,Color.black)) {
                landType = "Mystic-Grove";
            }
            else {
                Debug.LogError($"points missing a color: ");
            }

            nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMiddle,
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
                    > 0.16f => underlandLeft,
                    _ => "Fortune-Chest",
                };

            string underlandMiddle = GetRandomAttribute("Underland");
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
                underlandMiddle,
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
        // GenerateWaterSources();
        HandleTrees();
        HandleRocks();
        HandlePruneMines();
        Handle1of1s();
        OneOffFixes();
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
                case > 0.65f:
                    attributeName = "ResourceUnderland";
                    break;
                case > 0.60f:
                    attributeName = "PlaceOfInterestUnderland";
                    break;
                case > 0.55f:
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

    // [ContextMenu("Handle 1 of 1s")]//and Inns and Lighthouses and Tiki Lounges
    public void Handle1of1s(){
        int felisgarde = 1121;
        int ebisusBay = 2155;
        int brambleThorn = 662;
        int[] lightHouses = new int[] {2440, 2495, 2484, 2497, 2271, 2165, 2340, 2281, 2466, 2225, 1194, 892, 624, 350, 213, 163, 125, 781, 2322};
        int[] tikiLounge = new int[] {2303, 2352, 2441,2488,2473,2457,2460,2448,2444,2420,2385,2307,2210,2147,2111,2110,2119,2161,1043,905,816,800,586,610,670,382,499,718,360,408,305,259,161,360,599,169};
        //generate 60 values at somewhat intervals of 40 between 0 and 2500
        int[] etherianObelisk = new int[]{ 40, 80, 120, 160, 1068, 240, 280, 320, 360, 400, 500, 480, 520, 560, 475, 640, 680, 720, 760, 800, 840, 
            880, 920, 960, 990, 1040, 1080, 977, 1160, 336, 1240, 1280, 1320, 1360, 1400, 1440, 1480, 1520, 1502, 1564, 1640, 1680, 1720, 1760, 
            1800, 1840, 1880, 1920, 1960, 2000, 2040, 2080, 2120, 1886, 2200, 2240, 2125, 2320, 2295, 2400, 2440, 2480, 9};

        //Fortune-Chest artifact decrease amount 350-172
        //farmland and pond scaling
        //fresh water gone
        //move up felisgarde
        //663 titan
        //switch off ponds to rivers
        //cut down on mines, at 350
        //30 copper 10 gold on rock area
        //same for rest of map, 100 copper mine  maybe 30 gold
        //Legendary tag on 1 of 1s
        //bump up poi from 4-5
        //bump up underground resources to 35%
        //add obelisks manually
        //iron node heavier in rock area


        for(int i = 0; i<nftCollection.landDeeds.Count; i++){
            if(nftCollection.landDeeds[i].road){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMiddle,
                nftCollection.landDeeds[i].underlandRight,
                SeededRandom.Range(0f,1f) > 0.7f ? "Inn" : nftCollection.landDeeds[i].northSpot,
                nftCollection.landDeeds[i].southSpot,
                nftCollection.landDeeds[i].eastSpot,
                nftCollection.landDeeds[i].westSpot,
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType
                );
            }
            if(felisgarde == nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                "Subway",
                "Subway",
                "Subway",
                "",
                "",
                "",
                "",
                false,
                false,
                false,
                nftCollection.landDeeds[i].landType,
                "Felisgarde"
                );
            }
            if(brambleThorn == nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMiddle,
                nftCollection.landDeeds[i].underlandRight,
                "",
                "",
                "",
                "",
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType,
                "Bramblethorn-Titan"
                );
            }
            if(ebisusBay== nftCollection.landDeeds[i].id){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                nftCollection.landDeeds[i].id,
                nftCollection.landDeeds[i].longitude,
                nftCollection.landDeeds[i].lattitude,
                nftCollection.landDeeds[i].underlandLeft,
                nftCollection.landDeeds[i].underlandMiddle,
                nftCollection.landDeeds[i].underlandRight,
                "",
                "",
                "",
                "",
                nftCollection.landDeeds[i].waterSource,
                nftCollection.landDeeds[i].road,
                nftCollection.landDeeds[i].settlement,
                nftCollection.landDeeds[i].landType,
                "Ebisu's-Bay"
                );
            }
            for(int j = 0; j < lightHouses.Length; j++){
                if(lightHouses[j] == nftCollection.landDeeds[i].id+1){
                    nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
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
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    "Coastal-Tiki-Lounge",
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
            for(int j = 0; j < etherianObelisk.Length; j++){
                if(etherianObelisk[j] == nftCollection.landDeeds[i].id+1){
                    nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    "Etherian-Obelisk",
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
    private void HandlePruneMines(){
        float goldPercentage = 0.9f, copperPercentage = 0.8f, ironNodePercentage = 0.75f; 
        for(int i = 0; i<nftCollection.landDeeds.Count; i++){
            if(nftCollection.landDeeds[i].landType == "Celestial-Cliffs"){
                  if(nftCollection.landDeeds[i].underlandLeft == ""){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    SeededRandom.Range(0f, 1f) < .25f ? "Iron-Node" : "",
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
            if(nftCollection.landDeeds[i].underlandMiddle == ""){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude, 
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft, 
                    SeededRandom.Range(0f, 1f) < .25f ? "Iron-Node" : "",
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot, 
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot, 
                    nftCollection.landDeeds[i].westSpot, 
                    nftCollection.landDeeds[i].waterSource, 
                    nftCollection.landDeeds[i].road, 
                    nftCollection.landDeeds[i].settlement, 
                    nftCollection.landDeeds[i].landType
                );
            }
            if(nftCollection.landDeeds[i].underlandRight == ""){
                Debug.Log($"iron node found");
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id, 
                    nftCollection.landDeeds[i].longitude, 
                    nftCollection.landDeeds[i].lattitude, 
                    nftCollection.landDeeds[i].underlandLeft, 
                    nftCollection.landDeeds[i].underlandMiddle, 
                    SeededRandom.Range(0f, 1f) < .25 ? "Iron-Node" : "",
                    nftCollection.landDeeds[i].northSpot, 
                    nftCollection.landDeeds[i].southSpot, 
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot, 
                    nftCollection.landDeeds[i].waterSource, 
                    nftCollection.landDeeds[i].road, 
                    nftCollection.landDeeds[i].settlement, 
                    nftCollection.landDeeds[i].landType
                );
            }

                continue;
            }

            if(nftCollection.landDeeds[i].northSpot == "Copper-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    SeededRandom.Range(0f, 1f) > copperPercentage ? nftCollection.landDeeds[i].northSpot : "",
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            else if(nftCollection.landDeeds[i].northSpot == "Gold-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    SeededRandom.Range(0f, 1f) > goldPercentage ? nftCollection.landDeeds[i].northSpot : "",
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }

            if(nftCollection.landDeeds[i].southSpot == "Copper-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    SeededRandom.Range(0f, 1f) > copperPercentage ? nftCollection.landDeeds[i].southSpot : "",
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            else if(nftCollection.landDeeds[i].southSpot == "Gold-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    SeededRandom.Range(0f, 1f) > goldPercentage ? nftCollection.landDeeds[i].southSpot : "",
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
            
            if(nftCollection.landDeeds[i].underlandLeft == "Iron-Node"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    SeededRandom.Range(0f, 1f) > ironNodePercentage ? nftCollection.landDeeds[i].underlandLeft : "",
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
            if(nftCollection.landDeeds[i].underlandMiddle == "Iron-Node"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude, 
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft, 
                    SeededRandom.Range(0f, 1f) > ironNodePercentage ? nftCollection.landDeeds[i].underlandMiddle : "",
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot, 
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot, 
                    nftCollection.landDeeds[i].westSpot, 
                    nftCollection.landDeeds[i].waterSource, 
                    nftCollection.landDeeds[i].road, 
                    nftCollection.landDeeds[i].settlement, 
                    nftCollection.landDeeds[i].landType
                );
            }
            if(nftCollection.landDeeds[i].underlandRight == "Iron-Node"){
                Debug.Log($"iron node found");
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id, 
                    nftCollection.landDeeds[i].longitude, 
                    nftCollection.landDeeds[i].lattitude, 
                    nftCollection.landDeeds[i].underlandLeft, 
                    nftCollection.landDeeds[i].underlandMiddle, 
                    SeededRandom.Range(0f, 1f) > ironNodePercentage ? nftCollection.landDeeds[i].underlandRight : "",
                    nftCollection.landDeeds[i].northSpot, 
                    nftCollection.landDeeds[i].southSpot, 
                    nftCollection.landDeeds[i].eastSpot,
                    nftCollection.landDeeds[i].westSpot, 
                    nftCollection.landDeeds[i].waterSource, 
                    nftCollection.landDeeds[i].road, 
                    nftCollection.landDeeds[i].settlement, 
                    nftCollection.landDeeds[i].landType
                );
            }

            if(nftCollection.landDeeds[i].eastSpot == "Copper-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    SeededRandom.Range(0f, 1f) > copperPercentage ? nftCollection.landDeeds[i].eastSpot : "",
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            else if(nftCollection.landDeeds[i].eastSpot == "Gold-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    SeededRandom.Range(0f, 1f) > goldPercentage ? nftCollection.landDeeds[i].eastSpot : "",
                    nftCollection.landDeeds[i].westSpot,
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
            
            if(nftCollection.landDeeds[i].westSpot == "Copper-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    SeededRandom.Range(0f, 1f) > copperPercentage ? nftCollection.landDeeds[i].westSpot : "",
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            } 
            else if(nftCollection.landDeeds[i].westSpot == "Gold-Mine"){
                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    nftCollection.landDeeds[i].underlandLeft,
                    nftCollection.landDeeds[i].underlandMiddle,
                    nftCollection.landDeeds[i].underlandRight,
                    nftCollection.landDeeds[i].northSpot,
                    nftCollection.landDeeds[i].southSpot,
                    nftCollection.landDeeds[i].eastSpot,
                    SeededRandom.Range(0f, 1f) > goldPercentage ? nftCollection.landDeeds[i].westSpot : "",
                    nftCollection.landDeeds[i].waterSource,
                    nftCollection.landDeeds[i].road,
                    nftCollection.landDeeds[i].settlement,
                    nftCollection.landDeeds[i].landType
                );
            }
        }
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
                    nftCollection.landDeeds[i].underlandMiddle,
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
                    nftCollection.landDeeds[i].underlandMiddle,
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
                    nftCollection.landDeeds[i].underlandMiddle,
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
                    nftCollection.landDeeds[i].underlandMiddle,
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
            if(nftCollection.landDeeds[i].landType=="Celestial-Cliffs"){

                string northSpot = nftCollection.landDeeds[i].northSpot;
                string southSpot = nftCollection.landDeeds[i].southSpot;
                string eastSpot = nftCollection.landDeeds[i].eastSpot;
                string westSpot = nftCollection.landDeeds[i].westSpot;

                string underlandLeft = nftCollection.landDeeds[i].underlandLeft;
                string underlandMiddle = nftCollection.landDeeds[i].underlandMiddle;
                string underlandRight = nftCollection.landDeeds[i].underlandRight;
                
                if(GetTraitType(northSpot)=="Resource"){
                    northSpot = GetRandomRockThing();
                    // northSpot = SeededRandom.Range(0f,1f)>0.5 ?  GetRandomRockThing() : "";
                }
                if(GetTraitType(southSpot)=="Resource"){
                    southSpot = GetRandomRockThing();
                    // southSpot = SeededRandom.Range(0f,1f)>0.5 ?  GetRandomRockThing() : "";
                }
                if(GetTraitType(eastSpot)=="Resource"){
                    eastSpot = GetRandomRockThing();
                    // eastSpot = SeededRandom.Range(0f,1f)>0.5 ?  GetRandomRockThing() : "";
                }
                if(GetTraitType(westSpot)=="Resource"){
                    westSpot = GetRandomRockThing();
                }
                if(GetTraitType(underlandLeft)=="ResourceUnderland"){
                    underlandLeft = SeededRandom.Range(0f,1f)> 0.2 ?  "Iron-Node" : underlandLeft;
                }
                if(GetTraitType(underlandMiddle)=="ResourceUnderland"){
                    underlandMiddle = SeededRandom.Range(0f,1f)> 0.2 ?  "Iron-Node" : underlandMiddle;
                }
                if(GetTraitType(underlandRight)=="ResourceUnderland"){
                    underlandRight = SeededRandom.Range(0f,1f)> 0.2 ?  "Iron-Node" : underlandRight;
                }

                nftCollection.landDeeds[i] = new LandDeedMetadata(
                    nftCollection.landDeeds[i].id,
                    nftCollection.landDeeds[i].longitude,
                    nftCollection.landDeeds[i].lattitude,
                    underlandLeft,
                    underlandMiddle,
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
        }
        SaveMetaData();
    }    
    private string GetRandomRockThing(){
        return SeededRandom.Range(0f,1f) switch
        {
            > 0.9f => "Copper-Mine",
            > 0.85f => "Gold-Mine",
            _ => "Rock",
        };
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
        else if(landType=="Verdant-Forest")
            newTree = SeededRandom.FlipCoin() ? "Nightshade" : newTree;
        else if(landType=="Rolling-Plains")
            newTree = SeededRandom.FlipCoin() ? "Thornfire-lily" : newTree;
        else if(landType=="Highlands")
            newTree = SeededRandom.FlipCoin() ? "Mandrake-Root" : newTree;
            
        return newTree;
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
    private void OneOffFixes(){
        for(int i = 0; i < nftCollection.landDeeds.Count; i++) {
            // if(nftCollection.landDeeds[i].landType=="Mystic Grove"){
            //     nftCollection.landDeeds[i] = new LandDeedMetadata(
            //         nftCollection.landDeeds[i].id,
            //         nftCollection.landDeeds[i].longitude,
            //         nftCollection.landDeeds[i].lattitude,
            //         nftCollection.landDeeds[i].underlandLeft,
            //         nftCollection.landDeeds[i].underlandMiddle,
            //         nftCollection.landDeeds[i].underlandRight,
            //         nftCollection.landDeeds[i].northSpot,
            //         nftCollection.landDeeds[i].southSpot,
            //         nftCollection.landDeeds[i].eastSpot,
            //         nftCollection.landDeeds[i].westSpot,
            //         nftCollection.landDeeds[i].waterSource,
            //         nftCollection.landDeeds[i].road,
            //         nftCollection.landDeeds[i].settlement,
            //         "Mystic-Grove"
            //     );
            // }
            // if(nftCollection.landDeeds[i].landType=="Celestial Cliffs"){
            //     nftCollection.landDeeds[i] = new LandDeedMetadata(
            //         nftCollection.landDeeds[i].id,
            //         nftCollection.landDeeds[i].longitude,
            //         nftCollection.landDeeds[i].lattitude,
            //         nftCollection.landDeeds[i].underlandLeft,
            //         nftCollection.landDeeds[i].underlandMiddle,
            //         nftCollection.landDeeds[i].underlandRight,
            //         nftCollection.landDeeds[i].northSpot,
            //         nftCollection.landDeeds[i].southSpot,
            //         nftCollection.landDeeds[i].eastSpot,
            //         nftCollection.landDeeds[i].westSpot,
            //         nftCollection.landDeeds[i].waterSource,
            //         nftCollection.landDeeds[i].road,
            //         nftCollection.landDeeds[i].settlement,
            //         "Celestial-Cliffs"
            //     );
            // } 
            // if(nftCollection.landDeeds[i].landType=="Rolling Plains"){
            //     nftCollection.landDeeds[i] = new LandDeedMetadata(
            //         nftCollection.landDeeds[i].id,
            //         nftCollection.landDeeds[i].longitude,
            //         nftCollection.landDeeds[i].lattitude,
            //         nftCollection.landDeeds[i].underlandLeft,
            //         nftCollection.landDeeds[i].underlandMiddle,
            //         nftCollection.landDeeds[i].underlandRight,
            //         nftCollection.landDeeds[i].northSpot,
            //         nftCollection.landDeeds[i].southSpot,
            //         nftCollection.landDeeds[i].eastSpot,
            //         nftCollection.landDeeds[i].westSpot,
            //         nftCollection.landDeeds[i].waterSource,
            //         nftCollection.landDeeds[i].road,
            //         nftCollection.landDeeds[i].settlement,
            //         "Rolling-Plains"
            //     );
            // } 
            // if(nftCollection.landDeeds[i].landType=="Verdant Forest"){
            //     nftCollection.landDeeds[i] = new LandDeedMetadata(
            //         nftCollection.landDeeds[i].id,
            //         nftCollection.landDeeds[i].longitude,
            //         nftCollection.landDeeds[i].lattitude,
            //         nftCollection.landDeeds[i].underlandLeft,
            //         nftCollection.landDeeds[i].underlandMiddle,
            //         nftCollection.landDeeds[i].underlandRight,
            //         nftCollection.landDeeds[i].northSpot,
            //         nftCollection.landDeeds[i].southSpot,
            //         nftCollection.landDeeds[i].eastSpot,
            //         nftCollection.landDeeds[i].westSpot,
            //         nftCollection.landDeeds[i].waterSource,
            //         nftCollection.landDeeds[i].road,
            //         nftCollection.landDeeds[i].settlement,
            //         "Verdant-Forest"
            //     );
            // } 
        }
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
                                        nftCollection.landDeeds[hitObject.id].underlandMiddle,
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
                    nftCollection.landDeeds[hitObject.id].underlandMiddle,
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