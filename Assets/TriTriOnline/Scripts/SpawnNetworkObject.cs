using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SpawnNetworkObject : NetworkBehaviour
{
    [SerializeField] private NetworkObject networkPrefab;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        
        //make sure this belongs to us
        if (!IsOwner) {  return; }

        //make sure we hit the left mouse button
        if (!Input.GetMouseButtonDown(0)) { return; }
        //Find where our mouse hit
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (!hit.collider || !hit.collider.CompareTag("BoardTile")) { return; }
        Debug.Log("we clicked");
        
        SpawnNetworkObjectServerRpc(hit.point);

    }

    [ServerRpc]
    private void SpawnNetworkObjectServerRpc(Vector3 spawnPos)
    {
        Debug.Log("we are running the serverrpc");
        //spawn the object in normally on the server
        NetworkObject networkObjectInstance = Instantiate(networkPrefab, spawnPos, new Quaternion(0,0,0,0));
        //replicate the object to all clients and give
        // ownership to he client that owns this player
        networkObjectInstance.SpawnWithOwnership(OwnerClientId);
    }

}
