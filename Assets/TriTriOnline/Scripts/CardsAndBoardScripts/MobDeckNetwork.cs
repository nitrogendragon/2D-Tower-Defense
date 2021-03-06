using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobDeckNetwork : MonoBehaviour
{
    private int deckCardInitialCount = 30;
    //the index of the current card or technically next card to be drawn (so if we haven't drawn our current/next card index will be 0, then 1 if we draw, 2 ,3, 4 and so on
    private int currentDeckCardIndex = 0;
    private int typesOfMobs = 9;
    // Start is called before the first frame update
    //try to keep mobs first and magic stuff after
    private string[] mobNames = new string[] { "Cerberus of the black flame", "Core Kraken", "Headless Chicken", "PutridPuddle", "Murkblood Hydra", "Seaweed Hermit", "Conspiring Sorceror", "Wandering Knight", "OtherRealm Tammy",
        /*start ability cards here*/ "Dark Orb", "Flame Ball", "Holy", "Aqua Blast", "Tempest Whirl", "Rock Slide" };

    private int[,] mobStats = new int[,] { {/*Cerberus of the Black Flame */ 6, 6, 9, 6, 16 }, {/*Core Kraken */ 7, 7, 4, 8, 17 }, {/*Headless Chicken */ 3, 3, 9, 5, 2 }, {/*Putrid Puddle */ 5, 5, 2, 4, 11 },
        {/*murkblood hydra */ 7, 7, 7, 4, 18 }, {/*Seaweed Hermit */ 4, 4, 6, 2, 4 }, {/*Conspiring Sorceror */ 6, 6, 3, 5, 2 }, {/*Wandering Knight */ 6, 5, 4, 3, 4 }, {/*OtherRealm Tammy */ 6, 7, 5, 9, 15 },
        /*start ability cards here*/{/*Dark Orb*/ 3,3,3,3,4}, {/*Flame Ball*/ 3,3,3,3,4 }, {/*Holy*/ 3,3,3,3,4 }, {/*Aqua Blast*/ 3,3,3,3,4 }, {/*Tempest Whirl*/ 3,3,3,3,4 }, {/*Rock Slide*/ 3,3,3,3,4 } };

    private bool[] isMob = new bool[] { true, true, true, true, true, true, true, true, true,
        /*start ability cards here*/false, false, false, false, false, false };
    //if 0, we don't have an ability, otw we will use the number to reference functons
    private int[] abilityIndexes = new int[] {0,0,0,0,0,0,0,0,0,
    /*start ability cards here*/ 1,2,3,4,5,6};
    private int[] abilityRankMods = new int[] {6,6,3,3,3,0,3,0,6,
    /*start ability cards here*/ 1,3,3,6,6,3};
    private int[] deckCardMobIndexReferences = new int[30];
    //Lineages include Giant, Celestial, Construct, beast, humanoid, ooze, Aberration, Spectre, Monstrosity, Demon,Elemental, Plant, Dragon
    private string[] mobLineages = new string[] {"Beast", "Giant", "Beast", "Ooze", "Dragon", "Beast", "Humanoid", "Humanoid", "Humanoid",
    /*start ability cards here*/ "Spectre", "Elemental", "Celestial", "Elemental", "Elemental", "Elemental"};
    //will include sprites for ability cards to keep things simple, mobs first then abilities
    [SerializeField]private List<Sprite> mobSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attackValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> hpValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attributeSprites = new List<Sprite>();
    void Start()
    {
        CreateDeck();
    }

    //creates a random deck of cards 
    private void CreateDeck()
    {
        for(int i = 0; i < deckCardInitialCount; i++)
        {
            //get a reference index for the card to be used to create it on draw later;
            //deckCardMobIndexReferences[i] = 9;//just for testing to make sure the ability cards are functioning properly
            deckCardMobIndexReferences[i] = Random.Range(0, mobNames.Length);
        }
        //Debug.Log("we created our deck so to speak and the length is: " + deckCardMobIndexReferences.Length);
    }

    public Sprite getSprite(int mobSpriteindex)
    {
        Sprite tempSprite = mobSprites[mobSpriteindex];
        return tempSprite;
    }

    public void setUpCardOnDraw(ref Sprite mobSprite, ref int[] mobStatsList, ref int mobSpritesListSpriteIndex, ref Sprite[] attackAndHpValueSprites, ref Sprite attributeSprite,
        ref int attributeSpriteIndex, ref bool isAMob, ref int abilityIndex, ref int abilityRankMod)
    {

        //get the mob index from our decks mob Index List, since everything lines up, it is usable for finding the mob in questions index for anything
        int mobListIndex = deckCardMobIndexReferences[currentDeckCardIndex];
        mobSpritesListSpriteIndex = mobListIndex;//this should get us the index that we want for grabbing the appropriate sprite later
        isAMob = isMob[mobListIndex];//grabs our isMob bool at the index of the mob being set up
        abilityIndex = abilityIndexes[mobListIndex];//set up the index for our ability
        abilityRankMod = abilityRankMods[mobListIndex];
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
            case "Aberration":
                return 0;
                
            case "Beast":
                return 1;
                
            case "Celestial":
                return 2;
                
            case "Construct":
                return 3;
                
            case "Demon":
                return 4;
                
            case "Dragon":
                return 5;
                
            case "Elemental":
                return 6;
                
            case "Giant":
                return 7;
                
            case "Humanoid":
                return 8;
                
            case "Monstrosity":
                return 9;
               
            case "Ooze":
                return 10;
                
            case "Plant":
                return 11;
                
            case "Spectre":
                return 12;
            default:
                return -1;//should never happen
        }
        
    }

    
}
