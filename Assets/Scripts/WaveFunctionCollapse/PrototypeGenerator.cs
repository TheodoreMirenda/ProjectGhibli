using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
public class PrototypeGenerator : MonoBehaviour
{
    public List<Prototype> protoypePrefabs;
    public List<Prototype> prototypes;
    public string path = "Assets/Data/Prototypes";
    public string prototypeHolderPrefabPath = "Assets/Data/WFC";
    SocketSO posXHolder;
    SocketSO negXHolder;
    SocketSO posZHolder;
    SocketSO negZHolder;
    List<GameObject> prototypeHolder = new List<GameObject>();
    [SerializeField] private GameObject prototypeHolderPrefab;
    
    public void GeneratePrototypes()
    {
        prototypes.Clear();
        if (!Directory.Exists(path)) 
            Directory.CreateDirectory(path);

        // Generate rotations for all prototypes
        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string totalPath = path + "/" + protoypePrefabs[i].name +"_"+ j.ToString().Replace(" ", "") + ".asset";
                string filePath = path + "/" + protoypePrefabs[i].name +"_"+ j.ToString().Replace(" ", "");
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
                    Debug.Log($"Creating Prototype {protoypePrefabs[i].name + j.ToString().Replace(" ", "")}");
                    Prototype newProto = CreateMyAsset(path, protoypePrefabs[i].name, j.ToString().Replace(" ", ""));
                    prototypes.Add(newProto);
                }
            }
        }
        UpdatePrototypes();
        SavePrefab(prototypeHolderPrefab, prototypeHolderPrefabPath);
    }
    private void SavePrefab(GameObject prefab, string path)
    {
        var SceneObject = Instantiate(prototypeHolderPrefab);
        SceneObject.GetComponent<Cell>().possiblePrototypes = prototypes;
        PrefabUtility.SaveAsPrefabAsset(SceneObject, path + "/" + prefab.name + ".prefab");
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
        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                prototypes[i*4+j].prefab = protoypePrefabs[i].prefab;
                prototypes[i*4+j].validNeighbours = new NeighbourList();
                prototypes[i*4+j].meshRotation = j;
                prototypes[i*4+j].basePrototype = protoypePrefabs[i];
                prototypes[i*4+j].prototypeClass = protoypePrefabs[i].prototypeClass;

                prototypes[i*4+j].posX = protoypePrefabs[i].posX;
                prototypes[i*4+j].negX = protoypePrefabs[i].negX;
                prototypes[i*4+j].posZ = protoypePrefabs[i].posZ;
                prototypes[i*4+j].negZ = protoypePrefabs[i].negZ;

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

        for (int i = 0; i < protoypePrefabs.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject protoObj = Instantiate(protoypePrefabs[i].prefab, new Vector3(i*1.5f, 0f, j*1.5f), Quaternion.identity, this.transform);
                protoObj.transform.Rotate(new Vector3(0f, j*90, 0f), Space.Self);
                protoObj.name = (protoypePrefabs[i].prefab.name +"_"+j.ToString());
                prototypeHolder.Add(protoObj);
            }
        }
    }
}
