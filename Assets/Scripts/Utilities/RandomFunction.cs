using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using System;

public class RandomFunction
{
    private static Random random = new Random();

    public static T2 GetRandom<T1, T2> (T1 objects) where T1 : IEnumerable<T2>
    {
        int randomIndex = random.Next(objects.Count());
        return objects.ElementAt(randomIndex);
    }
}
