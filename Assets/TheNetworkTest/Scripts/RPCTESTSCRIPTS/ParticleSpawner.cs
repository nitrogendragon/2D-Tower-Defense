using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class ParticleSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject particlePrefab;

    private void Update()
    {
        if (!IsOwner) { return; }//if this isn't our player don't do anything
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }//if we aren't pressing space then end the update

        SpawnParticleServerRpc();
        //we can run the code from the SpawnParticleclientRPC locally here so that what we expect to happen does immediately, no need to wait for server for everything 
        GameObject particles = Instantiate(particlePrefab);
        particles.transform.position = this.transform.position;
        //
    }
    //by default it is reliable but if we don't want it to endlessly try on failure then we set it to be unreliable
    [ServerRpc(Delivery  = RpcDelivery.Unreliable)] //server calls this function for all clients, it's basically relaying client updates to everyone else
    //[ServerRpc(RequireOwnership = false] // this would let anyone run this function, so opening a door for example
    private void SpawnParticleServerRpc()
    {
        SpawnParticleClientRpc();
        
    }

    [ClientRpc(Delivery = RpcDelivery.Unreliable)] // your client calls this function
    private void SpawnParticleClientRpc()
    {
        if (IsOwner) { return; }
        GameObject particles = Instantiate(particlePrefab);
        particles.transform.position = this.transform.position;
    }
}
