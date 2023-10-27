using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TJ.Utilities;
using System.Xml.Serialization;
using System.Linq;
public class HeroMetadataGeneration : MonoBehaviour
{
    [System.Serializable] public enum HeroRarity { Common, Uncommon, Rare, Epic, Legendary, Mythic };
    [System.Serializable] public enum HeroClass { Rogue, Warrior, Mage, Druid, Paladin, Tinker, Priest}
    [System.Serializable] public enum HeroLocation { Celestial_Cliffs, Ebisus_Bay, Verdant_Forest, Felisgarde, Highlands}
    [System.Serializable] public enum HeroSkin { Fair, Medium, Tan}
    [System.Serializable] public enum HeroHair {Blonde, Blue, Brown, White, Green, Black}
    [System.Serializable] public enum HeroEyes{Blue, Brown, Gray, Green, Teal, Black};
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
    // [System.Serializable] public class RichStats{
    //     public int STR;
    //     public int DEX;
    //     public int INT;
    //     public int WIS;
    //     public int AGI;
    //     public int LUCK;
    //     public int CHA;
    //     public RichStats(HeroStats stats){
    //         STR = stats.STR;
    //         DEX = stats.DEX;
    //         INT = stats.INT;
    //         WIS = stats.WIS;
    //         AGI = stats.AGI;
    //         LUCK = stats.LUCK;
    //         CHA = stats.CHA;
    //     }
    // }
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
    [System.Serializable] public struct MetadataList {
        public List<Metadata> Hero;
    }
    [System.Serializable] public struct PowerTraitList {
        public PowerTrait Class;
        public LocationTrait Location;
        public RarityTrait RarityTrait;
        public SkinTrait Skin;
    }
    [System.Serializable] public struct StatsMetadataList {
        public List<StatsMetadata> Stats;
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
    [System.Serializable] public struct StatsMetadata {
        public string id;
        // public RichStats stats;
        public TraitNumber[] stats;
        public StatsMetadata(int id, TraitNumber[] stats) {
            this.id = id.ToString();
            this.stats = stats;
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
    [System.Serializable] public struct TraitNumber {
        public string trait_type;
        public int value;
        public string display_type;
        public TraitNumber(string trait_type, int value, string displayType) {
            this.trait_type = trait_type;
            this.value = value;
            this.display_type = displayType;
        }
    }
    [System.Serializable] public struct CountAndOccurance{
        public int count;
        public float occurance;
        public CountAndOccurance(int count){
            this.count = count;
            this.occurance = ((float)count/1050f);
        }
    }

    [System.Serializable] public struct PowerTrait
    {
        public CountAndOccurance Rogue;
        public CountAndOccurance Warrior;
        public CountAndOccurance Mage;
        public CountAndOccurance Druid;
        public CountAndOccurance Paladin;
        public CountAndOccurance Tinker;
        public CountAndOccurance Priest;
        public PowerTrait(CountAndOccurance Rogue, CountAndOccurance Warrior, CountAndOccurance Mage, CountAndOccurance Druid, CountAndOccurance Paladin, CountAndOccurance Tinker, CountAndOccurance Priest){
            this.Rogue = Rogue;
            this.Warrior = Warrior;
            this.Mage = Mage;
            this.Druid = Druid;
            this.Paladin = Paladin;
            this.Tinker = Tinker;
            this.Priest = Priest;
        }
    }
    [System.Serializable] public struct LocationTrait
    {
        public CountAndOccurance Celestial_Cliffs;
        public CountAndOccurance Ebisus_Bay;
        public CountAndOccurance Verdant_Forest;
        public CountAndOccurance Felisgarde;
        public CountAndOccurance Highlands;
        public LocationTrait(CountAndOccurance Celestial_Cliffs, CountAndOccurance Ebisus_Bay, CountAndOccurance Verdant_Forest, CountAndOccurance Felisgarde, CountAndOccurance Highlands){
            this.Celestial_Cliffs = Celestial_Cliffs;
            this.Ebisus_Bay = Ebisus_Bay;
            this.Verdant_Forest = Verdant_Forest;
            this.Felisgarde = Felisgarde;
            this.Highlands = Highlands;
        }
    }
    [System.Serializable] public struct RarityTrait
    {
        public CountAndOccurance Common;
        public CountAndOccurance Uncommon;
        public CountAndOccurance Rare;
        public CountAndOccurance Epic;
        public CountAndOccurance Legendary;
        public CountAndOccurance Mythic;
        public RarityTrait(CountAndOccurance Common, CountAndOccurance Uncommon, CountAndOccurance Rare, CountAndOccurance Epic, CountAndOccurance Legendary, CountAndOccurance Mythic){
            this.Common = Common;
            this.Uncommon = Uncommon;
            this.Rare = Rare;
            this.Epic = Epic;
            this.Legendary = Legendary;
            this.Mythic = Mythic;
        }
    }
    [System.Serializable] public struct SkinTrait
    {
        public CountAndOccurance Fair;
        public CountAndOccurance Medium;
        public CountAndOccurance Tan;
        public SkinTrait(CountAndOccurance Fair, CountAndOccurance Medium, CountAndOccurance Tan){
            this.Fair = Fair;
            this.Medium = Medium;
            this.Tan = Tan;
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

    private static readonly string[] prefixes = { "Aki", "Haru", "Jun", "Rin", "Yuki", "Taka", "Fumi", "Yori", "Masa", "Katsu", "Sora", "Hana", "Kai", "Nori", "Ryo", "Saku", "Rei", "Ren", "Aya", "Kei" };
    private static readonly string[] stems = { "moto", "yama", "hashi", "sawa", "zaki", "gawa", "naga", "bana", "saki", "kawa", "tani", "nashi", "tora", "ishi", "shira", "kaze", "hoshi", "neko", "sora", "hito" };
    // private static readonly string[] suffixes = { "son", "smith", "forge", "field", "vale", "ridge", "shire", "caster", "weld", "worth", "brook", "stone", "wood", "ridge", "hall", "bank", "brook", "dale", "hurst", "wood" };

    public static string GenerateName()
    {
        string prefix = prefixes[SeededRandom.Range(0, prefixes.Length)];
        string stem = stems[SeededRandom.Range(0, stems.Length)];
        // string suffix = suffixes[SeededRandom.Range(0, suffixes.Length)];

        return prefix + stem + "";
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
        int chance = id % 6;
        chance -= SeededRandom.Range(0, chance+1);

        return chance switch
        {
            0 => HeroHair.Blonde,
            1 => HeroHair.Blue,
            2 => HeroHair.Brown,
            3 => HeroHair.White,
            4 => HeroHair.Green,
            5 => HeroHair.Black,
            _ => HeroHair.Blonde,
        };
    }
    public HeroEyes GetHeroEyes(int id)
    {
        int chance = id % 6;
        chance -= SeededRandom.Range(0, chance+1);

        return chance switch
        {
            0 => HeroEyes.Blue,
            1 => HeroEyes.Brown,
            2 => HeroEyes.Gray,
            3 => HeroEyes.Green,
            4 => HeroEyes.Teal,
            5 => HeroEyes.Black,
            _ => HeroEyes.Blue,
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

            // RichStats richStats = new (stats);
            
            bool isShiny = false;
            serializedHeroes.heroes.Add(new Hero(i, hr, hc, stats, hl, hs, hh, he, hm, hcl, isShiny));
        }

        serializedHeroes.heroes = DistributeShinies(serializedHeroes.heroes);
        serializedHeroes.heroes = AddVariance(serializedHeroes.heroes);

        string json = JsonUtility.ToJson(serializedHeroes, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/heroes.json", json);

        // SaveAsMetadata(serializedHeroes);
        serializedHeroes = MapDistribution(serializedHeroes);

        // CreatePowerStats(serializedHeroes);

    }
    private void CreatePowerStats(SerializedHeroes serializedHeroes){
        //create a list of all the stats
        PowerTraitList powerTraitList = new(){
            Class = new PowerTrait()
        };

        int totalHeroClassPaladin = 0;
        int totalHeroClassDruid = 0;
        int totalHeroClassPriest = 0;
        int totalHeroClassTinker = 0;
        int totalHeroClassMage = 0;
        int totalHeroClassWarrior = 0;
        int totalHeroClassRogue = 0;
        int totalHeroLocationCelestialCliffs = 0;
        int totalHeroLocationEbisusBay = 0;
        int totalHeroLocationVerdantForest = 0;
        int totalHeroLocationFelisgarde = 0;
        int totalHeroLocationHighlands = 0;
        int totalHeroSkinFair = 0;
        int totalHeroSkinMedium = 0;
        int totalHeroSkinTan = 0;
        int totalHeroRarityCommon = 0;
        int totalHeroRarityUncommon = 0;
        int totalHeroRarityRare = 0;
        int totalHeroRarityEpic = 0;
        int totalHeroRarityLegendary = 0;
        int totalHeroRarityMythic = 0;


        Hero hero;
        for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
            hero = serializedHeroes.heroes[i];
            totalHeroClassRogue += hero.heroClass == "Rogue" ? 1 : 0;
            totalHeroClassWarrior += hero.heroClass == "Warrior" ? 1 : 0;
            totalHeroClassMage += hero.heroClass == "Mage" ? 1 : 0;
            totalHeroClassTinker += hero.heroClass == "Tinker" ? 1 : 0;
            totalHeroClassPriest += hero.heroClass == "Priest" ? 1 : 0;
            totalHeroClassDruid += hero.heroClass == "Druid" ? 1 : 0;
            totalHeroClassPaladin += hero.heroClass == "Paladin" ? 1 : 0;
            totalHeroLocationCelestialCliffs += hero.location == "Celestial_Cliffs" ? 1 : 0;
            totalHeroLocationEbisusBay += hero.location == "Ebisus_Bay" ? 1 : 0;
            totalHeroLocationVerdantForest += hero.location == "Verdant_Forest" ? 1 : 0;
            totalHeroLocationFelisgarde += hero.location == "Felisgarde" ? 1 : 0;
            totalHeroLocationHighlands += hero.location == "Highlands" ? 1 : 0;
            totalHeroSkinFair += hero.skin == "Fair" ? 1 : 0;
            totalHeroSkinMedium += hero.skin == "Medium" ? 1 : 0;
            totalHeroSkinTan += hero.skin == "Tan" ? 1 : 0;
            totalHeroRarityCommon += hero.rarity == "Common" ? 1 : 0;
            totalHeroRarityUncommon += hero.rarity == "Uncommon" ? 1 : 0;
            totalHeroRarityRare += hero.rarity == "Rare" ? 1 : 0;
            totalHeroRarityEpic += hero.rarity == "Epic" ? 1 : 0;
            totalHeroRarityLegendary += hero.rarity == "Legendary" ? 1 : 0;
            totalHeroRarityMythic += hero.rarity == "Mythic" ? 1 : 0;
        }
        powerTraitList = new(){
            Class = new PowerTrait(),
            Location = new LocationTrait(),
            RarityTrait = new RarityTrait(),
            Skin = new SkinTrait(),

        };
        powerTraitList.Class = new PowerTrait{ 
            Rogue = new CountAndOccurance(totalHeroClassRogue),
            Warrior = new CountAndOccurance(totalHeroClassWarrior),
            Mage = new CountAndOccurance(totalHeroClassMage),
            Tinker = new CountAndOccurance(totalHeroClassTinker),
            Priest = new CountAndOccurance(totalHeroClassPriest),
            Druid = new CountAndOccurance(totalHeroClassDruid),
            Paladin = new CountAndOccurance(totalHeroClassPaladin),
        };
        powerTraitList.Location = new LocationTrait{ 
            Celestial_Cliffs = new CountAndOccurance(totalHeroLocationCelestialCliffs),
            Ebisus_Bay = new CountAndOccurance(totalHeroLocationEbisusBay),
            Verdant_Forest = new CountAndOccurance(totalHeroLocationVerdantForest),
            Felisgarde = new CountAndOccurance(totalHeroLocationFelisgarde),
            Highlands = new CountAndOccurance(totalHeroLocationHighlands),
        };
        powerTraitList.RarityTrait = new RarityTrait{ 
            Common = new CountAndOccurance(518),
            Uncommon = new CountAndOccurance(280),
            Rare = new CountAndOccurance(126),
            Epic = new CountAndOccurance(77),
            Legendary = new CountAndOccurance(42),
            Mythic = new CountAndOccurance(7),
        };
        powerTraitList.Skin = new SkinTrait{ 
            Fair = new CountAndOccurance(totalHeroSkinFair),
            Medium = new CountAndOccurance(totalHeroSkinMedium),
            Tan = new CountAndOccurance(totalHeroSkinTan),
        };
            
        // powerTraitList.metadata.Add( new PowerTrait{ "totalHeroClassPaladin", 
        // new StatRow{thing = new CountAndOccurance(totalHeroClassPaladin, totalHeroClassPaladin)}});
        

        JSONFileHandler.SaveToJSON<PowerTraitList>(powerTraitList, "/Data/PoissonSampling/powerStats.json", true);
        // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/powerStats.json", json);
    }
    
    // private void SaveAsMetadata(SerializedHeroes serializedHeroes)
    // {
    //     MetadataList finalMetadata = new(){
    //         metadata = new List<Metadata>()
    //     };

    //     Hero hero;
    //     for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //         hero = serializedHeroes.heroes[i];
    //         List<Trait> traits = new()
    //         {
    //             new Trait("Class", hero.heroClass, "string"),
    //             new Trait("Rarity", hero.rarity, "string"),
    //             new Trait("Location", hero.location, "string"),
                
    //             new Trait("Skin", hero.skin, "string"),
    //             new Trait("Hair", hero.hair, "string"),
    //             new Trait("Eyes", hero.eyes, "string"),
    //             new Trait("Mouth", hero.mouth, "string"),
    //             new Trait("Clothes", hero.clothes, "string"),

    //             // new Trait("STR", hero.stats.STR.ToString(), "string"),
    //             // new Trait("DEX", hero.stats.DEX.ToString(), "string"),
    //             // new Trait("INT", hero.stats.INT.ToString(), "string"),
    //             // new Trait("WIS", hero.stats.WIS.ToString(), "string"),
    //             // new Trait("AGI", hero.stats.AGI.ToString(), "string"),
    //             // new Trait("LUCK", hero.stats.LUCK.ToString(), "string"),
    //             // new Trait("CHA", hero.stats.CHA.ToString(), "string"),

    //         };
    //         // RichStats rs = new RichStats(hero.stats);
    //         // traits.Add(new Trait("Stats", "deleteME", "number", hero.stats));


    //         if(hero.isShiny)
    //             traits.Add(new Trait("Shiny", "true", "boolean"));

    //         finalMetadata.metadata.Add(new Metadata(
    //             $"https://app.ebisusbay.com/heros/{hero.id+1}",
    //             "Ryoshi Heroes #"+(hero.id+1).ToString(),
    //             "Heroes play a crucial role in defining the overall gameplay experience. Each class brings a distinct play style, appearance, and a set of statistics and profession synergies that enhance the gameplay",
    //             (hero.id+1).ToString(),
    //             traits.ToArray()
    //         ));
    //         // finalMetadata.finalMetadata[i].attributes = traits;
    //     }

    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"rarity {serializedHeroes.heroes[i].rarity}");
    //     // }
    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"skin {serializedHeroes.heroes[i].skin}");
    //     // }
    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"hair {serializedHeroes.heroes[i].hair}");
    //     // }
    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"eyes {serializedHeroes.heroes[i].eyes}");
    //     // }
    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"mouth {serializedHeroes.heroes[i].mouth}");
    //     // }
    //     // for (int i = 0; i < serializedHeroes.heroes.Count; i++) {
    //     //     Debug.Log($"clothes {serializedHeroes.heroes[i].clothes}");
    //     // }

    //     string metaDataJson = JsonUtility.ToJson(finalMetadata, true);
    //     // System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/HeroesMetadata.json", metaDataJson);
    //     // CreateIndFiles(finalMetadata);
    //     Debug.Log($"saved metadata");

    // }
    private SerializedHeroes MapDistribution(SerializedHeroes serializedHeroes)
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

        List<Hero> idsForNaptimeNinja;
        List<Hero> idsForHakuri = new List<Hero>();
        List<Hero> idsForTakeishi = new List<Hero>();
        List<Hero> idsForBladestorm;
        List<Hero> idsForMint;
        List<Hero> idsForTeam;

        List<Hero> CommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Common.ToString());
        List<Hero> UncommonIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Uncommon.ToString());
        List<Hero> RareIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Rare.ToString());
        List<Hero> EpicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Epic.ToString());
        List<Hero> LegendaryIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Legendary.ToString());
        List<Hero> MythicIds = serializedHeroes.heroes.FindAll(hero => hero.rarity == HeroRarity.Mythic.ToString());

        UncommonIds.Shuffle();

        //add 
        idsForNaptimeNinja = CommonIds.Take(374).ToList();
        CommonIds.RemoveRange(0, 374);
        
        for(int i = 0; i < 84; i++){
            idsForHakuri.Add(CommonIds[i]);
            idsForHakuri.Add(UncommonIds[i]);
            idsForHakuri.Add(RareIds[i]);
        }
        CommonIds.RemoveRange(0, 84);
        UncommonIds.RemoveRange(0, 84);
        RareIds.RemoveRange(0, 84);

        for(int i = 0; i < 35; i++){
            idsForTakeishi.Add(CommonIds[i]);
            idsForTakeishi.Add(UncommonIds[i]);
            idsForTakeishi.Add(EpicIds[i]);
        }
        CommonIds.RemoveRange(0, 35);
        UncommonIds.RemoveRange(0, 35);
        EpicIds.RemoveRange(0, 35);

        idsForTeam = CommonIds.Take(4).ToList();
        CommonIds.RemoveRange(0, 4);
        idsForTeam.AddRange(LegendaryIds.Take(10).ToList());
        LegendaryIds.RemoveRange(0, 10);
        
        idsForBladestorm = MythicIds.Take(2).ToList();
        MythicIds.RemoveRange(0, 2);

        idsForMint = CommonIds.Take(CommonIds.Count).ToList();
        CommonIds.RemoveRange(0, CommonIds.Count);
        idsForMint.AddRange(UncommonIds.Take(UncommonIds.Count).ToList());
        UncommonIds.RemoveRange(0, UncommonIds.Count);
        idsForMint.AddRange(RareIds.Take(RareIds.Count).ToList());
        RareIds.RemoveRange(0, RareIds.Count);
        idsForMint.AddRange(EpicIds.Take(EpicIds.Count).ToList());
        EpicIds.RemoveRange(0, EpicIds.Count);
        idsForMint.AddRange(LegendaryIds.Take(LegendaryIds.Count).ToList());
        LegendaryIds.RemoveRange(0, LegendaryIds.Count);
        idsForMint.AddRange(MythicIds.Take(5).ToList());
        MythicIds.RemoveRange(0, 5);

        idsForMint.Shuffle();

        SerializedHeroes serializedHeroes2 = new(){
            heroes = new List<Hero>()
        };
        MetadataList heroJson = new(){
            Hero = new List<Metadata>()
        };
        StatsMetadataList statsJson = new(){
            Stats = new List<StatsMetadata>()
        };

        serializedHeroes2.heroes.AddRange(idsForNaptimeNinja);
        serializedHeroes2.heroes.AddRange(idsForHakuri);
        serializedHeroes2.heroes.AddRange(idsForTakeishi);
        serializedHeroes2.heroes.AddRange(idsForBladestorm);
        serializedHeroes2.heroes.AddRange(idsForTeam);
        serializedHeroes2.heroes.AddRange(idsForMint);

        Hero hero;

        for (int i = 0; i < serializedHeroes2.heroes.Count; i++) {
            hero = serializedHeroes2.heroes[i];
            List<Trait> traits = new()
            {
                new Trait("Class", hero.heroClass, "string"),
                new Trait("Rarity", hero.rarity, "string"),
                new Trait("Location", hero.location, "string"),
                new Trait("Skin", hero.skin, "string"),
                new Trait("Hair", hero.hair, "string"),
                new Trait("Eyes", hero.eyes, "string"),
                new Trait("Mouth", hero.mouth, "string"),
                new Trait("Clothes", hero.clothes, "string"),
            };

            if(hero.isShiny)
                traits.Add(new Trait("Shiny", "true", "boolean"));

            heroJson.Hero.Add(new Metadata(
                $"https://cdn-prod.ebisusbay.com/files/ryoshi/images/heroes/unrevealed.png",
                GenerateName(),
                "Heroes play a crucial role in defining the overall gameplay experience. Each class brings a distinct play style, appearance, and a set of statistics and profession synergies that enhance the gameplay",
                (i+1).ToString(),
                traits.ToArray()
            ));

            List<TraitNumber> traitNumbers = new()
            {
                new TraitNumber("STR", hero.stats.STR, "number"),
                new TraitNumber("DEX", hero.stats.DEX, "number"),
                new TraitNumber("INT", hero.stats.INT, "number"),
                new TraitNumber("WIS", hero.stats.WIS, "number"),
                new TraitNumber("AGI", hero.stats.AGI, "number"),
                new TraitNumber("LUCK", hero.stats.LUCK, "number"),
                new TraitNumber("CHA", hero.stats.CHA, "number"),
            };

            statsJson.Stats.Add(new StatsMetadata(
                i+1,
                traitNumbers.ToArray()
            ));
        }

        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/Heroes.json", JsonUtility.ToJson(heroJson, true));
        System.IO.File.WriteAllText(Application.dataPath + "/Data/PoissonSampling/HeroStats.json", JsonUtility.ToJson(statsJson, true));

        return serializedHeroes2;
    }
    // [System.Serializable] public struct SerializedHeroIds {
    //     public List<string> naptimeNinja;
    //     public List<string> hakuri;
    //     public List<string> takeishi;
    //     public List<string> bladestorm;
    //     public List<string> mint;
    //     public List<string> leftover;
    // }

}
