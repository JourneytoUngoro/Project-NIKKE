using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5.0f;

    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
}
