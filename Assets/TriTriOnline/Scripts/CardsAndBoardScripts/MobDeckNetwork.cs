using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobDeckNetwork : MonoBehaviour
{
    private int deckCardInitialCount = 30;
    //the index of the current card or technically next card to be drawn (so if we haven't drawn our current/next card index will be 0, then 1 if we draw, 2 ,3, 4 and so on
    private int currentDeckCardIndex = 0;
    // Start is called before the first frame update
    private string[] mobNames = new string[] { "Cerberus of the black flame", "Core Kraken", "Headless Chicken", "PutridPuddle", "Murkblood Hydra", "Seaweed Hermit", "Conspiring Sorceror", "Wandering Knight", "OtherRealm Tammy" };
    private int[,] cerberusStats = new int[,] { {/*Cerberus of the Black Flame */ 6, 6, 9, 6, 16 }, {/*Core Kraken */ 7, 7, 4, 8, 17 }, {/*Headless Chicken */ 3, 3, 9, 5, 12 }, {/*Putrid Puddle */ 5, 5, 2, 4, 11 },
        {/*murkblood hydra */ 7, 7, 7, 4, 18 }, {/*Seaweed Hermit */ 4, 4, 6, 2, 14 }, {/*Conspiring Sorceror */ 6, 6, 3, 5, 12 }, {/*Wandering Knight */ 6, 5, 4, 3, 14 }, {/*OtherRealm Tammy */ 6, 7, 5, 9, 15 }};
    [SerializeField]private List<Sprite> mobSprites = new List<Sprite>();
    void Start()
    {
        CreateDeck();
    }

    private void CreateDeck()
    {

    }

    
}
