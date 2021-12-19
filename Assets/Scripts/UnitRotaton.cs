using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotaton : MonoBehaviour
{
    //public Transform pivot;
    public Transform weaponpivot;

    private float angle;//angle between unit and enemy for determing rotation
    private Vector3 newRotation;//new angle to be oriented at (just rotating on z-axis to spin
    public Unit unit;
    private Vector2 relative;

    private void rotateUnit()
    {
        if (unit)
        {
            if (unit.currentTarget)//dont want to rotate if we dont have a target
            {
                relative = unit.currentTarget.transform.position - unit.transform.position;

                angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;//get the angle between unit and enemy in degrees

                newRotation = new Vector3(0, 0, angle);

                weaponpivot.transform.localRotation = Quaternion.Euler(newRotation);
            }
        }
    }

    private void Update()
    {
        rotateUnit();
    }
}
