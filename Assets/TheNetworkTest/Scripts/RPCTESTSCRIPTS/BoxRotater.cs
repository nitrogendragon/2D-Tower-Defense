using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BoxRotater : NetworkBehaviour
{
    [SerializeField] private GameObject box;// the box we will interact with
    private GameObject passwordNetworkManager;
    private GameObject boxInstance;
    private Color initialBoxColor;
    private ushort behaviorId;
    private ulong boxId;
    //private NetworkObject boxRef;
    private void Start()
    {
        if (!IsOwner) { return; }
        initialBoxColor = box.GetComponent<SpriteRenderer>().color;
        box.SetActive(true);
        //CreatePlayerBoxServerRpc();
        
        //if (!IsOwner) { return; }
        
        
        //boxId = boxInstance.GetComponent<NetworkObject>().NetworkObjectId;//this finds the networkObjects Id
        //Debug.Log(boxId);
        //Debug.Log(boxRef);
        //passwordNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        //Debug.Log(passwordNetworkManager);
        //passwordNetworkManager.GetComponent<PasswordNetworkManager>().GetBoxId(boxId);
    }


    //[ServerRpc]
    //void CreatePlayerBoxServerRpc()
    //{
    //        CreatePlayerBoxClientRpc();   
    //}
    //[ClientRpc]
    //void CreatePlayerBoxClientRpc()
    //{
    //        boxInstance = Instantiate(box);
    //        boxInstance.GetComponent<NetworkObject>().Spawn();
    //}
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        if (Input.GetMouseButtonDown(0))
        {
            AdjustBoxServerRpc();
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            DeactiveBoxServerRpc();
            return;
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    TurnBoxServerRpc();
        //}
        //if(Input.GetMouseButtonDown(0))
            //{
            //TurnBoxServerRpc();
            //boxInstance.GetComponent<SpriteRenderer>().color = Color.green;
            //boxInstance.transform.Rotate(45, 45, 0);
            //boxInstance.transform.position += new Vector3(1, 1, 1);
           
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        //    DespawnBoxInstanceServerRpc();
        //}
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        //if (IsOwner && boxInstance)
        //{
        //Destroy(boxInstance);
        //boxInstance.GetComponent<NetworkObject>().Despawn();

        if (IsOwner)
        {
            box.GetComponent<SpriteRenderer>().color = initialBoxColor;
            box.transform.rotation = new Quaternion(0, 0, 0, 0);
            DeactiveBoxServerRpc();
        }
    }

    //[ServerRpc]
    //private void TurnBoxServerRpc()
    //{
    //    TurnBoxClientRpc();
    //}

    //[ClientRpc]
    //private void TurnBoxClientRpc()
    //{
    //    //if (IsOwner) { return; }
    //    box.GetComponent<SpriteRenderer>().color = Color.green;
    //    box.transform.Rotate(45, 45, 0);
    //    //boxInstance.transform.position += new Vector3(1,1,1);
    //}

    //[ServerRpc]
    //public void DespawnBoxInstanceServerRpc()
    //{
    //    DespawnBoxInstanceClientRpc();
    //}

    //[ClientRpc]
    //private void DespawnBoxInstanceClientRpc()
    //{
    //    //if (!IsServer) { return; }
    //    Destroy(boxInstance);
    //    boxInstance.GetComponent<NetworkObject>().Despawn();
        
    //}

    [ServerRpc]
    private void DeactiveBoxServerRpc()
    {
        DeactiveBoxClientRpc();
    }

    [ClientRpc]
    private void DeactiveBoxClientRpc()
    {
        box.SetActive(false);
    }

    [ServerRpc]
    private void AdjustBoxServerRpc()
    {
        AdjustBoxClientRpc();
    }

    [ClientRpc]
    private void AdjustBoxClientRpc()
    {
        box.transform.rotation = box.transform.rotation;
        box.transform.Rotate(0, 22.5f, 0);
        box.GetComponent<SpriteRenderer>().color = Color.blue;
    }






}
