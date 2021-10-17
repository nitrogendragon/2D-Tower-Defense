using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 1f;//typically will be set elsewhere such as BasicUnit.cs attack() function, otw 1f
    public float expirationTime = 1f; //will be set when instantiated otw 1f
    public float attackPower = 0; // will be set when instantiated otw 0
    public Unit myUnit;// will be assigned when instantiated
    private void Start()
    {
        Destroy(gameObject, expirationTime);//remove from game after x period of time of not hitting something
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            myUnit.dealRangedDamage(attackPower,collision.gameObject);   
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.position += transform.right * speed*Time.deltaTime;
    }
}
