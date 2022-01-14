using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DamagePopUp : NetworkBehaviour
{
    [SerializeField]  private GameObject damagePopUp;
    private Vector3 initialPosition;
    private float movementRangeX = .01f;
    private float timeTilDeactive = 1.6f;
    private float timePassed;
    [SerializeField] private NetworkObject damageSpriteAnimation;
    private NetworkVariable<bool> tookDamage = new NetworkVariable<bool>(false);
    private NetworkVariable<int> damageTaken = new NetworkVariable<int>();
    // Start is called before the first frame update

    private void OnEnable()
    {
        tookDamage.OnValueChanged += OnTookDamageChanged;
        damageTaken.OnValueChanged += OnDamageTakenChanged;
    }

    private void OnDisable()
    {
        tookDamage.OnValueChanged -= OnTookDamageChanged;
        damageTaken.OnValueChanged -= OnDamageTakenChanged;
    }

    private void OnTookDamageChanged(bool oldVal, bool newVal)
    {
        //if we didn't take damage then we will change text to nothing
        if (newVal) { damagePopUp.GetComponent<MeshRenderer>().enabled = true; }
        else { damagePopUp.GetComponent<MeshRenderer>().enabled = false; }
    }

    private void OnDamageTakenChanged(int oldVal, int newVal)
    {
        if(damageTaken.Value > 0) 
        { 
            damagePopUp.GetComponent<TextMesh>().text = "-" + newVal.ToString();
            Instantiate(damageSpriteAnimation, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangeTextAndSetActiveServerRpc(int value)
    {
        if (!IsServer) { return; }
        if(value <= 0) { return; }
        
        Debug.Log("The damage value should is: " + value);
        tookDamage.Value = true;
        damageTaken.Value = value;
        //damagePopUp.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        //damagePopUp.GetComponent<MeshRenderer>().sortingOrder = 2;
        initialPosition = gameObject.transform.position;
        //Debug.Log(initialPosition);
        //Debug.Log(damagePopUp.activeSelf);
        damagePopUp.GetComponent<TextMesh>().text = "-" + value;
        //Debug.Log(damagePopUp.GetComponent<TextMesh>().text);
        Instantiate(damageSpriteAnimation, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        
        StartCoroutine(WaitToDisablePopUp());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisablePopUpServerRpc()
    {
        if (!IsServer) { return; }
        tookDamage.Value = false;
        damageTaken.Value = 0;
        damagePopUp.GetComponent<MeshRenderer>().enabled = false;
        //Debug.Log(damagePopUp.activeSelf);
    }

    private IEnumerator WaitToDisablePopUp()
    {
        //Debug.Log(Time.time);
        yield return new WaitForSeconds(.7f);
        //Debug.Log(Time.time);
        
        DisablePopUpServerRpc();
    }

    

    // Update is called once per frame
    void Update()
    {
        

    }
}
