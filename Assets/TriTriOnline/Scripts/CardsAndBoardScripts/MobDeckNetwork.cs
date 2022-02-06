
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class MobDeckNetwork : MonoBehaviour
{
    public TextAsset mobJsonData;
    [System.Serializable]
    public class MobData
    {
        public string[] Names;
        public string[] Lineages;
        public string[] Abilities;
        public string[] AbilityValues;
        
    }

    

        private int deckCardInitialCount = 30;
    //the index of the current card or technically next card to be drawn (so if we haven't drawn our current/next card index will be 0, then 1 if we draw, 2 ,3, 4 and so on
    private int currentDeckCardIndex = 0;
    private int typesOfMobs = 9;
    // Start is called before the first frame update
    //try to keep mobs first and magic stuff after
    private string[] mobNames = new string[] { /*"Cerberus of the black flame", "Core Kraken", "Headless Chicken", "PutridPuddle", "Murkblood Hydra", "Seaweed Hermit", "Conspiring Sorceror", "Wandering Knight", "OtherRealm Tammy",*/
        /*start ability cards here*/ "Dark Orb", "Flame Ball", "Holy", "Aqua Blast", "Tempest Whirl", "Rock Slide", "Temptation" };

    private string[] abilityNames = new string[] { };

    private int[,] mobStats = new int[,] { {/*Cerberus of the Black Flame */ 6, 6, 9, 6, 16 }, {/*Core Kraken */ 7, 7, 4, 8, 17 }, {/*Headless Chicken */ 3, 3, 9, 5, 2 }, {/*Putrid Puddle */ 5, 5, 2, 4, 11 },
        {/*murkblood hydra */ 7, 7, 7, 4, 18 }, {/*Seaweed Hermit */ 4, 4, 6, 2, 4 }, {/*Conspiring Sorceror */ 6, 6, 3, 5, 2 }, {/*Wandering Knight */ 6, 5, 4, 3, 4 }, {/*OtherRealm Tammy */ 6, 7, 5, 9, 15 },
        /*start ability cards here*/{/*Dark Orb*/ 3,3,3,3,4}, {/*Flame Ball*/ 3,3,3,3,4 }, {/*Holy*/ 3,3,3,3,4 }, {/*Aqua Blast*/ 3,3,3,3,4 }, {/*Tempest Whirl*/ 3,3,3,3,4 }, {/*Rock Slide*/ 3,3,3,3,4 }, {/*Temptation*/ 3,3,3,3,4 } };

    private bool[] isMob = new bool[] { true, true, true, true, true, true, true, true, true,
        /*start ability cards here*/false, false, false, false, false, false, false };
    //if 0, we don't have an ability, otw we will use the number to reference functons
    private int[] abilityIndexes = new int[] {0,0,0,0,0,0,0,0,0,
    /*start ability cards here*/ 1,2,3,4,5,6,7};
    private int[] abilityRankMods = new int[] {6,6,3,3,3,0,3,0,6,
    /*start ability cards here*/ 1,1,1,1,1,1,3};
    private int[] deckCardMobIndexReferences = new int[30];
    //Lineages include Giant, Celestial, Construct, beast, humanoid, ooze, Aberration, Spectre, Monstrosity, Demon,Elemental, Plant, Dragon
    private string[] mobLineages = new string[] {"beast", "giant", "Beast", "Ooze", "Dragon", "Beast", "Humanoid", "Humanoid", "Humanoid",
    /*start ability cards here*/ "Spectre", "Elemental", "Celestial", "Elemental", "Elemental", "Elemental", "Demon"};
    //will include sprites for ability cards to keep things simple, mobs first then abilities
    [SerializeField]private List<Sprite> mobSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attackValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> hpValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attributeSprites = new List<Sprite>();
    void Start()
    {
        MobData mobDataList = new MobData();
        mobDataList = JsonUtility.FromJson<MobData>(mobJsonData.text);
        //handle names
        string[] mobNamesTemp = mobDataList.Names;
        string[] spellNamesTemp = new string[] { "Dark Orb", "Flame Ball", "Holy", "Aqua Blast", "Tempest Whirl", "Rock Slide", "Temptation" };
        mobNamesTemp = mobNamesTemp.Concat(spellNamesTemp).ToArray();
        mobNames = mobNamesTemp;
        //foreach(var e in mobNames) { Debug.Log(e); }
        //handle abilityNames
        mobDataList = JsonUtility.FromJson<MobData>(mobJsonData.text);
        //handle ability names
        string[] abilityNamesTemp = mobDataList.Abilities;
        string[] spellAbilityNamesTemp = new string[] { "Dark Orb", "Flame Ball", "Holy", "Aqua Blast", "Tempest Whirl", "Rock Slide", "Temptation" };
        abilityNamesTemp = abilityNamesTemp.Concat(spellAbilityNamesTemp).ToArray();
        abilityNames = abilityNamesTemp;
        //foreach (var e in abilityNames) { Debug.Log(e); }
        //handle mobLineages
        string[] mobLineagesTemp = mobDataList.Lineages;
        string[] spellLineagesTemp = new string[] { "spectre", "elemental", "celestial", "elemental", "elemental", "elemental", "demon" };
        mobLineagesTemp = mobLineagesTemp.Concat(spellLineagesTemp).ToArray();
        mobLineages = mobLineagesTemp;
        //foreach (var e in mobLineages) { Debug.Log(e); }
        //handle abilityRankMods
        string[] abilityRanksTemp = mobDataList.AbilityValues;
        string[] abilityRanksSpellsTemp = new string[] { "3", "3", "3", "3", "3", "3", "3" };
        abilityRanksTemp = abilityRanksTemp.Concat(abilityRanksSpellsTemp).ToArray();
        List<int> tempAbilityRanksList = new List<int>();
        foreach(var e in abilityRanksTemp)
        {
            int temp = int.Parse(e);
            tempAbilityRanksList.Add(temp);
        }
        abilityRankMods = tempAbilityRanksList.ToArray();
        //foreach (var e in abilityRankMods) { Debug.Log(e); }
        //handle for loop for abilityIndexes, isMob bool
        List<int> tempAbilityIndexes = new List<int>();
        List<bool> tempIsMobBools = new List<bool>();
        for(int i = 0; i < 553; i++)
        {
            if(i < 546)
            {
                tempAbilityIndexes.Add(0);
                tempIsMobBools.Add(true);
            }
            else
            {
                tempAbilityIndexes.Add(i - 545);
                tempIsMobBools.Add(false);
            }
        }
        abilityIndexes = tempAbilityIndexes.ToArray();
        isMob = tempIsMobBools.ToArray();
        //foreach(var e in isMob) { Debug.Log(e); }
        //handle stat assignment
        ArrayList stats = new();
        int[,] stats2DArray = new int[553,5];
        int leftTemp;
        int rightTemp;
        int topTemp;
        int bottomTemp;
        int hpTemp;
        int hpMod;
        int rank;
        int totalStats;//12 base points for rank 1 and add 3 each rank up so 15,18,21,24,27,30
        for(int i = 0; i < abilityRankMods.Length; i++)
        {
            rank = abilityRankMods[i];
            leftTemp = Random.Range(rank - 1, 9);
            rightTemp = Random.Range(rank - 1, 9);
            topTemp = Random.Range(rank - 1, 9);
            bottomTemp = Random.Range(rank - 1, 9);
            totalStats = leftTemp + rightTemp + bottomTemp + topTemp;
            while(totalStats > 9 + 3 * rank)
            {
                int targetValueIndex = Random.Range(0, 3);
                switch (targetValueIndex){
                    case 0:
                        if(leftTemp > 0) { leftTemp -= 1; totalStats -= 1; }
                        break;
                    case 1:
                        if (rightTemp > 0) { rightTemp -= 1; totalStats -= 1; }
                        break;
                    case 2:
                        if (topTemp > 0) { topTemp -= 1; totalStats -= 1; }
                        break;
                    case 3:
                        if (bottomTemp > 0) { bottomTemp -= 1; totalStats -= 1; }
                        break;
                }
            }
            hpMod = 8 + rank;
            hpTemp = Random.Range(hpMod,hpMod+rank);
            //assign to 2Darray
            stats2DArray[i, 0] = leftTemp;
            stats2DArray[i, 1] = rightTemp;
            stats2DArray[i, 2] = topTemp;
            stats2DArray[i, 3] = bottomTemp;
            stats2DArray[i, 4] = hpTemp;
            //Debug.Log(stats2DArray[i, 0] + " " + stats2DArray[i, 1] + " " + stats2DArray[i, 2] + " " + stats2DArray[i, 3] + " " + stats2DArray[i, 4]);
        }
        mobStats = stats2DArray;
        //handle setting up all the sprites
        string targetDir;
        for (int i = 0; i < mobNames.Length; i++) 
        {
            targetDir = "ApiPulledSprites/" + mobNames[i] ;
            if (i >= 546) { targetDir = "ElementIcons/" + mobNames[i]; }
            mobSprites[i] = Resources.Load<Sprite>(targetDir);
            //Debug.Log(mobSprites[i]);
        }
        //Debug.Log(mobStats[552, 0] + " " + mobStats[552, 1] + " " + mobStats[552, 2] + " " + mobStats[552, 3] + " " + mobStats[552, 4]);
        CreateDeck();
    }

    //creates a random deck of cards 
    private void CreateDeck()
    {
        for(int i = 0; i < deckCardInitialCount; i++)
        {
            //get a reference index for the card to be used to create it on draw later;
            //deckCardMobIndexReferences[i] = 9;//just for testing to make sure the ability cards are functioning properly
           
            deckCardMobIndexReferences[i] = i<20 ? Random.Range(0, mobNames.Length) : Random.Range(546,mobNames.Length);
            //Debug.Log(deckCardMobIndexReferences[i]);
        }
        RandomizeArray(ref deckCardMobIndexReferences, 150);
        //Debug.Log("randomized below");
        //foreach(int e in deckCardMobIndexReferences) { Debug.Log(e); }
        //Debug.Log("we created our deck so to speak and the length is: " + deckCardMobIndexReferences.Length);
    }

    private void RandomizeArray(ref int[] targetList, int loopsTotal)
    {
        int randIndex;
        int randIndex2;
        int tempVal;
        int tempVal2;
        for(int i = 0; i < loopsTotal; i++)
        {
            randIndex = Random.Range(0, 29);
            tempVal = targetList[randIndex];
            randIndex2 = Random.Range(0, 29);
            tempVal2 = targetList[randIndex2];
            targetList[randIndex2] = tempVal;
            targetList[randIndex] = tempVal2;
        }
    }

    public Sprite getSprite(int mobSpriteindex)
    {
        Sprite tempSprite = mobSprites[mobSpriteindex];
        return tempSprite;
    }

    public void setUpCardOnDraw(ref Sprite mobSprite, ref int[] mobStatsList, ref int mobSpritesListSpriteIndex, ref Sprite[] attackAndHpValueSprites, ref Sprite attributeSprite,
        ref int attributeSpriteIndex, ref bool isAMob, ref int abilityIndex, ref int abilityRankMod, ref string mobName, ref string abilityName)
    {

        //get the mob index from our decks mob Index List, since everything lines up, it is usable for finding the mob in questions index for anything
        int mobListIndex = deckCardMobIndexReferences[currentDeckCardIndex];
        mobSpritesListSpriteIndex = mobListIndex;//this should get us the index that we want for grabbing the appropriate sprite later
        isAMob = isMob[mobListIndex];//grabs our isMob bool at the index of the mob being set up
        abilityIndex = abilityIndexes[mobListIndex];//set up the index for our ability
        abilityRankMod = abilityRankMods[mobListIndex];
        mobName = mobNames[mobListIndex];
        abilityName = abilityNames[mobListIndex];
        //increment currentDeckcard
        currentDeckCardIndex += 1;
        //get our stats for the mob
        int[] MobsStats = { mobStats[mobListIndex, 0], mobStats[mobListIndex, 1], mobStats[mobListIndex, 2], mobStats[mobListIndex, 3], mobStats[mobListIndex, 4] };//probably really bad but it should work
        mobStatsList = MobsStats;
        //get the value sprites in order of leftstat,rightstat,topstat, bottomstat, 
        Sprite[] atkAndHpValSprites = { attackValueSprites[mobStatsList[0]], attackValueSprites[mobStatsList[1]], attackValueSprites[mobStatsList[2]], attackValueSprites[mobStatsList[3]],null,null };
        int hpVal = mobStatsList[4];
        if(hpVal < 10)
        {
            atkAndHpValSprites[4] = null;
            atkAndHpValSprites[5] = hpValueSprites[hpVal];
        }
        else if(hpVal < 20)
        {
            atkAndHpValSprites[4] = hpValueSprites[1];
            atkAndHpValSprites[5] = hpValueSprites[hpVal-10];

        }
        else
        {
            atkAndHpValSprites[4] = hpValueSprites[2];
            atkAndHpValSprites[5] = hpValueSprites[hpVal-20];
        }

        //apply to the referenced parameter
        attackAndHpValueSprites = atkAndHpValSprites;
        //find the index of the sprite we need to pull when rendering the lineage sprite
        attributeSpriteIndex = DetermineLineage(mobLineages[mobListIndex]);
        //Debug.Log("the first attributeSpriteIndex is: " + attributeSpriteIndex);
        //just going to default to zero index for now
        attributeSprite = attributeSprites[attributeSpriteIndex];
        mobSprite = mobSprites[mobListIndex];
    }
    //Giant, Celestial, Construct, beast, humanoid, ooze, Aberration, Spectre, Monstrosity, Demon,Elemental, Plant, Dragon
    private int DetermineLineage(string lineage)
    {
        switch (lineage)
        {
            case "aberration":
                return 0;
                
            case "beast":
                return 1;
                
            case "celestial":
                return 2;
                
            case "construct":
                return 3;
                
            case "demon":
                return 4;
                
            case "dragon":
                return 5;
                
            case "elemental":
                return 6;
                
            case "giant":
                return 7;
                
            case "humanoid":
                return 8;
                
            case "monstrosity":
                return 9;
               
            case "ooze":
                return 10;
                
            case "plant":
                return 11;
                
            case "spectre":
                return 12;
            default:
                Debug.Log("We didn't find the lineage, the lineage passed was " + lineage);
                return -1;//should never happen
        }
        
    }

    
}
