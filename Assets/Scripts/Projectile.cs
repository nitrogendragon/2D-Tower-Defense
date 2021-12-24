using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 1f;//typically will be set elsewhere such as BasicUnit.cs attack() function, otw 1f
    public float expirationTime = 1f; //will be set when instantiated otw 1f
    public BasicUnit myUnit;// will be assigned when instantiated if fired from a unit
    public Enemy myEnemyUnit;// assigned when instantiated if fired from a mob
    public Animator spriteAnimator;// the sprite sheet animation we want to use when firing off an attack/skill
    public Transform spriteTransform;// the transform of our sprite/spriteAnimator
    private void Start()
    {
        Destroy(gameObject, expirationTime);//remove from game after x period of time of not hitting something
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && myUnit != null)
        {
            myUnit.dealRangedDamage(collision.gameObject);   
            Destroy(gameObject);
        }
        if(collision.gameObject.tag == "Player" && myEnemyUnit != null)
        {
            //Debug.Log("The enemy projectile got this far");
            myEnemyUnit.dealRangedDamage(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}
