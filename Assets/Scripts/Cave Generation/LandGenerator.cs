using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
// using Unity.Netcode;

namespace TJ
// 8669479589  1057 christopher danials support@heathenrollment.org multiplanppo mymemberinfo
//  wellheath 8555565049 diagnostics
{
public class LandGenerator : MonoBehaviour
{
    // [SerializeField] private GameBoard gameBoard;
    public bool debug, demoMode;
    public MapData mapData;
    Transform landParent;
    public List<Cell> cells, cellHolder = new List<Cell>();
    public SerializableDictionary <Vector2, Cell> edgeDictionary = new SerializableDictionary<Vector2, Cell>();
    public List<Cell> cellsAffected = new List<Cell>();
    public Weights weights;

    [Header("Noise Generation")]
	[SerializeField] private float noiseScale = 1;
	[SerializeField] private int octaves;
	[SerializeField] [Range(0,1)] private float persistance;
	[SerializeField] private float lacunarity;
	// [SerializeField] private int seed;
	// [HideInInspector] public TerrainType[] regions;
	[HideInInspector] public Vector2 offset;
    public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

    [Header("Cave Generation Forcing Intersection")]
    public SocketSO roadSocket;
    public Prototype intersectionPrototype, centralPrototype, grassPrototype;
    public GameObject verticeParent;
    public GameObject cellPrefab;

    #region cached variables
    int collapsed;
    List<Cell> cellsWithLowestEntropy = new List<Cell>();
    Cell cellWithLowestEntropy;
    Prototype finalPrototype;
    Cell[] cellsToCollapse;
    Cell startCell;
    List<Cell> roadCells = new List<Cell>();
    public Transform canvernSpawnPosition;
    [SerializeField] private List<Vector2> globalVerticies;//= new NetworkList<Vector2>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private List<Vector4> faces;// = new NetworkList<Vector4>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    #endregion

    [Header("Map Data")]
    // public MapDataFinal hexagonData;
    public List<Chunk> hexagons = new List<Chunk>();
    List<Transform> hexagonParents = new List<Transform>();

    [Header("Additional Settings")]
    public AnimationCurve animationCurve;
    public int rings, seed;
    public bool rootRoadAt_0_0_0;
    public float finalScale;

    [Header("Visual Testing")]
    public Transform hexParent;
    public Transform hexPrefab;
    public bool useHeightMap, spawnMeshes;

    [Header("Setting range for noise")]
    [SerializeField] float maxNoiseHeight = float.MinValue;
	[SerializeField] float minNoiseHeight = float.MaxValue;

    public Chunk CreateChunk(){
        hexagons = new List<Chunk>();
        Transform chunkHolder = new GameObject().transform;
        Chunk newChunk = new Chunk();
        newChunk.chunkTransform = chunkHolder;

        hexagons.Add(newChunk);

        maxNoiseHeight = float.MinValue;
        minNoiseHeight = float.MaxValue;

        // hexagons = HexGridLayout.CreateBorderingHexagons(hexagons, 0, rings);

        // clear children of hexParent
        for (int i = hexParent.childCount; i > 0; i--)
            DestroyImmediate(hexParent.GetChild(0).gameObject);

        for(int i = 0; i<hexagons.Count; i++)
        {
            hexagons[i] = GenerateHexagon(hexagons[i]);
        }

        chunkHolder.localScale = new Vector3(finalScale, finalScale, finalScale);
        return newChunk;
    }
    public void OfflineMapData()
    {
        hexagons = new List<Chunk>();
        Chunk hexagon = new Chunk();
        hexagons.Add(hexagon);
        canvernSpawnPosition.localScale = new Vector3(1, 1, 1);
        maxNoiseHeight = float.MinValue;
        minNoiseHeight = float.MaxValue;

        // hexagons = HexGridLayout.CreateBorderingHexagons(hexagons, 0, rings);

        //clear children of hexParent
        // for (int i = hexParent.childCount; i > 0; i--)
        //     DestroyImmediate(hexParent.GetChild(0).gameObject);

        //spawn objects just to look pretty
        // foreach(Hexagon hex in hexagons)
        // {
        //     Transform hexTransform = Instantiate(hexPrefab, hexParent);
        //     hexTransform.localPosition = new Vector3(hex.centroid.x, 0, hex.centroid.y);
        // }

        // destroy all children of the land parent
        for (int i = canvernSpawnPosition.childCount; i > 0; i--)
            DestroyImmediate(canvernSpawnPosition.GetChild(0).gameObject);

        for(int i = 0; i<hexagons.Count; i++)
        {
            hexagons[i] = GenerateHexagon(hexagons[i]);
        }

        canvernSpawnPosition.localScale = new Vector3(finalScale, finalScale, finalScale);
    }
    private Chunk GenerateHexagon(Chunk hexagon)
    {
        hexagon.mapData = NetworkMapGenerator.GenerateMap(seed, rings, animationCurve);

        for (int i = this.gameObject.transform.childCount; i > 0; i--)
            DestroyImmediate(this.gameObject.transform.GetChild(0).gameObject);

        mapData.faces = new List<Vector4>();
        mapData.verticesMap = new List<Vector3>();

        for(int i = 0; i<hexagon.mapData.globalVerticies.Count; i++)
            hexagon.mapData.globalVerticies[i]+=hexagon.centroid;

        //either add fancy height, or dont
        if(useHeightMap)
            hexagon = AddHeightData(hexagon);
        else {
            for(int i = 0; i<hexagon.mapData.globalVerticies.Count; i++) {
                mapData.verticesMap.Add(new Vector3(hexagon.mapData.globalVerticies[i].x, 0, hexagon.mapData.globalVerticies[i].y));
            }
        }
        
        //assign grid values to all verticies
        for(int i = 0; i<mapData.verticesMap.Count; i++)
            Instantiate(verticeParent, mapData.verticesMap[i], Quaternion.identity, this.transform);

        foreach(Vector4 face in hexagon.mapData.faces)
            mapData.faces.Add(new Vector4(face.x, face.y, face.z, face.w));

        SpawnLand();
        hexagon.mapData.cells = new List<Cell>();
        foreach(Cell c in cellHolder)
            hexagon.mapData.cells.Add(c);

        cellHolder.Clear();
        hexagonParents.Add(landParent);
        landParent = null;
        return hexagon;
    }
    private Chunk AddHeightData(Chunk hexagon)
    {
        //add a height curve to the noise map
        float noiseHeight = 0;
        float[] noiseMap = new float[hexagon.mapData.globalVerticies.Count];
        
        for(int i = 0; i<hexagon.mapData.globalVerticies.Count; i++)
        {
            noiseHeight = Noise.GenerateGlobalNoiseMapHex(hexagon.mapData.globalVerticies[i], seed, 
                noiseScale, octaves, persistance, lacunarity, new Vector2());
            
            //only on first
            if(hexagon.centroid == new Vector2(0,0))
            {
                //massaging the noise values to be between min and max only on first hexagon
                maxNoiseHeight = (noiseHeight > maxNoiseHeight) ? noiseHeight : maxNoiseHeight;
                minNoiseHeight = (noiseHeight < minNoiseHeight) ? noiseHeight : minNoiseHeight;
            }
            noiseMap[i] = noiseHeight;
        }

        for(int i = 0; i<hexagon.mapData.globalVerticies.Count; i++)
        {
            noiseMap[i] = noiseMap[i] > minNoiseHeight ? noiseMap[i] : minNoiseHeight;
            noiseMap[i] = noiseMap[i] < maxNoiseHeight ? noiseMap[i] : maxNoiseHeight;

			//wtf is this
            noiseHeight = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap[i]);
            mapData.verticesMap.Add(new Vector3(hexagon.mapData.globalVerticies[i].x, noiseHeight, hexagon.mapData.globalVerticies[i].y));
        }


        //evaluate height curve
        for(int i = 0; i<mapData.verticesMap.Count; i++)
        {
            mapData.verticesMap[i] = new Vector3(mapData.verticesMap[i].x, meshHeightCurve.Evaluate(mapData.verticesMap[i].y) * 
                                                 meshHeightMultiplier, mapData.verticesMap[i].z);
        }
        return hexagon;
    }
    public void CreateMapData()
    {
        // hexagonData = NetworkMapGenerator.GenerateMap(seed, rings, animationCurve);
        // SaveMapData(hexagonData.globalVerticies, hexagonData.faces);
    }
    public void SaveMapData(List<Vector2> verts, List<Vector4> mapDataFaces)
    {
        globalVerticies.Clear();
        faces.Clear();

        Vector2 offset2 = verts[0];
       
        foreach(Vector2 vert in verts)
            globalVerticies.Add(vert-offset2);
            
        foreach(Vector4 face in mapDataFaces)
            faces.Add(face);

        GetMapData();
        SpawnLand();
    }
    public void GetMapData()
    {
        for (int i = this.gameObject.transform.childCount; i > 0; --i)
            DestroyImmediate(this.gameObject.transform.GetChild(0).gameObject);

        mapData.faces = new List<Vector4>();
        mapData.verticesMap = new List<Vector3>();

        foreach(Vector2 vertex in globalVerticies)
            mapData.verticesMap.Add(new Vector3(vertex.x, 0, vertex.y));

        Vector3[] v = Noise.GenerateNoiseMapHex (mapData.verticesMap.ToArray(), seed, noiseScale, octaves, persistance, lacunarity, offset, 0);

        for(int i = 0; i<mapData.verticesMap.Count; i++)
            mapData.verticesMap[i] = v[i];

        //assign grid values to all verticies
        for(int i = 0; i<mapData.verticesMap.Count; i++)
        {
            mapData.verticesMap[i] = new Vector3(mapData.verticesMap[i].x, meshHeightCurve.Evaluate(mapData.verticesMap[i].y) * meshHeightMultiplier, mapData.verticesMap[i].z);
            Instantiate(verticeParent, mapData.verticesMap[i], Quaternion.identity, this.transform);
        }

        foreach(Vector4 face in faces)
            mapData.faces.Add(new Vector4(face.x, face.y, face.z, face.w));
    }
    public async void SpawnLand()
    {
        double funtionExecutionTime = EditorApplication.timeSinceStartup;
        int failCounter = 0;

        string response = await GenerateLand(failCounter);
        while(response == "fail" && failCounter<30)
        {
            response = await GenerateLand(failCounter);
            failCounter++;
        }
        //function time
        funtionExecutionTime = EditorApplication.timeSinceStartup - funtionExecutionTime;
        Debug.Log($"Generated in {funtionExecutionTime.ToString("00.00")} seconds with {failCounter} fails");
    }
    public async Task<string> GenerateLand(int fails)
    {
        if(landParent != null)
            DestroyImmediate(landParent.gameObject);
        
        landParent = new GameObject().transform;

        // Debug.Log($"seed: {seed} fails: {fails}");
        SeededRandom.Init(seed+fails);
        try
        {
            landParent.localScale = new Vector3(1, 1, 1);
            landParent.localPosition = new Vector3(0, 0, 0);
            ClearLand();
            CreateCells();
            FindNeighbours();
            foreach(Cell c in cells)
                c.GenerateWeight(weights);

            PreProcessCells();
            StartCollapse();

            //if the amount of roads does not meet the threshold, try again
            if(!ReviewCollapse())
                return "fail";

            SquishLand();

            //for gameboard
            cellHolder = cells;
        }
        catch (System.Exception)
        {
            if(!debug)
            {
                cells = new List<Cell>();
                edgeDictionary = new SerializableDictionary<Vector2, Cell>();
                cellsAffected = new List<Cell>();
                cellsWithLowestEntropy = new List<Cell>();
            }
            // throw;
            return "fail";
        }
        
        if(!debug)
        {
            cells = new List<Cell>();
            edgeDictionary = new SerializableDictionary<Vector2, Cell>();
            cellsAffected = new List<Cell>();
            cellsWithLowestEntropy = new List<Cell>();
        }
        
        // landParent.localScale = new Vector3(finalScale, finalScale, finalScale);
        landParent.SetParent(canvernSpawnPosition);

        if(rootRoadAt_0_0_0)
        {
            //make root road the parent
            for(int i = 0; i<landParent.childCount; i++)
            {
                if(landParent.GetChild(i).GetComponent<Cell>() == startCell)
                {
                    Transform rootRoad = landParent.GetChild(i).GetComponent<Cell>().collapsedGameObject.transform;
                    rootRoad.SetParent(null);
                    landParent.SetParent(rootRoad);

                    rootRoad.SetParent(canvernSpawnPosition);
                    rootRoad.localPosition = new Vector3(0, 0, 0);
                    break;
                }
            }
        }
        return "pass";
    }
    public void CreateCells()
    {
        foreach(Vector4 face in mapData.faces)
        {
            //position doesnt matter yet
            #if UNITY_EDITOR
                GameObject cellObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab as GameObject);
                PrefabUtility.UnpackPrefabInstance(cellObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            #endif

            cellObject.transform.SetParent(landParent);
            Cell cell = cellObject.gameObject.GetComponent<Cell>();
            cell.face = face;
            cells.Add(cell);

            edgeDictionary.Add(new Vector2(face.x, face.y), cell);
            edgeDictionary.Add(new Vector2(face.y, face.z), cell);
            edgeDictionary.Add(new Vector2(face.z, face.w), cell);
            edgeDictionary.Add(new Vector2(face.w, face.x), cell);
            
            Vector3 centroid = new Vector3(
                (mapData.verticesMap[(int)face.x].x + mapData.verticesMap[(int)face.y].x + mapData.verticesMap[(int)face.z].x + mapData.verticesMap[(int)face.w].x) / 4,
                (mapData.verticesMap[(int)face.x].y + mapData.verticesMap[(int)face.y].y + mapData.verticesMap[(int)face.z].y + mapData.verticesMap[(int)face.w].y) / 4,
                (mapData.verticesMap[(int)face.x].z + mapData.verticesMap[(int)face.y].z + mapData.verticesMap[(int)face.z].z + mapData.verticesMap[(int)face.w].z) / 4
            );
            cell.centroid = centroid;
            
            //this is just the ball
            // Instantiate(verticeParent, centroid, Quaternion.identity, this.transform);

            // cellObject.transform.position = new Vector3(cellObject.transform.position.x, 0, cellObject.transform.position.z);
        }
        // foreach(Cell c in cells)
        //     c.GenerateWeight(weights);
    }
    private void FindNeighbours()
    {
        foreach(Cell c in cells)
        {
            c.Neighbor_XY = GetCell(c.face.x,c.face.y);
            c.Neighbor_YZ = GetCell(c.face.y,c.face.z);
            c.Neighbor_ZW = GetCell(c.face.z,c.face.w);
            c.Neighbor_WX = GetCell(c.face.w,c.face.x);
        }
    }
    private Cell GetCell(float x, float z)
    {
        Cell cell = null;
        if(edgeDictionary.TryGetValue(new Vector2(z,x), out cell))
            return cell;
        else
            return null;
    }
    private void PreProcessCells()
    {
        // PreventRoadsOnEdges();

        //get random cell
        int cellNumber = (int)SeededRandom.Range(0,cells.Count);
        Cell randomCell = cells[cellNumber];

        bool roadFound = false;
        int failCounter = 0;

        while(!roadFound && failCounter < 10)
        {
            //if it contains the 4wayRoad, remove all other prototypes
            if(randomCell.possiblePrototypes.Contains(intersectionPrototype))
            {
                randomCell.possiblePrototypes = new List<Prototype>();
                randomCell.prototypeWeights = new List<int>();
                randomCell.possiblePrototypes.Add(centralPrototype);
                randomCell.prototypeWeights.Add(1);
                randomCell.rootRoad = true;
                startCell = randomCell;
                roadFound = true;
            }
            else
            {
                failCounter++;
                randomCell = (cellNumber + failCounter < cells.Count) ? cells[cellNumber+failCounter] : cells[cellNumber+failCounter-cells.Count];
            }
        }

        if(failCounter >= 10)
            Debug.Log("Big fail");
        // else 
            // Debug.Log($"Road found : {cellNumber + failCounter}");
    }
    private void PreventRoadsOnEdges()
    {
        //prevent Roads on edges
        foreach(Cell c in cells)
        {
            if(c.Neighbor_XY == null)
            {
                //remove all prototypes with a road in this socket
                for (int i = 0; i < c.possiblePrototypes.Count; i++)
                {
                    if(c.possiblePrototypes[i].negX == roadSocket)
                    {
                        c.possiblePrototypes.RemoveAt(i);
                        c.prototypeWeights.RemoveAt(i);
                        i-=1;
                    }
                }
            }
            if(c.Neighbor_WX == null)
            {
                //remove all prototypes with a road in this socket
                for (int i = 0; i < c.possiblePrototypes.Count; i++)
                {
                    if(c.possiblePrototypes[i].posZ == roadSocket)
                    {
                        c.possiblePrototypes.RemoveAt(i);
                        c.prototypeWeights.RemoveAt(i);
                        i-=1;
                    }
                }
            }
            if(c.Neighbor_YZ == null)
            {
                //remove all prototypes with a road in this socket
                for (int i = 0; i < c.possiblePrototypes.Count; i++)
                {
                    if(c.possiblePrototypes[i].negZ == roadSocket)
                    {
                        c.possiblePrototypes.RemoveAt(i);
                        c.prototypeWeights.RemoveAt(i);
                        i-=1;
                    }
                }
            }
            if(c.Neighbor_ZW == null)
            {
                //remove all prototypes with a road in this socket
                for (int i = 0; i < c.possiblePrototypes.Count; i++)
                {
                    if(c.possiblePrototypes[i].posX == roadSocket)
                    {
                        c.possiblePrototypes.RemoveAt(i);
                        c.prototypeWeights.RemoveAt(i);
                        i-=1;
                    }
                }
            }
        }
    }
    private void StartCollapse()
    {
        collapsed=0;
        cellsToCollapse = new Cell[cells.Count];

        while(!isCollapsed())
            Iterate();
    }
    private bool ReviewCollapse()
    {
        //get a count of all remaining prototypes prototypeClass
        Dictionary<PrototypeClass, int> prototypeCount = new Dictionary<PrototypeClass, int>();
        int rootRoadCount = 0;
        foreach(Cell c in cells)
        {
            if(!c.isCollapsed)
                return false;

            if(prototypeCount.ContainsKey(c.possiblePrototypes[0].prototypeClass))
                prototypeCount[c.possiblePrototypes[0].prototypeClass]++;
            else
                prototypeCount.Add(c.possiblePrototypes[0].prototypeClass, 1);

            if(c.rootRoad)
                rootRoadCount++;

            if(c.possiblePrototypes[0].prototypeClass == PrototypeClass.Road && !c.rootRoad)
            {
                // Debug.Log($"Need to scrub this road");
                c.possiblePrototypes[0] = grassPrototype;
            }
        }

        int totalCells = cells.Count;
        // int roadCells = prototypeCount[PrototypeClass.Road];
        float roadPercentage = (float)rootRoadCount / (float)totalCells;

        // Debug.Log("Road percentage: " + roadPercentage);

        if(roadPercentage > weights.roadThreshold/100f)
            return true;
        else
            return false;

        
    }
    private bool isCollapsed()
    {
        //check if any cells contain more than one entry
        foreach(Cell c in cells)
            if(c.possiblePrototypes.Count>1)
                return false;

        return true;
    }
    private void Iterate()
    {
        cellWithLowestEntropy = GetCellWithLowestEntropy();
        CollapseAt(cellWithLowestEntropy);
        Propagate(cellWithLowestEntropy);
    }
    private void CollapseAt(Cell cell)
    {
        // cell = PrunePrototypes(cell);
        int selectedPrototype = SelectPrototype(cell.prototypeWeights);
        finalPrototype = cell.possiblePrototypes[selectedPrototype];
        finalPrototype.prefab = cell.possiblePrototypes[selectedPrototype].prefab;

        cell.possiblePrototypes.Clear();
        cell.possiblePrototypes.Add(finalPrototype);

        cell.name = cell.face.ToString()+"_"+ collapsed.ToString();
        cellsToCollapse[collapsed] = cellWithLowestEntropy;
        collapsed++;
        cell.isCollapsed = true;

        cell.socket_XY = finalPrototype.posX;
        cell.socket_YZ = finalPrototype.negZ;
        cell.socket_ZW = finalPrototype.negX;
        cell.socket_WX = finalPrototype.posZ;

        if(cell.Neighbor_XY!=null && cell.Neighbor_XY.rootRoad)
            cell.rootRoad = true;
        if(cell.Neighbor_YZ!=null && cell.Neighbor_YZ.rootRoad)
            cell.rootRoad = true;
        if(cell.Neighbor_ZW!=null && cell.Neighbor_ZW.rootRoad)
            cell.rootRoad = true;
        if(cell.Neighbor_WX!=null && cell.Neighbor_WX.rootRoad)
            cell.rootRoad = true;
    }
    private Cell PrunePrototypes(Cell cell)
    {
        List<Prototype> prunedPrototypes = cell.possiblePrototypes;
        List<int> prunedWeights = cell.prototypeWeights;

        if(!cell.rootRoad)
        {
            // Debug.Log($"Cell {cell.name} cannot have a road");
            for (int i = 0; i < prunedPrototypes.Count; i++)
            {
                if(prunedPrototypes[i].prototypeClass == PrototypeClass.Road)
                {
                    prunedPrototypes.RemoveAt(i);
                    prunedWeights.RemoveAt(i);
                    i-=1;
                }
            }
        }
        
        cell.possiblePrototypes = prunedPrototypes;
        cell.prototypeWeights = prunedWeights;
        return cell;
    }
    private int SelectPrototype(List<int> prototypeWeights)
    {
        //multiply each possible prototype by their weight 
        //which is the average of the weights of their attributes
        //then add all those together
        //pick a random value 0-allAtributes weight
        //if less than the random number, return it, else keep going
        int total = 0;
        foreach(int weight in prototypeWeights)
            total+=weight;

        total = (int)SeededRandom.Range(0, total);

        foreach(int weight in prototypeWeights)
        {
            for (int i = 0; i < prototypeWeights.Count; i++)
            {
                if(total<=prototypeWeights[i])
                {
                    return i;
                }
                else
                    total-=weight;
            }
        }
        return 0;
    }
    private void Propagate(Cell cell)
    {
        cellsAffected.Add(cell);
        int y = 0;
        while(cellsAffected.Count > 0)
        {
            Cell currentCell = cellsAffected[0];
            cellsAffected.Remove(currentCell);

            //get neighbor to the right
            Cell neighbor_ZW = currentCell.Neighbor_ZW;
            if(neighbor_ZW!=null && !neighbor_ZW.isCollapsed)
            {
                //Get sockets that we have available on our Right
                List<SocketSO> possibleConnections = GetPossibleSockets_YZ(currentCell.possiblePrototypes);
                // Debug.Log($"Possible connections for {currentCell.name} are {possibleConnections[0]}");

                bool constrained = false;
                for (int i = 0; i < neighbor_ZW.possiblePrototypes.Count; i++)
                {
                    //if the list of sockets that we have on the right does not contain the connector on the other cell to the left...
                    if(!possibleConnections.Contains(neighbor_ZW.possiblePrototypes[i].negZ))
                    {
                        //then that is not a valid possibility and must be removed
                        neighbor_ZW.possiblePrototypes.RemoveAt(i);
                        neighbor_ZW.prototypeWeights.RemoveAt(i);
                        i-=1;
                        constrained = true;
                    }
                }

                if(constrained)
                    cellsAffected.Add(neighbor_ZW);
            }

            //not
            Cell otherCell = currentCell.Neighbor_XY;
            if(otherCell!=null && !otherCell.isCollapsed)
            {
                List<SocketSO> possibleConnections = GetPossibleSockets_WX(currentCell.possiblePrototypes);
                bool hasBeenConstrained = false;
        
                //check all neighbours
                for (int i = 0; i < otherCell.possiblePrototypes.Count; i++)
                {
                    if(!possibleConnections.Contains(otherCell.possiblePrototypes[i].posZ))
                    {
                        otherCell.possiblePrototypes.RemoveAt(i);
                        otherCell.prototypeWeights.RemoveAt(i);
                        i-=1;
                        hasBeenConstrained = true;
                    }
                }
                if(hasBeenConstrained)
                    cellsAffected.Add(otherCell);
            }
            //not
            otherCell = currentCell.Neighbor_WX;
            if(otherCell!=null && !otherCell.isCollapsed)
            {
                List<SocketSO> possibleConnections = GetPossibleSockets_XY(currentCell.possiblePrototypes);
                bool hasBeenConstrained = false;
                for (int i = 0; i < otherCell.possiblePrototypes.Count; i++)
                {
                    if(!possibleConnections.Contains(otherCell.possiblePrototypes[i].negX))
                    {
                        otherCell.possiblePrototypes.RemoveAt(i);
                        otherCell.prototypeWeights.RemoveAt(i);
                        i-=1;
                        hasBeenConstrained = true;
                    }
                }
                if(hasBeenConstrained)
                    cellsAffected.Add(otherCell);
            }
            
            //good
            Cell neighbor_YZ = currentCell.Neighbor_YZ;
            if(neighbor_YZ!=null && !neighbor_YZ.isCollapsed)
            {
                List<SocketSO> possibleConnections = GetPossibleSockets_ZW(currentCell.possiblePrototypes);
                bool hasBeenConstrained = false;
                for (int i = 0; i < neighbor_YZ.possiblePrototypes.Count; i++)
                {
                    if(!possibleConnections.Contains(neighbor_YZ.possiblePrototypes[i].posX))
                    {
                        neighbor_YZ.possiblePrototypes.RemoveAt(i);
                        neighbor_YZ.prototypeWeights.RemoveAt(i);
                        i-=1;
                        hasBeenConstrained = true;
                    }
                }
                if(hasBeenConstrained)
                    cellsAffected.Add(neighbor_YZ);
            }
            y++;
        }
        // Debug.Log($"cellsAffected: {y}");
    }
    private List<SocketSO> GetPossibleSockets_XY(List<Prototype> prototypesAvailable)
    {
        List<SocketSO> socketsAccepted = new List<SocketSO>();
        foreach (Prototype proto in prototypesAvailable)
        {
            if(!socketsAccepted.Contains(proto.posZ))
                socketsAccepted.Add(proto.posZ);
        }
        return socketsAccepted;
    }
    private List<SocketSO> GetPossibleSockets_YZ(List<Prototype> prototypesAvailable)
    {
        List<SocketSO> socketsAccepted = new List<SocketSO>();
        foreach (Prototype proto in prototypesAvailable)
        {
            if(!socketsAccepted.Contains(proto.posX))
                socketsAccepted.Add(proto.posX);
        }
        return socketsAccepted;
    }
    private List<SocketSO> GetPossibleSockets_ZW(List<Prototype> prototypesAvailable)
    {
        List<SocketSO> socketsAccepted = new List<SocketSO>();
        foreach (Prototype proto in prototypesAvailable)
        {
            if(!socketsAccepted.Contains(proto.negZ))
                socketsAccepted.Add(proto.negZ);
        }
        return socketsAccepted;
    }
    private List<SocketSO> GetPossibleSockets_WX(List<Prototype> prototypesAvailable)
    {
        List<SocketSO> socketsAccepted = new List<SocketSO>();
        foreach (Prototype proto in prototypesAvailable)
        {
            if(!socketsAccepted.Contains(proto.negX))
                socketsAccepted.Add(proto.negX);
        }
        return socketsAccepted;
    }
    private Cell GetCellWithLowestEntropy()
    {
        //returns a radom cell with the lowest entropy
        cellsWithLowestEntropy.Clear();
        float x = Mathf.Infinity;

        if(roadCells.Count>0)
            return roadCells[(int)SeededRandom.Range(0, roadCells.Count)];

        foreach(Cell c in cells)
        {
            if(!c.isCollapsed)
            {
                if(c.possiblePrototypes.Count==x)
                {
                    cellsWithLowestEntropy.Add(c);
                }
                else if(c.possiblePrototypes.Count<x)
                {
                    cellsWithLowestEntropy.Clear();
                    cellsWithLowestEntropy.Add(c);
                    x = c.possiblePrototypes.Count;
                }
            }
        }
        return cellsWithLowestEntropy[(int)SeededRandom.Range(0, cellsWithLowestEntropy.Count)];
    }
    public async void SquishLand()
    {
        foreach(Cell cell in cellsToCollapse)
        {
            SquishCell(cell);

            if(demoMode)
                await Task.Delay(5);
        }
    }
    private void SquishCell(Cell cell)
    {
        GameObject thingToSpawn = spawnMeshes ? cell.possiblePrototypes[0].prefab : cell.emptyPrefab;
        cell.collapsedGameObject = Instantiate(thingToSpawn, cell.centroid, Quaternion.identity, cell.transform);
        // SquishOtherObject(cell);
        HandlePrefabMesh(cell);

        Tile tile = cell.collapsedGameObject.GetComponent<Tile>();

        //move the position to face x
        tile.tileMeshFilter.gameObject.transform.position = new Vector3(
            mapData.verticesMap[(int)cell.face.w].x, 
            mapData.verticesMap[(int)cell.face.w].y, 
            mapData.verticesMap[(int)cell.face.w].z);

        // if(tile.floorMeshFilter!=null)
        // {
        //     tile.floorMeshFilter.gameObject.transform.position = new Vector3(
        //         mapData.verticesMap[(int)cell.face.w].x, 
        //         mapData.verticesMap[(int)cell.face.w].y, 
        //         mapData.verticesMap[(int)cell.face.w].z);
        // }

        //draws the lines
        if(!spawnMeshes)
        {
            Vector3[] lineRenderPoint = new Vector3[4];
            lineRenderPoint[0] = mapData.verticesMap[(int)cell.face.w]*finalScale;
            lineRenderPoint[1] = mapData.verticesMap[(int)cell.face.x]*finalScale;
            lineRenderPoint[2] = mapData.verticesMap[(int)cell.face.y]*finalScale;
            lineRenderPoint[3] = mapData.verticesMap[(int)cell.face.z]*finalScale;

            LineRenderer lr = cell.GetComponentInChildren<LineRenderer>();
            lr.positionCount = 5;
            lr.SetPosition(0, lineRenderPoint[0]);
            lr.SetPosition(1, lineRenderPoint[1]);
            lr.SetPosition(2, lineRenderPoint[2]);
            lr.SetPosition(3, lineRenderPoint[3]);
            lr.SetPosition(4, lineRenderPoint[0]);

            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.startColor = Color.cyan;
            lr.endColor = Color.cyan;

        }

        // decorations
        if(tile.decorations==null)
            return;

        tile.decorations.gameObject.transform.localPosition = Vector3.zero;
        foreach(Transform child in tile.decorations.transform)
        {
            if(tile.trueNorth != null && child.name == "RotateMe")
            {
                child.transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(tile.trueNorth.position - 
                    child.transform.position, Vector3.up).eulerAngles.y, 0);
            }
        }

        //activate decorations
        for(int i = 0; i < tile.decorationData.Length; i++)
        {
            // SeededRandom.Init(seed+i);
            if(SeededRandom.Range(0f,1f) > tile.decorationData[i].activationChance)
                tile.decorationData[i].decorationGameObject.gameObject.SetActive(false);
        }
    }
    public void HandlePrefabMesh(Cell cell)
    {
        Vector4 face = cell.face;
        float desiredRotation = (cell.possiblePrototypes[0].meshRotation -1)*90;
        Tile tile = cell.collapsedGameObject.GetComponent<Tile>();

        //Get mesh of land
        Mesh mesh = tile.tileMeshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;
        Vector3[] normals = mesh.normals;

        Quaternion newRotation = new Quaternion();
        newRotation.eulerAngles = new Vector3(0,desiredRotation,0);//the degrees the vertices are to be rotated, for example (0,90,0) 
        
        //vertices being the array of vertices of your mesh
        for(int i = 0; i < vertices.Length; i++) {
            // Debug.Log($"mesh.bounds.center: {mesh.bounds.center}");
            //used to use mesh.bounds.center, but that was not working for cave tiles
            vertices[i] = newRotation * (vertices[i] - mesh.bounds.center) + mesh.bounds.center;
            // vertices[i] = newRotation * (vertices[i] - new Vector3(0.25f, -0.02f, 0.25f)) + new Vector3(0.25f, -0.02f, 0.25f);
        }

        //rotate the decorations 
        foreach(Transform child in tile.decorations.transform)
            child.transform.localPosition = newRotation * (child.transform.localPosition - new Vector3(0.25f, 0, 0.25f)) +new Vector3(0.25f, 0, 0.25f);

        //Get coords of the underlying face
        Vector3 a = mapData.verticesMap[(int)face.w];
        Vector3 b = mapData.verticesMap[(int)face.z];
        Vector3 c = mapData.verticesMap[(int)face.y];
        Vector3 d = mapData.verticesMap[(int)face.x];

        b-=a;
        c-=a;
        d-=a;
        a-=a;

        //get the x and y value of all verticies of the origional mesh
        for(int i = 0; i < vertices.Length; i++)
        {   
            //get the x and y value of the vertex
            float x = vertices[i].x*2;
            float y = vertices[i].y*2;
            float z = vertices[i].z*2;

            Vector3 q = Vector3.Lerp(a, b, x);
            Vector3 r = Vector3.Lerp(d, c, x);
            Vector3 p = Vector3.Lerp(r, q, z);

            vertices[i].x = p.x;
            vertices[i].z = p.z;
            vertices[i].y += p.y;
        }

        //rotate decoration
        if(tile.decorations!=null)
        {
            //get each child gameobject of decorations
            foreach(Transform child in tile.decorations.transform)
            {
                //rotate the local rotation of the child
                //get the x and y value of the vertex
                float x = (child.localPosition.x)*2;
                float y = (child.localPosition.y)*2;
                float z = (child.localPosition.z)*2;

                Vector3 q = Vector3.Lerp(a, b, x);
                Vector3 r = Vector3.Lerp(d, c, x);
                Vector3 p = Vector3.Lerp(r, q, z);

                child.localPosition = new Vector3(p.x, child.localPosition.y+p.y, p.z);
            }
        }

        //create new mesh and assign it to the land
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        // mesh.RecalculateNormals();
        tile.tileMeshFilter.sharedMesh = mesh;
        tile.tileMeshFilter.GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    public void ClearLand()
    {
        cells.Clear();
        // activeCells.Clear();
        edgeDictionary.Clear();

        for (int i = landParent.transform.childCount; i > 0; --i)
            DestroyImmediate(landParent.transform.GetChild(0).gameObject);

        for (int i = this.gameObject.transform.childCount; i > 0; --i)
            DestroyImmediate(this.gameObject.transform.GetChild(0).gameObject);
    }
    [System.Serializable] public struct MapData
    {
        public List<Vector4> faces;
        public List<Vector3> verticesMap;
    }
}
}
