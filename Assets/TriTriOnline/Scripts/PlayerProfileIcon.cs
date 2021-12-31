using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerProfileIcon : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Sprite[] playerSpriteOptions;
    

    //are we player 1? if not we will be player2
    private NetworkVariable<byte> playerIndex = new NetworkVariable<byte>();
    
    [ServerRpc (RequireOwnership = false)]
    public void SetPlayerIndexServerRpc(byte newPlayerIndex)
    {
        //we only have 4 options rn
        if(newPlayerIndex > 3) { return; }

        playerIndex.Value = newPlayerIndex;
    }

    private void OnEnable()
    {
        //start listening for the team index being updated
        playerIndex.OnValueChanged += OnPlayerIndexChanged;
    }

    // the network variable has an event that triggers whenever its value changes and we can subscribe to a function /run it
    private void OnDisable()
    {
        //stop listening for the team index being updated
        playerIndex.OnValueChanged -= OnPlayerIndexChanged;
    }

    private void OnPlayerIndexChanged(byte oldPlayerIndex, byte newPlayerIndex)
    {
        if (!IsClient) { return; }

        playerSpriteRenderer.sprite = playerSpriteOptions[newPlayerIndex];
    }
}
