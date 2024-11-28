using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledObject : MonoBehaviour
{
    public IObjectPool<GameObject> objectPool;

    public virtual void ReleaseObject()
    {
        objectPool.Release(gameObject);
    }
}
