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
    
    [SerializeField] private List<Sprite> mobSprites = new List<Sprite>();
    public SpritesLists spritesReferenceHolder;

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
    }

    private void OnDisable()
    {
        playerOwnerIndex.OnValueChanged -= OnPlayerOwnershipAndColorChanged;
        mobSpriteIndex.OnValueChanged += OnMobSpriteIndexChanged;
        topStatSpriteIndex.OnValueChanged += OnTopStatSpriteIndexChanged;
        bottomStatSpriteIndex.OnValueChanged += OnBottomStatSpriteIndexChanged;
        leftStatSpriteIndex.OnValueChanged += OnLeftStatSpriteIndexChanged;
        rightStatSpriteIndex.OnValueChanged += OnRightStatSpriteIndexChanged;
        hpTensSpriteIndex.OnValueChanged += OnHpTensSpriteIndexChanged;
        hpOnesSpriteIndex.OnValueChanged += OnHpOnesSpriteIndexChanged;
        attributeSpriteIndex.OnValueChanged += OnAttributeSpriteIndexChanged;

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
    public void CreateMobCardServerRpc(int initLeftStat, int initRightStat, int initTopStat, int initBottomStat, int initHitPoints, int playerOwnrIndex, int mobSpriteIndexReference, int attributeSpriteIndexReference)
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
        
        
    }

    public int[] GrabStats()
    {
        int[] tempStats = new int[] { leftStat, rightStat, topStat, bottomStat, hitPoints };
        return tempStats;
    }

    


}
