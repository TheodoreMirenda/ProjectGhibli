using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TJ.Utilities;
using System.Xml.Serialization;

public class HeroMetadataGeneration : MonoBehaviour
{
    [System.Serializable] public enum HeroRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic };
    [System.Serializable] public enum HeroClass { Rogue, Warrior, Mage, Druid, Paladin, Tinker, Priest}
    public struct HeroRarityClass { public HeroRarity rarity; public HeroClass heroClass; }
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
        public bool isShiny;  
        public Hero (int id, HeroRarity rarity, HeroClass heroClass, HeroStats stats, bool isShiny) {
            this.id = id;
            this.rarity = rarity.ToString();
            this.heroClass = heroClass.ToString();
            this.stats = stats;
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

    [System.Serializable] public struct FinalMetadata {
        public string image;
        public string name;
        public string description;
        public string id;
        public Trait[] attributes;
        public FinalMetadata(string image, string name, string description, string id, Trait[] attributes) {
            this.image = image;
            this.name = name;
            this.description = description;
            this.id = id;
            this.attributes = attributes;
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
            HeroRarity.Common => 500,
            HeroRarity.Uncommon => 250,
            HeroRarity.Rare => 125,
            HeroRarity.Epic => 75,
            HeroRarity.Legendary => 43,
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
        // use modulo to get the hero class
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
                Debug.Log($"wtf");
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
        int heroesToGenerate = 1000;
        for(int i = 0; i < heroesToGenerate; i++){
            HeroRarity hr = GetHeroRarity(i);
            HeroClass hc = GetHeroClass(i);
            HeroStats hs = heroStatsDict[new HeroRarityClass { rarity = hr, heroClass = hc }];
            bool isShiny = false;
            serializedHeroes.heroes.Add(new Hero(i, hr, hc, hs, isShiny));
            // Debug.Log($"hr {hr} hc {hc} hs {hs} isShiny {isShiny}");
        }

        serializedHeroes.heroes = DistributeShinies(serializedHeroes.heroes);
        serializedHeroes.heroes = AddVariance(serializedHeroes.heroes);

        string json = JsonUtility.ToJson(serializedHeroes, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/heroes.json", json);
    }
}
