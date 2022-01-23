using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
public class CardInfoOnHover : NetworkBehaviour
{
    [SerializeField] private GameObject InfoContainer;
    [SerializeField]  private Image cardArt;
    [SerializeField] private List<Image> statusIcons = new List<Image>();
    [SerializeField]  private TMP_Text cardName;
    [SerializeField]  private TMP_Text abilityInfo;
    [SerializeField]  private TMP_Text statusText;
    private string[] abilityDescriptions = new string[] {"A basic attack using any available means necessary.", "Poisons all who come in contact, slowly leading any creature injected to an unenviable demise.", 
        "Burns any entity foolish or unfortunate enough to be licked by this searing flame", "Corrodes all who come in contact. Metal or not, corrosion comes for all afflicted by this strange ability", 
        "The spriits gather to rejoice the beauty of life and tend to the wounds, Grants Regen", "A threatening battle cry causes all who hear it to cower in fear, Weakens those in range", "A rallying call to allies, raises fighting abilities of those in range", 
            "The call of the tender sweet night. Fall prey to the luschious wilds of the night and find yourself fighting friend for foe."};
    // Start is called before the first frame update
    void Start()
    {
       
        CleanAndHideOrRevealInfo(false);
    }

    //puts everything to a blank or reveals it
    private void CleanAndHideOrRevealInfo(bool isRevealed)
    {
        InfoContainer.SetActive(isRevealed);
        foreach (Image icon in statusIcons)
        {
            icon.enabled = isRevealed;
        }
        cardArt.enabled = isRevealed;
        cardName.enabled = isRevealed;
        abilityInfo.enabled = isRevealed;
        statusText.enabled = isRevealed;
    }
    
    private void CheckForCardToDisplayInfoFor()
    {
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider && (hit.collider.tag == "AbilityCard" || hit.collider.tag == "MobCard"))
        {
            if (hit.collider.GetComponent<NetworkObject>())
            {
                //set everything to enabled for now
                CleanAndHideOrRevealInfo(true);
                //MobCardNetwork hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>() = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>();
                //set the card name text
                string tempName = "";
                tempName = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().GetCardName();
                Debug.Log("our tempname is " + tempName);
                cardName.text = tempName;
                //set the card art sprite
                cardArt.sprite = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().GetCardSprite();
                //enable or disable status effect icons
                bool[] statusEffectsBools = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().GetStatusEffectStates();
                for (int i = 0; i < statusIcons.Count; i++)
                {
                    statusIcons[i].enabled = statusEffectsBools[i];
                }
                //determine ability index and update ability Info text
                int abilityIndexTemp = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().GetAbilityIndex();
                abilityInfo.text = " Ability Info: " + abilityDescriptions[abilityIndexTemp];
            }
            
        }
        else { CleanAndHideOrRevealInfo(false); }

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForCardToDisplayInfoFor();
        }

    }
}
