using NPack;
using UnityEngine;

public sealed class URandom
{
    private MersenneTwister _rand;

    public URandom()
    {
        _rand = new MersenneTwister();
    }

    public URandom(int seed)
    {
        _rand = new MersenneTwister(seed);
    }

    /// <summary>
    /// Creates an already seeded (by system.ticks) urandom
    /// </summary>
    public static URandom CreateSeeded()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        URandom rand = new URandom(seed);
        return new URandom(rand.Next());
    }

    public static URandom CreateSeeded(out int seedOut)
    {
        int seed = (int)System.DateTime.Now.Ticks;
        URandom rand = new URandom(seed);
        seedOut = rand.Next();
        return new URandom(seedOut);
    }
    
    public static URandom CreateSeeded(int seedMod)
    {
        int seed = (int)System.DateTime.Now.Ticks + seedMod;
        URandom rand = new URandom(seed);
        return new URandom(rand.Next());
    }

    public static int GenSeed()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        URandom rand = new URandom(seed);
        return rand.Next();
    }

    public bool Chance(int percent)
    {
        return (Next(0, 100) <= percent);
    }

    public bool Chancef(float chance)
    {
        return NextFloat(0f, 1f) < chance;
    }

    public bool NextBool()
    {
        return NextFloat(0f, 1f) < 0.5f;
    }

    public int Next()
    {
        return _rand.Next();
    }

    public int Next(int maxValue)
    {
        return _rand.Next(maxValue);
    }

    /// <summary>
    /// Random between minValue(inclusive) and maxValue(inclusive)
    /// </summary>
    /// <returns>The next.</returns>
    /// <param name="minValue">Minimum value.</param>
    /// <param name="maxValue">Max value.</param>
    public int Next(int minValue, int maxValue)
    {
        if (maxValue <= minValue)
        {
            return minValue;
        }
        return _rand.Next(minValue, maxValue);
    }
    /// <summary>
    /// Random float value between minValue(inclusive) and maxValue(exclusive).
    /// </summary>
    /// <returns>The next.</returns>
    /// <param name="minValue">Minimum float value.</param>
    /// <param name="maxValue">Max float value.</param>
    public float NextFloat(float minValue, float maxValue)
    {
        if (maxValue < minValue)
        {
            var tmp = maxValue;
            minValue = maxValue;
            maxValue = tmp;
        }
        float f = _rand.NextSingle();
        return minValue + f * (maxValue - minValue);
    }
    
    public float NextSingle()
    {
        return _rand.NextSingle(true);
    }

}