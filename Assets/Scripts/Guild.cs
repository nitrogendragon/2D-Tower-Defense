using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    private int healthRemaining = 10;
    private int maxHealth = 15;
    public HealthBar healthBar;
   
    
    // Start is called before the first frame update
    private void Start()
    {
        
        InitializeGuild();
        
    }

    private void InitializeGuild()
    {   
        healthRemaining = maxHealth;
        healthBar.setHealth(healthRemaining, maxHealth);
        
        
        
    }

    
    // Take damage and destroy the guild if it's health reaches zero
    public void TakeDamage(int damage)
    {
        int tempHealth = healthRemaining - damage;
        healthRemaining = tempHealth;
        
        healthBar.setHealth(healthRemaining, maxHealth);
        if (healthRemaining <= 0)
        {
            DestroyGuild();
        }
    }

    private void DestroyGuild()
    {
        //end the game or something
        //Debug.Log("The guild was destroyed, we lost");
    }
}
