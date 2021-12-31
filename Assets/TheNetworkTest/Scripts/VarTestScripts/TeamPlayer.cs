using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamPlayer : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer teamColourRenderer;
    [SerializeField] private Color[] teamColours;

    private NetworkVariable<byte> teamIndex = new NetworkVariable<byte>();//creates networkVariable object for storing teamIndex in bytes on the network
    
    
    [ServerRpc (RequireOwnership = false)]
    public void SetTeamServerRpc(byte newTeamIndex)
    {
        //make sure index is valid
        if(newTeamIndex > 3) { return; }

        //Update the teamIndex network variable
        teamIndex.Value = newTeamIndex;//have to change value, changing directly causes errors since networkVariable<Byte> isn't same type and Byte
    }

    // the network variable has an event that triggers whenever its value changes and we can subscribe to a function /run it
    private void OnEnable()
    {
        //start listening for the team index being updated
        teamIndex.OnValueChanged += OnTeamChanged;
    }

    // the network variable has an event that triggers whenever its value changes and we can subscribe to a function /run it
    private void OnDisable()
    {
        //stop listening for the team index being updated
        teamIndex.OnValueChanged -= OnTeamChanged;
    }

    //have to pass in two of the same data type we are syncing, basically reference to variables oldValue and the new value to change it to the new one
    private void OnTeamChanged(byte oldTeamIndex, byte newTeamIndex)
    {
        //only client needs to update the spriteRenderer
        if (!IsClient) { return; }
        // update the color of the playere's spriteRenderer
        teamColourRenderer.color = teamColours[newTeamIndex];
    }

    

}
