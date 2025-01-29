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
    public static int RandomInteger(int minValue, int maxValue) => random.Next(minValue, maxValue);
    public static int RandomInteger(int maxValue) => random.Next(maxValue);
    public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    public static int RandomOption(IEnumerable<float> possibilities)
    {
        int totalOption = possibilities.Count();
        float totalPossibility = possibilities.Sum();
        float randomFloat = RandomFloat(0, totalPossibility);

        float currentChance = 0.0f;
        for (int option = 0; option < totalOption; option++)
        {
            currentChance += possibilities.ElementAt(option);
            if (randomFloat <= currentChance)
            {
                return option;
            }
        }

        return totalOption - 1;
    }
}
