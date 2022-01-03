using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
public class SquareNetObj : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer squareRenderer;
    [SerializeField] private TMP_Text squareText;
    [SerializeField] private Canvas myCanvas;

    private static FixedString32Bytes[] wordsList = new FixedString32Bytes[] { "test", "pie", "apples", "banana", "hooray!" };
    private NetworkVariable<Color> squareColor = new NetworkVariable<Color>();
    private NetworkVariable<FixedString32Bytes> squareTextString = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        Debug.Log("We spawned");
        base.OnNetworkSpawn();
        if (!IsServer) { return; }
        Debug.Log("We are the server");
        myCanvas.worldCamera = Camera.main;
        squareColor.Value = Random.ColorHSV();
        squareTextString.Value = "Temp Starter Test Text";
    }

    private void Update()
    {

        //make sure this belongs to us
        if (!IsOwner) { return; }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeColorServerRpc();
            ChangeTextServerRpc();
        }
        //make sure we hit the left mouse button
        if (!Input.GetKeyDown(KeyCode.Space)) { return; }
        DestroyNetworkObjectServerRpc();
        
    }


    private void OnEnable()
    {
        squareColor.OnValueChanged += OnSquareColorChanged;
        squareTextString.OnValueChanged += OnSquareTextStringChanged;
    }

    private void OnDisable()
    {
        squareColor.OnValueChanged -= OnSquareColorChanged;
        squareTextString.OnValueChanged -= OnSquareTextStringChanged;
    }

    private void OnSquareColorChanged(Color oldColor, Color newColor)
    {
        if (!IsClient) { return; }
        squareRenderer.color = newColor;
    }

    private void OnSquareTextStringChanged(FixedString32Bytes oldString, FixedString32Bytes newString)
    {
        if (!IsClient) { return; }
        squareText.text = newString.ToString();
    }

    [ServerRpc]
    private void DestroyNetworkObjectServerRpc()
    {
        //one option is to despawn the network object, stays on server but disappears for the clients
        //GetComponent<NetworkObject>().Despawn();
        //as long as we do this on the server it will update for everyone
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangeColorServerRpc()
    {
        if (!IsServer) { return; }
        squareColor.Value = Random.ColorHSV();
    }

    [ServerRpc(RequireOwnership =false)]
    private void ChangeTextServerRpc()
    {
        int index = Random.Range(0, 4);
        squareTextString.Value = wordsList[index];
    }

    //[ServerRpc]
    //private void ChangeTextServerRpc()
    //{
    //    if (IsHost)
    //    {
    //        Debug.Log("host changed text");
    //        squareTextString.Value = "we tried";
    //    }
    //    ChangeTextClientRpc();


    //}

    //[ClientRpc]
    //private void ChangeTextClientRpc()
    //{
    //    //only the server can write 
    //    if (IsLocalPlayer)
    //    {
    //        Debug.Log("client changed its text itself");
    //        squareText.text = "we tried";
    //    }
    //}
}
