using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnit : Unit
{
    public GameObject projectile;
    //public List<GameObject> activeProjectiles = new List<GameObject>();
    //private List<GameObject> tempProjectiles = new List<GameObject>();
    public Transform unit; 
    public Transform weapon; // the units weapons transform, used in UnitRotation script
    protected override void attack()
    {
        base.attack();
        GameObject newProjectile = Instantiate(projectile, weapon.position, unit.rotation);
        newProjectile.GetComponent<Projectile>().expirationTime = 3f; // will be determined by unit stats and specific abilities later
        newProjectile.GetComponent<Projectile>().attackPower = 5f;// will update this to be based on unit stats later
        newProjectile.GetComponent<Projectile>().speed = .02f; // will be determined by unit stat and or specific abilities later
        newProjectile.GetComponent<Projectile>().myUnit = this;
    }

    //could be a useful reference later
    //private void cleanProjectileList()
    //{
    //    foreach (GameObject projectile in activeProjectiles)
    //    {
    //        if (projectile != null)
    //        {
    //            tempProjectiles.Add(projectile);
    //        }
    //    }
    //    activeProjectiles = tempProjectiles;//update active projectiles to a clean list without nulls-unused indexes
    //    tempProjectiles.Clear();//empty the list
    //}

    

    
}
