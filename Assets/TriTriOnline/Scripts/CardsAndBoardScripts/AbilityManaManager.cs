using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AbilityManaManager : NetworkBehaviour
{
    private NetworkVariable<int> abilityManaAvailable = new NetworkVariable<int>(0);
    private NetworkVariable<int> abilityManaLimit = new NetworkVariable<int>(0);
    private NetworkVariable<int> p2AbilityManaAvailable = new NetworkVariable<int>(0);
    private NetworkVariable<int> p2AbilityManaLimit = new NetworkVariable<int>(0);
    private GameObject[] manaClustersP1 = new GameObject[9];
    private GameObject[] manaClustersP2 = new GameObject[9];
    private Color inactiveColor = new Color(0.2924528f, 0.2774754f, 0.2774754f, .75f);
    private Color activeColor = new Color(0.316915f, 0.9716981f, 0.7512923f, .75f);
    private bool isP1Init = false;
    private bool isP2Init = false;
    [SerializeField] GameObject[] manaClusters;
    
    private void SetupAbilityMana()
    {
        

            int i = 0;
            foreach (GameObject cluster in manaClusters)
            {

                manaClustersP1[i] = cluster;
                Debug.Log(manaClustersP1[i]);
                i++;
            }
            i = 0;
            

        
    

    }

   

   

    private void OnEnable()
    {
       abilityManaAvailable.OnValueChanged += OnAbilityManaAvailableChanged;
       p2AbilityManaAvailable.OnValueChanged += OnP2AbilityManaAvailableChanged;
       //abilityManaLimit.OnValueChanged += OnAbilityManaLimitChanged;
    }

    private void OnDisable()
    {
        abilityManaAvailable.OnValueChanged -= OnAbilityManaAvailableChanged;
        p2AbilityManaAvailable.OnValueChanged -= OnP2AbilityManaAvailableChanged;
        //abilityManaLimit.OnValueChanged -= OnAbilityManaLimitChanged;
    }

    

    private void OnAbilityManaAvailableChanged(int oldManaAvailable, int newManaAvailable)
    {
        
        if (!IsHost) { return; }//only want to update the host
        Debug.Log("updated ability mana available is " + newManaAvailable);
        for (int i = 0; i < 9; i++)
        {
            if (i < newManaAvailable)
            {
                manaClusters[i].GetComponent<SpriteRenderer>().color = activeColor;
            }
            else
            {
                manaClusters[i].GetComponent<SpriteRenderer>().color = inactiveColor;
            }
        }
    }

    private void OnP2AbilityManaAvailableChanged(int oldManaAvailable, int newManaAvailable)
    {

        if (IsHost || IsServer) { return; }//only want to update the client
        Debug.Log("updated p2 ability mana available is " + newManaAvailable);
        for (int i = 0; i < 9; i++)
        {
            if (i < newManaAvailable)
            {
                manaClustersP1[i].GetComponent<SpriteRenderer>().color = activeColor;
            }
            else
            {
                manaClustersP1[i].GetComponent<SpriteRenderer>().color = inactiveColor;
            }
        }
    }



    //increase our mana limit and update available mana .. only to be ran when turn ends
    [ServerRpc (RequireOwnership = false)]
    public void IncreaseManaLimitServerRpc(bool isPlayer1)
    {
        //increase player 1's mana limit and reset available mana
        if(!isPlayer1 && IsClient)
        {
            Debug.Log("increased player 2's manaLimit to " + (p2AbilityManaLimit.Value + 1));
            p2AbilityManaAvailable.Value = p2AbilityManaLimit.Value < 9 ? p2AbilityManaLimit.Value + 1 : p2AbilityManaLimit.Value;
            p2AbilityManaLimit.Value = p2AbilityManaLimit.Value < 9 ? p2AbilityManaLimit.Value + 1 : p2AbilityManaLimit.Value;
        }
        //increase player 2's mana limit and reset available mana
        else if (isPlayer1 && IsHost)
        {
            Debug.Log("increased player 1's manaLimit " + (abilityManaAvailable.Value + 1));
            abilityManaAvailable.Value = abilityManaLimit.Value < 9 ? abilityManaLimit.Value + 1 : abilityManaLimit.Value;
            abilityManaLimit.Value = abilityManaLimit.Value < 9 ? abilityManaLimit.Value + 1 : abilityManaLimit.Value;
        }
    }

    //use our mana when activating an ability
    [ServerRpc(RequireOwnership = false)]
    public void UseManaServerRpc(bool isPlayer1, int manaConsumed)
    {
        Debug.Log("The mana being used to cast is: " + manaConsumed);
        if (isPlayer1 && IsHost)
        {
            
            abilityManaAvailable.Value =  abilityManaAvailable.Value - manaConsumed;
        }
        else if (!isPlayer1 && IsClient)
        {

            p2AbilityManaAvailable.Value = p2AbilityManaAvailable.Value - manaConsumed;
        }

    }


    public int GetManaAvailable(bool isPlayer1)
    {
        
        
        if (isPlayer1)
        {
            SetupAbilityMana(); 
            Debug.Log("mana available is: " + abilityManaAvailable.Value);
            return abilityManaAvailable.Value; 
        }
        else
        {
            SetupAbilityMana();
            Debug.Log("mana available is: " + p2AbilityManaAvailable.Value); 
            return p2AbilityManaAvailable.Value; 
        }
    }
}
