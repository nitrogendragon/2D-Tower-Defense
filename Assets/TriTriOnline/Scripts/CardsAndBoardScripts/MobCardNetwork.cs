using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MobCardNetwork : NetworkBehaviour
{
    public bool isInHand = true;

    //all the impoortant stats for the mob to display and use for interactions
    private int topStat, bottomStat, leftStat, rightStat, curTopStat, curBottomStat, curLeftStat, curRightStat, curHitPoints, hitPoints, abilityIndex, abilityRankMod;
    private Color player1mobBackgroundColor = new Color(.4f,0,0);
    private Color player2mobBackgroundColor = new Color(.1f, .1f, 1);
    [SerializeField]private GameObject mobSpriteRenderer;
    [SerializeField]private GameObject topStatSprite,bottomStatSprite,leftStatSprite, rightStatSprite, hpTensSprite, hpOnesSprite, attributeSprite;//technically renderers but for some reason i named them as just xSprite
    [SerializeField]private GameObject mobBackground;
    [SerializeField] private GameObject damagePopUp;
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
    
    //[SerializeField] private List<Sprite> mobSprites = new List<Sprite>();
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
        Debug.Log(this.tag);
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
        
        //we don't want to attack if we are not a mob
        if (!isMob.Value) { return; }
        AttackServerRpc(cBoardIndex);
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

    //handles attacks for all four directions 
    [ServerRpc(RequireOwnership =false)]
    public void AttackServerRpc(int cardBoardPosIndex)
    {
        if(!IsServer){return;}
        int fieldSize = cardBoardIndexManager.GetFieldSizeCount();
        //the value we need to adjust our cardBoardPosIndex by to get the top and bottom attack target board positions
        int topBottomAttackIndexMod = (int)Mathf.Sqrt(fieldSize);
        //Debug.Log(topBottomAttackIndexMod + "is the topbottomattackindexMod");
        //Debug.Log("our card is at board index: " + cardBoardPosIndex);
        int leftAttackTargetBoardIndex = cardBoardPosIndex - 1;
        int rightAttackTargetBoardIndex = cardBoardPosIndex + 1;
        int topAttackTargetBoardIndex = cardBoardPosIndex + topBottomAttackIndexMod;
        int bottomAttackTargetBoardIndex = cardBoardPosIndex - topBottomAttackIndexMod;
        // defenseSideIndex notes: 1 means leftStat, 2 means RightStat, 3 means TopStat, otherwise 4 means BottomStat but we don't show that in the logic below
        //checkcardindex notes: we are currently using a 6x6 board so obv left right add subtract 1, but top bottom could change later if size changes so be aware of that, for now though plus minus 6

        //left attack
        //make sure there is a card to our left and specifically to our left not just in the index 1 below us in the list/array
        if (cardBoardIndexManager.CheckIfCardAtIndex(leftAttackTargetBoardIndex) && cardBoardPosIndex != 0 && cardBoardPosIndex % topBottomAttackIndexMod != 0)
        {
            //Debug.Log("There is a card to our left"); 
            //make sure we don't own the card we are targeting 
            if(!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, leftAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their right stat so we need the defense stst index to be the right, thus 2
                cardBoardIndexManager.RunTargetCardsDamageCalculations(leftStat, leftAttackTargetBoardIndex, 2);
            }
        }

        //right attack
        //make sure there is a card to our right and specifically to our right not just in the index 1 above us in the list/array
        if (cardBoardIndexManager.CheckIfCardAtIndex(rightAttackTargetBoardIndex))
        {
            //Debug.Log("There is a card to our right");
            //make sure we don't own the card we are targeting
            if (!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, rightAttackTargetBoardIndex))
            {
                //Debug.Log("The card we are attacking is not ours so we should deal damage");
                //we are attacking their left stat so we need the defense stat index to be the left, thus 1
                cardBoardIndexManager.RunTargetCardsDamageCalculations(rightStat, rightAttackTargetBoardIndex, 1);
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
                cardBoardIndexManager.RunTargetCardsDamageCalculations(topStat, topAttackTargetBoardIndex, 4);
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
                //we are attacking their bottom stat so we need the defense stat index to be the bottom, thus 3
                cardBoardIndexManager.RunTargetCardsDamageCalculations(bottomStat, bottomAttackTargetBoardIndex, 3);
            }
        }

    }

    public void TakeDamage(int attackersValue,int defenseSideIndex)
    {
        int defendersValue = 0;
        // defenseSideIndex notes: 1 means leftStat, 2 means RightStat, 3 means TopStat, otherwise 4 means BottomStat but we don't show that in the logic below
        defendersValue = defenseSideIndex == 1 ? leftStat : defenseSideIndex == 2 ? rightStat : defenseSideIndex == 3 ? topStat : bottomStat;
       
        //if our attack is higher than their attack stat or the atk/2 is greater than or equal to their curHitPoints, we defeat them
        if(attackersValue - defendersValue > 0 || curHitPoints - (attackersValue/2) <= 0)
        {
            curHitPoints = hitPoints / 2;//revive with half health
            ChangePlayerOwnerAndColorServerRpc();
        }
        else
        { 
            curHitPoints -= attackersValue / 2;
            int damageDealt = attackersValue / 2;
            GetComponent<DamagePopUp>().ChangeTextAndSetActiveServerRpc(damageDealt);
        }
        UpdateHpSpritesServerRpc(curHitPoints);
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
        else
        {
            hpTensSpriteIndex.Value = 2;
            hpOnesSpriteIndex.Value = newCurHitPoints - 20;
        }
    }

    


}
