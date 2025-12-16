using UnityEngine;

public static class NoiseUtil
{
    public static float Perlin2D(float x, float z, int seed)
    {
        return Mathf.PerlinNoise(x + seed, z + seed);
    }

    public static float Perlin3D(float x, float y, float z, int seed)
    {
        float xy = Mathf.PerlinNoise(x + seed, y + seed);
        float yz = Mathf.PerlinNoise(y + seed, z + seed);
        float xz = Mathf.PerlinNoise(x + seed, z + seed);
        return (xy + yz + xz) / 3f;
    }

    public static float Ridge(float v)
    {
        return 2f * (0.5f - Mathf.Abs(0.5f - v));
    }
}