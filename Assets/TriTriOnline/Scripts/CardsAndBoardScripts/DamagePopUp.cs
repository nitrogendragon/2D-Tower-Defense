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
    [SerializeField] private Animator spriteAnimator;// the sprite sheet animation we want to use when firing off an attack/skill
    [SerializeField] private SkillAnimations skillAnimations;
    private NetworkVariable<bool> tookDamage = new NetworkVariable<bool>(false);
    private NetworkVariable<int> damageTaken = new NetworkVariable<int>();
    private NetworkVariable<int> abilityIndex = new NetworkVariable<int>();
    // Start is called before the first frame update

    private void OnEnable()
    {
        tookDamage.OnValueChanged += OnTookDamageChanged;
        damageTaken.OnValueChanged += OnDamageTakenChanged;
        abilityIndex.OnValueChanged += OnSkillIndexChanged;
    }

    private void OnDisable()
    {
        tookDamage.OnValueChanged -= OnTookDamageChanged;
        damageTaken.OnValueChanged -= OnDamageTakenChanged;
        abilityIndex.OnValueChanged += OnSkillIndexChanged;
    }

    private void OnSkillIndexChanged(int oldIndex, int newIndex)
    {
        spriteAnimator.runtimeAnimatorController = skillAnimations.skillAnimators[newIndex].GetComponent<Animator>().runtimeAnimatorController;
    }

    private void OnTookDamageChanged(bool oldVal, bool newVal)
    {
        //if we didn't take damage then we will change text to nothing
        if (newVal) { 
            damagePopUp.GetComponent<MeshRenderer>().enabled = true;
            Instantiate(damageSpriteAnimation, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            
        }
        else { damagePopUp.GetComponent<MeshRenderer>().enabled = false; }
    }

    private void OnDamageTakenChanged(int oldVal, int newVal)
    {
        if(damageTaken.Value > 0) 
        { 
            damagePopUp.GetComponent<TextMesh>().text = "-" + newVal.ToString();
            
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangeTextAndSetActiveServerRpc(int value, int abilIndex)
    {
        if (!IsServer) { return; }
        if(value <= 0) { return; }
        abilityIndex.Value = abilIndex;
        //Debug.Log("abilIndex is: " + abilIndex + " abilityIndex.Value is: " + abilityIndex.Value);
        //Debug.Log("The damage value should is: " + value);
        tookDamage.Value = true;//change this first so the meshrenderer gets enabled and the damage animation starts playing
        damageTaken.Value = value;
        
        //damagePopUp.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        //damagePopUp.GetComponent<MeshRenderer>().sortingOrder = 2;
        initialPosition = gameObject.transform.position;
        //Debug.Log(initialPosition);
        //Debug.Log(damagePopUp.activeSelf);
        damagePopUp.GetComponent<TextMesh>().text = "-" + value;
        //Debug.Log(damagePopUp.GetComponent<TextMesh>().text);
        
        
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
        yield return new WaitForSeconds(2f);
        //Debug.Log(Time.time);
        
        DisablePopUpServerRpc();
    }

    

    // Update is called once per frame
    void Update()
    {
        

    }
}
