using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class RandomnessExperiments : MonoBehaviour
{
    [SerializeField] private int seed = 2;
    [SerializeField] private float[] bucketWeights;

    [ContextMenu("Test Randomness")]
    public void TestRandomness()
    {
        ClearLog();
        SeededRandom.Init(seed);
        float radomFloat = SeededRandom.Range(0,1);

        // Bucket percentages should add up to 1
        if(!ValidateBucketPercentages())
            return;
        
        //Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.

        for(int i = 0; i < bucketWeights.Length; i++)
        {
            if(radomFloat < bucketWeights[i])
            {
                Debug.Log($"Bucket {i} was chosen");
                break;
            }
            else
                radomFloat -= bucketWeights[i];
        }

    }
    private bool ValidateBucketPercentages()
    {
        float total = 0;
        for(int i = 0; i < bucketWeights.Length; i++)
            total += bucketWeights[i];

        if(total != 1)
        {
            Debug.LogError("Bucket percentages do not add up to 1");
            return false;
        }
        return true;
    }
    public int GetBucketToLandIn(float[] bucketWeights)
    {
        // random.NextDouble();
        
        return Random.Range(0, 2);
    }
    public void SetBucketPercentages()
    {
        System.Random random = new System.Random(2);
        for(int i = 0; i < 2; i++)
        {
            Debug.Log(random.NextDouble());
        }
    }

    private void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
public static class SeededRandom
{
    private static System.Random random;

    public static void Init(int seed)
    {
        random = new System.Random(seed);
    }

    public static float Range(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }
    public static int Range(int min, int max)
    {
        return (int)(random.NextDouble() * (max - min) + min);
    }
    public static bool FlipCoin(){
        return random.NextDouble() > 0.5;
    }
    private static bool ValidateBucketPercentages(float[] bucketWeights)
    {
        float total = 0;
        for(int i = 0; i < bucketWeights.Length; i++)
            total += bucketWeights[i];

        if(total != 1)
        {
            Debug.LogError("Bucket percentages do not add up to 1");
            return false;
        }
        return true;
    }
}