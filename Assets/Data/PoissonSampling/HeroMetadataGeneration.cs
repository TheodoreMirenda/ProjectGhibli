using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TJ.Utilities;
using System.Xml.Serialization;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class HeroMetadataGeneration : MonoBehaviour
{
    [System.Serializable] public enum HeroRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic };
    [System.Serializable] public enum HeroClass { Rogue, Warrior, Mage, Druid, Paladin, Tinker, Priest}
    [System.Serializable] public enum HeroLocation { Celestial_Cliffs, Ebisus_Bay, Verdant_Forest, Felisgarde, Highlands}
    [System.Serializable] public enum HeroSkin { Fair, Medium, Tan}
    [System.Serializable] public enum HeroHair {Blonde, Blue, Brown, Electrified, Green, Hairbun, Samurai}
    [System.Serializable] public enum HeroEyes{Bewildered, Canadian, Devout, Green, Raised_EyeBrows, Squint};
    [System.Serializable] public enum HeroMouth{Circle_Beard, Crazed, Goatee, Green_Beard, Open, Smile, Smug};
    [System.Serializable] public enum HeroClothes{Normal, Light, Dark};
    public struct HeroRarityClass { public HeroRarity rarity; public HeroClass heroClass; public HeroLocation heroLocation;}
    [System.Serializable] public struct HeroStats {
        public int STR;
        public int DEX;
        public int INT;
        public int WIS;
        public int AGI;
        public int LUCK;
        public int CHA;
        public void ModifyStat(int statId, int amount){
            switch(statId){
                case 0:
                    STR += amount;
                    break;
                case 1:
                    DEX += amount;
                    break;
                case 2:
                    INT += amount;
                    break;
                case 3:
                    WIS += amount;
                    break;
                case 4:
                    AGI += amount;
                    break;
                case 5:
                    LUCK += amount;
                    break;
                case 6:
                    CHA += amount;
                    break;
                default: 
                    Debug.Log($"wtf");
                    break;
            }
        }
    }
    public Dictionary<HeroRarityClass, HeroStats> heroStatsDict = new ();
    [System.Serializable] public struct SerializedHeroes { public List<Hero> heroes; }
    [System.Serializable] public class Hero {
        public int id;
        public string rarity;
        public string heroClass;
        public HeroStats stats;
        public string location;
        public string skin;
        public string hair;
        public string eyes;
        public string mouth;
        public string clothes;
        public bool isShiny;  
        public Hero (int id, HeroRarity rarity, HeroClass heroClass, HeroStats stats, HeroLocation heroLocation, 
            HeroSkin heroSkin, HeroHair heroHair, HeroEyes heroEyes, HeroMouth heroMouth, HeroClothes heroClothes, bool isShiny) {
            this.id = id;
            this.rarity = rarity.ToString();
            this.heroClass = heroClass.ToString();
            this.stats = stats;
            this.location = heroLocation.ToString();
            this.skin = heroSkin.ToString();
            this.hair = heroHair.ToString();
            this.eyes = heroEyes.ToString();
            this.mouth = heroMouth.ToString();
            this.clothes = heroClothes.ToString();
            this.isShiny = isShiny;
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

    public void CreateHeroStatsDict(){
        heroStatsDict.Clear();
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Rogue }, new HeroStats { STR = 5, DEX = 8, INT = 4, WIS = 4, AGI = 7, LUCK = 4, CHA = 3 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Rogue }, new HeroStats { STR = 6, DEX = 8, INT = 5, WIS = 4, AGI = 8, LUCK = 4, CHA = 5 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Rogue }, new HeroStats { STR = 7, DEX = 8, INT = 6, WIS = 5, AGI = 8, LUCK = 5, CHA = 6 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Rogue }, new HeroStats { STR = 7, DEX = 9, INT = 6, WIS = 5, AGI = 9, LUCK = 6, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Rogue }, new HeroStats { STR = 7, DEX = 9, INT = 6, WIS = 6, AGI = 9, LUCK = 8, CHA = 10 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Rogue }, new HeroStats { STR = 7, DEX = 10, INT = 6, WIS = 6, AGI = 10, LUCK = 10, CHA = 11 });
        
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Warrior }, new HeroStats { STR = 8, DEX = 5, INT = 3, WIS = 4, AGI = 5, LUCK = 4, CHA = 6 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Warrior }, new HeroStats { STR = 9, DEX = 5, INT = 4, WIS = 5, AGI = 5, LUCK = 5, CHA = 7 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Warrior }, new HeroStats { STR = 9, DEX = 6, INT = 4, WIS = 6, AGI = 6, LUCK = 6, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Warrior }, new HeroStats { STR = 10, DEX = 7, INT = 5, WIS = 6, AGI = 7, LUCK = 6, CHA = 9 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Warrior }, new HeroStats { STR = 10, DEX = 8, INT = 6, WIS = 6, AGI = 7, LUCK = 9, CHA = 9 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Warrior }, new HeroStats { STR = 12, DEX = 9, INT = 6, WIS = 6, AGI = 7, LUCK = 10, CHA = 10 });

        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Mage }, new HeroStats { STR = 3, DEX = 4, INT = 8, WIS = 7, AGI = 4, LUCK = 4, CHA = 5 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Mage }, new HeroStats { STR = 4, DEX = 4, INT = 9, WIS = 8, AGI = 4, LUCK = 5, CHA = 6 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Mage }, new HeroStats { STR = 4, DEX = 6, INT = 10, WIS = 8, AGI = 5, LUCK = 5, CHA = 7 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Mage }, new HeroStats { STR = 5, DEX = 6, INT = 10, WIS = 9, AGI = 6, LUCK = 6, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Mage }, new HeroStats { STR = 6, DEX = 7, INT = 11, WIS = 10, AGI = 6, LUCK = 6, CHA = 9 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Mage }, new HeroStats { STR = 7, DEX = 8, INT = 11, WIS = 11, AGI = 7, LUCK = 7, CHA = 9 });

        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Druid }, new HeroStats { STR = 4, DEX = 4, INT = 7, WIS = 8, AGI = 4, LUCK = 4, CHA = 4 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Druid }, new HeroStats { STR = 5, DEX = 5, INT = 8, WIS = 9, AGI = 5, LUCK = 4, CHA = 4 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Druid }, new HeroStats { STR = 5, DEX = 5, INT = 9, WIS = 10, AGI = 6, LUCK = 5, CHA = 5 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Druid }, new HeroStats { STR = 6, DEX = 6, INT = 9, WIS = 10, AGI = 6, LUCK = 6, CHA = 7 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Druid }, new HeroStats { STR = 6, DEX = 7, INT = 10, WIS = 11, AGI = 6, LUCK = 7, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Druid }, new HeroStats { STR = 6, DEX = 8, INT = 12, WIS = 12, AGI = 6, LUCK = 7, CHA = 9 }); 

        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Paladin }, new HeroStats { STR = 7, DEX = 4, INT = 4, WIS = 6, AGI = 4, LUCK = 4, CHA = 6 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Paladin }, new HeroStats { STR = 8, DEX = 5, INT = 4, WIS = 7, AGI = 5, LUCK = 4, CHA = 7 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Paladin }, new HeroStats { STR = 9, DEX = 5, INT = 5, WIS = 8, AGI = 5, LUCK = 5, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Paladin }, new HeroStats { STR = 9, DEX = 6, INT = 6, WIS = 8, AGI = 6, LUCK = 6, CHA = 9 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Paladin }, new HeroStats { STR = 10, DEX = 6, INT = 7, WIS = 9, AGI = 6, LUCK = 7, CHA = 10 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Paladin }, new HeroStats { STR = 11, DEX = 7, INT = 7, WIS = 10, AGI = 6, LUCK = 7, CHA = 12 });

        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Tinker }, new HeroStats { STR = 4, DEX = 7, INT = 7, WIS = 4, AGI = 5, LUCK = 4, CHA = 4 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Tinker }, new HeroStats { STR = 5, DEX = 8, INT = 8, WIS = 5, AGI = 5, LUCK = 4, CHA = 5 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Tinker }, new HeroStats { STR = 5, DEX = 8, INT = 9, WIS = 6, AGI = 5, LUCK = 5, CHA = 6 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Tinker }, new HeroStats { STR = 6, DEX = 9, INT = 9, WIS = 6, AGI = 7, LUCK = 6, CHA = 7 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Tinker }, new HeroStats { STR = 6, DEX = 10, INT = 10, WIS = 7, AGI = 7, LUCK = 7, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Tinker }, new HeroStats { STR = 6, DEX = 11, INT = 12, WIS = 7, AGI = 8, LUCK = 7, CHA = 9 });

        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Common, heroClass = HeroClass.Priest }, new HeroStats { STR = 3, DEX = 4, INT = 7, WIS = 8, AGI = 4, LUCK = 4, CHA = 5 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Uncommon, heroClass = HeroClass.Priest }, new HeroStats { STR = 3, DEX = 5, INT = 8, WIS = 9, AGI = 5, LUCK = 4, CHA = 6 });    
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Rare, heroClass = HeroClass.Priest }, new HeroStats { STR = 4, DEX = 6, INT = 9, WIS = 9, AGI = 6, LUCK = 4, CHA = 7 });    
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Epic, heroClass = HeroClass.Priest }, new HeroStats { STR = 5, DEX = 6, INT = 9, WIS = 10, AGI = 6, LUCK = 6, CHA = 8 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Legendary, heroClass = HeroClass.Priest }, new HeroStats { STR = 5, DEX = 6, INT = 10, WIS = 11, AGI = 6, LUCK = 8, CHA = 9 });
        heroStatsDict.Add(new HeroRarityClass { rarity = HeroRarity.Mythic, heroClass = HeroClass.Priest }, new HeroStats { STR = 5, DEX = 7, INT = 11, WIS = 12, AGI = 6, LUCK = 9, CHA = 10 }); 
    }
    [ContextMenu("Test")]
    public void Test(){
        CreateHeroStatsDict();

        //print out the total number of all the hero stats attributes for each level
        // for (int i = 0; i < 6; i++){
        //     foreach (KeyValuePair<HeroRarityClass, HeroStats> kvp in heroStatsDict){
        //         public int AddUpAllStats(HeroStats stats){
        //     return stats.STR + stats.DEX + stats.INT + stats.WIS + stats.AGI + stats.LUCK + stats.CHA;
        // }
        //         int amountOfStats = AddUpAllStats(kvp.Value);
        //         print(amountOfStats);

        //     }
        // }

    }

    public int GetStatsPerLevel (HeroRarity rarity)
    {
        return rarity switch
        {
            HeroRarity.Common => 35,
            HeroRarity.Uncommon => 40,
            HeroRarity.Rare => 45,
            HeroRarity.Epic => 50,
            HeroRarity.Legendary => 55,
            HeroRarity.Mythic => 60,
            _ => 0,
        };
    }
    public int GetBaseHonorGuard(HeroClass heroClass)
    {
        return heroClass switch
        {
            HeroClass.Rogue => 10,
            HeroClass.Warrior => 15,
            HeroClass.Priest => 7,
            HeroClass.Druid => 10,
            HeroClass.Paladin => 7,
            HeroClass.Tinker => 20,
            HeroClass.Mage => 10,
            _ => 0,
        };
    }
    public int GetMaxArmySize(HeroClass heroClass)
    {
        return heroClass switch
        {
            HeroClass.Rogue => 60,
            HeroClass.Warrior => 75,
            HeroClass.Priest => 45,
            HeroClass.Druid => 60,
            HeroClass.Paladin => 50,
            HeroClass.Tinker => 70,
            HeroClass.Mage => 50,
            _ => 0,
        };
    }
    public int GetTotalToGenerate(HeroRarity rarity)
    {
        return rarity switch
        {
            HeroRarity.Common => 518,
            HeroRarity.Uncommon => 280,
            HeroRarity.Rare => 126,
            HeroRarity.Epic => 77,
            HeroRarity.Legendary => 42,
            HeroRarity.Mythic => 7,
            _ => 0,
        };
    }
    public int GetShinyCount(HeroRarity rarity)
    {
        return rarity switch
        {
            HeroRarity.Common => 50,
            HeroRarity.Uncommon => 20,
            HeroRarity.Rare => 8,
            HeroRarity.Epic => 3,
            HeroRarity.Legendary => 1,
            HeroRarity.Mythic => 0,
            _ => 0,
        };
    }
    public void MapOwners()
    {

    }
    public HeroRarity GetHeroRarity(int id)
    {
        return id switch
        {
            < 500 => HeroRarity.Common,
            < 750 => HeroRarity.Uncommon,
            < 875 => HeroRarity.Rare,
            < 950 => HeroRarity.Epic,
            < 993 => HeroRarity.Legendary,
            < 1000 => HeroRarity.Mythic,
            _ => HeroRarity.Common,
        };
    }
    public HeroClass GetHeroClass(int id)
    {
        int heroClass = id % 7;
        return heroClass switch
        {
            0 => HeroClass.Rogue,
            1 => HeroClass.Warrior,
            2 => HeroClass.Mage,
            3 => HeroClass.Druid,
            4 => HeroClass.Paladin,
            5 => HeroClass.Tinker,
            6 => HeroClass.Priest,
            _ => HeroClass.Rogue,
        };
    }
    public HeroLocation GetHeroLocation(int id)
    {
        int heroClass = id % 5;
        return heroClass switch
        {
            0 => HeroLocation.Celestial_Cliffs,
            1 => HeroLocation.Ebisus_Bay,
            2 => HeroLocation.Verdant_Forest,
            3 => HeroLocation.Felisgarde,
            4 => HeroLocation.Highlands,
            _ => HeroLocation.Celestial_Cliffs,
        };
    }
    public HeroSkin GetHeroSkin(int id)
    {
        int heroClass = id % 3;
        return heroClass switch
        {
            0 => HeroSkin.Fair,
            1 => HeroSkin.Medium,
            2 => HeroSkin.Tan,
            _ => HeroSkin.Fair,
        };
    }
    public HeroHair GetHeroHair(int id)
    {
        int chance = id % 7;
        chance -= SeededRandom.Range(0, chance+1);

        return chance switch
        {
            0 => HeroHair.Blonde,
            1 => HeroHair.Blue,
            2 => HeroHair.Brown,
            3 => HeroHair.Electrified,
            4 => HeroHair.Green,
            5 => HeroHair.Hairbun,
            6 => HeroHair.Samurai,
            _ => HeroHair.Blonde,
        };
    }
    public HeroEyes GetHeroEyes(int id)
    {
        int chance = id % 6;
        chance -= SeededRandom.Range(0, chance+1);

        return chance switch
        {
            0 => HeroEyes.Bewildered,
            1 => HeroEyes.Canadian,
            2 => HeroEyes.Devout,
            3 => HeroEyes.Green,
            4 => HeroEyes.Raised_EyeBrows,
            5 => HeroEyes.Squint,
            _ => HeroEyes.Bewildered,
        };
    }
    public HeroMouth GetHeroMouth(int id)
    {
        int chance = id % 7;
        chance -= SeededRandom.Range(0, chance+1);

        return chance switch
        {
            0 => HeroMouth.Circle_Beard,
            1 => HeroMouth.Crazed,
            2 => HeroMouth.Goatee,
            3 => HeroMouth.Green_Beard,
            4 => HeroMouth.Open,
            5 => HeroMouth.Smile,
            6 => HeroMouth.Smug,
            _ => HeroMouth.Circle_Beard,
        };
    }
    public HeroClothes GetHeroClothes(int id)
    {
        int chance = id % 6;
        return chance switch
        {
            0 => HeroClothes.Normal,
            1 => HeroClothes.Normal,
            2 => HeroClothes.Normal,
            3 => HeroClothes.Normal,
            4 => HeroClothes.Light,
            5 => HeroClothes.Dark,
            _ => HeroClothes.Normal,
        };
    }
    public List<Hero> DistributeShinies(List<Hero> heroes)
    {
        int shinies = 0;
        List<Hero> localHeroes = new ();
        List<HeroRarity> heroRarities = new ();
        heroRarities.AddRange(new HeroRarity[] { HeroRarity.Common, HeroRarity.Uncommon, HeroRarity.Rare, HeroRarity.Epic, HeroRarity.Legendary, HeroRarity.Mythic });
        foreach(HeroRarity heroRarity in heroRarities){
            shinies = GetShinyCount(heroRarity);
            localHeroes = heroes.FindAll(hero => hero.rarity == heroRarity.ToString());
            localHeroes.Shuffle();
            for (int i = 0; i < shinies; i++) {
                // Debug.Log($"Hero {localHeroes[i].id} is shiny");
                heroes[localHeroes[i].id].isShiny = true;
            }
        }
        return heroes;
    }
    public List<Hero> AddVariance(List<Hero> heroes)
    {
        for(int i = 0; i < heroes.Count; i++){
            //modify 2-4 stats on each hero, by 1
            int statsToModify = SeededRandom.Range(0,3);
            if(statsToModify == 0){
                continue;
            } else if(statsToModify == 1){
                statsToModify = 2;
            } else if(statsToModify == 2){
                statsToModify = 4;
            } else {
                Debug.Log($"broke");
            }

            while(statsToModify>0){
                int statId = SeededRandom.Range(0,7);
                //up on even, down on odd
                heroes[i].stats.ModifyStat(statId, statsToModify % 2 == 0 ? 1 : -1);
                statsToModify--;
            }
        }

        return heroes;
    }
    
    [ContextMenu("Generate Heroes")]
    void GenerateHeroes()
    {
        SeededRandom.Init(0);
        CreateHeroStatsDict();

        SerializedHeroes serializedHeroes = new(){
            heroes = new List<Hero>()
        };
        
        //generate heroes
        int heroesToGenerate = 1050;
        for(int i = 0; i < heroesToGenerate; i++){
            HeroRarity hr = GetHeroRarity(i);
            HeroClass hc = GetHeroClass(i);
            HeroLocation hl = GetHeroLocation(i);
            HeroSkin hs = GetHeroSkin(i);
            HeroHair hh = GetHeroHair(i);
            HeroEyes he = GetHeroEyes(i);
            HeroMouth hm = GetHeroMouth(i);
            HeroClothes hcl = GetHeroClothes(i);
            HeroStats stats = heroStatsDict[new HeroRarityClass { rarity = hr, heroClass = hc }];
            bool isShiny = false;
            serializedHeroes.heroes.Add(new Hero(i, hr, hc, stats, hl, hs, hh, he, hm, hcl, isShiny));
        }

        serializedHeroes.heroes = DistributeShinies(serializedHeroes.heroes);
        serializedHeroes.heroes = AddVariance(serializedHeroes.heroes);

        string json = JsonUtility.ToJson(serializedHeroes, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/heroes.json", json);

        SaveAsMetadata(serializedHeroes);
        MapDistribution(serializedHeroes);
    }
    private void SaveAsMetadata(SerializedHeroes serializedHeroes)
    {
        MetadataList finalMetadata = new(){
            metadata = new List<Metadata>()
        };

        Hero hero;
        for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
            hero = serializedHeroes.heroes[i];
            List<Trait> traits = new()
            {
                new Trait("Class", hero.heroClass, "string"),
                new Trait("Rarity", hero.rarity, "string"),
                new Trait("Location", hero.location, "string"),
                new Trait("STR", hero.stats.STR.ToString(), "string"),
                new Trait("DEX", hero.stats.DEX.ToString(), "string"),
                new Trait("INT", hero.stats.INT.ToString(), "string"),
                new Trait("WIS", hero.stats.WIS.ToString(), "string"),
                new Trait("AGI", hero.stats.AGI.ToString(), "string"),
                new Trait("LUCK", hero.stats.LUCK.ToString(), "string"),
                new Trait("CHA", hero.stats.CHA.ToString(), "string"),
                new Trait("Skin", hero.skin, "string"),
                new Trait("Hair", hero.hair, "string"),
                new Trait("Eyes", hero.eyes, "string"),
                new Trait("Mouth", hero.mouth, "string"),
                new Trait("Clothes", hero.clothes, "string"),
            };

            if(hero.isShiny)
                traits.Add(new Trait("Shiny", "true", "boolean"));

            finalMetadata.metadata.Add(new Metadata(
                $"https://app.ebisusbay.com/heros/{hero.id+1}",
                "Ryoshi Heroes #"+(hero.id+1).ToString(),
                "Heroes play a crucial role in defining the overall gameplay experience. Each class brings a distinct play style, appearance, and a set of statistics and profession synergies that enhance the gameplay",
                (hero.id+1).ToString(),
                traits.ToArray()
            ));
            // finalMetadata.finalMetadata[i].attributes = traits;
        }

        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"rarity {serializedHeroes.heroes[i].rarity}");
        // }
        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"skin {serializedHeroes.heroes[i].skin}");
        // }
        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"hair {serializedHeroes.heroes[i].hair}");
        // }
        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"eyes {serializedHeroes.heroes[i].eyes}");
        // }
        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"mouth {serializedHeroes.heroes[i].mouth}");
        // }
        // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
        //     Debug.Log($"clothes {serializedHeroes.heroes[i].clothes}");
        // }

        string metaDataJson = JsonUtility.ToJson(finalMetadata, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/HeroesMetadata.json", metaDataJson);
        // CreateIndFiles(finalMetadata);
        Debug.Log($"saved metadata");

    }
    private void MapDistribution(SerializedHeroes serializedHeroes)
    {
        // 706 total free ones
        // 14 - team
        // 280 - dutch auction

        //common
        // - 374 Naptime Ninja
        // - 84 Hakuri
        // - 35 Takeishi
        // - 34 Mint

        //uncommon
        //- 84 Hakuri
        // - 35 Takeishi
        // - 131 Mint

        //rare
        // - 84 Hakuri
        // - 41 Mint

        //Epic
        // - 35 Takeishi
        // - 40 Mint

        //Legendary
        // 43 - all Mint

        //mything
        // 2 - bladestorm
        // 5 mint

        // List<string> idsForNaptimeNinja;
        // List<string> idsForHakuri;
        // List<string> idsForTakeishi;
        // List<string> idsForBladestorm;
        // List<string> idsForMint;
        // List<string> idsLeftOver;
        List<Hero> idsForNaptimeNinja;
        List<Hero> idsForHakuri;
        List<Hero> idsForTakeishi;
        List<Hero> idsForBladestorm;
        List<Hero> idsForMint;
        List<Hero> idsForTeam;

        //get all common ids
        // List<string> CommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Common.ToString()).Select(hero => hero.id.ToString()).ToList();
        // List<string> UncommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Uncommon.ToString()).Select(hero => hero.id.ToString()).ToList();
        // List<string> RareIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Rare.ToString()).Select(hero => hero.id.ToString()).ToList();
        // List<string> EpicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Epic.ToString()).Select(hero => hero.id.ToString()).ToList();
        // List<string> LegendaryIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Legendary.ToString()).Select(hero => hero.id.ToString()).ToList();
        // List<string> MythicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Mythic.ToString()).Select(hero => hero.id.ToString()).ToList();

        List<Hero> CommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Common.ToString());
        List<Hero> UncommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Uncommon.ToString());
        List<Hero> RareIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Rare.ToString());
        List<Hero> EpicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Epic.ToString());
        List<Hero> LegendaryIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Legendary.ToString());
        List<Hero> MythicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Mythic.ToString());
        
        //shuffle all
        // CommonIds.Shuffle();
        // UncommonIds.Shuffle();
        // RareIds.Shuffle();
        // EpicIds.Shuffle();
        // LegendaryIds.Shuffle();
        // MythicIds.Shuffle();

        //add 
        idsForNaptimeNinja = CommonIds.Take(374).ToList();
        CommonIds.RemoveRange(0, 374);
        idsForHakuri = CommonIds.Take(84).ToList();
        CommonIds.RemoveRange(0, 84);
        idsForTakeishi = CommonIds.Take(35).ToList();
        CommonIds.RemoveRange(0, 35);
        idsForTeam = CommonIds.Take(4).ToList();
        CommonIds.RemoveRange(0, 4);
        idsForMint = CommonIds.Take(CommonIds.Count).ToList();
        CommonIds.RemoveRange(0, CommonIds.Count);

        idsForHakuri.AddRange(UncommonIds.Take(84).ToList());
        UncommonIds.RemoveRange(0, 84);
        idsForTakeishi.AddRange(UncommonIds.Take(35).ToList());
        UncommonIds.RemoveRange(0, 35);
        idsForMint.AddRange(UncommonIds.Take(UncommonIds.Count).ToList());
        UncommonIds.RemoveRange(0, UncommonIds.Count);

        idsForHakuri.AddRange(RareIds.Take(84).ToList());
        RareIds.RemoveRange(0, 84);
        idsForMint.AddRange(RareIds.Take(RareIds.Count).ToList());
        RareIds.RemoveRange(0, RareIds.Count);

        idsForTakeishi.AddRange(EpicIds.Take(35).ToList());
        EpicIds.RemoveRange(0, 35);
        idsForMint.AddRange(EpicIds.Take(EpicIds.Count).ToList());
        EpicIds.RemoveRange(0, EpicIds.Count);

        idsForTeam.AddRange(LegendaryIds.Take(10).ToList());
        LegendaryIds.RemoveRange(0, 10);
        idsForMint.AddRange(LegendaryIds.Take(LegendaryIds.Count).ToList());
        LegendaryIds.RemoveRange(0, LegendaryIds.Count);

        idsForBladestorm = MythicIds.Take(2).ToList();
        MythicIds.RemoveRange(0, 2);
        idsForMint.AddRange(MythicIds.Take(5).ToList());
        MythicIds.RemoveRange(0, 5);

        // idsForTeam = CommonIds;
        // idsForTeam.AddRange(UncommonIds);
        // idsForTeam.AddRange(RareIds);
        // idsForTeam.AddRange(EpicIds);
        // idsForTeam.AddRange(LegendaryIds);
        // idsForTeam.AddRange(MythicIds);

        Debug.Log($"Total: {serializedHeroes.heroes.Count}");
        Debug.Log($"Naptime Ninja: {idsForNaptimeNinja.Count}");
        Debug.Log($"Hakuri: {idsForHakuri.Count}");
        Debug.Log($"Takeishi: {idsForTakeishi.Count}");
        Debug.Log($"Bladestorm: {idsForBladestorm.Count}");
        Debug.Log($"Mint: {idsForMint.Count}");
        Debug.Log($"Ids for team: {idsForTeam.Count}");

        SerializedHeroes serializedHeroes2 = new(){
            heroes = new List<Hero>()
        };
        serializedHeroes2.heroes.AddRange(idsForNaptimeNinja);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        serializedHeroes2.heroes.AddRange(idsForHakuri);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        serializedHeroes2.heroes.AddRange(idsForTakeishi);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        serializedHeroes2.heroes.AddRange(idsForBladestorm);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        serializedHeroes2.heroes.AddRange(idsForTeam);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        serializedHeroes2.heroes.AddRange(idsForMint);
        Debug.Log($"length of naptime ninja: {serializedHeroes2.heroes.Count}");
        

        MetadataList finalMetadata = new(){
            metadata = new List<Metadata>()
        };

        // serialize all lists and save to a json
        // SerializedHeroIds ids = new(){
        //     naptimeNinja = idsForNaptimeNinja,
        //     hakuri = idsForHakuri,
        //     takeishi = idsForTakeishi,
        //     bladestorm = idsForBladestorm,
        //     mint = idsForMint,
        //     leftover = idsLeftOver
        // };
        Hero hero;

        for (int i = 0; i < serializedHeroes2.heroes.Count; i++) {
            hero = serializedHeroes2.heroes[i];
            List<Trait> traits = new()
            {
                new Trait("Class", hero.heroClass, "string"),
                new Trait("Rarity", hero.rarity, "string"),
                new Trait("Location", hero.location, "string"),

                // new Trait("STR", hero.stats.STR.ToString(), "string"),
                // new Trait("DEX", hero.stats.DEX.ToString(), "string"),
                // new Trait("INT", hero.stats.INT.ToString(), "string"),
                // new Trait("WIS", hero.stats.WIS.ToString(), "string"),
                // new Trait("AGI", hero.stats.AGI.ToString(), "string"),
                // new Trait("LUCK", hero.stats.LUCK.ToString(), "string"),
                // new Trait("CHA", hero.stats.CHA.ToString(), "string"),
                new Trait("Stats", hero.stats.ToString(), "string"),
                // new Trait("Genetics")

                new Trait("Skin", hero.skin, "string"),
                new Trait("Hair", hero.hair, "string"),
                new Trait("Eyes", hero.eyes, "string"),
                new Trait("Mouth", hero.mouth, "string"),
                new Trait("Clothes", hero.clothes, "string"),
            };

            if(hero.isShiny)
                traits.Add(new Trait("Shiny", "true", "boolean"));

            finalMetadata.metadata.Add(new Metadata(
                $"https://app.ebisusbay.com/heros/{i+1}",
                "Ryoshi Heroes #"+(i+1).ToString(),
                "Heroes play a crucial role in defining the overall gameplay experience. Each class brings a distinct play style, appearance, and a set of statistics and profession synergies that enhance the gameplay",
                (i+1).ToString(),
                traits.ToArray()
            ));
            // finalMetadata.finalMetadata[i].attributes = traits;
        }

        string json = JsonUtility.ToJson(finalMetadata, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/ditribution_heroes.json", json);
    }
    [System.Serializable] public struct SerializedHeroIds {
        public List<string> naptimeNinja;
        public List<string> hakuri;
        public List<string> takeishi;
        public List<string> bladestorm;
        public List<string> mint;
        public List<string> leftover;
    }
    [System.Serializable] public struct MetadataList {
        public List<Metadata> metadata;
    }
    [System.Serializable] public struct Metadata {
        public string image;
        public string name;
        public string description;
        public string id;
        public Trait[] attributes;
        public Metadata(string image, string name, string description, string id, Trait[] attributes) {
            this.image = image;
            this.name = name;
            this.description = description;
            this.id = id;
            this.attributes = attributes;
        }
    }
}
