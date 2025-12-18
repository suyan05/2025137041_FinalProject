using UnityEngine;

public class WorldSampler
{
    private readonly VoxelSettings s;
    public WorldSampler(VoxelSettings settings) { s = settings; }

    // Noise 단계: 높이맵 생성
    public int SampleHeight(int wx, int wz)
    {
        float nx = (wx + s.noiseOffset.x) * s.heightNoiseScale;
        float nz = (wz + s.noiseOffset.z) * s.heightNoiseScale;
        float h = NoiseUtil.Perlin2D(nx, nz, s.worldSeed) * s.heightNoiseAmp;
        return Mathf.Clamp(Mathf.FloorToInt(s.baseHeight + h), 1, s.chunkHeight - 2);
    }

    // Surface 단계: 토양 깊이
    public int SampleSoilDepth(int wx, int wz)
    {
        float n = NoiseUtil.Perlin2D((wx + s.noiseOffset.x) * s.biomeScale,
                                     (wz + s.noiseOffset.z) * s.biomeScale,
                                     s.worldSeed);
        return Mathf.RoundToInt(Mathf.Lerp(s.soilDepthMin, s.soilDepthMax, n));
    }

    // Carvers 단계: 동굴
    public bool IsCave(int wx, int wy, int wz)
    {
        float n = NoiseUtil.Perlin3D(wx * s.caveNoiseScale, wy * s.caveNoiseScale, wz * s.caveNoiseScale, s.worldSeed);
        return n > s.caveThreshold;
    }

    // Biomes 단계: 모래/잔디
    public bool IsSandy(int wx, int wz, int heightAt)
    {
        float b = NoiseUtil.Perlin2D((wx + s.noiseOffset.x) * s.biomeScale,
                                     (wz + s.noiseOffset.z) * s.biomeScale,
                                     s.worldSeed);
        return b > 0.62f && heightAt <= s.seaLevel + 2;
    }

    // Features 단계: 광석
    public BlockId SampleOre(int wx, int wy, int wz)
    {
        float n = NoiseUtil.Perlin3D(wx * s.oreNoiseScale, wy * s.oreNoiseScale, wz * s.oreNoiseScale, s.worldSeed);
        if (wy <= s.diamondMaxHeight && n < s.diamondChance) return BlockId.DiamondOre;
        if (wy <= s.goldMaxHeight && n < s.goldChance) return BlockId.GoldOre;
        if (wy <= s.ironMaxHeight && n < s.ironChance) return BlockId.IronOre;
        if (wy <= s.coalMaxHeight && n < s.coalChance) return BlockId.CoalOre;
        return BlockId.Stone;
    }
}