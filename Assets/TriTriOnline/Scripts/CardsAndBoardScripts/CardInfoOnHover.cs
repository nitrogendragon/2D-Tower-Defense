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
        "Burns any entity foolish or unfortunate enough to be licked by this searing flame", "The spriits gather to rejoice the beauty of life and tend to the wounds, Grants Regen", 
        "A threatening battle cry causes all who hear it to cower in fear, Weakens those in range", "A rallying call to allies, raises fighting abilities of those in range",
            "Corrodes all who come in contact. Metal or not, corrosion comes for all afflicted by this strange ability","The call of the tender sweet night. Fall prey to the luschious wilds of the night and find yourself fighting friend for foe."};
    private Dictionary<string, string> abilityNameAndDescriptionPairs = new() 
    {
        ["cut"] = "cleanly, in one stroke, cuts through the target like jello",
        ["smash"] = "pounces on its foe bashing it with tremendous force into a pancake",
        ["claw"] = "Viciously slashes at everything slicing and tearing apart anything in reach in search of scraps",
        ["bite"] = "like a ravenous dog, bites and rips at the foe as if it were a donut to feast on",
        ["stab"] = "carlll!! you stabbed him 47 times in the chest!",
        ["crush"] = "donuts should always be flattened before eating",
        ["ensnare"] = "bait them with the appetizer, so you can enjoy dessert",
        ["arrows"] = "fire away young cupid, it's the only way you'll get chocolates this year",
        ["confuse"] = "nfts are like chocolate, they can look like shit and still be amazing",
        ["scream"] = "thinking of you makes me want to..",
        ["blast"] = "cuz I'm TNT... dynomite",
        ["hypnotize"] = "You're already under my genjustu",
        ["illusion"] = "you thought it was Jonathan!, but it was me, Dio!",
        ["earth"] = "it's just dirt",
        ["air"] = "you need it to breathe",
        ["water"] = "have a drink, it's fresh from the swamp",
        ["fire"] = "burn baby, burn!",
        ["poison"] = "I'm fine if I don't drink the whole bottle right?",
        ["ice"] = "keep it fresh",
        ["nature"] = "it's beautiful until it eats you",
        ["lust"] = "you know you want that waifu",
        ["death"] = "eternal rest = no more responsibilities = sounds good",
        ["light"] = "I can see again!",
        ["shadow"] = "I swear someone is following me",
        ["lava"] = "it's just really hot chili",
        ["time"] = "no, I'm not short on time and randomly making joke descriptions",
        ["mind"] = "I honestly don't know how it works, but it doesn't work very well",
        ["snow"] = "you know you wanna eat it",
        ["lightning"] = "zap",
        ["smoke"] = "it causes cancer",
        ["cosmic"] = "look at all those beautiful stars...",
        ["metal"] = "it's hard, yet flexible, giggity",
        ["sand"] = "how does this turn clear?",
        ["raise dead"] = "no, no zombies, just getting out of bed",
        ["trap"] = "a male or female who is too good at crossdressing",
        ["beam"] = "special beam Cannon!!!",
        ["blood"] = "not sure how to weaponize this tbh",
        ["petrify"] = "doesn't actually turn your enemies to stone",
        ["acid"] = "you know what this is"


    };
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
                //Debug.Log("our tempname is " + tempName);
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
                string abilityNameTemp = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>().GetAbilityName();
                abilityInfo.text = abilityIndexTemp == 0 ? abilityNameTemp + ": " + abilityNameAndDescriptionPairs[abilityNameTemp] :
                abilityNameTemp + ": " + abilityDescriptions[abilityIndexTemp];
            }
            else
            {
                //set everything to enabled for now
                CleanAndHideOrRevealInfo(true);
                //MobCardNetwork hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>() = hit.collider.GetComponent<NetworkObject>().GetComponent<MobCardNetwork>();
                //set the card name text
                string tempName = "";
                tempName = hit.collider.GetComponent<MobCard>().GetMobName();
                //Debug.Log("our tempname is " + tempName);
                cardName.text = tempName;
                //set the card art sprite
                cardArt.sprite = hit.collider.GetComponent<MobCard>().GetSprite();
                for (int i = 0; i < statusIcons.Count; i++)
                {
                    statusIcons[i].enabled = false;
                }
                //determine ability index and update ability Info text
                int abilityIndexTemp = hit.collider.GetComponent<MobCard>().GetAbilityIndex();
                string abilityNameTemp = hit.collider.GetComponent<MobCard>().GetAbilityName();
                abilityInfo.text = abilityIndexTemp == 0 ? abilityNameTemp + ": " + abilityNameAndDescriptionPairs[abilityNameTemp] :
                abilityNameTemp + ": " + abilityDescriptions[abilityIndexTemp];
            }
            
        }
        //else { CleanAndHideOrRevealInfo(false); }

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
