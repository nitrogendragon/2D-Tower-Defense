using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BoxRotater : NetworkBehaviour
{
    [SerializeField] private GameObject box;// the box we will interact with
    private GameObject boxInstance;
    private void Start()
    {
        boxInstance = Instantiate(box);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !Input.GetMouseButtonDown(0)) { return; }

        TurnBoxServerRpc();
        boxInstance.GetComponent<SpriteRenderer>().color = Color.green;
        boxInstance.transform.Rotate(45, 45, 0);
        //boxInstance.transform.position += new Vector3(1, 1, 1);
        Debug.Log(boxInstance.transform.position);

    }

    [ServerRpc]
    private void TurnBoxServerRpc()
    {
        TurnBoxClientRpc();
    }

    [ClientRpc]
    private void TurnBoxClientRpc()
    {
        if (IsOwner) { return; }
        boxInstance.GetComponent<SpriteRenderer>().color = Color.green;
        boxInstance.transform.Rotate(45, 45, 0);
        //boxInstance.transform.position += new Vector3(1,1,1);
    }
}
