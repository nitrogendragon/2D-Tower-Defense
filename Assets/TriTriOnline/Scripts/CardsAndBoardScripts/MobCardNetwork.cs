using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MobCardNetwork : NetworkBehaviour
{
    public bool isInHand = true;

    //all the impoortant stats for the mob to display and use for interactions
    private int topStat, bottomStat, leftStat, rightStat, curTopStat, curBottomStat, curLeftStat, curRightStat, leftStatBuff, rightStatBuff, topStatBuff, bottomStatBuff,
        leftStatDebuff, rightStatDebuff, topStatDebuff, bottomStatDebuff, curHitPoints, hitPoints, abilityIndex, abilityRankMod;
    private Color player1mobBackgroundColor = new Color(.6f, .3f, .3f);
    private Color player2mobBackgroundColor = new Color(.3f, .3f, .6f);
    [SerializeField]private GameObject mobSpriteRenderer;
    [SerializeField]private GameObject topStatSprite,bottomStatSprite,leftStatSprite, rightStatSprite, hpTensSprite, hpOnesSprite, attributeSprite;//technically renderers but for some reason i named them as just xSprite
    [SerializeField]private GameObject mobBackground;
    [SerializeField] private GameObject damagePopUp;
    [SerializeField]private List<GameObject> statusIcons = new List<GameObject>();
    private NetworkVariable<Color> mobBackgroundColor = new NetworkVariable<Color>(new Color(.4f,0,0));//default is player 1 color
    private NetworkVariable<int> playerOwnerIndex = new NetworkVariable<int>(0);//0 for no ownership by default
    private NetworkVariable<int> mobSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> topStatSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> leftStatSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> rightStatSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> bottomStatSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> hpTensSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> hpOnesSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> attributeSpriteIndex = new NetworkVariable<int>();//won't initialize to start on this one
    private NetworkVariable<int> cardPlacementBoardIndex = new NetworkVariable<int>();//the index of the board tile in the boardTiles list that we placed the card on
    private NetworkVariable<bool> isMob = new NetworkVariable<bool>();//whether the card is a mob or an ability card
    //we will have 7 types of status effects in total, poison, burn, corrosion, regen, weakened, buffed and charmed.
    private bool[] myStatusEffectBools = new bool[] { false /*poison*/, false /*burn*/, false /*corrosion*/, false /*regen*/, false /*weakened*/, false /*buffed*/, false/*charmed*/ };
    private int[] myStatusEffectTurnsRemaining = new int[] { 0, 0, 0, 0, 0, 0, 0 };
    private int[] myStatusEffectRanks = new int[] { 0, 0, 0, 0, 0, 0, 0 };
    //note that multiple abilities can cause poison potentially so even if the ability has an ability index of 15 and another 4, the status effect index will be the same for all abilities that inflict poison, currently don't have animation specifically
    //for status effects though so these are temps
    private int[] myStatusEffectAbilityAnimationIndex = new int[] { 8 /*poison*/, 9 /*burn*/, 10 /*corrosion*/, 11 /*regen*/, 12 /*weakened*/, 13 /*buffed*/, 14 /*charmed*/};
    //all the ability indexes that cause poison
    private int[] poisonAbilityIndexes = new int[] {1,8 };
    //all the ability indexes that cause burn
    private int[] burnAbilityIndexes = new int[] {2,9 };
    //all the ability indexes that cause corrosion
    private int[] corrosionAbilityIndexes = new int[] {6,10 };
    //all the ability indexes that cause regen
    private int[] regenAbilityIndexes = new int[] {3,11 };
    //all the ability indexes that cause weakened
    private int[] weakenedAbilityIndexes = new int[] {4,12 };
    //all the ability indexes that cause buffed
    private int[] buffedAbilityIndexes = new int[] {5,13 };
    //all the ability indexes that cause charm
    private int[] charmAbilityIndexes = new int[] {7,14 };
    //the messages for logging purposes for now to use when a status effect is inflicted
    private string[] myStatusEffectMessages = new string[] { "the ability was a poison ability and we now have it." , "the ability was a burn ability and we now have it." , "the ability was a corrosion ability and we now have it." ,
        "the ability was a regen ability and we now have it.","the ability was a weakened ability and we now have it.","the ability was a buffed ability and we now have it.","the ability was a charmed ability and we now have it." };
    public SpritesLists spritesReferenceHolder;
    public CardBoardIndexManager cardBoardIndexManager;

    private void Start()
    {
       
    }
    private void OnEnable()
    {
        playerOwnerIndex.OnValueChanged += OnPlayerOwnershipAndColorChanged;
        mobSpriteIndex.OnValueChanged += OnMobSpriteIndexChanged;
        topStatSpriteIndex.OnValueChanged += OnTopStatSpriteIndexChanged;
        bottomStatSpriteIndex.OnValueChanged += OnBottomStatSpriteIndexChanged;
        leftStatSpriteIndex.OnValueChanged += OnLeftStatSpriteIndexChanged;
        rightStatSpriteIndex.OnValueChanged += OnRightStatSpriteIndexChanged;
        hpTensSpriteIndex.OnValueChanged += OnHpTensSpriteIndexChanged;
        hpOnesSpriteIndex.OnValueChanged += OnHpOnesSpriteIndexChanged;
        attributeSpriteIndex.OnValueChanged += OnAttributeSpriteIndexChanged;
        cardPlacementBoardIndex.OnValueChanged += OnCardPlacementBoardIndexChanged;
        isMob.OnValueChanged += OnIsMobChanged;
    }

    private void OnDisable()
    {
        playerOwnerIndex.OnValueChanged -= OnPlayerOwnershipAndColorChanged;
        mobSpriteIndex.OnValueChanged -= OnMobSpriteIndexChanged;
        topStatSpriteIndex.OnValueChanged -= OnTopStatSpriteIndexChanged;
        bottomStatSpriteIndex.OnValueChanged -= OnBottomStatSpriteIndexChanged;
        leftStatSpriteIndex.OnValueChanged -= OnLeftStatSpriteIndexChanged;
        rightStatSpriteIndex.OnValueChanged -= OnRightStatSpriteIndexChanged;
        hpTensSpriteIndex.OnValueChanged -= OnHpTensSpriteIndexChanged;
        hpOnesSpriteIndex.OnValueChanged -= OnHpOnesSpriteIndexChanged;
        attributeSpriteIndex.OnValueChanged -= OnAttributeSpriteIndexChanged;
        cardPlacementBoardIndex.OnValueChanged -= OnCardPlacementBoardIndexChanged;
        isMob.OnValueChanged -= OnIsMobChanged;

    }

    //don't feel a need for two functions, the way it works makes more sense to me to do it like this.
    private void OnPlayerOwnershipAndColorChanged(int oldPlayerOwnerIndex, int newPlayerOwnerIndex)
    {
        if (!IsClient) { return; }//testing these parameters
        //Debug.Log("the player owners index: " + newPlayerOwnerIndex);
        if(newPlayerOwnerIndex == 1)
        {
            mobBackground.GetComponent<SpriteRenderer>().color = player1mobBackgroundColor;
            return;
        }
        mobBackground.GetComponent<SpriteRenderer>().color = player2mobBackgroundColor;

    }

    private void OnMobSpriteIndexChanged(int oldIndex,int newIndex)
    {
        if (!IsClient) { return; }
        //mobSpriteRenderer.GetComponent<SpriteRenderer>().sprite = mobSprites[newIndex];//old version
        mobSpriteRenderer.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetMobSprite(newIndex);

    }

    private void OnTopStatSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        topStatSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetAttackValueSprite(newIndex);
    }

    private void OnBottomStatSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        bottomStatSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetAttackValueSprite(newIndex);
    }
    private void OnLeftStatSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        leftStatSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetAttackValueSprite(newIndex);
    }
    private void OnRightStatSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        rightStatSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetAttackValueSprite(newIndex);
    }
    private void OnHpTensSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        hpTensSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetHpValueSprite(newIndex);
    }
    private void OnHpOnesSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        hpOnesSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetHpValueSprite(newIndex);
    }
    private void OnAttributeSpriteIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        attributeSprite.GetComponent<SpriteRenderer>().sprite = spritesReferenceHolder.GetAttributeSprite(newIndex);
    }

    private void OnCardPlacementBoardIndexChanged(int oldIndex, int newIndex)
    {
        if (!IsClient) { return; }
        //assigns this networkObject Card to the board index newIndex. so we can find and compare with other cards later
        cardBoardIndexManager.SetCardIndex(this.NetworkObject, newIndex);
        //Debug.Log("we set the card in the board index manager");
        //we want to attack when placed on the field
       
    }

    private void OnIsMobChanged(bool oldBool, bool newBool)
    {
        //if not a mob change tag to be a mob
        if (!newBool) { this.tag = "AbilityCard"; }
        //Idk what to do here atm or if anything should be done
    }



    [ServerRpc(RequireOwnership = false)]
    public void DestroyNetworkObjectServerRpc()
    {
        if (!IsServer) { return; }
        //one option is to despawn the network object, stays on server but disappears for the clients
        //GetComponent<NetworkObject>().Despawn();
        //as long as we do this on the server it will update for everyone
        Destroy(gameObject);
    }

    //could work well for changing colors /owners when a unit dies for example and switches teams
    [ServerRpc(RequireOwnership = false)]
    public void ChangePlayerOwnerAndColorServerRpc()
    {
        //only server can do the work to be done
        if (!IsServer) { return; }
        
        //switch to player 2 and change color to player 2 color
        if(playerOwnerIndex.Value == 1)
        {
            playerOwnerIndex.Value = 2;
            mobBackgroundColor.Value = player2mobBackgroundColor;
            return;
        }
        //otw we go to player 1 and change color to player 1 color
        playerOwnerIndex.Value = 1;
        mobBackgroundColor.Value = player1mobBackgroundColor;
       
    }
    //will be ran when the card is drawn/instantiated
    [ServerRpc]
    public void CreateMobCardServerRpc(int initLeftStat, int initRightStat, int initTopStat, int initBottomStat, int initHitPoints, int playerOwnrIndex, bool initIsMob, int mobSpriteIndexReference,
        int attributeSpriteIndexReference, int cBoardIndex, int initAbilityIndex, int initAbilityRankMod )
    {
        if (!IsServer) { return; }
        
        
        //handle stat setup and other card setup
        topStat = initTopStat;
        bottomStat = initBottomStat;
        rightStat = initRightStat;
        leftStat = initLeftStat;
        hitPoints = initHitPoints;
        curTopStat = topStat;
        curBottomStat = bottomStat;
        curLeftStat = leftStat;
        curRightStat = rightStat;
        curHitPoints = hitPoints;
        abilityIndex = initAbilityIndex;
        abilityRankMod = initAbilityRankMod;
        cardPlacementBoardIndex.Value = cBoardIndex;
        playerOwnerIndex.Value = playerOwnrIndex;
        mobSpriteIndex.Value = mobSpriteIndexReference;
        //new stuff added that needs onChange listeners and functions
        attributeSpriteIndex.Value = attributeSpriteIndexReference;
        topStatSpriteIndex.Value = initTopStat;
        bottomStatSpriteIndex.Value = initBottomStat;
        leftStatSpriteIndex.Value = initLeftStat;
        rightStatSpriteIndex.Value = initRightStat;
        isMob.Value = initIsMob;
        if (!initIsMob) { this.tag = "AbilityCard"; }
        //Debug.Log(this.tag);
        if (initHitPoints < 10)
        {
            hpTensSpriteIndex.Value = 0;//will use this for when we don't want to render a sprite for the ten's digit
            hpOnesSpriteIndex.Value = initHitPoints;
        }
        else if (initHitPoints < 20)
        {
            hpTensSpriteIndex.Value = 1;
            hpOnesSpriteIndex.Value = initHitPoints - 10;

        }
        else
        {
            hpTensSpriteIndex.Value = 2;
            hpOnesSpriteIndex.Value = initHitPoints - 20;
        }
        //Debug.Log("The board position we are going to be placed at is: " + cBoardIndex);
        
        //attack if we are a mob, not an ability card
        if (isMob.Value)
        {
            AttackServerRpc();
        }
        //we will play a card every turn so we want to go through each mob on the board and update their status effect counters if they are ours that is
        //we are delaying so that there is hopefully no issues with other animations and damage or buff/debuff calculations etc
        StartCoroutine(DelayStatusEffectChecks(playerOwnrIndex));

    }

    private IEnumerator DelayStatusEffectChecks(int playerOwnrIndex)
    {
        //run statuseffect update ater 1 seconds
        yield return new WaitForSeconds(.5f);
        //Debug.Log("we waited .5 seconds and will now check status effects and update them");
        cardBoardIndexManager.UpdateMyCardsStatusEffects(playerOwnrIndex);
    }
   

    public int[] GrabStats()
    {
        int[] tempStats = new int[] { leftStat, rightStat, topStat, bottomStat, hitPoints };
        return tempStats;
    }

    public int GetPlayerOwner()
    {
        //1 is player 1 and 2 is player 2 ezpz
        return playerOwnerIndex.Value;
    }

    public bool GetIsMob()
    {
        return isMob.Value;
    }

    public int GrabCardPlacementBoardIndex()
    {
        return cardPlacementBoardIndex.Value;
    }

    //handles attacks for all four directions, mobs run this, we will never run a heal ability or anything but attack through here
    [ServerRpc(RequireOwnership =false)]
    public void AttackServerRpc()
    {
        if(!IsServer){return;}
        int fieldSize = cardBoardIndexManager.GetFieldSizeCount();
        //the value we need to adjust our cardBoardPosIndex by to get the top and bottom attack target board positions
        int topBottomAttackIndexMod = (int)Mathf.Sqrt(fieldSize);
        //Debug.Log(topBottomAttackIndexMod + "is the topbottomattackindexMod");
        //Debug.Log("our card is at board index: " + cardBoardPosIndex);
        int leftAttackTargetBoardIndex = cardPlacementBoardIndex.Value - 1;
        int rightAttackTargetBoardIndex = cardPlacementBoardIndex.Value + 1;
        int topAttackTargetBoardIndex = cardPlacementBoardIndex.Value + topBottomAttackIndexMod;
        int bottomAttackTargetBoardIndex = cardPlacementBoardIndex.Value - topBottomAttackIndexMod;
        // defenseSideIndex notes: 1 means leftStat, 2 means RightStat, 3 means TopStat, otherwise 4 means BottomStat but we don't show that in the logic below
        //checkcardindex notes: we are currently using a 6x6 board so obv left right add subtract 1, but top bottom could change later if size changes so be aware of that, for now though plus minus 6

        //left attack
        //make sure there is a card to our left and specifically to our left not just in the index 1 below us in the list/array
        if (cardBoardIndexManager.CheckIfCardAtIndex(leftAttackTargetBoardIndex) && cardPlacementBoardIndex.Value != 0 && cardPlacementBoardIndex.Value % topBottomAttackIndexMod != 0)
        {
            //Debug.Log("There is a card to our left"); 
            //make sure we don't own the card we are targeting 
            if(!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, leftAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their right stat so we need the defense stst index to be the right, thus 2
                cardBoardIndexManager.RunTargetCardsDamageCalculations(leftStat, abilityIndex, abilityRankMod, leftAttackTargetBoardIndex, 2, 0);
               
            }
        }

        //right attack
        //make sure there is a card to our right and specifically to our right not just in the index 1 above us in the list/array
        if (cardBoardIndexManager.CheckIfCardAtIndex(rightAttackTargetBoardIndex) && (cardPlacementBoardIndex.Value + 1) % topBottomAttackIndexMod != 0)
        {
            //Debug.Log(cardPlacementBoardIndex.Value % topBottomAttackIndexMod);
            //Debug.Log("There is a card to our right");
            //make sure we don't own the card we are targeting
            if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, rightAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their left stat so we need the defense stat index to be the left, thus 1
                cardBoardIndexManager.RunTargetCardsDamageCalculations(rightStat, abilityIndex, abilityRankMod, rightAttackTargetBoardIndex, 1, 0);
                
            }
        }
        //top attack
        //make sure ther is a card above us
        if (cardBoardIndexManager.CheckIfCardAtIndex(topAttackTargetBoardIndex))
        {
            //Debug.Log("There is a card above");
            //make sure we don't own the card we are targeting
            if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, topAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their top stat so we need the defense stat index to be the bottom, thus 4
                cardBoardIndexManager.RunTargetCardsDamageCalculations(topStat, abilityIndex, abilityRankMod, topAttackTargetBoardIndex, 4, 0);
                
            }
        }
        //bottom attack
        //make sure there is a card below us
        if (cardBoardIndexManager.CheckIfCardAtIndex(bottomAttackTargetBoardIndex))
        {
            //Debug.Log("There is a card below");
            //make sure we don't own the card we are targeting
            if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, bottomAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their bottom stat so we need the defense stat index to be the top, thus 3
                cardBoardIndexManager.RunTargetCardsDamageCalculations(bottomStat, abilityIndex, abilityRankMod, bottomAttackTargetBoardIndex, 3, 0);
                
                
            }
        }

    }

    [ServerRpc (RequireOwnership = false)]
    public void abilityAttackServerRpc()
    {
        if (!IsServer) { return; }
        int fieldSize = cardBoardIndexManager.GetFieldSizeCount();
        //the value we need to adjust our cardBoardPosIndex by to get the top and bottom attack target board positions
        int topBottomAttackIndexMod = (int)Mathf.Sqrt(fieldSize);
        //Debug.Log(topBottomAttackIndexMod + "is the topbottomattackindexMod");
        //Debug.Log("our card is at board index: " + cardBoardPosIndex);
        //don't get confused by the names, leftAttack refers to our left side attacking their right, rightAttack means our right attacking their left, top attacks their bottom and bottom attacks their top
        int myCardPlacementBoardIndex = cardPlacementBoardIndex.Value;
        int leftAttackTargetBoardIndexMod = -1;//decreasing thus negative
        int rightAttackTargetBoardIndexMod = 1;
        int topAttackTargetBoardIndexMod = topBottomAttackIndexMod;
        int bottomAttackTargetBoardIndexMod = -topBottomAttackIndexMod;//decreasing thus negative
        int upperRightAttackTargetBoardIndexMod = topBottomAttackIndexMod + 1;
        int upperLeftAttackTargetBoardIndexMod = topBottomAttackIndexMod - 1;
        int lowerRightAttackTargetBoardIndexMod = -(topBottomAttackIndexMod - 1);//decreasing thus negative
        int lowerLeftAttackTargetBoardIndexMod = -(topBottomAttackIndexMod + 1);//decreasing thus negative
        int[] attackDirectionMods = new int[] {leftAttackTargetBoardIndexMod, rightAttackTargetBoardIndexMod, topAttackTargetBoardIndexMod, bottomAttackTargetBoardIndexMod,
            lowerLeftAttackTargetBoardIndexMod, lowerRightAttackTargetBoardIndexMod, upperLeftAttackTargetBoardIndexMod, upperRightAttackTargetBoardIndexMod};
        int[] defenseSideIndexes = new int[] {2, 1, 4, 3, 5, 6, 7, 8 };//gets the appropriate index for the take damage function to figure out which stat side the target uses for defense
        int[] myAttackSideStats = new int[] {curLeftStat, curRightStat, curTopStat, curBottomStat, (curLeftStat + curBottomStat) / 2, (curRightStat + curBottomStat) / 2,
        (curLeftStat + curTopStat) / 2, (curRightStat + curTopStat) / 2};
        int range = abilityRankMod < 3 ? 1 : abilityRankMod < 5 ? 2 : 3; //below 3 only go 1 in 4 basic directions, otw add 1 tile in all directions and then again add one more once abilityRankMod hits 6 
        //we will go through each 'side' of the card and handle the abilities for the respective indexes, there are 8 angles of attack thus we go until i is 8 
        for(int i = 0; i < 8; i++)
        {
            // we will increase cards to check for by 1 at rank 3 and 5, note that they start at zero
            for(int y = 0; y < range; y++)
            {
                if(i > 3 && y == 0) { }//do nothing
                else
                {
                    //to simplify the logic below, we will store the target in this attackTargetBoardIndex integer variable
                    int attackTargetBoardIndex;
                    // in case i > 3 thus dealing with angle/corner attacks do this way otw do the other version
                    if (i > 3)
                    {
                        attackTargetBoardIndex = y == 1 ? myCardPlacementBoardIndex + attackDirectionMods[i] :
                            myCardPlacementBoardIndex + attackDirectionMods[i] + attackDirectionMods[i] * (y - 1);
                    }
                    //for basic attacks left, right, top and bottom that don't have an exception of not activating when range is 1
                    else
                    {
                        attackTargetBoardIndex = y == 0 ? myCardPlacementBoardIndex + attackDirectionMods[i] :
                        myCardPlacementBoardIndex + attackDirectionMods[i] + attackDirectionMods[i] * y;
                    }
                    //Debug.Log(attackTargetBoardIndex);
                    //should handle y = 0 where we just add base mod otw add directionMod + directionMod * y
                    if (cardBoardIndexManager.CheckIfCardAtIndex(attackTargetBoardIndex))
                    {
                        //Debug.Log("There is a card at index: " + attackTargetBoardIndex);
                        
                        //handle left, lower left and upper left edge target exceptions
                        if ( (i == 0 || i == 4 || i == 6)  && ( (attackTargetBoardIndex) % topBottomAttackIndexMod == 5 || attackTargetBoardIndex  < 0) ) { Debug.Log("We reached an exception on the left side");  break; }
                        //handle right, lower right and upper right edge target exceptions
                        if ( (i == 1 || i == 3 || i == 7) && ((attackTargetBoardIndex) % topBottomAttackIndexMod == 0 ) || attackTargetBoardIndex >= fieldSize) { Debug.Log("We reached an exception on the right side"); break; }
                        ////handle lower left edge target exceptions
                        //if(i == 4 && (attackTargetBoardIndex  % topBottomAttackIndexMod == 5 || attackTargetBoardIndex + 1 == 0)) { break; }
                        ////handle lower right edge target exceptions
                        //if (i == 5 && (attackTargetBoardIndex % topBottomAttackIndexMod == 0)) { break; }
                        ////handle upper left edge target exceptions
                        //if (i == 6 && (attackTargetBoardIndex % topBottomAttackIndexMod == 5 || attackTargetBoardIndex + 1 == 0)) { break; }
                        ////handle upper right edge target exceptions
                        //if (i == 7 && (attackTargetBoardIndex % topBottomAttackIndexMod == 0)) { break; }



                        //create an int for extraConditions 0 means attack, 1 is regen, 2 is buff, 3 is debuff, 4 is charm
                        int extraCondition = 0;
                        //make sure this isn't a regen ability
                        int[][] statusEffectAbilityIndexesLists = new int[][] { regenAbilityIndexes, buffedAbilityIndexes, weakenedAbilityIndexes, charmAbilityIndexes };
                        for (int i2 = 0; i2 < statusEffectAbilityIndexesLists.Length; i2++)
                        {
                            for(int y2 = 0; y2 < statusEffectAbilityIndexesLists[i2].Length; y2++)
                            {
                                if (abilityIndex == statusEffectAbilityIndexesLists[i2][y2])
                                {
                                    //if i2 is 0 then extraCondition is 1 for regen, then 1 is 2 for buffing, 2 is 3 for weakening and otw it will be 3 is 4 for charm
                                    extraCondition = i2 == 0 ? 1: i2 == 1 ? 2: i2 == 2 ? 3: 4;
                                }
                            }
                            
                        }
                        
                        //Debug.Log("the extra condition on ability cards is: " + extraCondition);


                        //Debug.Log("There is a card below");
                        //make sure we don't own the card we are targeting and we are attacking
                        if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, attackTargetBoardIndex) && extraCondition == 0)
                        {
                            //Debug.Log("The card we are attacking is not ours so we should deal damage");
                            //handles all attacks taking in whatever our attackside stat is, the ability Index for determining the sprite atm, the targets board location index, and defense side index for figuring out
                            //which stat to use for defending later in the takeDamage function
                            //if(i > 4 && y > 0) { Debug.Log("We're getting to the end from time to time at the very least"); }
                            cardBoardIndexManager.RunTargetCardsDamageCalculations(myAttackSideStats[i], abilityIndex, abilityRankMod, attackTargetBoardIndex, defenseSideIndexes[i], extraCondition);
                        }
                        //in case we are activating a healing ability we want to target our cards and heal them
                        else if(cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, attackTargetBoardIndex) && extraCondition == 1)
                        {
                            Debug.Log("we activated a regen ability card on an ally");
                            cardBoardIndexManager.RunTargetCardsDamageCalculations(0, abilityIndex, abilityRankMod, attackTargetBoardIndex, defenseSideIndexes[i], extraCondition);
                        }
                        //in case we are activating a buff ability we want to target our cards and buff them
                        else if(cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, attackTargetBoardIndex) && extraCondition == 2)
                        {
                            Debug.Log("we activated a buff ability card on an ally card");
                            cardBoardIndexManager.RunTargetCardsDamageCalculations(0, abilityIndex, abilityRankMod, attackTargetBoardIndex, defenseSideIndexes[i], extraCondition);
                        }
                        //in case we are activating a debuff ability we want to target our opponents cards and debuff them
                        else if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, attackTargetBoardIndex) && extraCondition == 3)
                        {
                            Debug.Log("we activated a debuff ability card on an enemy card");
                            cardBoardIndexManager.RunTargetCardsDamageCalculations(0, abilityIndex, abilityRankMod, attackTargetBoardIndex, defenseSideIndexes[i], extraCondition);
                        }
                        //in case we are activating a debuff ability we want to target our opponents cards and debuff them
                        else if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, attackTargetBoardIndex) && extraCondition == 4)
                        {
                            Debug.Log("we activated a charm ability card on an enemy card");
                            cardBoardIndexManager.RunTargetCardsDamageCalculations(0, abilityIndex, abilityRankMod, attackTargetBoardIndex, defenseSideIndexes[i], extraCondition);
                        }
                    }
                    

                }

            }

        }
    }

    public void TakeDamage(int attackersValue,int defenseSideIndex, int attackersAbilityIndex, int abilityRankM, int extraConditions)
    {
        HandleApplyingStatusEffects(attackersAbilityIndex, abilityRankM);
        //handle buffs, debuffs,regen and charm here


        int defendersValue = 0;
        // defenseSideIndex notes: 1 means leftStat, 2 means RightStat, 3 means TopStat, otherwise 4 means BottomStat but we don't show that in the logic below
        defendersValue = defenseSideIndex == 1 ? curLeftStat : defenseSideIndex == 2 ? curRightStat : defenseSideIndex == 3 ? curTopStat : defenseSideIndex == 4 ? curBottomStat :
             defenseSideIndex == 4 ? (curRightStat + curBottomStat) / 2 : defenseSideIndex == 5 ? (curLeftStat + curBottomStat) / 2:
         defenseSideIndex == 6 ? (curRightStat + curTopStat) / 2 : defenseSideIndex == 7 ? (curLeftStat + curTopStat) / 2 :
         (curLeftStat + curRightStat + curTopStat + curBottomStat) / 4 /*this is for taking ability damage so we will take the average of our 4 stats*/;
        
        //Handle restoring hp
        if(extraConditions == 1)
        {
            int hpRestored = attackersValue;
            //check to make sure our hp isn't going to go over 50
            if (curHitPoints + hpRestored <= 50 )
            {
                curHitPoints += hpRestored;

                GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(-hpRestored, attackersAbilityIndex);
                Debug.Log("we handled healing");
            }

        }

        //handle applying buffs to our allies
        else if(extraConditions == 2)
        {
            BuffOrDebuffServerRpc(true, abilityRankM);
            //we only want the animation to play, no hp changes
            GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(-abilityRankM, attackersAbilityIndex);
            Debug.Log("we handled buffed");
        }
        //we were debuffed so weaken stats
        else if (extraConditions == 3)
        {
            BuffOrDebuffServerRpc(false, abilityRankM);
            //we only want the animation to play, no hp changes
            GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(abilityRankM, attackersAbilityIndex);
            Debug.Log("we handled weaken");
        }
        //we were charmed so change owner
        else if(extraConditions == 4)
        {
            ChangePlayerOwnerAndColorServerRpc();
            Debug.Log("we handled charm");
            //we only want the animation to play, no hp changes
            GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(0, attackersAbilityIndex);
        }

        //if our attack is higher than their attack stat or the atk/2 is greater than or equal to their curHitPoints, we defeat them
        else if(attackersValue - defendersValue > 0 || curHitPoints - (attackersValue/2) <= 0 && extraConditions == 0)
        {
            //reset all status effects on death
            ResetStatusEffectsServerRpc();
            //for (int i = 0; i < myStatusEffectBools.Length; i++)
            //{
            //    myStatusEffectBools[i] = false;
            //    //Debug.Log(myStatusEffectBools[i]);
            //    //make sure everything gets properly cleared
            //}
            ////run this once only
            //CheckStatusEffectsAndUpdateServerRpc();

            GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(0, attackersAbilityIndex);
            curHitPoints = hitPoints / 2;//revive with half health
            ChangePlayerOwnerAndColorServerRpc();
        }
        //handle dealing damage
        else
        {
            //check to make sure our hp isn't going to go over 50
            if (curHitPoints - attackersValue / 2 <= 50)
            {
                curHitPoints -= attackersValue / 2;
                int damageDealt = attackersValue / 2;
                GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(damageDealt, attackersAbilityIndex);
            }
        }
        UpdateHpSpritesServerRpc(curHitPoints);
    }
    //handle adjusting stats when buffs or debuffs wear off
    [ServerRpc(RequireOwnership = false)]
    private void UndoBuffOrDebuffServerRpc(bool isBuff)
    {
        if (!IsServer) { return; }
        Debug.Log("The server is going through the undobuffordebuff function");
        if (isBuff)
        {
            curLeftStat -= leftStatBuff;
            curRightStat -= rightStatBuff;
            curTopStat -= topStatBuff;
            curBottomStat -= bottomStatBuff;
            leftStatBuff = 0;
            rightStatBuff = 0;
            topStatBuff = 0;
            bottomStatBuff = 0;
        }
        else
        {
            curLeftStat += leftStatDebuff;
            curRightStat += rightStatDebuff;
            curTopStat += topStatDebuff;
            curBottomStat += bottomStatDebuff;
            leftStatDebuff = 0;
            rightStatDebuff = 0;
            topStatDebuff = 0;
            bottomStatDebuff = 0;
        }
        leftStatSpriteIndex.Value = curLeftStat;
        rightStatSpriteIndex.Value = curRightStat;
        topStatSpriteIndex.Value = curTopStat;
        bottomStatSpriteIndex.Value = curBottomStat;
        //Debug.Log("left stat is now " + leftStatSpriteIndex.Value);
        //Debug.Log("right stat is now " + rightStatSpriteIndex.Value);
        //Debug.Log("top stat is now " + topStatSpriteIndex.Value);
        //Debug.Log("bottom stat is now " + bottomStatSpriteIndex.Value);
    }

    //Handles buffing or debuffing mobs and updating their statSprites to reflect it
    [ServerRpc(RequireOwnership = false)]
    private void BuffOrDebuffServerRpc(bool isBuffing, int modifierVal)
    {
        if (!IsServer) { return; }
        if (isBuffing)
        {
            Debug.Log("The server is handling buffing the card");
            if (leftStatSpriteIndex.Value != 9 && leftStatSpriteIndex.Value + modifierVal <= 9)
            {
                curLeftStat += modifierVal;
                leftStatBuff += modifierVal;
                leftStatSpriteIndex.Value += modifierVal;
                
            }
            if (rightStatSpriteIndex.Value != 9 && rightStatSpriteIndex.Value + modifierVal <= 9)
            {
                curRightStat += modifierVal;
                rightStatBuff += modifierVal;
                rightStatSpriteIndex.Value += modifierVal;
                
            }
            if (topStatSpriteIndex.Value != 9 && topStatSpriteIndex.Value + modifierVal <= 9)
            {
                curTopStat += modifierVal;
                topStatBuff += modifierVal;
                topStatSpriteIndex.Value += modifierVal;
                
            }
            if (bottomStatSpriteIndex.Value + modifierVal <= 9)
            {
                curBottomStat += modifierVal;
                bottomStatBuff += modifierVal;
                bottomStatSpriteIndex.Value += modifierVal; 
            }
        }
        //now handle debuff so same thing but subtract and adjust xStatDebuff
        else
        {
            Debug.Log("The server is handling debuffing the card");
            if (leftStatSpriteIndex.Value - modifierVal >= 0)
            {
                curLeftStat -= modifierVal;
                leftStatDebuff += modifierVal;
                leftStatSpriteIndex.Value -= modifierVal;
                
            }
            if (rightStatSpriteIndex.Value - modifierVal >= 0)
            {
                curRightStat -= modifierVal;
                rightStatDebuff += modifierVal;
                rightStatSpriteIndex.Value -= modifierVal;

            }
            if (topStatSpriteIndex.Value - modifierVal >= 0)
            {
                curTopStat -= modifierVal;
                topStatDebuff += modifierVal;
                topStatSpriteIndex.Value -= modifierVal;
            }
            if (bottomStatSpriteIndex.Value - modifierVal >= 0)
            {
                curBottomStat -= modifierVal;
                bottomStatDebuff += modifierVal;
                bottomStatSpriteIndex.Value -= modifierVal;
            }
            
        }
        //leftStatSpriteIndex.Value = curLeftStat;
        //rightStatSpriteIndex.Value = curRightStat;
        //topStatSpriteIndex.Value = curTopStat;
        //bottomStatSpriteIndex.Value = curBottomStat;
        //Debug.Log("left stat is now " + curLeftStat);
        //Debug.Log("right stat is now " + curRightStat);
        //Debug.Log("top stat is now " + curTopStat);
        //Debug.Log("bottom stat is now " + curBottomStat);
        //Debug.Log("left sprite stat is now " + leftStatSpriteIndex.Value);
        //Debug.Log("right sprite stat is now " + rightStatSpriteIndex.Value);
        //Debug.Log("top sprite stat is now " + topStatSpriteIndex.Value);
        //Debug.Log("bottom sprite stat is now " + bottomStatSpriteIndex.Value);
    }



    private void HandleApplyingStatusEffects(int abilityIndex, int abilityRankMod)
    {
        //Debug.Log("The attackers ability index was: " + abilityIndex);
        int[][] statusEffectAbilityIndexLists = new int[][] { poisonAbilityIndexes, burnAbilityIndexes,corrosionAbilityIndexes, regenAbilityIndexes, weakenedAbilityIndexes, buffedAbilityIndexes, charmAbilityIndexes };
        for (int i = 0; i < statusEffectAbilityIndexLists.Length; i++)
        {
            for (int y = 0; y < statusEffectAbilityIndexLists[i].Length; y++)
            {

                //hopefully will make sure that this only runs when we don't yet have the status effect
                if (abilityIndex == statusEffectAbilityIndexLists[i][y] && myStatusEffectBools[i] == false)
                {
                    Debug.Log("we applied an ability");
                    myStatusEffectBools[i] = true;
                    myStatusEffectTurnsRemaining[i] = 3;//we will have all status effects default to 3 turns, for now no exceptions
                    myStatusEffectRanks[i] = abilityRankMod;
                    statusIcons[i].SetActive(true);//turn on the respective status icon 
                    UpdateStatusIconsClientRpc(i, true);//the server/host is taken care of above, now we update the client
                    Debug.Log(myStatusEffectMessages[i]);
                    break;
                }
            }
        }
        
        ////check to see if abilityIndex matches any valid indexes in the poisonAbilityIndexes array
        //for (int i = 0; i < poisonAbilityIndexes.Length; i++)
        //{
        //    if(abilityIndex == poisonAbilityIndexes[i]) 
        //    {
        //        myStatusEffectBools[0] = true;
        //        myStatusEffectTurnsRemaining[0] = 3;//we will have all status effects default to 3 turns, for now no exceptions
        //        myStatusEffectRanks[0] = abilityRankMod;
        //        Debug.Log("the ability was a poison ability and we now have it.");
        //        break;
        //    }
        //}

        ////check to see if abilityIndex matches any valid indexes in the poisonAbilityIndexes array
        //for (int i = 0; i < burnAbilityIndexes.Length; i++)
        //{
        //    if (abilityIndex == burnAbilityIndexes[i])
        //    {
        //        myStatusEffectBools[0] = true;
        //        myStatusEffectTurnsRemaining[0] = 3;//we will have all status effects default to 3 turns, for now no exceptions
        //        myStatusEffectRanks[0] = abilityRankMod;
        //        Debug.Log("the ability was a burn ability and we now have it.");
        //        break;
        //    }
        //}


    }

    [ServerRpc(RequireOwnership=false)]
    private void ResetStatusEffectsServerRpc()
    {
        if (!IsServer) { return; }
        for(int i = 0; i < myStatusEffectBools.Length; i++)
        {
            myStatusEffectBools[i] = false;
            myStatusEffectTurnsRemaining[i] = 0;
            UndoBuffOrDebuffServerRpc(false);
            UndoBuffOrDebuffServerRpc(true);
            statusIcons[i].SetActive(false);//we don't want our status icons showing anymore
            UpdateStatusIconsClientRpc(i, false);//the server/host is taken care of above, now we update the client
        }
        Debug.Log("We reset all our status effects and their turn counts");
    }

    [ClientRpc]
    private void UpdateStatusIconsClientRpc(int index, bool activeState)
    {
        if (IsServer) { return; }
        statusIcons[index].SetActive(activeState);
    }

    
    [ServerRpc (RequireOwnership = false)]
    public void CheckStatusEffectsAndUpdateServerRpc()
    {
        if (!IsServer) { return; }
        //go through through every status effect and update
        for(int i = 0; i < myStatusEffectBools.Length; i++)
        {
            if (!myStatusEffectBools[i]) { }//do nothing if we don't have the status effect
            //if we have the status effect update our turns remaining and deal appropriate dmg
            else
            {
                //Debug.Log("We have a status effect and thusly should be about to run takeDamage");
                //Debug.Log("The damage that should be dealt by this status effect is: " + ((myStatusEffectRanks[i] + 1) / 2 + 2));
                myStatusEffectTurnsRemaining[i] -= 1;
                Debug.Log("turns remaining: " + myStatusEffectTurnsRemaining[i]);
                ////handle regen
                if (i == 3)
                {
                    //Debug.Log("We are healing");
                    //take negative damage
                    TakeDamage((myStatusEffectRanks[i] + 1) / 2 + 2, 8, myStatusEffectAbilityAnimationIndex[i], myStatusEffectRanks[i],1);

                }
                //if we are 4,5 or 6 so buff, debuff, or charm, we don't want to actually do anything because the effect has already been ran
                else if (i > 3 ) { }//do nothing
                ////otw take damage
                else
                {
                    TakeDamage((myStatusEffectRanks[i] + 1) / 2 + 2, 8, myStatusEffectAbilityAnimationIndex[i], myStatusEffectRanks[i],0);
                }

                if (myStatusEffectTurnsRemaining[i] == 0)
                {
                    Debug.Log("ability: " + i + " ran out");
                    myStatusEffectBools[i] = false;
                    statusIcons[i].SetActive(false);//we shouldn't have this status icon showing anymore
                    UpdateStatusIconsClientRpc(i, false);//the server/host is taken care of above, now we update the client
                    if (i == 4/*buff*/)
                    {
                        UndoBuffOrDebuffServerRpc(false);
                        Debug.Log("We undid debuffs");
                    }
                    if (i == 5/*debuff*/)
                    {
                        UndoBuffOrDebuffServerRpc(true);
                        Debug.Log("We undid buffs");
                    }
                    if (i == 6)
                    {
                        //go back to what we were before
                        ChangePlayerOwnerAndColorServerRpc();
                    }
                }
            }
            
        }
    }

    

    [ServerRpc(RequireOwnership =false)]
    public void UpdateHpSpritesServerRpc(int newCurHitPoints)
    {
        //just want the server to do the work and update
        if (!IsServer) { return; }
        if (newCurHitPoints < 10)
        {
            hpTensSpriteIndex.Value = 0;//will use this for when we don't want to render a sprite for the ten's digit
            hpOnesSpriteIndex.Value = newCurHitPoints;
        }
        else if (newCurHitPoints < 20)
        {
            hpTensSpriteIndex.Value = 1;
            hpOnesSpriteIndex.Value = newCurHitPoints - 10;

        }
        else if (newCurHitPoints < 30)
        {
            hpTensSpriteIndex.Value = 2;
            hpOnesSpriteIndex.Value = newCurHitPoints - 20;
        }
        else if(newCurHitPoints < 40)
        {
            hpTensSpriteIndex.Value = 3;
            hpOnesSpriteIndex.Value = newCurHitPoints - 30;
        }
        else if (newCurHitPoints < 50)
        {
            hpTensSpriteIndex.Value = 4;
            hpOnesSpriteIndex.Value = newCurHitPoints - 40;
        }
        else
        {
            hpTensSpriteIndex.Value = 5;
            hpOnesSpriteIndex.Value = newCurHitPoints - 50;
        }
    }

    


}
