using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnit : Unit
{
    public GameObject projectile;

    public Transform unit;
    public Transform weapon;
    protected override void attack()
    {
        base.attack();
        GameObject newProjectile = Instantiate(projectile, weapon.position, unit.rotation);
    }
}
