using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
public class Cube : MonoBehaviour
{
public int cubeX, cubeZ;
    // [SerializeField] private TMP_Text tileNumber;
    // [SerializeField] private BiomeHolder biomeType;
    // public BiomeHolder BiomeType => biomeType;

    [Header("CubeSpecifics")]
    public CubeSO cubeSO;
    public enum CubeType{Land,Coast,Water,Trees,Rock,Building}
    [SerializeField] private bool hasBlockingItem;
    public bool HasBlockingItem => hasBlockingItem;
    [SerializeField] private bool beingWorkedOn;
    public bool BeingWorkedOn => beingWorkedOn;
    [SerializeField] private bool canBeBuiltOn;
    public bool CanBeBuiltOn => canBeBuiltOn;

    [Header("Position Data")]
    [SerializeField] private LayerMask waterLayer;

    [Header("TilePrefab")]
    [SerializeField] private GameObject landTileObject;
    public GameObject LWWW, WLWW, WWLW, WWWL;
    [SerializeField] private Transform buildSpot;
    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject rock;
    // [SerializeField] private TileTimer upgradeTimer;
    // [SerializeField] private Slider upgradeTimerSlider;
    // [SerializeField] private Transform timerSpot;
    // [SerializeField] private Building building;
    // public Building Building => building;
    private bool jadeOverride;
    private bool cancelOverride;
    public bool CancelOverride => cancelOverride;
    [SerializeField] private float secondsRemaining;
    public float SecondsRemaining => secondsRemaining;
    // private Outline outline;
    // private UIManager uIManager;
    // private GameplayActionManager gameplayActionManager;
    // private GameplayAction gameplayAction;
    [SerializeField] private List<Cube> oneXOneNeighbors = new List<Cube>();
    public List<Cube> OneTileNeighbors => oneXOneNeighbors;
    private List<Cube>  twoXTwoNeighbors = new List<Cube>();
    [HideInInspector] public List<Cube> TwoTileNeighbors => twoXTwoNeighbors;
    private List<Cube>  threeXthreeNeighbors = new List<Cube>();
    [HideInInspector] public List<Cube> ThreeTileNeighbors => threeXthreeNeighbors;
    // GridGenerator gridGenerator;
    [SerializeField] private bool tutorialStep;
    // private void Start()
    // {
    //     if(GameManager.instance==null)
    //         return;

    //     outline = GetComponent<Outline>();
    //     uIManager = GameManager.instance.uIManager;
    //     gameplayActionManager = GameManager.instance.gameplayActionManager;
    //     gridGenerator = GameManager.instance.gridGenerator;
    // }
    public void FindNeighbors()
    {
        Debug.Log($"Found neighbors");
        // if(gridGenerator==null)
        //     gridGenerator = FindObjectOfType<GridGenerator>();

        oneXOneNeighbors.Add(this);

        // twoXTwoNeighbors.Add(this);
        // twoXTwoNeighbors.Add(gridGenerator.GetCube(cubeZ,cubeX+1));
        // twoXTwoNeighbors.Add(gridGenerator.GetCube(cubeZ+1,cubeX+1));
        // twoXTwoNeighbors.Add(gridGenerator.GetCube(cubeZ+1,cubeX));

        // threeXthreeNeighbors.Add(twoXTwoNeighbors[0]);
        // threeXthreeNeighbors.Add(twoXTwoNeighbors[1]);
        // threeXthreeNeighbors.Add(twoXTwoNeighbors[3]);
        // threeXthreeNeighbors.Add(gridGenerator.GetCube(cubeZ+2,cubeX+2));
        // threeXthreeNeighbors.Add(gridGenerator.GetCube(cubeZ+2,cubeX));
        // threeXthreeNeighbors.Add(gridGenerator.GetCube(cubeZ+2,cubeX+1));
        // threeXthreeNeighbors.Add(gridGenerator.GetCube(cubeZ+1,cubeX+2));
        // threeXthreeNeighbors.Add(gridGenerator.GetCube(cubeZ,cubeX+2));
    }
    public void CreateBuilding(GameObject prefab, Vector3 pos, Quaternion buildingRotation)
    {
        // building = Instantiate(prefab, pos, buildingRotation, buildSpot).GetComponent<Building>();
    }
    public void LockAbilityToBuildOn()
    {
        canBeBuiltOn = false;
    }
    public void WorkOnCube()
    {
        beingWorkedOn = true;
    }
    // public IEnumerator StartCubeTimer(GameplayAction _gameplayAction)
    // {
    //     WorkOnCube();
    //     gameplayAction = _gameplayAction;
    //     DateTime actionStartTime = JsonUtility.FromJson<JsonDateTime>(gameplayAction.actionStartTime);

    //     float actionDuration = 0;
    //     if(building!=null)
    //         actionDuration = building.BuildingSO.buildTime;
    //     else
    //         actionDuration = GameManager.instance.GetTimeValue(gameplayAction.actionType);

    //     secondsRemaining = actionDuration - (float)(System.DateTime.UtcNow - actionStartTime).TotalSeconds;
    //     uIManager.RefreshTilePopUp(this);

    //     upgradeTimer = Instantiate(uIManager.TimerPrefab, uIManager.TimerParent).GetComponent<TileTimer>();
    //     upgradeTimer.upgradeBarTarget = timerSpot;

    //     upgradeTimerSlider = upgradeTimer.GetComponent<Slider>();
    //     upgradeTimerSlider.maxValue = 1;
    //     TMP_Text upgradeTimeText = upgradeTimer.GetComponentInChildren<TMP_Text>();

    //     float diffInSeconds = (float)(System.DateTime.UtcNow - actionStartTime).TotalSeconds;
    //     jadeOverride = false;
    //     cancelOverride = false;

    //     while(secondsRemaining>0 && !jadeOverride && !cancelOverride)
    //     {
    //         upgradeTimerSlider.value = (actionDuration - secondsRemaining) / actionDuration;
    //         secondsRemaining = actionDuration - (float)(System.DateTime.UtcNow - actionStartTime).TotalSeconds;
    //         upgradeTimeText.text = TimeParser.ParseSecondsRemaining(secondsRemaining);
    //         yield return new WaitForSeconds(0.1f);
    //     }

    //     // uIManager.upgradeTimers.Remove(upgradeTimer);
    //     Destroy(upgradeTimer.gameObject);
    //     beingWorkedOn = false;
    //     uIManager.RefreshTilePopUp(this);
    // }
    // public void SetBiomeType(BiomeHolder bh, CubeSO treeSO)
    // {
    //     biomeType = bh;
    //     SetTreeTile(treeSO);
    // }
    public void SetTileNumber (string num)
    {
        // tileNumber.text = num;
    }
    public void EnableOutline(bool en)
    {
        // outline.enabled = en;

        //tutorial
        // if(tutorialStep)
        //     GameManager.instance.tutorialManager.TreesSelected();

        // if(cubeSO.cubeType == CubeType.Trees)
        //     GameManager.instance.tutorialManager.CompleteStep(GameManager.instance.tutorialManager.TreeSelectionStep);

        // if(building!=null && building.BuildingSO == GameManager.instance.sawmill)
        //     GameManager.instance.tutorialManager.CompleteStep(GameManager.instance.tutorialManager.SelectSawmillBuildingStep);
    }
    public string GetCubeTypeName()
    {
        switch(cubeSO.cubeType)
        {
            case CubeType.Land:
                return cubeSO.cubeName;
            case CubeType.Coast:
                return cubeSO.cubeName;
            case CubeType.Water:
                return cubeSO.cubeName;
            case CubeType.Trees:
                return cubeSO.cubeName;
            case CubeType.Rock:
                return cubeSO.cubeName;
            // case CubeType.Building:
            //     return building.BuildingSO.name;
            default:
                return "Getting cube name broke";
        }
    }
    public void CompleteActionOnTile()
    {
        Debug.Log($"Completed action");
        jadeOverride = true;
        // gameplayActionManager.CompleteActionNow(gameplayAction.actionLocationX, gameplayAction.actionLocationZ);
    }
    public void CancelActionOnTile()
    {
        cancelOverride = true;
        beingWorkedOn = false;
        
        // if(building!=null)
        //     Destroy(building.gameObject);
            
        // gameplayActionManager.CancelAction(gameplayAction.actionLocationX, gameplayAction.actionLocationZ);
    }
    public void SetTreeTile(CubeSO _cubeSo)
    {
        SetCubeSO(_cubeSo);
        int t = UnityEngine.Random.Range(0,trees.Length);
        trees[t].SetActive(true);
        trees[t].transform.rotation = Quaternion.Euler(0f,UnityEngine.Random.Range(0.00f,0.36f),0f);
        hasBlockingItem = true;
        canBeBuiltOn = false;
    }
    public void ClearLand(CubeSO _cubeSO)
    {
        for (int i = 0; i < trees.Length; i++)
            trees[i].SetActive(false);
        
        rock.SetActive(false);
        hasBlockingItem = false;
        canBeBuiltOn = true;
        SetCubeSO(_cubeSO);
    }
    public void ResetCube(CubeSO _cubeSo)
    {
        SetCubeSO(_cubeSo);
    }
    public void AssignXZ(int x, int z)
    {
        cubeX = x;
        cubeZ = z;
    }
    public void LoadCoastTile(CubeSO _cubeSo, string neighbors)
    {
        SetCubeSO(_cubeSo);

        if(neighbors.Substring(0,1)=="l")
            LWWW.SetActive(true);

        if(neighbors.Substring(1,1)=="l")
            WLWW.SetActive(true);

        if(neighbors.Substring(2,1)=="l")
            WWLW.SetActive(true);

        if(neighbors.Substring(3,1)=="l")
            WWWL.SetActive(true);

        canBeBuiltOn = false;
    }
    public void LoadRock(CubeSO _cubeSo)
    {
        ClearLand(_cubeSo);
        canBeBuiltOn = false;
        hasBlockingItem = true;
        rock.SetActive(true);
    }
    public void SetLandTile(CubeSO _cubeSo)
    {
        ClearLand(_cubeSo);
        landTileObject.SetActive(true);
    }
    public void AssignBiomeType(CubeSO waterSO, CubeSO landSO, CubeSO treeSO)
    {
        RaycastHit hit;
        //Debug.Log($"{transform.position}");
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 5f))
        {
            // Debug.Log($"hit.transform.gameObject.layer{hit.transform.gameObject.layer}");
            if(1<<hit.transform.gameObject.layer == waterLayer.value)
            {
                // Debug.Log("Hit water");
                transform.position = new Vector3(transform.position.x, transform.position.y - 1.55f, transform.position.z);
                cubeSO = waterSO;
            }
            else
            {
                // Debug.Log($"Did not hit water");
                // biomeType = hit.transform.gameObject.GetComponentInParent<BiomeHolder>();
                transform.position = new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z);
                SetLandTile(landSO);

                // if(biomeType!=null)
                // {
                //     tileNumber.text = biomeType.tileNumber.ToString();
                //     // Debug.Log($"Tile Number {biomeType.tileNumber}");
                //     // if(biomeType.tileType == BiomeHolder.TileType.forest)
                //         SetTreeTile(treeSO);
                // }
                // Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * 5f, Color.red);
            }
        }
    }
    public void SetCubeSO(CubeSO _cubeSO)
    {
        cubeSO = _cubeSO;
    }
}
    [CreateAssetMenu(menuName = "ScriptableObjects/Cube")]
public class CubeSO : ScriptableObject
{
    public string cubeName;
    [SerializeField] private Cube.CubeType CubeType;
    public Cube.CubeType cubeType => CubeType;
}
}
