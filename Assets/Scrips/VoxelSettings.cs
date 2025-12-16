using UnityEngine;

[System.Serializable]
public class VoxelSettings
{
    [Header("청크 Settings")]
    public int chunkSizeX = 16;
    public int chunkSizeZ = 16;
    public int chunkHeight = 128;

    [Header("월드 시드")]
    public int worldSeed = 12345;

    [Header("지형 Settings")]
    public float baseHeight = 50f;
    public float heightNoiseScale = 0.0075f;
    public float heightNoiseAmp = 18f;

    [Header("바이옴 Settings")]
    public float biomeScale = 0.0025f;
    public int soilDepthMin = 3;
    public int soilDepthMax = 6;

    [Header("산 Settings")]
    public float mountainNoiseScale = 0.0015f;
    public float mountainNoiseAmp = 100f;
    public float mountainThreshold = 0.65f;

    [Header("강 Settings")]
    public float riverNoiseScale = 0.002f;
    public float riverWidth = 0.08f;
    public float riverDepth = 6f;

    [Header("동굴 Settings")]
    public float caveNoiseScale = 0.035f;
    public float caveThreshold = 0.58f;
    public float caveRidgeScale = 0.012f;

    [Header("광석 Settings")]
    public float oreNoiseScale = 0.05f;
    public float coalChance = 0.12f;
    public float ironChance = 0.08f;
    public float goldChance = 0.04f;
    public float diamondChance = 0.02f;

    [Header("광석 최대 높이")]
    public int coalMaxHeight = 128;
    public int ironMaxHeight = 80;
    public int goldMaxHeight = 40;
    public int diamondMaxHeight = 20;

    [Header("수면 Settings")]
    public int seaLevel = 48;

    [Header("노이즈 오프셋")]
    public Vector3 noiseOffset = new Vector3(1000, 1000, 1000);
}