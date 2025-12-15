using UnityEngine;

/// <summary>
/// 노이즈 관련 유틸리티 함수 모음
/// </summary>
public static class NoiseUtil
{
    // 2D Perlin Noise (시드 포함)
    public static float Perlin2D(float x, float z, int seed)
    {
        return Mathf.PerlinNoise(x + seed * 0.001f, z + seed * 0.001f);
    }

    // 3D Perlin Noise 대용 (xy, yz, zx 평면 노이즈 합성)
    public static float Perlin3D(float x, float y, float z, int seed)
    {
        float xy = Mathf.PerlinNoise(x + seed * 0.001f, y + seed * 0.002f);
        float yz = Mathf.PerlinNoise(y + seed * 0.003f, z + seed * 0.004f);
        float zx = Mathf.PerlinNoise(z + seed * 0.005f, x + seed * 0.006f);
        return (xy + yz + zx) / 3f;
    }

    // Ridge 변형 (산 능선/강 러닝 등 자연스러운 패턴)
    public static float Ridge(float v)
    {
        v = Mathf.Abs(v * 2f - 1f);
        return 1f - v;
    }
}