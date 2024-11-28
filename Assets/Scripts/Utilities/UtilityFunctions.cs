using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using System;

public class UtilityFunctions
{
    private static Random random = new Random();

    public static T2 GetRandom<T1, T2> (T1 objects) where T1 : IEnumerable<T2> => objects.ElementAt(random.Next(objects.Count()));
    public static float RandomFloat(float minValue, float maxValue) => (float)random.NextDouble() * (maxValue - minValue) + minValue;
    public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
