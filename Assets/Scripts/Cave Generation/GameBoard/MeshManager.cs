using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;

namespace TJ
{
public class MeshManager : MonoBehaviour
{
    public static MeshManager Instance;
    [SerializeField] private SocketDictionary socketDictionary;
    public List<Prototype> basePrototypes = new List<Prototype>();
    public List<Prototype> prototypes = new List<Prototype>();
    [SerializeField] string path = "Assets/Data/WFC/Prototypes_Castle";
    [SerializeField] string prototypeHolderPrefabPath = "Assets/Data/WFC";
    SocketSO posXHolder;
    SocketSO negXHolder;
    SocketSO posZHolder;
    SocketSO negZHolder;
    List<GameObject> prototypeHolder = new List<GameObject>();
    [SerializeField] private GameObject prototypeHolderPrefab;

    [SerializeField] private GameObject grassPrefab;
    public GameObject GrassPrefab => grassPrefab;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public Prototype GetPrefab(SocketSO socket_ZW, SocketSO socket_WX, SocketSO socket_XY, SocketSO socket_YZ, Node node)
    {
        foreach(Prototype p in prototypes)
        {
            if(p.posX == socket_ZW && p.negZ == socket_WX && p.negX == socket_XY && p.posZ == socket_YZ)
            // if(p.posX == socket_ZW && p.negZ == socket_WX && p.negX == socket_XY && p.posZ == socket_YZ)
            // if(p.posX == socket_01 && p.negZ == socket_12 && p.negX == socket_23 && p.posZ == socket_30)
            {
                // Debug.Log($"Found for {socket_ZW.name}, {socket_WX.name}, {socket_XY.name}, {socket_YZ.name}");
                return p;
            }
        }
        Debug.Log($"No prefab found for {socket_ZW.name}, {socket_WX.name}, {socket_XY.name}, {socket_YZ.name}");
        return null;
    }
    [ContextMenu("Generate Prototypes")]
    public void GeneratePrototypes()
    {
        prototypes.Clear();
        if (!Directory.Exists(path)) 
            Directory.CreateDirectory(path);

        // Generate rotations for all prototypes
        for (int i = 0; i < basePrototypes.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string totalPath = path + "/" + basePrototypes[i].name +"_"+ j.ToString().Replace(" ", "") + ".asset";
                string filePath = path + "/" + basePrototypes[i].name +"_"+ j.ToString().Replace(" ", "");
                //check to see if prototpye exists
                if (File.Exists(totalPath))
                {
                    // Debug.Log($"Prototype @ {totalPath} already exists");
                    Prototype yourObject = (Prototype)AssetDatabase.LoadAssetAtPath(totalPath, typeof(Prototype));
                    // if(yourObject != null)
                    //     Debug.Log("Found Asset File !!!");

                    prototypes.Add(yourObject);
                }
                else
                {
                    Debug.Log($"Creating Prototype {basePrototypes[i].name + j.ToString().Replace(" ", "")}");
                    Prototype newProto = CreateMyAsset(path, basePrototypes[i].name, j.ToString().Replace(" ", ""));
                    prototypes.Add(newProto);
                }
            }
        }
        UpdatePrototypes();
        SavePrefab(prototypeHolderPrefab);
    }
    private void SavePrefab(GameObject prefab)
    {
        var SceneObject = Instantiate(prototypeHolderPrefab);
        SceneObject.GetComponent<Cell>().possiblePrototypes = prototypes;
        PrefabUtility.SaveAsPrefabAsset(SceneObject, prototypeHolderPrefabPath + "/" + prefab.name + ".prefab");
        DestroyImmediate (SceneObject);
    }
    public void ClearPrototypes()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    public void UpdatePrototypes()
    {
        // Generate rotations for all prototypes
        for (int i = 0; i < basePrototypes.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                prototypes[i*4+j].prefab = basePrototypes[i].prefab;
                prototypes[i*4+j].validNeighbours = new NeighbourList();
                prototypes[i*4+j].meshRotation = j;
                prototypes[i*4+j].basePrototype = basePrototypes[i];
                prototypes[i*4+j].prototypeClass = basePrototypes[i].prototypeClass;

                prototypes[i*4+j].posX = basePrototypes[i].posX;
                prototypes[i*4+j].negX = basePrototypes[i].negX;
                prototypes[i*4+j].posZ = basePrototypes[i].posZ;
                prototypes[i*4+j].negZ = basePrototypes[i].negZ;

                if(j==0)
                {
                    posXHolder = prototypes[i*4+j].posX;
                    negXHolder = prototypes[i*4+j].negX;
                    posZHolder = prototypes[i*4+j].posZ;
                    negZHolder = prototypes[i*4+j].negZ;
                }
                else
                {
                    prototypes[i*4+j].negZ = posXHolder;
                    prototypes[i*4+j].negX = negZHolder;
                    prototypes[i*4+j].posZ = negXHolder;
                    prototypes[i*4+j].posX = posZHolder;

                    posXHolder = prototypes[i*4+j].posX;
                    negXHolder = prototypes[i*4+j].negX;
                    posZHolder = prototypes[i*4+j].posZ;
                    negZHolder = prototypes[i*4+j].negZ;
                }

                EditorUtility.SetDirty(prototypes[i*4+j]);
            }
        }

        // // Generate valid neighbors
        for (int i = 0; i < prototypes.Count; i++)
            prototypes[i].validNeighbours = GetValidNeighbors(prototypes[i]);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static Prototype CreateMyAsset(string assetFolder, string name, string j)
    {
        Prototype asset = ScriptableObject.CreateInstance<Prototype>();
        AssetDatabase.CreateAsset(asset, assetFolder+"/"+name+"_"+j+".asset");
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        return asset;
    }
    private NeighbourList GetValidNeighbors(Prototype proto)
    {
        NeighbourList neighbourList = new NeighbourList();
        foreach(Prototype p in prototypes)
        {
            if(proto.posX==p.negX)
                neighbourList.posX.Add(p);
            if(proto.negX==p.posX)
                neighbourList.negX.Add(p);
            if(proto.posZ==p.negZ)
                neighbourList.posZ.Add(p);
            if(proto.negZ==p.posZ)
                neighbourList.negZ.Add(p);
        }
        return neighbourList;
    }
    public void DisplayPrototypes()
    {
        if(prototypeHolder.Count!=0)
        {
            foreach(GameObject p in prototypeHolder)
                DestroyImmediate(p);

            prototypeHolder = new List<GameObject>();
        }

        for (int i = 0; i < basePrototypes.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject protoObj = Instantiate(basePrototypes[i].prefab, new Vector3(i*1.5f, 0f, j*1.5f), Quaternion.identity, this.transform);
                protoObj.transform.Rotate(new Vector3(0f, j*90, 0f), Space.Self);
                protoObj.name = (basePrototypes[i].prefab.name +"_"+j.ToString());
                prototypeHolder.Add(protoObj);
            }
        }
    }
    public SocketSO GetSocket(int a, int b)
    {
        return socketDictionary.GetSocket(a,b);
    }
}
}
