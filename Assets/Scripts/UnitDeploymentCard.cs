using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDeploymentCard : MonoBehaviour
{
    private int cost;//cost to deploy the unit
    private int unitID;
    private string unitName;//name of the unit
    public Text unitNameText;//reference to the text UI for displaying the name
    public Text unitCostText;//reference to the text UI for displaying the cost
    public Sprite unitImgSprite;//may use this may not
    public void setCost(int newCost)
    {
        cost = newCost;
        unitCostText.text = cost.ToString();
    }

    public Sprite GetSprite()
    {
        return unitImgSprite;
    }

    public int GetCost()
    {
        return cost;
    }

    public void setName(string newName)
    {
        unitName = newName;
        unitNameText.text = unitName;
    }

    public void setID(int id)
    {
        unitID = id;
    }

    public int getID()
    {
        return unitID;
    }

    public string GetName()
    {
        return unitName;
    }
}
