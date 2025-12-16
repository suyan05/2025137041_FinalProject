using UnityEngine;

/// <summary>
/// 월드 샘플링: 높이맵, 강, 동굴, 바이옴, 광석 등을 계산하는 클래스
/// </summary>
public class WorldSampler
{
    private readonly VoxelSettings s;

    // 생성자: VoxelSettings를 받아 내부에서 사용
    public WorldSampler(VoxelSettings settings)
    {
        s = settings;
    }

    // 높이맵 계산
    public int SampleHeight(int wx, int wz)
    {
        float nx = (wx + s.noiseOffset.x) * s.heightNoiseScale;
        float nz = (wz + s.noiseOffset.z) * s.heightNoiseScale;

        float h = NoiseUtil.Perlin2D(nx, nz, s.worldSeed) * s.heightNoiseAmp;
        h += NoiseUtil.Perlin2D(nx * 2.1f, nz * 2.1f, s.worldSeed + 11) * (s.heightNoiseAmp * 0.5f);
        h += NoiseUtil.Perlin2D(nx * 4.3f, nz * 4.3f, s.worldSeed + 29) * (s.heightNoiseAmp * 0.25f);

        // 산맥 추가
        float mx = (wx + s.noiseOffset.x) * s.mountainNoiseScale;
        float mz = (wz + s.noiseOffset.z) * s.mountainNoiseScale;
        float ridge = NoiseUtil.Ridge(NoiseUtil.Perlin2D(mx, mz, s.worldSeed + 999));

        if (ridge > s.mountainThreshold)
        {
            h += (ridge - s.mountainThreshold) * s.mountainNoiseAmp;
        }

        // 강에 의해 지형 깎기
        float r = SampleRiverMask(wx, wz);
        float riverCarve = r * s.riverDepth;
        int height = Mathf.FloorToInt(s.baseHeight + h - riverCarve);

        // 수면 보정
        if (height < s.seaLevel - 8) height = s.seaLevel - 8;

        return Mathf.Clamp(height, 1, s.chunkHeight - 2);
    }

    // 강 마스크 계산
    public float SampleRiverMask(int wx, int wz)
    {
        float nx = (wx + s.noiseOffset.x) * s.riverNoiseScale;
        float nz = (wz + s.noiseOffset.z) * s.riverNoiseScale;
        float v = NoiseUtil.Perlin2D(nx, nz, s.worldSeed + 777);
        float ridge = NoiseUtil.Ridge(v);
        float mask = Mathf.InverseLerp(1f, 1f - s.riverWidth, ridge);
        return Mathf.Clamp01(mask);
    }

    // 동굴 여부 판정
    public bool IsCave(int wx, int wy, int wz)
    {
        if (wy > s.seaLevel + 6) return false;

        float nx = (wx + s.noiseOffset.x) * s.caveNoiseScale;
        float ny = (wy + s.noiseOffset.y) * s.caveNoiseScale;
        float nz = (wz + s.noiseOffset.z) * s.caveNoiseScale;

        float n = NoiseUtil.Perlin3D(nx, ny, nz, s.worldSeed + 1337);
        float ridge = NoiseUtil.Ridge(NoiseUtil.Perlin3D(nx * 0.7f, ny * 0.7f, nz * 0.7f, s.worldSeed + 4242));
        float v = (n * 0.7f + ridge * 0.3f);

        return v > s.caveThreshold;
    }

    // 간단한 바이옴 판정 (모래/잔디)
    public bool IsSandy(int wx, int wz, int heightAt)
    {
        float b = NoiseUtil.Perlin2D((wx + s.noiseOffset.x) * s.biomeScale,
                                     (wz + s.noiseOffset.z) * s.biomeScale,
                                     s.worldSeed + 55);
        float river = SampleRiverMask(wx, wz);
        float dryness = Mathf.Lerp(b, 1f, river * 0.6f);
        return dryness > 0.62f && heightAt <= s.seaLevel + 2;
    }

    // 광석 샘플링
    public BlockId SampleOre(int wx, int wy, int wz)
    {
        float n = NoiseUtil.Perlin3D(
            (wx + s.noiseOffset.x) * s.oreNoiseScale,
            (wy + s.noiseOffset.y) * s.oreNoiseScale,
            (wz + s.noiseOffset.z) * s.oreNoiseScale,
            s.worldSeed + 2025);

        if (wy <= s.diamondMaxHeight && n < s.diamondChance) return BlockId.DiamondOre;
        if (wy <= s.goldMaxHeight && n < s.goldChance) return BlockId.GoldOre;
        if (wy <= s.ironMaxHeight && n < s.ironChance) return BlockId.IronOre;
        if (wy <= s.coalMaxHeight && n < s.coalChance) return BlockId.CoalOre;

        return BlockId.Stone;
    }

    // 토양 깊이 샘플링
    public int SampleSoilDepth(int wx, int wz)
    {
        float n = NoiseUtil.Perlin2D((wx + s.noiseOffset.x) * (s.biomeScale * 1.7f),
                                     (wz + s.noiseOffset.z) * (s.biomeScale * 1.7f),
                                     s.worldSeed + 101);
        return Mathf.RoundToInt(Mathf.Lerp(s.soilDepthMin, s.soilDepthMax, n));
    }
}