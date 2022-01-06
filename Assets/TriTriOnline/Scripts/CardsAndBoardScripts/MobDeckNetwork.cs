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
    private string[] mobNames = new string[] { "Cerberus of the black flame", "Core Kraken", "Headless Chicken", "PutridPuddle", "Murkblood Hydra", "Seaweed Hermit", "Conspiring Sorceror", "Wandering Knight", "OtherRealm Tammy" };
    private int[,] mobStats = new int[,] { {/*Cerberus of the Black Flame */ 6, 6, 9, 6, 16 }, {/*Core Kraken */ 7, 7, 4, 8, 17 }, {/*Headless Chicken */ 3, 3, 9, 5, 12 }, {/*Putrid Puddle */ 5, 5, 2, 4, 11 },
        {/*murkblood hydra */ 7, 7, 7, 4, 18 }, {/*Seaweed Hermit */ 4, 4, 6, 2, 14 }, {/*Conspiring Sorceror */ 6, 6, 3, 5, 12 }, {/*Wandering Knight */ 6, 5, 4, 3, 14 }, {/*OtherRealm Tammy */ 6, 7, 5, 9, 15 }};
    private int[] deckCardMobIndexReferences = new int[30];
    [SerializeField]private List<Sprite> mobSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attackValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> hpValueSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> attributeSprites = new List<Sprite>();
    void Start()
    {
        CreateDeck();
    }

    private void CreateDeck()
    {
        for(int i = 0; i < deckCardInitialCount; i++)
        {
            //get a reference index for the card to be used to create it on draw later;
            deckCardMobIndexReferences[i] = Random.Range(0, 8);
        }
        Debug.Log("we created our deck so to speak and the length is: " + deckCardMobIndexReferences.Length);
    }

    public Sprite getSprite(int mobSpriteindex)
    {
        Sprite tempSprite = mobSprites[mobSpriteindex];
        return tempSprite;
    }

    public void setUpCardOnDraw(ref Sprite mobSprite, ref int[] mobStatsList, ref int mobSpritesListSpriteIndex, ref Sprite[] attackAndHpValueSprites, ref Sprite attributeSprite, ref int attributeSpriteIndex)
    {

        //get the mob index from our decks mob Index List
        int mobListIndex = deckCardMobIndexReferences[currentDeckCardIndex];
        mobSpritesListSpriteIndex = mobListIndex;//this should get us the index that we want for grabbing the appropriate sprite later
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
        //just going to default to zero index for now
        attributeSprite = attributeSprites[0];
        //for now it will be zero
        attributeSpriteIndex = 0;
        mobSprite = mobSprites[mobListIndex];
    }

    
}
