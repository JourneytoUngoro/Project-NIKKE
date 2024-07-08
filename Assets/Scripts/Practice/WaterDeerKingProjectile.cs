using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class WaterDeerKingProjectile : MonoBehaviour
{
    [SerializeField] private float explosionTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private int projectileCount;
    [SerializeField] private GameObject projectile;

    private Rigidbody2D rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, explosionTime);
    }

    private void Update()
    {
        rigidBody.velocity = transform.right * projectileSpeed;
    }

    private void OnDestroy()
    {
        Random rand = new Random();
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (float)rand.NextDouble() * 360.0f;
            Instantiate(projectile, transform.position, Quaternion.Euler(0.0f, 0.0f, angle));
        }
        
    }
}
