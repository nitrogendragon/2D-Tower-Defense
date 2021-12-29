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
        StartCoroutine(AssignPlayerBox());
    }

    IEnumerator AssignPlayerBox()
    {
        yield return new WaitForSeconds(1f);
        boxInstance = Instantiate(box);
        Debug.Log(boxInstance);
    }
    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) { return; }
        if (Input.GetMouseButtonDown(0) && boxInstance)
        {
            TurnBoxServerRpc();
            boxInstance.GetComponent<SpriteRenderer>().color = Color.green;
            boxInstance.transform.Rotate(45, 45, 0);
            //boxInstance.transform.position += new Vector3(1, 1, 1);
            Debug.Log(boxInstance.transform.position);
        }
        if (Input.GetMouseButtonDown(1))
        {
            DestroyBoxInstanceServerRpc();
            Destroy(boxInstance);
        }

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

    [ServerRpc]
    public void DestroyBoxInstanceServerRpc()
    {
        DestroyBoxInstanceClientRpc();
    }

    [ClientRpc]
    private void DestroyBoxInstanceClientRpc()
    {
        if (IsOwner) { return; }
        Destroy(boxInstance);
    }
}
