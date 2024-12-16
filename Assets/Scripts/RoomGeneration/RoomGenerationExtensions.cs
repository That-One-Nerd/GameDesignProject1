using System;

public static class RoomGenerationExtensions
{
    public static string GetRandom(this TagWeight[] weights)
    {
        float sumWeight = 0;
        for (int i = 0; i < weights.Length; i++) sumWeight += weights[i].Weight;

        float chosen = UnityEngine.Random.Range(0, sumWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            float expected = weights[i].Weight;
            if (chosen < expected) return weights[i].Tag;
            else chosen -= expected;
        }
        throw new Exception("How did this happen?");
    }
}
