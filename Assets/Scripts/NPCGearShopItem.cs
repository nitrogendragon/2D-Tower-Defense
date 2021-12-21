using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCGearShopItem : MonoBehaviour
{
    //public ShopManager shopManager;
    //public ClickManager clickManager;
    public GameObject storeMenu;
    public Text strStatText;
    public Text intStatText;
    public Text agiStatText;
    public Text wisStatText;
    public Text lucStatText;
    public Text chaStatText;
    public Text gearNameText;
    public Text aurCostStatText;
    public Image gearIcon;
    private int strStat;
    private int intStat;
    private int agiStat;
    private int chaStat;
    private int wisStat;
    private int lucStat;
    private int aurCostStat;
    private bool isWeapon;
    private string gearName;
    private Sprite gearIconSprite;
    private Button myButton;
    
    
    //public Button GearBuyButton;//the button we click to purchase, may not need this reference but will put it here for now

    private void Start()
    {
        myButton = this.GetComponent<Button>();
        myButton.onClick.AddListener(TaskOnClick);
        storeMenu.GetComponent<NPCGearShop>().createShopGear(ref strStat, ref intStat, ref agiStat, ref wisStat, ref lucStat, ref chaStat,
            ref aurCostStat, ref gearName, ref isWeapon, ref gearIconSprite);
        //Debug.Log(strStat + " " + intStat + " " + agiStat + " " + lucStat + " " + chaStat + " " + wisStat + " " + aurCostStat + " "
        //    + gearName);
        
        setUpUIElements();
        
    }

    private void TaskOnClick()
    {
        if(storeMenu.GetComponent<NPCGearShop>().VerifyUnitToEquipGear() == true)
        {
            if(storeMenu.GetComponent<NPCGearShop>().BuyGear(aurCostStat, gearName) == true)
            {
                
                storeMenu.GetComponent<NPCGearShop>().EquipGearToSelectedUnit(strStat, intStat, agiStat, wisStat, chaStat, lucStat, gearName,
                    isWeapon, gearIconSprite);
            }
        }
       
    }

   

    private void setUpUIElements()
    {
        strStatText.text = strStat.ToString();
        intStatText.text = intStat.ToString();
        agiStatText.text = agiStat.ToString();
        wisStatText.text = wisStat.ToString();
        lucStatText.text = lucStat.ToString();
        chaStatText.text = chaStat.ToString();
        aurCostStatText.text = aurCostStat.ToString();
        gearNameText.text = gearName;
        gearIcon.sprite = gearIconSprite;
        
    }

    
}
