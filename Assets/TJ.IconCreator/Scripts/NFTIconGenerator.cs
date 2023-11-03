using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TJ.Utilities;
using UnityEngine.UI;
using System.IO;
using System.Linq;

namespace TJ.IconCreator
{
public class NFTIconGenerator : MonoBehaviour
{
    public bool takeScreenShots;
    public Image[] images;
    public string filePath = "TJ.IconCreator/Metadata/metadata.json";
    [SerializeField] private ScreenshotHandler screenshotHandler;
    [SerializeField] private Transform canvasParent;
    [SerializeField] private int width, height;
    [SerializeField] private NFTMetaDataGenerator metaDataGenerator;
    private WaitForSeconds wait = new (0.1f);

    private void Start()
    {
        List<Metadata> metaData = LoadMetadata();

        //get the length of attributes
        int attributeLength = metaDataGenerator.layers.Count;
        Debug.Log($"Attribute Length: {attributeLength}");
        //create GameObjects

        images = new Image[attributeLength];
        GameObject go;
        Image image;
        for(int i = 0; i < attributeLength; i++)
        {
            go = new GameObject();
            go.transform.parent = canvasParent;
            image = go.AddComponent<Image>();
            image.rectTransform.anchorMin = new Vector2(0, 0);
            image.rectTransform.anchorMax = new Vector2(1, 1);
            image.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            image.rectTransform.sizeDelta = new Vector2(0, 0);
            image.rectTransform.anchoredPosition = new Vector2(0, 0);
            images[i] = image;
        }

        if(takeScreenShots){
            //delete everyting in filepath folder
            DirectoryInfo dir = new DirectoryInfo(screenshotHandler.filepath);
            foreach (FileInfo file in dir.GetFiles()) {
                file.Delete(); 
            }
            StartCoroutine(GenerateScreenShots(metaData));
        }
    }
    [ContextMenu("Load Metadata")]
    public List<Metadata> LoadMetadata()
    {
        MetadataList metaData = new(){
            data = JSONFileHandler.ReadListFromJSON<MetadataList>(filePath).data
        };

        return metaData.data;
    }
    public IEnumerator GenerateScreenShots(List<Metadata> metaData)
    {
        //pause for 1 second to allow the scene to load
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < metaData.Count; i++)
        {
            // Debug.Log($"Displaying {i}");
            DisplayTraits(metaData[i].attributes);

            yield return wait;
            screenshotHandler.TakeScreenshot(width, height, (i+1).ToString());
            yield return wait;
        }
    }
    private void DisplayClothes(int j)
    {
        // Debug.Log($"Displaying Clothes");
        //disable all images
        for(int i = j; i < j+6; i++){
            //get trait class

            // sprite = metaDataGenerator.GetClothesSprite(i);/
            // images[i].sprite = sprite;
            images[i].enabled = true;
        }
    }
    private bool IsDruid (Trait[] traits) {
    for(int i = 0; i < traits.Length; i++){
      if(traits[i].trait_type == "Class"){
        return traits[i].value == "Druid";
      }
    }
        return false;
    }
   private bool IsTinkerer (Trait[] traits) {
    for(int i = 0; i < traits.Length; i++){
        if(traits[i].trait_type == "Class"){
            return traits[i].value == "Tinkerer";
        }
    }
        return false;
    }
    public void DisplayTraits(Trait[] traits)
    {
        //disable all images
        for(int i = 0; i < images.Length; i++){
            images[i].enabled = false;
        }
        Sprite sprite;
        for(int i = 0; i < traits.Length; i++){
            // Debug.Log($"Displaying {traits[i].value} for {traits[i].trait_type}");
            // if(traits[i].trait_type == "Event")
            //     continue;
            
            if(traits[i].trait_type == "Rarity")
            {
                //check if traits contain isShiny
                if(traits.Any(x => x.value == "Shiny"))
                {
                    sprite = metaDataGenerator.GetTraitSprite(traits[i].value+"Shiny", traits[i].trait_type);
                    images[i].sprite = sprite;
                    images[i].enabled = true;
                    continue;
                } else {
                    sprite = metaDataGenerator.GetTraitSprite(traits[i].value, traits[i].trait_type);
                    images[i].sprite = sprite;
                    images[i].enabled = true;
                    continue;
                }
            }

            sprite = metaDataGenerator.GetTraitSprite(traits[i].value, traits[i].trait_type);
            images[i].sprite = sprite;
            images[i].enabled = true;
        }
        DisplayClothes(traits.Length);
        //refresh canvas
        canvasParent.gameObject.SetActive(false);
        canvasParent.gameObject.SetActive(true);
    }
}
}