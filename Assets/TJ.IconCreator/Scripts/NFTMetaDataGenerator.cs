using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TJ.Utilities;

namespace TJ.IconCreator
{
public class NFTMetaDataGenerator : MonoBehaviour
{
    public string filePath = "TJ.IconCreator/Metadata/",filePathIndv ="TJ.IconCreator/Metadata/Indv/", fileName = "metadata";
    public int totalCount;
    public List<NFTLayer> layers;
    public List<OneOfOne> oneOfOnes;
    public List<OverrideTrait> overrideTraits;

    [ContextMenu("Generate MetaData")]
    public void GenerateMetaData()
    {
        SeededRandom.Init(0);

        MetadataList metaData = new(){
            data = new List<Metadata>()
        };


        for(int i = 1; i <= totalCount; i++){
            string id = i.ToString();
            string name = "Goblin Gala Goer " + id;
            string description = "Meet a whimsical attendee of the Goblin Gala, a part of the enchanting Ryoshi Tales collection. Each Gala Goer is unique, bringing a touch of magic and mystery to the Ryoshi universe. Collect them and discover their individual tales, and prepare for a journey into the fanciful world of Ryoshi Tales.";
            string imagePath = $"https://cdn-prod.ebisusbay.com/files/ryoshi/images/goblingala/{id}.png";
            
            List<Trait> attributes = new();
            for(int j = 0; j < layers.Count; j++){

                static bool FailsToApply(float chance) => chance < SeededRandom.Range(0f, 1f);
                if(FailsToApply(layers[j].layerChance))
                    continue;

                //get a number between 0 and 1
                float chance = SeededRandom.Range(0f, 1f);
                // Debug.Log($"Chance: {chance}");
                //get the total occurance of all attributes
                float totalOccurance = 0;

                for(int k = 0; k < layers[j].attributes.Count; k++) {
                    totalOccurance += layers[j].attributes[k].occurance;
                }
                // Debug.Log($"Total Occurance: {layers[j].trait_type} - {totalOccurance}");
                //get the chance of each attribute
                float[] attributeChances = new float[layers[j].attributes.Count];
                for(int k = 0; k < layers[j].attributes.Count; k++) {
                    attributeChances[k] = layers[j].attributes[k].occurance / totalOccurance;
                }
                // Debug.Log($"Attribute Chances: {string.Join(", ", attributeChances)}");
                //get the attribute index
                int attributeIndex = 0;
                float attributeChance = attributeChances[0];
                while(chance > attributeChance) {
                    attributeIndex++;
                    attributeChance += attributeChances[attributeIndex];
                }
                // Debug.Log($"Attribute Index: {attributeIndex}");
                //add the attribute to the list

                attributes.Add(new Trait(
                    layers[j].trait_type, 
                    layers[j].attributes[attributeIndex].traitName, 
                    layers[j].displayType == DisplayType.string_DT ? "string" : "number" ));
            }

            metaData.data.Add(new Metadata(imagePath, name, description, id, attributes.ToArray()));
        }

        //apply overrides
        int t = 0;
        foreach(OneOfOne oOo in oneOfOnes) {

            List<Trait> attributes = new();
            for(int j = 0; j < layers.Count; j++){
                for(int k = 0; k < layers[j].attributes.Count; k++) {

                    if(oOo.attributesNames.Contains(layers[j].attributes[k].traitName)) {
                        attributes.Add(new Trait(
                            layers[j].trait_type, 
                            layers[j].attributes[k].traitName, 
                            layers[j].displayType == DisplayType.string_DT ? "string" : "number" ));
                        break;
                    }
                }
            }
            metaData.data[t] = new Metadata(
                metaData.data[t].imagePath, 
                metaData.data[t].name, 
                metaData.data[t].description, 
                metaData.data[t].id, 
                attributes.ToArray());

            t++;
        }

        string GetPirchforkName() {
            if(SeededRandom.Range(0f, 1f) < 0.90f) {
                return "Red Pitchfork";
            } else {
                return "Black Pitchfork";
            }
        }
        string GetWitchHat() {
            if(SeededRandom.Range(0f, 1f) < 0.80f) {
                return "Midnight Witch Hat";
            } else {
                return "Purple Witch Hat";
            }
        }
        for(int i = 0; i < metaData.data.Count; i++) {
            bool hasMask = false, isDevil = false, isGoblin = false, isWitch = false, isRobot = false;
            Metadata m = metaData.data[i];

            for(int j = 0; j < metaData.data[i].attributes.Length; j++) {
                if(metaData.data[i].attributes[j].trait_type == "Masks") {
                    if(metaData.data[i].attributes[j].value.Contains("Devil")){
                        isDevil = true;
                    } else if(metaData.data[i].attributes[j].value.Contains("Goblin")){
                        isGoblin = true;
                    } else if(metaData.data[i].attributes[j].value.Contains("Witch")){
                        isWitch = true;
                    }

                    hasMask = true;
                }
                if(metaData.data[i].attributes[j].trait_type == "Clothing") {
                    if(metaData.data[i].attributes[j].value.Contains("Robot")){
                        // Debug.Log($"Robot: {m.name}");
                        isRobot = true;
                    }
                }
            }

            if(isDevil){
                //95% chance to get a redtail
                if(SeededRandom.Range(0f, 1f) < 0.95f) {
                    List<Trait> traits_2 = new List<Trait>(m.attributes);
                    traits_2.Insert(1, new Trait("Tails", "Red Devil Tail", "string"));
                    m.attributes = traits_2.ToArray();
                } else {
                    List<Trait> traits_2 = new List<Trait>(m.attributes);
                    traits_2.Insert(1, new Trait("Tails", "Black Devil Tail", "string"));
                    m.attributes = traits_2.ToArray();
                }

                //90% chance to get pitchfork attribute
                if(SeededRandom.Range(0f, 1f) < 0.90f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", GetPirchforkName(), "string")
                    }.ToArray();
                }
            }

            if(isGoblin){
                //5% chance of broom
                if(SeededRandom.Range(0f, 1f) < 0.05f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", "Broom", "string")
                    }.ToArray();
                }
            }

            if(isWitch){
                //90% chance of broom
                if(SeededRandom.Range(0f, 1f) < 0.90f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", "Broom", "string")
                    }.ToArray();
                }
            }
            
            if(!hasMask) {
                //5% chanve to get a tail
                if(SeededRandom.Range(0f, 1f) < 0.05f) {
                    //insert the tail as the second of the list
                    List<Trait> traits_2 = new List<Trait>(m.attributes);
                    traits_2.Insert(1, new Trait("Tails", "Red Devil Tail", "string"));
                    m.attributes = traits_2.ToArray();
                }

                //25% chance to get pitchfork attribute
                if(SeededRandom.Range(0f, 1f) < 0.25f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", GetPirchforkName(), "string")
                    }.ToArray();
                }

                //15% chance of broom
                if(SeededRandom.Range(0f, 1f) < 0.15f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", "Broom", "string")
                    }.ToArray();
                }

                //25% chance of witchhat
                if(SeededRandom.Range(0f, 1f) < 0.25f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Hair", GetWitchHat(), "string")
                    }.ToArray();
                }

                //15% chance of robotpan
                if(SeededRandom.Range(0f, 1f) < 0.15f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Hair", "Robot Pan", "string")
                    }.ToArray();
                }

                //2% chance of goblin-totem
                if(SeededRandom.Range(0f, 1f) < 0.02f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", "Goblin Totem", "string")
                    }.ToArray();
                }

                //10% goblin lamp
                if(SeededRandom.Range(0f, 1f) < 0.10f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Accessories", "Goblin Lamp", "string")
                    }.ToArray();
                }
            }

            if(isRobot){
                //85% chance to get robotpan
                if(SeededRandom.Range(0f, 1f) < 0.85f) {
                    m.attributes = new List<Trait>(m.attributes) {
                        new Trait("Hair", "Robot Pan", "string")
                    }.ToArray();
                }
            }

            //remove any duplicate traits that share the same trait_type
            List<Trait> traits = new();
            for(int j = 0; j < m.attributes.Length; j++) {
                bool found = false;
                for(int k = 0; k < traits.Count; k++) {
                    if(m.attributes[j].trait_type == traits[k].trait_type) {
                        found = true;
                        break;
                    }
                }
                if(!found) {
                    traits.Add(m.attributes[j]);
                }
            }
            m.attributes = traits.ToArray();
            //if isGoblin, move the goblin mask to last in the list
            // if(isGoblin || isWitch) {
            //     for(int j = 0; j < m.attributes.Length; j++) {
            //         if(m.attributes[j].trait_type == "Masks" && m.attributes[j].value.Contains("Goblin")) {
            //                 (m.attributes[m.attributes.Length-1], m.attributes[j]) = (m.attributes[j], m.attributes[m.attributes.Length-1]);
            //                 break;
            //         }
            //         if(m.attributes[j].trait_type == "Hait" && m.attributes[j].value.Contains("Witch")) {
            //                 (m.attributes[m.attributes.Length-1], m.attributes[j]) = (m.attributes[j], m.attributes[m.attributes.Length-1]);
            //                 break;
            //         }
            //     }
            // }
            //sort list of attributes so they appear in this order
            List<string> attributeOrder = new() {
                "Background",
                "Tails",
                "Body",
                "Eyes",
                "Mouth",
                "Accessories",
                "Clothing",
                "Masks",
                "Hair",
            };

            List<Trait> sortedTraits = new();
            for(int j = 0; j < attributeOrder.Count; j++) {
                for(int k = 0; k < m.attributes.Length; k++) {
                    if(m.attributes[k].trait_type == attributeOrder[j]) {
                        sortedTraits.Add(m.attributes[k]);
                        break;
                    }
                }
            }
            m.attributes = sortedTraits.ToArray();
            

            m.attributes = new List<Trait>(m.attributes) { new Trait("Event", "Goblin Gala", "string") }.ToArray();

            metaData.data[i] = m;
        }


        //get each trait and count how many times it appears
        // Dictionary<string, int> traitCount = new();
        // foreach(Metadata m in metaData.data) {
        //     foreach(Trait trt in m.attributes) {
        //         if(traitCount.ContainsKey(trt.value)) {
        //             traitCount[trt.value]++;
        //         } else {
        //             traitCount.Add(trt.value, 1);
        //         }
        //     }
        // }
        // //log out the traits and their count
        // foreach(KeyValuePair<string, int> kvp in traitCount) {
        //     // Debug.Log($"{kvp.Key}: {kvp.Value}");
        // }


        JSONFileHandler.SaveToJSON<MetadataList>(metaData, filePath + fileName +".json", true);
    }
    [ContextMenu("Indv Metadata")]
    public void GenerateIndvMetaData()
    {
        MetadataList metaData = new(){
            data = JSONFileHandler.ReadListFromJSON<MetadataList>(filePath+fileName+".json").data
        };

        //save each metadata as a seperate file
        for(int i = 0; i < metaData.data.Count; i++) {
            JSONFileHandler.SaveToJSON<Metadata>(metaData.data[i], filePathIndv + metaData.data[i].id +".json", true);
        }
    }
    
    public Sprite GetTraitSprite(string trait, string layerName)
    {
        Sprite sprite = null;
        int layerIndex = -1;
        for(int i = 0; i < layers.Count; i++) {
            if(layers[i].trait_type == layerName) {
                layerIndex = i;
                break;
            }
        }
        // Debug.Log($"Getting {trait} for {layerIndex}");
        for(int i = 0; i < layers[layerIndex].attributes.Count; i++) {
            // Debug.Log($"Checking {layers[layerIndex].trait_type} for {trait}");
            if(layers[layerIndex].attributes[i].traitName == trait) {
                sprite = layers[layerIndex].attributes[i].image;
                break;
            }
        }
        if(sprite == null) {
            Debug.LogError($"Could not find {trait} for {layers[layerIndex].trait_type}");
        }

        return sprite;
    }
}
[System.Serializable] public struct MetadataList 
{
    public List<Metadata> data;
}
[System.Serializable] public struct Metadata
{
    public string imagePath;
    public string name;
    public string description;
    public string id;
    public Trait[] attributes;
    public Metadata(string imagePath, string name, string description, string id, Trait[] attributes) {
        this.imagePath = imagePath;
        this.name = name;
        this.description = description;
        this.id = id;
        this.attributes = attributes;
    }
}
[System.Serializable] public struct NFTLayer
{
    public string trait_type;
    public float layerChance;
    public DisplayType displayType;
    public List<LayerAttribute> attributes;
}
[System.Serializable] public struct OneOfOne
{
    public List<string> attributesNames;
}
[System.Serializable] public struct LayerAttribute 
{
    public string traitName;
    public bool useSpecificCount;
    public float occurance;
    public int count;
    public Sprite image;
}
[System.Serializable] public struct Trait {
    public string trait_type;
    public string value;
    public string displayType;
    public Trait(string trait_type, string value, string displayType) {
        this.trait_type = trait_type;
        this.value = value;
        this.displayType = displayType;
    }
}
[System.Serializable] public struct OverrideTrait {
    public string layerName;
    public string trait_type;
    public List<OverrideTraitValue> overrideTraitValue;
}
[System.Serializable] public struct OverrideTraitValue {
    public Trait trait;
    public float chance;

}
    public enum DisplayType {string_DT, number_DT};
}