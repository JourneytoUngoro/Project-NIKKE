using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        // Debug.Log("Loaded by the persist objects form the Initializer script");
        // Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("PersistantObjects")));
    }
}
