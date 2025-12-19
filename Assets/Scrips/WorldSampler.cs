using UnityEngine;
using System.Collections.Generic;

public class WorldSampler
{
    private readonly VoxelSettings s;
    public WorldSampler(VoxelSettings settings)
    {
        s = settings;
        s.InitializeOffsets(); // 랜덤 오프셋 옵션 반영
    }

    // 공통 헬퍼: 2D Perlin
    private float Sample2DNoise(int wx, int wz, float scale)
    {
        float nx = (wx + s.noiseOffset.x) * scale;
        float nz = (wz + s.noiseOffset.z) * scale;
        return NoiseUtil.Perlin2D(nx, nz, s.worldSeed);
    }

    // 공통 헬퍼: 3D Perlin
    private float Sample3DNoise(int wx, int wy, int wz, float scale)
    {
        return NoiseUtil.Perlin3D(wx * scale, wy * scale, wz * scale, s.worldSeed);
    }

    // Noise 단계: 높이맵 생성
    public int SampleHeight(int wx, int wz)
    {
        float h = Sample2DNoise(wx, wz, s.heightNoiseScale) * s.heightNoiseAmp;
        return Mathf.Clamp(Mathf.FloorToInt(s.baseHeight + h), 1, s.chunkHeight - 2);
    }

    // Surface 단계: 토양 깊이
    public int SampleSoilDepth(int wx, int wz)
    {
        float n = Sample2DNoise(wx, wz, s.biomeScale);
        return Mathf.RoundToInt(Mathf.Lerp(s.soilDepthMin, s.soilDepthMax, n));
    }

    // Carvers 단계: 동굴
    public bool IsCave(int wx, int wy, int wz)
    {
        float n = Sample3DNoise(wx, wy, wz, s.caveNoiseScale);
        return n > s.caveThreshold;
    }

    // Biomes 단계: 모래/잔디
    public bool IsSandy(int wx, int wz, int heightAt)
    {
        float b = Sample2DNoise(wx, wz, s.biomeScale);
        return b > s.sandThreshold && heightAt <= s.seaLevel + 2;
    }

    // Features 단계: 광석
    public BlockId SampleOre(int wx, int wy, int wz)
    {
        float n = Sample3DNoise(wx, wy, wz, s.oreNoiseScale);

        foreach (var ore in s.oreEntries)
        {
            if (wy <= ore.maxHeight && n < ore.chance)
                return ore.blockId;
        }

        return BlockId.Stone;
    }
}