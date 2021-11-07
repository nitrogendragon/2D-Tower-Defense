using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<GameObject> activeUnits;

    public void addToActiveUnits(GameObject newUnit)
    {
        activeUnits.Add(newUnit);
    }
}
