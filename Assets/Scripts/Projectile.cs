using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        transform.position += transform.right * speed;
    }
}
