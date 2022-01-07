using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MobCardNetwork : NetworkBehaviour
{
    public bool isInHand = true;
    //all the impoortant stats for the mob to display and use for interactions
    private int topStat, bottomStat, leftStat, rightStat, curTopStat, curBottomStat, curLeftStat, curRightStat, curHitPoints, hitPoints;
    private Color player1mobBackgroundColor = new Color(.4f,0,0);
    private Color player2mobBackgroundColor = new Color(.1f, .1f, 1);
    [SerializeField]private GameObject mobSpriteRenderer;
    [SerializeField]private GameObject topStatSprite,bottomStatSprite,leftStatSprite, rightStatSprite, hpTensSprite, hpOnesSprite, attributeSprite;//technically renderers but for some reason i named them as just xSprite
    [SerializeField]private GameObject mobBackground;
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
    
    [SerializeField] private List<Sprite> mobSprites = new List<Sprite>();
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

    }

    //don't feel a need for two functions, the way it works makes more sense to me to do it like this.
    private void OnPlayerOwnershipAndColorChanged(int oldPlayerOwnerIndex, int newPlayerOwnerIndex)
    {
        if (!IsClient) { return; }//testing these parameters
        Debug.Log("the player owners index: " + newPlayerOwnerIndex);
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
        mobSpriteRenderer.GetComponent<SpriteRenderer>().sprite = mobSprites[newIndex];//this should get the sprite from the script and let us set our sprite, hopefully
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
        Debug.Log("we set the card in the board index manager");
        //we want to attack when placed on the field
       
    }



    [ServerRpc]
    private void DestroyNetworkObjectServerRpc()
    {
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
        playerOwnerIndex.Value = 2;
        mobBackgroundColor.Value = player2mobBackgroundColor;
       
    }
    //will be ran when the card is drawn/instantiated
    [ServerRpc]
    public void CreateMobCardServerRpc(int initLeftStat, int initRightStat, int initTopStat, int initBottomStat, int initHitPoints, int playerOwnrIndex, int mobSpriteIndexReference, int attributeSpriteIndexReference, int cBoardIndex)
    {
        if (!IsServer) { return; }
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
        cardPlacementBoardIndex.Value = cBoardIndex;
        playerOwnerIndex.Value = playerOwnrIndex;
        mobSpriteIndex.Value = mobSpriteIndexReference;
        //new stuff added that needs onChange listeners and functions
        attributeSpriteIndex.Value = attributeSpriteIndexReference;
        topStatSpriteIndex.Value = initTopStat;
        bottomStatSpriteIndex.Value = initBottomStat;
        leftStatSpriteIndex.Value = initLeftStat;
        rightStatSpriteIndex.Value = initRightStat;
        if (initHitPoints < 10)
        {
            hpTensSpriteIndex.Value = -1;//will use this for when we don't want to render a sprite for the ten's digit
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
        Debug.Log("The board position we are going to be placed at is: " + cBoardIndex);
        Attack(cBoardIndex);//could use cBoardIndex but want to try this first
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

    public void Attack(int cardBoardPosIndex)
    {
        Debug.Log("our card is at board index: " + cardBoardPosIndex);
        int leftAttackTargetIndex = cardBoardPosIndex - 1;
        //left attack
        if(cardBoardIndexManager.CheckIfCardAtIndex(cardBoardPosIndex - 1))
        {
            Debug.Log("There is a card here"); 
            if(!cardBoardIndexManager.CheckIfCardAtIndexIsOwnedByMe(playerOwnerIndex.Value, leftAttackTargetIndex))
            {
                Debug.Log("The card we are attacking is not ours so we should deal damage");
            }
        }

    }

    public void TakeDamage(int attackersValue,int defendersValue)
    {
        if(attackersValue - defendersValue > 0)
        {
            curHitPoints -= attackersValue - defendersValue;
            if(curHitPoints <= 0)
            {
                curHitPoints = 0;
                //we will run our death function here later
            }
            UpdateHpSpritesServerRpc(curHitPoints);
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
        else
        {
            hpTensSpriteIndex.Value = 2;
            hpOnesSpriteIndex.Value = newCurHitPoints - 20;
        }
    }

    


}
