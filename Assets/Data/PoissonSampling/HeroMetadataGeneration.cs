using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMetadataGeneration : MonoBehaviour
{
    public enum HeroRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic };
    public enum HeroClass { Rogue, Warrior, Mage, Druid, Paladin, Tinker, Priest}
    public struct HeroStats {
        public int STR;
        public int DEX;
        public int INT;
        public int WIS;
        public int AGI;
        public int LUCK;
        public int CHA;
    }
    public struct HeroRarityClass {
        public HeroRarity rarity;
        public HeroClass heroClass;
    }

    public Dictionary<HeroRarityClass, HeroStats> heroStatsDict = new ();

    public void CreateHeroStatsDict(){
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
        heroStatsDict.Clear();
        CreateHeroStatsDict();

        //print out the total number of all the hero stats attributes for each level
        // for (int i = 0; i < 6; i++){
        //     foreach (KeyValuePair<HeroRarityClass, HeroStats> kvp in heroStatsDict){
        //         int amountOfStats = AddUpAllStats(kvp.Value);
        //         print(amountOfStats);

        //     }
        // }

    }
    public int AddUpAllStats(HeroStats stats){
        return stats.STR + stats.DEX + stats.INT + stats.WIS + stats.AGI + stats.LUCK + stats.CHA;
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

    public void GenerateMetaData(){

    }
}
