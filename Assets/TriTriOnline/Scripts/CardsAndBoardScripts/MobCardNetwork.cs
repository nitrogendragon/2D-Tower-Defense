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
    [SerializeField]private SpriteRenderer mobSpriteRenderer;
    [SerializeField]private SpriteRenderer mobBackground;//technically the
    private NetworkVariable<Color> mobBackgroundColor = new NetworkVariable<Color>();
    private NetworkVariable<int> playerOwnerIndex = new NetworkVariable<int>(1);

    private void OnEnable()
    {
        playerOwnerIndex.OnValueChanged += OnPlayerOwnershipAndColorChanged;
    }

    private void OnDisable()
    {
        playerOwnerIndex.OnValueChanged -= OnPlayerOwnershipAndColorChanged;
    }

    //don't feel a need for two functions, the way it works makes more sense to me to do it like this.
    private void OnPlayerOwnershipAndColorChanged(int oldPlayerOwnerIndex, int newPlayerOwnerIndex)
    { 
        if (!IsClient) { return; }
        mobBackground.color = mobBackgroundColor.Value;
    }

    [ServerRpc]
    private void DestroyNetworkObjectServerRpc()
    {
        //one option is to despawn the network object, stays on server but disappears for the clients
        //GetComponent<NetworkObject>().Despawn();
        //as long as we do this on the server it will update for everyone
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerOwnerAndColorServerRpc()
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
    private void CreateMobCard(int initTopStat, int initBottomStat, int initLeftStat, int initRightStat, int initHitPoints)
    {
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
    }


}
